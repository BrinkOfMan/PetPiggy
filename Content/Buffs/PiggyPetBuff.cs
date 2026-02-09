using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using PetPiggy.Content.Projectiles;
using PetPiggy.Content.PiggyPlayer;
using Terraria.ID;

namespace PetPiggy.Content.Buffs
{
    public class PiggyPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ModdedPlayer>().piggy = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<PiggyPetProjectile>()] == 0)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item59, player.Center);
                Projectile.NewProjectile(
                    player.GetSource_Buff(buffIndex),
                    player.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<PiggyPetProjectile>(),
                    0,
                    0f,
                    player.whoAmI
                );
            }
        }

    }
}