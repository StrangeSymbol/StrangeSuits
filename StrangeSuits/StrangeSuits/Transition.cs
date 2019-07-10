using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StrangeSuits
{
    class Transition
    {
        int width;
        int height;
        readonly float time;
        int elapsedTime = 0;
        bool isRunning;
        Rectangle rect;
        Texture2D blackTexture;
        GraphicsDevice graphics;

        public Transition(GraphicsDevice graphics, int width, int height, float time)
        {
            this.graphics = graphics;
            this.width = width;
            this.height = height;
            this.time = time;
            rect = new Rectangle(0, 0, width, height);
            blackTexture = new Texture2D(graphics, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < width * height; i++)
                data[i] = Color.Black;
            blackTexture.SetData<Color>(data);
        }

        public bool IsRunning { get { return isRunning; } set { isRunning = value; } }

        public void UpdateCurtainTransition(GameTime gameTime)
        {
            if (isRunning)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime <= time / 2 && elapsedTime + gameTime.ElapsedGameTime.Milliseconds > time / 2)
                    rect = new Rectangle(0, 0, width, height);
                else if (elapsedTime <= time / 2)
                    rect = new Rectangle(0, 0, (int)(2 * width * elapsedTime / time), height);
                else
                    rect = new Rectangle(0, 0, (int)(2 * width * (1 - elapsedTime / time)), height);
                if (elapsedTime >= time)
                {
                    elapsedTime = 0;
                    isRunning = false;
                }
            }
        }
        public void UpdateSlideTransition(GameTime gameTime)
        {
            if (isRunning)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime <= time / 2 && elapsedTime + gameTime.ElapsedGameTime.Milliseconds > time / 2)
                    rect = new Rectangle(0, 0, width, height);
                else if (elapsedTime <= time / 2)
                    rect = new Rectangle(0, 0, (int)(2 * width * elapsedTime / time), height);
                else
                    rect = new Rectangle((int)(2 * width * (elapsedTime - time / 2) / time), 0, width, height);
                if (elapsedTime >= time)
                {
                    elapsedTime = 0;
                    isRunning = false;
                }
            }
        }
        public void UpdateFadeTransition(GameTime gameTime)
        {
            if (isRunning)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime <= time / 2 && elapsedTime + gameTime.ElapsedGameTime.Milliseconds > time / 2)
                {
                    blackTexture = new Texture2D(graphics, width, height);
                    Color[] data = new Color[width * height];
                    for (int i = 0; i < width * height; i++)
                        data[i] = Color.Black;
                    blackTexture.SetData<Color>(data);
                }
                else if (elapsedTime <= time / 2)
                {
                    blackTexture = new Texture2D(graphics, width, height);
                    Color[] data = new Color[width * height];
                    for (int i = 0; i < width * height; i++)
                        data[i] = new Color(0, 0, 0, 2 * elapsedTime / time * 255);
                    blackTexture.SetData<Color>(data);
                }
                else
                {
                    blackTexture = new Texture2D(graphics, width, height);
                    Color[] data = new Color[width * height];
                    for (int i = 0; i < width * height; i++)
                        data[i] = new Color(0, 0, 0, 2 * (1 - elapsedTime / time) * 255);
                    blackTexture.SetData<Color>(data);
                }
                if (elapsedTime >= time)
                {
                    elapsedTime = 0;
                    isRunning = false;
                }
            }
            
        }
        public void DrawCurtainTransition(SpriteBatch spriteBatch)
        {
            if (isRunning)
                spriteBatch.Draw(blackTexture, rect, Color.White);
        }
        public void DrawSlideTransition(SpriteBatch spriteBatch)
        {
            if (isRunning)
                spriteBatch.Draw(blackTexture, rect, Color.White);
        }
        public void DrawFadeTransition(SpriteBatch spriteBatch)
        {
            if (isRunning)
                spriteBatch.Draw(blackTexture, Vector2.Zero, Color.White);
        }
    }
}
