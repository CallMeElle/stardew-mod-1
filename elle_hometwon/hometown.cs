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

namespace CallMeElle.hometown {

	public class Hometown : StardewValley.GameLocation {

		private IModHelper Helper;

		public Hometown( IModHelper helper) : this() {
			this.Helper = helper;

			string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/parents.tmx", ContentSource.ModFolder);
			IsOutdoors = true;
			IsFarm = false;
			this.mapPath.Set(mapAssetKey);
			this.name.Value = "hometown";
			reloadMap();
			loadObjects();
		}

		public Hometown() : base() {

	  	}

		public const int busDefaultXTile = 17;

		public const int busDefaultYTile = 24;

		private TemporaryAnimatedSprite busDoor;

		private Vector2 busPosition;

		private Vector2 busMotion;

		public bool drivingOff;

		public bool drivingBack;

		public bool leaving;

		private int chimneyTimer = 500;

		public static bool warpedToDesert;

		private Microsoft.Xna.Framework.Rectangle busSource = new Microsoft.Xna.Framework.Rectangle(288, 1247, 128, 64);

		private Microsoft.Xna.Framework.Rectangle pamSource = new Microsoft.Xna.Framework.Rectangle(384, 1311, 15, 19);

		private Microsoft.Xna.Framework.Rectangle transparentWindowSource = new Microsoft.Xna.Framework.Rectangle(0, 0, 21, 41);

