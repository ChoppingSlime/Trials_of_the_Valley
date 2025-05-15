using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace Trials_of_the_Valley
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private TrialManager? _trialManager;


        public override void Entry(IModHelper helper)
        {
            _trialManager = new TrialManager(this.Monitor);


            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Content.AssetRequested += OnAssetRequested;

            TrialManager.LoadTrialDefinitions(this.Helper);

            // Add the mail entry at runtime
            var mail = Game1.content.Load<Dictionary<string, string>>("Data\\Mail");
            mail["AddNewQuest"] = "you got a new quest [#]New Quest";
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, string>();
                    foreach (var mail in _trialManager.PendingMails)
                    {
                        editor.Data[mail.Key] = mail.Value;
                    }
                });
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.dayOfMonth.Equals(1)){
                this.Monitor.Log("Day 1", LogLevel.Info);
                Game1.addMail("test 1");
            }

            if (Game1.dayOfMonth.Equals(2)){
                _trialManager.TryGenerateNewTrial();
                this.Monitor.Log("Day 2", LogLevel.Info);
            }

            if (Game1.dayOfMonth.Equals(3))
            {
                this.Monitor.Log("Day 3", LogLevel.Info);
            }

        }
        

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;


            Trial? randomTrial = null;

            // Get a random trial
            if (_trialManager != null) 
                randomTrial = _trialManager.GetRandomTrial();
            
            // If a trial is returned, log its details; otherwise, log that no trial was found
            if (randomTrial != null)
                this.Monitor.Log($"Random Trial: {randomTrial.Name} - {randomTrial.Description}", LogLevel.Info);
            else
                this.Monitor.Log("No random trial available.", LogLevel.Error);
            

        }

    }
}