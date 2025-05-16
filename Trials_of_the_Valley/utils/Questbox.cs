using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace Trials_of_the_Valley.utils
{
    public class Questbox
    {
        private readonly IMonitor Monitor;
        private readonly IGameContentHelper GameContent;

        // Centralized dict for all quests
        public readonly Dictionary<string, string> PendingQuests = new();

        public Questbox(IMonitor monitor, IGameContentHelper gameContent)
        {
            Monitor = monitor;
            GameContent = gameContent;
        }

        /// <summary>
        /// Registers new mail and refreshes the in-memory game content.
        /// </summary>
        public void AddQuest(string questId, string questContent)
        {
            if (!PendingQuests.ContainsKey(questId))
            {
                PendingQuests[questId] = questContent;

                // Invalidate the Data/Mail cache so SMAPI will call OnAssetRequested again
                GameContent.InvalidateCache("Data/Quests");

                Monitor.Log($"Queued quest '{questId}': {questContent}", LogLevel.Info);
                // Actually give the mail to the player for today
                if (!Game1.player.mailbox.Contains(questId)) { 
                     Game1.player.mailbox.Add(questId);
                     Game1.player.mailReceived.Add(questId);
                }
            }
            else
            {
                Monitor.Log($"Mail ID '{questId}' already exists in pending list", LogLevel.Warn);
            }
        }
    }

}
