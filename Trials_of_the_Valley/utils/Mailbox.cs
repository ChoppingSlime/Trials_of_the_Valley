using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace Trials_of_the_Valley.utils
{
    public class Mailbox
    {
        private readonly IMonitor Monitor;
        private readonly IGameContentHelper GameContent;

        // Centralized dict for all mails
        public readonly Dictionary<string, string> PendingMails = new();

        public Mailbox(IMonitor monitor, IGameContentHelper gameContent)
        {
            Monitor = monitor;
            GameContent = gameContent;
        }

        /// <summary>
        /// Registers new mail and refreshes the in-memory game content.
        /// </summary>
        public void AddMail(string mailId, string mailContent)
        {
            if (!PendingMails.ContainsKey(mailId))
            {
                PendingMails[mailId] = mailContent;

                // Invalidate the Data/Mail cache so SMAPI will call OnAssetRequested again
                GameContent.InvalidateCache("Data/Mail");

                Monitor.Log($"Queued mail '{mailId}': {mailContent}", LogLevel.Info);
                // Actually give the mail to the player for today
                if (!Game1.player.mailbox.Contains(mailId)) { 
                     Game1.player.mailbox.Add(mailId);
                     Game1.player.mailReceived.Add(mailId);
                }
            }
            else
            {
                Monitor.Log($"Mail ID '{mailId}' already exists in pending list", LogLevel.Warn);
            }
        }
    }

}
