using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace StrangeSuits
{
    class SelectPanel
    {
        const int cTextureWidth = 112;
        const int cTextureHeight = 133;
        const int cSpace = 20;
        const int cSpaceWidth = 21;
        int selectNumber;
        List<int> selectedIndices;
        Texture2D panel;
        Vector2 position;
        Button okButton;
        Vector2[] positions;
        SpriteFont font;
        string[] selectedText;
        List<Texture2D> pile;

        public SelectPanel(ContentManager content, params string[] selectedText)
        {
            this.panel = content.Load<Texture2D>(@"Panel\Panel");
            this.position = new Vector2(2 * SSEngine.QuarterSceenWidth - panel.Width / 2,
                2 * SSEngine.QuarterSceenHeight - panel.Height / 2);
            this.selectedText = selectedText;
            Texture2D texture = content.Load<Texture2D>(@"Panel\OkButton");
            this.okButton = new Button(texture, new Vector2(2 * SSEngine.QuarterSceenWidth - texture.Width / 2,
                position.Y + panel.Height - texture.Height - 20), content.Load<Texture2D>(@"Panel\OkButtonCover"));
            positions = new Vector2[4];
            for (int i = 0; i < positions.Length; i++)
            {
                if (i == 0)
                    positions[i] = new Vector2(this.position.X + cSpace + cSpaceWidth,
                        this.position.Y + panel.Height / 2 - cTextureHeight / 2);
                else
                    positions[i] = new Vector2(positions[i - 1].X + cTextureWidth + cSpaceWidth, positions[i - 1].Y);
            }
            font = content.Load<SpriteFont>(@"Panel\PanelFont");
            this.selectNumber = 1;
            selectedIndices = new List<int>();
            pile = new List<Texture2D>() { content.Load<Texture2D>(@"Panel\Club"), content.Load<Texture2D>(@"Panel\Diamond"),
                content.Load<Texture2D>(@"Panel\Heart"), content.Load<Texture2D>(@"Panel\Spade") };
        }

        public Suit? UpdatePanel(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            for (int i = 0; i < pile.Count; i++)
            {
                if (mouse.LeftButton == ButtonState.Pressed &&
                    new Rectangle((int)positions[i].X, (int)positions[i].Y, cTextureWidth,
                    cTextureHeight).Contains(mouse.X, mouse.Y) && !selectedIndices.Contains(i)
                    && selectedIndices.Count < selectNumber)
                {
                    selectedIndices.Add(i);
                    break;
                }
                else if (mouse.LeftButton == ButtonState.Pressed &&
                    new Rectangle((int)positions[i].X, (int)positions[i].Y, cTextureWidth,
                    cTextureHeight).Contains(mouse.X, mouse.Y) && !selectedIndices.Contains(i)
                    && selectedIndices.Count == selectNumber)
                {
                    selectedIndices.Clear();
                    selectedIndices.Add(i);
                    break;
                }
                else if (mouse.RightButton == ButtonState.Pressed &&
                     new Rectangle((int)positions[i].X, (int)positions[i].Y, cTextureWidth,
                    cTextureHeight).Contains(mouse.X, mouse.Y) && selectedIndices.Contains(i))
                {
                    selectedIndices.Remove(i);
                    break;
                }
            }
            Suit? suit = null;
            if (okButton.UpdateButton(mouse, gameTime) && selectedIndices.Count == selectNumber)
            {
                switch (selectedIndices[0])
                {
                    case 0:
                        suit = Suit.Club; break;
                    case 1:
                        suit = Suit.Diamond; break;
                    case 2:
                        suit = Suit.Hearts; break;
                    case 3:
                        suit = Suit.Spades; break;
                    default:
                        throw new IndexOutOfRangeException("This index value " + selectedIndices[0] + " shouldn't be.");
                }
                selectedIndices.Clear();
            }
            return suit;
        }

        public void DrawPanel(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(panel, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);

            if (selectedIndices.Count == selectNumber)
                okButton.Draw(spriteBatch, 0.55f);
            string text = "Selected (" + selectedIndices.Count + "/" + selectNumber + ")";
            spriteBatch.DrawString(font, text, position, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.55f);

            for (int i = 0; i < pile.Count; i++)
            {
                spriteBatch.DrawString(font, (i + 1).ToString(),
                    new Vector2(positions[i].X + cTextureWidth / 2 - font.MeasureString((i + 1).ToString()).X / 2,
                        positions[i].Y - cTextureHeight / 4), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.55f);
                spriteBatch.Draw(pile[i], new Rectangle((int)positions[i].X, (int)positions[i].Y, cTextureWidth,
                    cTextureHeight), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.55f);
                if (selectedIndices.Contains(i))
                {
                    int k = selectedIndices.IndexOf(i);
                    spriteBatch.DrawString(font, selectedText[k], new Vector2(positions[i].X + cTextureWidth / 2 -
                               font.MeasureString(selectedText[k]).X / 2,
                           positions[i].Y + cTextureHeight / 2 - font.MeasureString(selectedText[k]).Y / 2), 
                           Color.Blue, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                }
            }
        }
    }
}