using System;
using System.Collections.Generic;
using System.Linq;
//using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using xTile.Tiles;


namespace CallMeElle.hometown {
	public class Crop_Properties {

		/// <summary>
		/// The index of the seed. Should be unique.
		/// </summary>
		public int seedIndex; 

		/// <summary>
		/// The tile x of crop position.
		/// </summary>
		public int tileX; 

		/// <summary>
		/// The tile y of crop position.
		/// </summary>
		public int tileY; 

		/// <summary>
		/// The length of each of the growth stages.
		/// </summary>
		public int[] growth_stages_length;

		/// <summary>
		/// The growth season(s).
		/// </summary>
		public string[] growth_season;

		/// <summary>
		/// The index on the spritesheet. Gives the location of the crop images.
		/// </summary>
		public int index_spritesheet;

		/// <summary>
		/// The index of the harvest. In other words: The unique number of the item you recieve after harvest.
		/// </summary>
		public int harvest_index;

		/// <summary>
		/// Regrow after harvest. (-1 for no regrow, otherwise the number of days until next harvest.)
		/// </summary>
		public int regrow_after_harvest;

		/// <summary>
		/// The harvest method. (1 Scythe, 0 for others)
		/// </summary>
		public int harvest_method;

		/// <summary>
		/// The chance for extra harvest.
		/// </summary>
		public bool chance_extra_harvest;

		/// <summary>
		/// The type of the extra harvest. (0 minHarvest, 1 maxHarvest, 2 maxHarvestIncreasePerFarmingLevel, 3 chanceForExtraCrops)
		/// </summary>
		public int[] extra_harvest_type;

		/// <summary>
		/// The walkthrough able. True for trellis crops (Grapes, Hops, Green Beans), false for all others.  
		/// </summary>
		public bool not_walkthrough_able;

		/// <summary>
		/// The color of the tint. Are there other color combiantions possible for this crop (e.g. flowers).
		/// </summary>
		public bool tint_color;

		/// <summary>
		/// The color possibilities in RGB.
		/// </summary>
		public Color[] color_possibilities;

		public Crop_Properties() {
			seedIndex = 0;
			tileX = 0;
			tileY = 0;
			growth_stages_length = null;
			growth_season = new string[1];
			growth_season[0] = "spring";
			index_spritesheet = 0;
			harvest_index = 0;
			regrow_after_harvest = -1;
			harvest_method = 0;
			chance_extra_harvest = false;
			extra_harvest_type = null;
			not_walkthrough_able = false;
			tint_color = false;
			color_possibilities = null;
		}
	}
}
