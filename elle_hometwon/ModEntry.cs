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
		private Hometown location;
		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
        {
			helper.Events.GameLoop.DayEnding += this.BeforeSaved;
			helper.Events.GameLoop.DayStarted += this.AfterSaved;
        }
        

		private void BeforeSaved( object sender, DayEndingEventArgs args) {
			// add the location

			if ( Game1.locations.Contains(location) ) {
				Game1.locations.Remove(location);
				this.Monitor.Log("removed hometown", LogLevel.Info);
			} else {
				this.Monitor.Log("cannot find hometown", LogLevel.Warn);
			}

		}

		private void AfterSaved( object sender, DayStartedEventArgs args ) {
			// add the location

			if ( location == null ) {
				location = new Hometown(this.Helper);
				this.Monitor.Log("created hometown", LogLevel.Info);
			}

			if( Game1.locations.Contains(location) ) {
				this.Monitor.Log("hometown already added", LogLevel.Error);
			} else {
				Game1.locations.Add(location);
				this.Monitor.Log("added hometown", LogLevel.Info);
			}



		}



	}
    
}