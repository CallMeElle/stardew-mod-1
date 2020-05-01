using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CallMeElle
{
    namespace mod1
    {
        /// <summary>The mod entry point.</summary>
        public class ModEntry : Mod
        {
            /*********
            ** Public methods
            *********/
            /// <summary>The mod entry point, called after the mod is first loaded.</summary>
            /// <param name="helper">Provides simplified APIs for writing mods.</param>
            public override void Entry(IModHelper helper)
            {
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            }
            private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
            {
                // get the internal asset key for the map file
                string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/parents.tmx", ContentSource.ModFolder);

                // add the location
                GameLocation location = new GameLocation(mapAssetKey, "Hometown") { IsOutdoors = true, IsFarm = false };
                Game1.locations.Add(location);
            }


            /*********
            ** Private methods
            *********/
            /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The event data.</param>
            private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
            {
                // ignore if player hasn't loaded a save yet
                if (!Context.IsWorldReady)
                    return;

                // print button presses to the console window
                this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            }
        }
    }
}