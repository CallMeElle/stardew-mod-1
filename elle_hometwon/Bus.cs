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
	public class Bus {
		public Bus() {
			Reset();
		}

		/// <summary>
		/// This is the tile in x direction the player can warp to
		/// </summary>
		public int warpDestination_X = 9;

		/// <summary>
		/// This is the tile in y direction the player can warp to
		/// </summary>
		public int warpDestination_Y = 9;

		/// <summary>
		/// True if the player arrived by warping to warpDestination
		/// </summary>
		private bool warped_here = false;

		/// <summary>
		/// The default tile position of the bus door (lower tile)
		/// </summary>
		public int defaultBusDoorPosition_X = 7;

		/// <summary>
		/// The default tile position of the bus door (lower tile)
		/// </summary>
		public int defaultBusDoorPosition_Y = 4;

		/// <summary>
		/// The bus position offset relative to the defaultBusDoorPosition
		/// </summary>
		private Vector2 busPositionOffset = new Vector2(-1, -3);

		/// <summary>
		/// The bus door position offset relative to the defaultBusDoorPosition
		/// </summary>
		private Vector2 busDoorPositionOffset = new Vector2(0, -1f - 0.375f);

		/// <summary>
		/// The temporary sprite for the bus Door
		/// </summary>
		private TemporaryAnimatedSprite busDoor;

		/// <summary>
		/// The current position of the bus
		/// </summary>
		private Vector2 busPosition;

		/// <summary>
		/// The speed the bus currently has
		/// </summary>
		private Vector2 busMotion;

		/// <summary>
		/// Is true when the bus drives to Stardew Valley
		/// </summary>
		public bool drivingOff;

		/// <summary>
		/// Is true when the bus drives to this location
		/// </summary>
		public bool driving_here;

		/// <summary>
		/// Is true when you leave for Stardew Valley
		/// </summary>
		public bool leaving;

		/// <summary>
		/// The bus source.
		/// </summary>
		private Microsoft.Xna.Framework.Rectangle busSource = new Microsoft.Xna.Framework.Rectangle(288, 1247, 128, 64);

		/// <summary>
		/// The pam source.
		/// </summary>
		private Microsoft.Xna.Framework.Rectangle pamSource = new Microsoft.Xna.Framework.Rectangle(384, 1311, 15, 19);

		/// <summary>
		/// The transparent window source.
		/// </summary>
		private Microsoft.Xna.Framework.Rectangle transparentWindowSource = new Microsoft.Xna.Framework.Rectangle(0, 0, 21, 41);

		/// <summary>
		/// The window offset.
		/// </summary>
		private Vector2 windowOffset = new Vector2(0f, 19f);

		/// <summary>
		/// The pam offset.
		/// </summary>
		private Vector2 driverOffset = new Vector2(0f, 2f);

		public void Reset() {
			leaving = false;
			Game1.ambientLight = Color.White;

			// If player wapred to 9 9 (e.g. used a totem) set var
			if ( Game1.player.getTileX() == warpDestination_X && Game1.player.getTileY() == warpDestination_Y ) {
				warped_here = true;
			}

			//add bus and bus door
			busPosition = new Vector2(defaultBusDoorPosition_X + busPositionOffset.X, defaultBusDoorPosition_Y + busPositionOffset.Y);
			busPosition *= 64f;

			busDoor = new TemporaryAnimatedSprite(
				textureName: "LooseSprites\\Cursors",
				sourceRect: new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38),
				position: new Vector2(defaultBusDoorPosition_X + busDoorPositionOffset.X, defaultBusDoorPosition_Y + busDoorPositionOffset.Y) * 64f,
				flipped: false,
				alphaFade: 0f,
				color: Color.White
			 ) {
				interval = 999999f,
				animationLength = 6,
				holdLastFrame = true,
				layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
				scale = 4f
			};

			//Player has not entered by bus
			if (!( Game1.player.getTileY() > defaultBusDoorPosition_Y - 3 && Game1.player.getTileY() < defaultBusDoorPosition_Y + 2 )) {

				drivingOff = false;
				driving_here = false;
				busMotion = Vector2.Zero;

				Game1.changeMusicTrack("wavy");

			} else { //Player has entered by bus
				busDoor.sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 1311, 16, 38);
				Game1.displayFarmer = false;
				BusArrive();
			}
		}//Reset

		public void BusDriveToValley() {
			GameLocation world = Game1.currentLocation;

			Game1.player.position.X = -10000f;
			Game1.freezeControls = true;
			Game1.player.CanMove = false;

			busDoor = new TemporaryAnimatedSprite(
				textureName: "LooseSprites\\Cursors",
				sourceRect: new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38),
				position: new Vector2(defaultBusDoorPosition_X + busDoorPositionOffset.X, defaultBusDoorPosition_Y + busDoorPositionOffset.Y) * 64f,
				flipped: false,
				alphaFade: 0f,
				color: Color.White
			) {
				interval = 999999f,
				animationLength = 6,
				holdLastFrame = true,
				layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
				scale = 4f
			};

			busDoor.timer = 0f;
			busDoor.interval = 70f;
			busDoor.endFunction = busStartMovingOff;
			world.localSound("trashcanlid");
			driving_here = false;
			busDoor.paused = false;

			world.playSound("stoneStep");
		}//BusDriveToValley

		public void BusArrive() {
			GameLocation world = Game1.currentLocation;

			busPosition.X = world.map.GetLayer("Back").TileWidth * 32f;
			busPosition.Y = defaultBusDoorPosition_Y * 64f;
			busPosition += busPositionOffset*64f;

			busDoor.Position = new Vector2(
				busPosition.X + ( busDoorPositionOffset.X - busPositionOffset.X ) * 64f,
				busPosition.Y + ( busDoorPositionOffset.Y - busPositionOffset.Y ) * 64f
			);

			driving_here = true;
			drivingOff = false;
			world.localSound("busDriveOff");
			busMotion = new Vector2(-6f, 0f);
		}//BusArrive

		private void busStartMovingOff( int extraInfo ) {
			GameLocation world = Game1.currentLocation;

			Game1.globalFadeToBlack(delegate {
				Game1.globalFadeToClear();
				world.localSound("batFlap");
				drivingOff = true;
				world.localSound("busDriveOff");
				Game1.changeMusicTrack("none");
			});
		}//busStartMovingOff

		private void exitBus( int extraInfo ) {
			GameLocation world = Game1.currentLocation;

			world.localSound("batFlap");
			busDoor = new TemporaryAnimatedSprite(
				textureName: "LooseSprites\\Cursors",
				sourceRect: new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38),
				position: new Vector2(
					defaultBusDoorPosition_X + busDoorPositionOffset.X, 
					defaultBusDoorPosition_Y + busDoorPositionOffset.Y
				) * 64f,
				flipped: false,
				alphaFade: 0f,
				color: Color.White
			) {
				interval = 999999f,
				animationLength = 6,
				holdLastFrame = true,
				layerDepth = ( busPosition.Y + 192f ) / 10000f + 1E-05f,
				scale = 4f
			};

			Game1.player.Position = new Vector2(defaultBusDoorPosition_X, defaultBusDoorPosition_Y + 1) * 64f;
			world.lastTouchActionLocation = Game1.player.getTileLocation();
			Game1.displayFarmer = true;
			Game1.player.forceCanMove();
			Game1.player.faceDirection(2);
			Game1.changeMusicTrack("wavy");
		}//exitBus

		private void busLeftToValley() {
			Game1.viewport.Y = -100000;
			Game1.viewportFreeze = true;
			Game1.warpFarmer("BusStop", 12, 10, flip: true);
			Game1.player.previousLocationName = "Desert";
			Game1.locationRequest.Location.resetForPlayerEntry();
		}//busLeftToValley

		public void update(GameTime time) {
			GameLocation world = Game1.currentLocation;

			if ( drivingOff && !leaving ) {
				busMotion.X -= 0.075f;
				if ( busPosition.X + 512f < 0f ) {
					leaving = true;
					Game1.globalFadeToBlack(busLeftToValley, 0.01f);
				}
			}

			if ( driving_here && busMotion != Vector2.Zero ) {
				Game1.player.Position = busDoor.position;
				Game1.player.freezePause = 100;

				if ( busPosition.X/64f - defaultBusDoorPosition_X + busPositionOffset.X < 4f ) {
					busMotion.X = Math.Min(-1f, busMotion.X *  0.98f );
				}

				if ( Math.Abs(busPosition.X - (defaultBusDoorPosition_X + busPositionOffset.X) * 64f) <= Math.Abs(busMotion.X * 1.5f) ) {
					busPosition.X = ( defaultBusDoorPosition_X + busPositionOffset.X ) * 64f;
					busMotion = Vector2.Zero;
					Game1.globalFadeToBlack(delegate {
						driving_here = false;
						busDoor.Position = busPosition - busPositionOffset * 64f + busDoorPositionOffset * 64f;
						busDoor.pingPong = true;
						busDoor.interval = 70f;
						busDoor.currentParentTileIndex = 5;
						busDoor.endFunction = exitBus;
						world.localSound("trashcanlid");
						Game1.globalFadeToClear();
					});
				}
			}

			if ( busMotion != Vector2.Zero ) {
				busPosition += busMotion;
				if ( busDoor != null ) {
					busDoor.Position += busMotion;
				}
			}

			if ( busDoor != null ) {
				busDoor.update(time);
			}

		}//update

		public void draw(SpriteBatch spriteBatch ) {
			spriteBatch.Draw(
				texture: Game1.mouseCursors,
				position: Game1.GlobalToLocal(Game1.viewport, new Vector2((int) busPosition.X, (int) busPosition.Y)),
				sourceRectangle: busSource,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: 4f,
				effects: SpriteEffects.None,
				layerDepth: ( busPosition.Y + 192f ) / 10000f
			);

			if ( busDoor != null ) {
				busDoor.draw(spriteBatch);
			}

			if ( drivingOff || driving_here ) {
				if ( drivingOff && warped_here ) {
					Game1.player.faceDirection(3);
					Game1.player.blinkTimer = -1000;

					Game1.player.FarmerRenderer.draw(
						b: spriteBatch,
						animationFrame: new FarmerSprite.AnimationFrame(
							117,
							99999,
							0,
							secondaryArm: false,
							flip: true),
						currentFrame: 117,
						sourceRect: new Microsoft.Xna.Framework.Rectangle(48, 608, 16, 32),
						position: Game1.GlobalToLocal(new Vector2((int) ( busPosition.X + 4f ), (int) ( busPosition.Y - 8f )) + driverOffset * 64f),
						origin: Vector2.Zero,
						layerDepth: ( busPosition.Y + 192f + 4f ) / 10000f,
						overrideColor: Color.White,
						rotation: 0f,
						scale: 1f,
						who: Game1.player
					);

					spriteBatch.Draw(
						texture: Game1.mouseCursors2,
						position: Game1.GlobalToLocal(
							Game1.viewport,
							new Vector2((int) busPosition.X, (int) busPosition.Y) + windowOffset * 4f
						),
						sourceRectangle: transparentWindowSource,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: 4f,
						effects: SpriteEffects.None,
						layerDepth: ( busPosition.Y + 192f + 8f ) / 10000f
					);
				} else {
					spriteBatch.Draw(
						texture: Game1.mouseCursors,
						position: Game1.GlobalToLocal(
							Game1.viewport,
							new Vector2((int) busPosition.X, (int) busPosition.Y) + driverOffset * 64f),
						sourceRectangle: pamSource,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: 4f,
						effects: SpriteEffects.None,
						layerDepth: ( busPosition.Y + 192f + 4f ) / 10000f
					);
				}
			}
		}//draw
	}
}
