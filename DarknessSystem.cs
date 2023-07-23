using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace DarknessSystem
{
    public class DarknessOverlay : UIState
    {
        private UIImage darknessImage;
        private Asset<Texture2D> darknessTexture;

        private const int TextureWidth = 1920 * 2;
        private const int TextureHeight = 1080 * 2;
        private int currentVisibility;
        private static readonly Dictionary<int, string> TexturePaths = new Dictionary<int, string>
        {
            { 1, "TrueDarkness/darkness_gradient_DEATH" },
            { 2, "TrueDarkness/darkness_gradient_hard" },
            { 3, "TrueDarkness/darkness_gradient_medium" },
            { 4, "TrueDarkness/darkness_gradient_easy" }
        };

        public override void OnInitialize()
        {
            // chooses a texture based on the difficulty setting
            int difficulty = (int)Math.Round(TrueDarkness.TrueDarkness.Config.Visibility);
            string texturePath = TexturePaths[difficulty];
            // loads the darkness texture
            darknessTexture = ModContent.Request<Texture2D>(texturePath);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (darknessTexture != null)
            {
                Player player = Main.LocalPlayer;

                // the position of the hole based on the player's position relative to the camera
                Vector2 playerCenter = player.MountedCenter - new Vector2(185, 100);
                // for some reason the shift is not 1:1, so we need to scale around the pivot and scale by 0.9
                Vector2 pivot = new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                Vector2 scaledPlayerCenter = ((playerCenter - Main.screenPosition) - pivot) * 0.9f + pivot;
                Vector2 holePosition = new Vector2(Main.screenWidth, Main.screenHeight) - scaledPlayerCenter;

                // calculates the source rectangle based on the hole's position
                int sourceX = (int)MathHelper.Clamp(holePosition.X + TextureWidth / 4 - Main.screenWidth / 2, 0, TextureWidth - Main.screenWidth);
                int sourceY = (int)MathHelper.Clamp(holePosition.Y + TextureHeight / 4 - Main.screenHeight / 2, 0, TextureHeight - Main.screenHeight);
                Rectangle sourceRectangle = new Rectangle(sourceX, sourceY, Main.screenWidth, Main.screenHeight);
                // draws the part of the texture that corresponds to the source rectangle
                spriteBatch.Draw(darknessTexture.Value, destinationRectangle: new Rectangle(0, 0, Main.screenWidth+1, Main.screenHeight+1), sourceRectangle: sourceRectangle, color: Color.White);
            }
        }
        public override void Update(GameTime gameTime)
        {
            // checks if the difficulty setting has changed
            if (TrueDarkness.TrueDarkness.Config.Visibility != currentVisibility)
            {
                // updates the current difficulty
                currentVisibility = (int)Math.Round(TrueDarkness.TrueDarkness.Config.Visibility);

                // reloads the texture
                OnInitialize();
            }
        }
    }
    public class DarknessSystem : ModSystem
    {
        public DarknessOverlay darknessOverlay;
        public UserInterface darknessInterface;

        public override void Load()
        {
            // initializes the overlay and interface
            darknessOverlay = new DarknessOverlay();
            darknessInterface = new UserInterface();
            darknessInterface.SetState(darknessOverlay);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "TrueDarkness: Darkness Overlay",
                    delegate
                    {
                        // only draws the overlay if a boss is present or permanent darkness is enabled
                        bool bossPresent = Main.npc.Take(Main.maxNPCs).Any(x => x.active && x.boss);
                        bool permanentDarkness = TrueDarkness.TrueDarkness.Config.AlwaysEnabled;
                        if (bossPresent || permanentDarkness)
                        {
                            darknessInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // only updates the overlay if a boss is present or permanent darkness is enabled
            bool bossPresent = Main.npc.Take(Main.maxNPCs).Any(x => x.active && x.boss);
            bool permanentDarkness = TrueDarkness.TrueDarkness.Config.AlwaysEnabled;
            if (bossPresent || permanentDarkness) 
            {
                darknessInterface?.Update(gameTime);
            }
        }
    }
}