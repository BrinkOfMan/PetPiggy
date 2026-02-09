using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PetPiggy.Content.PiggyPlayer;

public partial class ModdedPlayer : ModPlayer
{

    public bool piggy = false;

    public override void ResetEffects()
    {
        piggy = false;
    }
}