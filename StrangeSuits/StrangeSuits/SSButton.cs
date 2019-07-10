using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StrangeSuits
{
    class SSButton : Button
    {
        MenuStage stage;

        public SSButton(Texture2D sprite, Vector2 position, Texture2D overlay, MenuStage stage)
            : base(sprite, position, overlay)
        {
            this.stage = stage;
        }
        public MenuStage Stage { get { return stage; } }
    }
}