using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CallMeElle.hometown
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
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
           // get the internal asset key for the map file
			string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/parents.tmx", ContentSource.ModFolder);

			// add the location
			GameLocation location = new Hometown(mapAssetKey);
			Game1.locations.Add(location);
        }

    }
    
}