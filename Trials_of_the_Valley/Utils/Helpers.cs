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

    public static class GetItemHelper
    {
        /// <summary>
        /// Returns a list of ALL the crop IDs that are available in the current season.
        /// </summary>
        /// <returns>A list of crop IDs as strings.</returns>
        public static List<String> GetAvailableCrops()
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