		private Vector2 pamOffset = new Vector2(0f, 29f);

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
					playerReachedBusDoor(Game1.player, this);
					return true;
				}
			}
			return base.answerDialogue(answer);
		}

		protected override void resetLocalState() {
			base.resetLocalState();
			leaving = false;
			Game1.ambientLight = Color.White;
			if ( Game1.player.getTileX() == 35 && Game1.player.getTileY() == 43 ) {
				warpedToDesert = true;
			}
			if ( Game1.player.getTileY() > 40 || Game1.player.getTileY() < 10 ) {
				drivingOff = false;
				drivingBack = false;
				busMotion = Vector2.Zero;
				busPosition = new Vector2(17f, 24f) * 64f;
				busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), busPosition + new Vector2(16f, 26f) * 4f, flipped: false, 0f, Color.White) {
					interval = 999999f,
					animationLength = 6,
					holdLastFrame = true,
					layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
					scale = 4f
				};
				Game1.changeMusicTrack("wavy");
			} else {
				if ( Game1.isRaining ) {
					Game1.changeMusicTrack("none");
				}
				busPosition = new Vector2(17f, 24f) * 64f;
				busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 1311, 16, 38), busPosition + new Vector2(16f, 26f) * 4f, flipped: false, 0f, Color.White) {
					interval = 999999f,
					animationLength = 1,
					holdLastFrame = true,
					layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
					scale = 4f
				};
				Game1.displayFarmer = false;
				busDriveBack();
			}
		}

		public override void performTenMinuteUpdate( int timeOfDay ) {
			base.performTenMinuteUpdate(timeOfDay);
			if ( Game1.currentLocation != this ) {
				return;
			}
		}

		public override void cleanupBeforePlayerExit() {
			base.cleanupBeforePlayerExit();
			if ( farmers.Count() <= 1 ) {
				busDoor = null;
			}
		}

		public void busDriveOff() {
			busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), busPosition + new Vector2(16f, 26f) * 4f, flipped: false, 0f, Color.White) {
				interval = 999999f,
				animationLength = 6,
				holdLastFrame = true,
				layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
				scale = 4f
			};
			busDoor.timer = 0f;
			busDoor.interval = 70f;
			busDoor.endFunction = busStartMovingOff;
			localSound("trashcanlid");
			drivingBack = false;
			busDoor.paused = false;
		}

		public void busDriveBack() {
			busPosition.X = map.GetLayer("Back").DisplayWidth;
			busDoor.Position = busPosition + new Vector2(16f, 26f) * 4f;
			drivingBack = true;
			drivingOff = false;
			localSound("busDriveOff");
			busMotion = new Vector2(-6f, 0f);
		}

		private void busStartMovingOff( int extraInfo ) {
			Game1.globalFadeToBlack(delegate {
				Game1.globalFadeToClear();
				localSound("batFlap");
				drivingOff = true;
				localSound("busDriveOff");
				Game1.changeMusicTrack("none");
			});
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

		private void doorOpenAfterReturn( int extraInfo ) {
			localSound("batFlap");
			busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), busPosition + new Vector2(16f, 26f) * 4f, flipped: false, 0f, Color.White) {
				interval = 999999f,
				animationLength = 6,
				holdLastFrame = true,
				layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
				scale = 4f
			};
			Game1.player.Position = new Vector2(18f, 27f) * 64f;
			lastTouchActionLocation = Game1.player.getTileLocation();
			Game1.displayFarmer = true;
			Game1.player.forceCanMove();
			Game1.player.faceDirection(2);
			Game1.changeMusicTrack("wavy");
		}

		private void busLeftToValley() {
			Game1.viewport.Y = -100000;
			Game1.viewportFreeze = true;
			Game1.warpFarmer("BusStop", 12, 10, flip: true);
			Game1.player.previousLocationName = "Desert";
			Game1.currentLocation.resetForPlayerEntry();
		}

		public override void UpdateWhenCurrentLocation( GameTime time ) {
			base.UpdateWhenCurrentLocation(time);
			if ( drivingOff && !leaving ) {
				busMotion.X -= 0.075f;
				if ( busPosition.X + 512f < 0f ) {
					leaving = true;
					Game1.globalFadeToBlack(busLeftToValley, 0.01f);
				}
			}
			if ( drivingBack && busMotion != Vector2.Zero ) {
				Game1.player.Position = busDoor.position;
				Game1.player.freezePause = 100;
				if ( busPosition.X - 1088f < 256f ) {
					busMotion.X = Math.Min(-1f, busMotion.X * 0.98f);
				}
				if ( Math.Abs(busPosition.X - 1088f) <= Math.Abs(busMotion.X * 1.5f) ) {
					busPosition.X = 1088f;
					busMotion = Vector2.Zero;
					Game1.globalFadeToBlack(delegate {
						drivingBack = false;
						busDoor.Position = busPosition + new Vector2(16f, 26f) * 4f;
						busDoor.pingPong = true;
						busDoor.interval = 70f;
						busDoor.currentParentTileIndex = 5;
						busDoor.endFunction = doorOpenAfterReturn;
						localSound("trashcanlid");
						Game1.globalFadeToClear();
					});
				}
			}
			if ( !busMotion.Equals(Vector2.Zero) ) {
				busPosition += busMotion;
				if ( busDoor != null ) {
					busDoor.Position += busMotion;
				}
			}
			if ( busDoor != null ) {
				busDoor.update(time);
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
			if ( !drivingOff ) {
				return drivingBack;
			}
			return true;
		}

		public override void draw( SpriteBatch spriteBatch ) {
			base.draw(spriteBatch);
			spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int) busPosition.X, (int) busPosition.Y)), busSource, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ( busPosition.Y + 192f ) / 10000f);
			if ( busDoor != null ) {
				busDoor.draw(spriteBatch);
			}
			if ( drivingOff || drivingBack ) {
				if ( drivingOff && warpedToDesert ) {
					Game1.player.faceDirection(3);
					Game1.player.blinkTimer = -1000;
					Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, secondaryArm: false, flip: true), 117, new Microsoft.Xna.Framework.Rectangle(48, 608, 16, 32), Game1.GlobalToLocal(new Vector2((int) ( busPosition.X + 4f ), (int) ( busPosition.Y - 8f )) + pamOffset * 4f), Vector2.Zero, ( busPosition.Y + 192f + 4f ) / 10000f, Color.White, 0f, 1f, Game1.player);
					spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((int) busPosition.X, (int) busPosition.Y - 40) + pamOffset * 4f), transparentWindowSource, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ( busPosition.Y + 192f + 8f ) / 10000f);
				} else {
					spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int) busPosition.X, (int) busPosition.Y) + pamOffset * 4f), pamSource, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ( busPosition.Y + 192f + 4f ) / 10000f);
				}
			}
		}

	}
}
