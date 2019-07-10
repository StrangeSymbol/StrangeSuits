using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StrangeSuits
{
    class Button : Sprite
    {
        #region Fields
        double elapsedTime;
        const float ButtonWait = 100f;
        #endregion
        #region Properties
        public bool IsClicked { get; set; }
        #endregion
        #region Constructors
        public Button(Texture2D sprite, Vector2 position, Texture2D overlay)
            : base(sprite, position, overlay)
        {
        }
        #endregion
        #region Methods
        public bool UpdateButton(MouseState mouse, GameTime gameTime)
        {
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (CollisionRectangle.Contains(new Point(mouse.X, mouse.Y)))
                {
                    IsClicked = true;
                    IsCovered = true;
                    elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
            else
                IsCovered = false;

            if (elapsedTime != 0 && gameTime.TotalGameTime.TotalMilliseconds - elapsedTime >= ButtonWait)
            {
                IsClicked = false;
                elapsedTime = 0f;
                return true;
            }
            return false;
        }
        #endregion
    }
}
