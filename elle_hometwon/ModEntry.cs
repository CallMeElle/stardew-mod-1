using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CallMeElle.hometown
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod{

        private Hometown hometown;
        private Parenthouse_inside parents;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper){
            //Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            helper.Events.GameLoop.DayEnding += this.BeforeSaved;
            helper.Events.GameLoop.DayStarted += this.AfterSaved;
        }
        

        private void BeforeSaved( object sender, DayEndingEventArgs args) {
            // remove the location

            //remove hometown before save
            if ( Game1.locations.Contains(hometown) ) {
            	Game1.locations.Remove(hometown);
            	this.Monitor.Log("removed hometown", LogLevel.Info);
            } else {
            	this.Monitor.Log("cannot find hometown", LogLevel.Warn);
            }

            //remove parent(house) before save
            if ( Game1.locations.Contains(parents) ) {
                Game1.locations.Remove(parents);
                this.Monitor.Log("removed parents(house)", LogLevel.Info);
            } else {
                this.Monitor.Log("cannot find parents(house)", LogLevel.Warn);
            }

        }



        private void AfterSaved( object sender, DayStartedEventArgs args ) {
            // add the location

            //add hometown after save
            if ( hometown == null ) {
            	hometown = new Hometown(this.Helper);
            	this.Monitor.Log("created hometown", LogLevel.Info);
            }

            if( Game1.locations.Contains(hometown) ) {
            	this.Monitor.Log("hometown already added", LogLevel.Error);
            } else {
            	Game1.locations.Add(hometown);
            	this.Monitor.Log("added hometown", LogLevel.Info);
            }

            //add parent(house) after save
            if ( parents == null ) {
                parents = new Parenthouse_inside(this.Helper);
                this.Monitor.Log("created parents(house)", LogLevel.Info);
            }

            if ( Game1.locations.Contains(parents) ) {
                this.Monitor.Log("parents(house) already added", LogLevel.Error);
            } else {
                Game1.locations.Add(parents);
                this.Monitor.Log("added parents(house)", LogLevel.Info);
            }

        }


	}
    
}
