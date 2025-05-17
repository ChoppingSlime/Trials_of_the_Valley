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
        public static Mailbox? _mailBox;
        public static Questbox? _questbox;


        public override void Entry(IModHelper helper)
        {
            _questbox = new Questbox(this.Monitor, helper.GameContent);
            _mailBox = new Mailbox(this.Monitor, helper.GameContent); 
            _trialManager = new TrialManager(this.Monitor, _mailBox);


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
                    this.Monitor.Log("adding mail asset", LogLevel.Info);

                    var editor = asset.AsDictionary<string, string>();
                    foreach (var mail in _mailBox.PendingMails)
                    {
                        editor.Data[mail.Key] = mail.Value;
                    }

                    this.Monitor.Log($"{editor}", LogLevel.Info);
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests"))
            {
                e.Edit(asset =>
                {
                    this.Monitor.Log("adding quest asset", LogLevel.Info);

                    var editor = asset.AsDictionary<string, string>();
                    foreach (var quest in _questbox.PendingQuests)
                    {
                        editor.Data[quest.Key] = quest.Value;
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