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
        private Questbox _questbox;
        private readonly IMonitor _monitor;

        

        public TrialManager(IMonitor monitor, Mailbox mailbox, Questbox questbox)
        {

            _monitor = monitor;
            _mailbox = mailbox;
            _questbox = questbox;
        }


        /// <summary>
        /// A dictionary that holds the trial definitions grouped by their condition type.
        /// The key is the <see cref="TrialConditionType"/> and the value is a list of trials that meet that condition.
        /// </summary>
        private static Dictionary<TrialConditionType, List<Trial>>? TrialsByCondition;


        /// <summary>
        /// Loads trial definitions from the embedded JSON file and groups them by <see cref="TrialConditionType"/>.
        /// If the file is missing or empty, an empty dictionary will be initialized.
        /// </summary>
        /// <param name="helper">The mod helper instance used to access content files.</param>
        public static void LoadTrialDefinitions(IModHelper helper)
        {
            TrialsByCondition = helper.ModContent
        .Load<Dictionary<TrialConditionType, List<Trial>>>("assets/TrialDefinitions.json")
        ?? new();
        }


        /// <summary>
        /// Selects a random trial that meets current conditions (tags, year, etc.).
        /// Logs an error and returns null if no trials are available.
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
        /// Determines whether a trial can currently be assigned based on its tags.
        /// </summary>
        /// <returns>True if the trial can be assigned; otherwise, false.</returns>
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
        /// Sets up a random trial by creating its quest and sending the associated mail.
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
        /// /// Builds the quest content string for the given trial.
        /// </summary>
        private void CreateTrialQuest(Trial trial)
        {
            
  
            string questContent = GenerateTrialQuestContent(trial);
            _questbox.AddQuest(trial.ID, questContent);

            _monitor.Log($"Quest '{trial.ID}' sent", LogLevel.Info);
            

        }

        /// <summary>
        /// Creates a proper quest content for the trial.
        /// </summary>
        /// <returns> The content of Trial quest containing all fields.</returns>
        private string GenerateTrialQuestContent(Trial trial)
        {
            if (string.IsNullOrWhiteSpace(trial.ObjectiveHint))
                throw new InvalidOperationException($"Trial '{trial.ID}' is missing an ObjectiveHint.");

            if (string.IsNullOrWhiteSpace(trial.CompletionRequirements))
                throw new InvalidOperationException($"Trial '{trial.ID}' is missing CompletionRequirements.");

            string type = trial.Type.ToString();
            string title = trial.Title;
            string description = trial.Description;
            string objectiveHint = trial.ObjectiveHint;
            string completionRequirements = trial.CompletionRequirements;
            string nextQuestIds = "";
            string moneyReward = "";
            string rewardDescription = "";
            string canBeCancelled = "";
            string reactionText = "";

            // Slash-separated quest content
            string questContent = string.Join("/", new[]
            {
                type,
                title,
                description,
                objectiveHint,
                completionRequirements,
                nextQuestIds,
                moneyReward,
                rewardDescription,
                canBeCancelled,
                reactionText
            });

            //TO-DO: add functio that substitutes the {AMOUNT} with a the actual amount!!

            return questContent;
        }

        /// <summary>
        /// Creates the  mail data containing description of the trial. 
        /// Immedatly sent to the player.
        /// </summary>
        private void SendTrialMail(Trial trial)
        {
            string mailContent = GenerateTrialMailContent(trial);
            _mailbox.AddMail(trial.ID, mailContent);

            _monitor.Log($"Mail '{trial.ID}' sent", LogLevel.Info);

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