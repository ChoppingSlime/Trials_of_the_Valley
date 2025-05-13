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
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            TrialManager.LoadTrialDefinitions(this.Helper);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            
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