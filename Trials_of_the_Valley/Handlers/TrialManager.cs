using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using System;
using Microsoft.Xna.Framework;


namespace Trials_of_the_Valley
{
    public class TrialManager
    {

        private readonly IMonitor Monitor;

        public TrialManager(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// A dictionary that holds the trial definitions grouped by their condition type.
        /// The key is the <see cref="TrialConditionType"/> and the value is a list of trials that meet that condition.
        /// </summary>
        private static Dictionary<TrialConditionType, List<Trial>>? _trialsByCondition;


        /// <summary>
        /// Loads all trial definitions from the mod's embedded JSON file into memory,
        /// deserializing them into a dictionary grouped by <see cref="TrialConditionType"/>.
        /// </summary>
        /// <param name="helper">The mod helper instance used to access content files.</param>
        public static void LoadTrialDefinitions(IModHelper helper)
        {
            _trialsByCondition = helper.ModContent
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
            if (_trialsByCondition == null || _trialsByCondition.Count == 0)
            { 
                this.Monitor.Log("No trial definitions loaded", LogLevel.Error);
                return null;
            }

            // Flattens the huge list of trials and excludes the not available ones
            List<Trial> allTrials = _trialsByCondition
                .SelectMany(kvp => kvp.Value)
                .Where(IsTrialDoable)
                .ToList();

            // If not available trails to select from >> send an error
            if (allTrials.Count == 0)
            {
                this.Monitor.Log("No available trials to select from", LogLevel.Error);
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



    }
}