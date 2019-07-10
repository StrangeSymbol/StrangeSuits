using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StrangeSuits
{
    public abstract class Sprite
    {
        #region Fields
        protected Texture2D sprite;
        protected Texture2D overlay;
        protected Vector2 position;
        #endregion
        #region Properties
        public Vector2 Position { get { return position; } set { position = value; } }
        public Texture2D Texture { get { return sprite; } }
        public bool IsCovered { get; set; }
        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
            }
        }
        #endregion
        #region Constructors
        public Sprite(Texture2D sprite, Vector2 position, Texture2D overlay)
        {
            this.sprite = sprite;
            this.position = position;
            this.overlay = overlay;
        }
        #endregion
        #region Methods
        public void Draw(SpriteBatch spriteBatch, float layerDepth=1f)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth);
            if (IsCovered)
                spriteBatch.Draw(overlay, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth-0.05f);
        }
        #endregion
    }
}
