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
    public class Parenthouse_inside : StardewValley.GameLocation {
        private IModHelper Helper;

        public Parenthouse_inside( IModHelper helper ) : this() {
            this.Helper = helper;

            string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/parenthouse_inside.tmx", ContentSource.ModFolder);
            //string tileAssetKey = this.Helper.Content.GetActualAssetKey("TileSheets/furniture", ContentSource.GameContent);

            IsOutdoors = false;
            IsFarm = false;
            this.mapPath.Set(mapAssetKey);
            name.Value = "parenthouse_inside";
            reloadMap();
            loadObjects();

            //this.addTilesheet(tileAssetKey, 512, 1024, "TileSheets/furniture.png");

        }
            public Parenthouse_inside() {
            
        }

        public override void performTouchAction( string fullActionString, Vector2 playerStandingPosition ) {
            string text = fullActionString.Split(' ')[0];

            if ( text != null && text == "Hometown" ) {
                Game1.warpFarmer("Hometown", 16, 17, flip: true);

                return;
            }

            base.performTouchAction(fullActionString, playerStandingPosition);

        }


        public void addTilesheet( string tilesheetPath, int width, int hight, string tilesheetName ) {
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
