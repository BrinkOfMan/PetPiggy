using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using PetPiggy.Content.PiggyPlayer;
using Terraria.GameContent;
using Terraria.GameContent.UI;

namespace PetPiggy.Content.Projectiles
{
    public class PiggyPetProjectile : ModProjectile
    {
        private int jumpTimer = 0;
        private int piggyJumpTimer = 0;
        private int oinkTimer = 3600;
        private int thoughtTimer = 100;
        private bool piggyHasNoticedAFruit = false;
        private int fruitIconLifeTimer = 0;
        
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
            Projectile.width = 30;
            Projectile.height = 23;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.Puppy;
            Main.projFrames[Projectile.type] = 5;
            Projectile.penetrate = -1;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.myPlayer == Projectile.owner) 
            {
                Player player = Main.LocalPlayer;
                Item heldItem = player.HeldItem;
                bool holdingFruit = !heldItem.IsAir && RecipeGroup.recipeGroups[RecipeGroupID.Fruit].ContainsItem(heldItem.type);
                
                if (holdingFruit && Projectile.Distance(Main.MouseWorld) < 40f) 
                {
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = heldItem.type;
                    player.cursorItemIconText = ""; 
                }
            }
            return true;
        }
        
        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            ModdedPlayer modPlayer = player.GetModPlayer<ModdedPlayer>();

            if (modPlayer.piggy && !player.dead && player.active) Projectile.timeLeft = 2; 
        }
        
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            ModdedPlayer modPlayer = player.GetModPlayer<ModdedPlayer>();
            
            if (modPlayer.piggy && !player.dead && player.active)
            {
                Projectile.timeLeft = 2;
                if (Projectile.velocity.X is <= 0.05f and >= -0.05f) Projectile.frame = 0;
                if (Projectile.frame >= 5) Projectile.frame = 0;
            }
            else Projectile.Kill();
            
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
                if (player.justJumped) jumpTimer = 10;
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
            if (oinkTimer > 0) oinkTimer--;
            else
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item59 with { Volume = 0.8f, Pitch = 0.2f }, Projectile.position);
                oinkTimer = Main.rand.Next(7200, 10801);
            }
            
            Item heldItem = player.HeldItem;
            bool holdingFruit = !heldItem.IsAir && RecipeGroup.recipeGroups[RecipeGroupID.Fruit].ContainsItem(heldItem.type);
            if (player.HeldItem.active && holdingFruit) {
                thoughtTimer++;
                if (thoughtTimer >= 20 && Main.netMode != NetmodeID.Server && !piggyHasNoticedAFruit) {
                    piggyHasNoticedAFruit = true;
                    Projectile.direction = (player.Center.X < Projectile.Center.X) ? -1 : 1;
                    EmoteBubble.NewBubble(3, new WorldUIAnchor(Projectile), 90);
                }
                float distanceToMouse = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                if (Main.mouseRight && Main.mouseRightRelease) {
                    if (distanceToMouse < 40f) {
                        for (int i = 0; i < 5; i++) {
                            int dustType = Main.rand.NextBool() ? DustID.Blood : DustID.Cloud;
                            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
                            d.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(0f, 0.25f));
                            d.noGravity = false;
                            d.scale = Main.rand.NextFloat(0.6f, 1.1f);
                            d.fadeIn = 0.2f;
                        }
                        
                        if (Projectile.owner == Main.myPlayer) {
                            heldItem.stack--;
                            if (heldItem.stack <= 0) heldItem.SetDefaults(0);
                        }
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item2 with { Pitch = 0.5f }, Projectile.position);
                        if (Main.netMode != NetmodeID.Server) {
                            EmoteBubble.NewBubble(EmoteID.EmotionLove, new WorldUIAnchor(Projectile), 120);
                        }
                        
                        Projectile.velocity.Y = -4f;
                        piggyJumpTimer = 15;
                    }
                }
            }
            else {
                thoughtTimer = 0;
                piggyHasNoticedAFruit = false;
            }
        }
    }
}