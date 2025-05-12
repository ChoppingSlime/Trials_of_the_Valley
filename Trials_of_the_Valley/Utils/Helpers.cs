using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Crops;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Trials_of_the_Valley
{
    public static class RandomValueHelper
    {

        


        public Trial GenerateTrialFromTemplate(TrialTemplate template, int year)
        {
            var trial = new Trial
            {
                ID = template.ID,
                Type = template.Type,
                DurationDays = template.DurationDays,
                StartDate = SDate.Now(),
            };

            string description = template.Description;

            if (description.Contains("{CROP}"))
            {
                trial.TargetCrop = GetRandomCropForSeason(Game1.currentSeason);
                description = description.Replace("{CROP}", trial.TargetCrop);
            }


            if (description.Contains("{AMOUNT}"))
            {
                trial.GoalAmount = template.GoalAmountBase * year; // or whatever scale logic you want
                description = description.Replace("{AMOUNT}", trial.GoalAmount.ToString());
            }

            trial.Description = description;

            return trial;
        }
    }
    
    public static class GetItemHelper
    {
        /// <summary>
        /// Returns a list of ALL the crop IDs that are available in the current season.
        /// </summary>
        /// <returns>A list of crop IDs as strings.</returns>
        private static List<String> GetAvailableCrops()
        {
            var availableCrops = new List<string>();

            foreach (var entry in Game1.cropData)
            {
                string cropID = entry.Key;
                var data = entry.Value;

                if (data.Seasons != null && data.Seasons.Contains(Game1.season))
                {
                    availableCrops.Add(cropID);
                }
            }
            return availableCrops;
        }
    }
}