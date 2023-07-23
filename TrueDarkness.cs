using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

public class TrueDarknessConfig : ModConfig
{
    // adds difficulty settings and a permanent darkness option to mod config
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Range(1f, 4f)]
    [Label("Visibility")]
    [DrawTicks]
    [DefaultValue(1f)]
    [Increment(1f)]
    public float Visibility { get; set; } = 1f;

    [Label("Permanent Darkness")]
    [DefaultValue(false)]
    public bool AlwaysEnabled { get; set; } = false;
}

namespace TrueDarkness
{
    public class TrueDarkness : Mod
    {
        public static DarknessSystem.DarknessSystem Darkness { get; private set; }
        public static TrueDarknessConfig Config => ModContent.GetInstance<TrueDarknessConfig>();

        public override void Load()
        {
            Darkness = new DarknessSystem.DarknessSystem();
        }
    }
}