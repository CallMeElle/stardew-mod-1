using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CallMeElle.hometown {
	public class Hometown : GameLocation {

		public Hometown( string mapAssetKey ) : base(mapAssetKey, "Hometown") {
			IsOutdoors = true;
			IsFarm = false;
		}

		public override void performTouchAction( string fullActionString, Vector2 playerStandingPosition ) {
			string text = fullActionString.Split(' ')[0];
			if ( text != null && text == "DesertBus" ) {
				Response[] answerChoices = {
					new Response("Yes", Game1.content.LoadString("Strings\\Locations:Desert_Return_Yes")),
					new Response("Not", Game1.content.LoadString("Strings\\Locations:Desert_Return_No"))
				};
				createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Desert_Return_Question"), answerChoices, "DesertBus");
			} else {
				base.performTouchAction(fullActionString, playerStandingPosition);
			}
		}

		private void playerReachedBusDoor( Character c, GameLocation l ) {
			Game1.player.position.X = -10000f;
			Game1.freezeControls = true;
			Game1.player.CanMove = false;
			busDriveOff();
			playSound("stoneStep");
		}

		public override bool answerDialogue( Response answer ) {
			if ( lastQuestionKey != null && afterQuestion == null ) {
				string text = lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey;
				if ( text != null && text == "DesertBus_Yes" ) {
					//playerReachedBusDoor(Game1.player, this);
					warpHome();
					return true;
				}
			}
			return base.answerDialogue(answer);
		}

		public void warpHome() {
			/*Game1.globalFadeToBlack(delegate {
				Game1.globalFadeToClear();
				localSound("batFlap");
				//drivingOff = true;
				localSound("busDriveOff");
				Game1.changeMusicTrack("none");
			});*/
			//Game1.viewportFreeze = true;
			Game1.warpFarmer("BusStop", 12, 10, flip: true);
		}

		public void busDriveOff() {
			/*busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), busPosition + new Vector2(16f, 26f) * 4f, flipped: false, 0f, Color.White) {
				interval = 999999f,
				animationLength = 6,
				holdLastFrame = true,
				layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
				scale = 4f
			};
			busDoor.timer = 0f;
			busDoor.interval = 70f;
			busDoor.endFunction = busStartMovingOff;*/
			busStartMovingOff(0);		
			/*localSound("trashcanlid");
			drivingBack = false;
			busDoor.paused = false;*/
		}

		private void busStartMovingOff( int extraInfo ) {
		
		}

	}
}
