using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using PetPiggy.Content.PiggyPlayer;

namespace PetPiggy.Content.Projectiles
{
    public class PiggyPetProjectile : ModProjectile
    {
        private int jumpTimer = 0;
        private int piggyJumpTimer = 0;
        private int oinkTimer = 3600;
        
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 5, 15)
                    .WithOffset(-20f, 0f)
                    .WithSpriteDirection(1);
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.width = 26;
            Projectile.height = 23;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.Puppy;
            Main.projFrames[Projectile.type] = 5;
            Projectile.penetrate = -1;
        }
        
        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            ModdedPlayer modPlayer = player.GetModPlayer<ModdedPlayer>();

            if (modPlayer.piggy && !player.dead && player.active)
            {
                Projectile.timeLeft = 2; 
            }
        }
        
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            ModdedPlayer modPlayer = player.GetModPlayer<ModdedPlayer>();
            
            if (modPlayer.piggy && !player.dead && player.active)
            {
                Projectile.timeLeft = 2;
                if (Projectile.velocity.X is <= 0.05f and >= -0.05f)
                {
                    Projectile.frame = 0;
                }
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                }
            }
            else 
            {
                Projectile.Kill();
            }
            
            bool isFlying = Projectile.ai[0] == 1;
            if (isFlying)
            {
                Projectile.frame = 1; 
                
                if (Main.rand.NextBool(3)) 
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0, 0, 150, default, 0.8f);
                }
            }
            else
            {
                if (player.justJumped)
                {
                    jumpTimer = 10;
                }
                if (jumpTimer > 0)
                {
                    jumpTimer--;
                    if (jumpTimer <= 5 && jumpTimer > 0)
                    {
                        if (jumpTimer == 1) 
                        {
                            piggyJumpTimer = 20;
                            Projectile.velocity.Y = -5.5f; 
                            Dust.NewDust(Projectile.Bottom, Projectile.width, 2, DustID.PinkCrystalShard, 0, 0, 100, default, 0.8f);
                        }
                    }
                }
                else
                {
                    if (piggyJumpTimer > 0)
                    {
                        Projectile.frame = 1;
                        piggyJumpTimer--;
                    }
                }
            }
            
            Projectile.spriteDirection = Projectile.direction;
            
            if (oinkTimer > 0)
            {
                oinkTimer--;
            }
            else
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item59 with { Volume = 0.8f, Pitch = 0.2f }, Projectile.position);
                oinkTimer = Main.rand.Next(7200, 10801);
            }
        }
    }
}