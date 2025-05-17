using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using System;
using Microsoft.Xna.Framework;
using StardewValley.Quests;
using Trials_of_the_Valley.utils;


namespace Trials_of_the_Valley
{
    public class TrialManager
    {
 
        private Mailbox _mailbox;
        private readonly IMonitor _monitor;

        

        public TrialManager(IMonitor monitor, Mailbox mailbox)
        {

            _monitor = monitor;
            _mailbox = mailbox;
        }


        /// <summary>
        /// A dictionary that holds the trial definitions grouped by their condition type.
        /// The key is the <see cref="TrialConditionType"/> and the value is a list of trials that meet that condition.
        /// </summary>
        private static Dictionary<TrialConditionType, List<Trial>>? TrialsByCondition;


        /// <summary>
        /// Loads all trial definitions from the mod's embedded JSON file into memory,
        /// deserializing them into a dictionary grouped by <see cref="TrialConditionType"/>.
        /// </summary>
        /// <param name="helper">The mod helper instance used to access content files.</param>
        public static void LoadTrialDefinitions(IModHelper helper)
        {
            TrialsByCondition = helper.ModContent
        .Load<Dictionary<TrialConditionType, List<Trial>>>("assets/TrialDefinitions.json")
        ?? new();
        }


        /// <summary>
        /// Retrieves a random trial from the available trials that are marked as "doable".
        /// If no trials are loaded or if no available trials are found, logs an error and returns null.
        /// </summary>
        /// <returns>A random <see cref="Trial"/> if available, otherwise null.</returns>
        public Trial? GetRandomTrial()
        {
            // if no trial definition loaded or null >> send an error
            if (TrialsByCondition == null || TrialsByCondition.Count == 0)
            {
                this._monitor.Log("No trial definitions could be loaded", LogLevel.Error);
                return null;
            }

            // Flattens the huge list of trials and excludes the not available ones
            List<Trial> allTrials = TrialsByCondition
                .SelectMany(kvp => kvp.Value)
                .Where(IsTrialDoable)
                .ToList();

            // If not available trails to select from >> send an error
            if (allTrials.Count == 0)
            {
                this._monitor.Log("No available trials to select from", LogLevel.Error);
                return null;
            }

            // Picks one trial from the flattened list
            Random rand = new();
            return allTrials[rand.Next(allTrials.Count)];
        }


        /// <summary>
        /// Checks if the Trial is doable or not based on it's tags.
        /// </summary>
        /// <returns> "false" if the trial is not doable, else "true".</returns>
        public static bool IsTrialDoable(Trial trial)
        {
            foreach (string tag in trial.Tags)
            {
                switch (tag)
                {
                    case "FirstYear":
                        if (Game1.year != 1) return false;
                        break;

                    case "PastFirstYear":
                        if (Game1.year <= 1) return false;
                        break;

                    case "SecondYear":
                        if (Game1.year != 2) return false;
                        break;

                    case "PastSecondYear":
                        if (Game1.year <= 2) return false;
                        break;

                    case "Coop":
                        if (!Context.IsMultiplayer) return false;
                        break;

                    case "Singleplayer":
                        if (Context.IsMultiplayer) return false;
                        break;

                    case "BusUnlocked":
                        if (!Game1.player.mailReceived.Contains("ccVault") &&
                            !Game1.player.mailReceived.Contains("jojaBus"))
                            return false;
                        break;

                    case "ShipUnlocked":
                        if (!Game1.player.mailReceived.Contains("WillyIslandIntroduction"))
                            return false;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a random trial, sets up a quest for it and sends the mail containing it.
        /// </summary>
        public void GenerateNewTrialAndSendMail()
        {
            Trial? trial = GetRandomTrial();
            if (trial == null)
            {
                _monitor.Log("No valid trial coul be selected.", LogLevel.Warn);
                return;
            }
            if (!Game1.player.mailReceived.Contains(trial.ID))
            {
                CreateTrialQuest(trial);
                SendTrialMail(trial);
            }
            else
            {
                _monitor.Log($"Mail '{trial.ID}' already received.", LogLevel.Debug);
            }



        }

        /// <summary>
        /// Creates the data for the Trial's quest and adds it to the cache.
        /// </summary>
        private void CreateTrialQuest(Trial trial)
        {
  
            string mailContent = GenerateTrialMailContent(trial);
            _mailbox.AddMail(trial.ID, mailContent);

            _monitor.Log($"Mail '{trial.ID}' sent for trial '{trial.ID}'", LogLevel.Info);
            

        }

        /// <summary>
        /// Sends a mail containing description of the trial. When opened it will trigger the new quest.
        /// </summary>
        private void SendTrialMail(Trial trial)
        {
            string mailContent = GenerateTrialMailContent(trial);
            _mailbox.AddMail(trial.ID, mailContent);

            _monitor.Log($"Mail '{trial.ID}' sent for trial '{trial.ID}'", LogLevel.Info);

        }

        /// <summary>
        /// Creates a proper mail content for the trial.
        /// </summary>
        /// <returns> The content of Trial mail.</returns>
        private string GenerateTrialMailContent(Trial trial)
        {
            // Mail title
            string mailTitle = $"{trial.Title} Trial";

            
            string TrialMailPrefix = "A new quest has arrived!";
            string questTrigger = $"% item quest {trial.ID} true %%";

            // Actual mail content
            string mailContent = $"{TrialMailPrefix} {trial.Description}{questTrigger}[#]{mailTitle}";

            return mailContent;
        }



    }
}