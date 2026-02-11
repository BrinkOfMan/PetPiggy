using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using PetPiggy.Content.Buffs;
using PetPiggy.Content.Projectiles;
using Terraria.Utilities;

namespace PetPiggy.Content.Pets
{
	public class PiggyPet : ModItem
	{
		private ModPlayer modPlayer;
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.DogWhistle);
			Item.width = 26;
			Item.height = 26;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 11);
			
			Item.DefaultToVanitypet(ModContent.ProjectileType<PiggyPetProjectile>(), ModContent.BuffType<PiggyPetBuff>());
			
			Item.useTime = 0;
			Item.useAnimation = 0;
			Item.useStyle = 0; 
			Item.UseSound = null;
		}
		
		public override bool? PrefixChance(int pre, UnifiedRandom rand) => false; // Not relevant, but good practice for pets
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddBuff(Item.buffType, 18000, true); 
		}
		
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			Recipe recipe2 = CreateRecipe();

			if (ModLoader.HasMod("CalamityMod"))
			{
				recipe.AddIngredient(ModLoader.GetMod("CalamityMod"), "PiggyItem");
				recipe.AddRecipeGroup(RecipeGroupID.Fruit);
			}
			else
			{
				recipe.AddIngredient(ItemID.Bacon);
				recipe.AddTile(TileID.CookingPots);
				recipe2.AddIngredient(ItemID.PiggyBank);
				recipe2.AddTile(TileID.DemonAltar);
				recipe2.Register();
			}


			recipe.Register();
		}

	}
}
