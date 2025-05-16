using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Trials_of_the_Valley.utils;


namespace Trials_of_the_Valley
{
    /// <summary>The mod entry point.</summary>
    public  class ModEntry : Mod
    {
        private TrialManager? _trialManager;
        public static Mailbox? Mailbox;


        public override void Entry(IModHelper helper)
        {
            
            Mailbox = new Mailbox(this.Monitor, helper.GameContent);

            _trialManager = new TrialManager(this.Monitor, Mailbox);


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
                this.Monitor.Log("receiving request to mail asset", LogLevel.Info);

                e.Edit(asset =>
                {
                    this.Monitor.Log("adding asset", LogLevel.Info);

                    var editor = asset.AsDictionary<string, string>();
                    foreach (var mail in Mailbox.PendingMails)
                    {
                        editor.Data[mail.Key] = mail.Value;
                    }

                    this.Monitor.Log($"{editor}", LogLevel.Info);
                });
            }
        }


        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.dayOfMonth.Equals(1)){
                this.Monitor.Log("Day 1", LogLevel.Info);
                Game1.addMail("test 1");
            }


            if (Game1.dayOfMonth.Equals(3))
            {
               
                this.Monitor.Log("Day 3", LogLevel.Info);
                _trialManager.GenerateNewTrialAndSendMail();

            }

        }
        

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;


         

            
            

        }

    }
}