using System;
using System.Linq;
//using Entoarox.Framework;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using xTile.Tiles;

namespace CallMeElle.hometown {

	public class Hometown : StardewValley.GameLocation {

		private int BusPosition_x;
		private int BusPosition_Y;

		private IModHelper Helper;

		public Hometown( IModHelper helper) : this() {
			this.Helper = helper;

			string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/parents.tmx", ContentSource.ModFolder);
			string tileAssetKey = this.Helper.Content.GetActualAssetKey("assets/Parenthouse.png", ContentSource.ModFolder);

			IsOutdoors = true;
			IsFarm = false;
			this.mapPath.Set(mapAssetKey);
			name.Value = "hometown";
			reloadMap();
			loadObjects();

			this.addTilesheet(tileAssetKey, 16, 16, "Parenthouse");

			BusPosition_x = 13;
			BusPosition_Y = 4;


		}

		public Hometown() : base() {

	  	}

		/// <summary>
		/// If true it can rain on this map
		/// </summary>
		public bool allowRaining = true;



		/// <summary>
		/// The warped to desert.
		/// </summary>
		public static bool warpedToDesert;

		/// <summary>
		/// The bus you use to arrive and drive back
		/// </summary>
		public Bus bus = null;

		/// <summary>
		/// Answers the dialogue.
		/// </summary>
		/// <returns><c>true</c>, if dialogue was answered, <c>false</c> otherwise.</returns>
		/// <param name="answer">Answer.</param>
		public override bool answerDialogue( Response answer ) {

			if ( lastQuestionKey != null && afterQuestion == null ) {
				string text = lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey;
				if ( text != null && text == "DesertBus_Yes" ) {

					if(bus != null ) {
						//Prepare to leave for valley
						bus.BusDriveToValley();
					} else {
						Game1.warpFarmer("BusStop", 12, 10, flip: true);
					}


					//PlayerReachedBusDoor(Game1.player, this);
					return true;
				}
			}
			return base.answerDialogue(answer);
		}

		/// <summary>
		/// Resets the state of the world.
		/// </summary>
		protected override void resetLocalState() {
			base.resetLocalState();

			if ( Game1.isRaining && !allowRaining ) {
				Game1.changeMusicTrack("none");
			}

			if ( bus != null ) {
				bus.Reset();
			} else {
				bus = new Bus {
					defaultBusDoorPosition_X = BusPosition_x,
					defaultBusDoorPosition_Y = BusPosition_Y
				};

				bus.Reset();
			}
		}

		/*
		public override void performTenMinuteUpdate( int timeOfDay ) {
			base.performTenMinuteUpdate(timeOfDay);
			if ( Game1.currentLocation != this ) {
				return;
			}
		}*/

		public override void cleanupBeforePlayerExit() {
			base.cleanupBeforePlayerExit();
			if ( farmers.Count() <= 1 ) {
				bus = null;
			}
		}



		public override void performTouchAction( string fullActionString, Vector2 playerStandingPosition ) {
			string text = fullActionString.Split(' ')[0];
			if ( text != null && text == "DesertBus" ) {
				Response[] answerChoices = new Response[2]
				{
					new Response("Yes", Game1.content.LoadString("Strings\\Locations:Desert_Return_Yes")),
					new Response("Not", Game1.content.LoadString("Strings\\Locations:Desert_Return_No"))
				};
				createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Desert_Return_Question"), answerChoices, "DesertBus");
			} else {
				base.performTouchAction(fullActionString, playerStandingPosition);
			}
		}



		public override void UpdateWhenCurrentLocation( GameTime time ) {
			base.UpdateWhenCurrentLocation(time);

			if(bus != null ) {
				bus.update(time);
			}

		}

		public override void DayUpdate( int dayOfMonth ) {
			base.DayUpdate(dayOfMonth);
			for ( int i = 33; i < 46; i++ ) {
				for ( int j = 20; j < 25; j++ ) {
					removeEverythingExceptCharactersFromThisTile(i, j);
				}
			}
		}

		public override bool isTilePlaceable( Vector2 v, Item item = null ) {
			if ( v.X >= 33f && v.X < 46f && v.Y >= 20f && v.Y < 25f ) {
				return false;
			}
			return base.isTilePlaceable(v, item);
		}

		public override bool shouldHideCharacters() {
			if(bus != null && !bus.drivingOff){
				return bus.driving_here;
			}
			return true;
		}

		public override void draw( SpriteBatch spriteBatch ) {
			base.draw(spriteBatch);

			if(bus != null ) {
				bus.draw(spriteBatch);
			}

		}

		public void addTilesheet(string tilesheetPath, int width, int hight, string tilesheetName) {
			// Add the tilesheet.
			TileSheet tilesheet = new TileSheet(
			   id: "z_" + tilesheetName, // a unique ID for the tilesheet
			   map: this.map,
			   imageSource: tilesheetPath,
			   sheetSize: new xTile.Dimensions.Size(width, hight), // the tile size of your tilesheet image.
			   tileSize: new xTile.Dimensions.Size(16, 16) // should always be 16x16 for maps
			);
			this.map.AddTileSheet(tilesheet);
			this.map.LoadTileSheets(Game1.mapDisplayDevice);
		
		}

	}
}
