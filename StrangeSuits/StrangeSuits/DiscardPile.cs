using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StrangeSuits
{
    class DiscardPile
    {
        List<CardSprite> discardPile;
        CardSprite discardPanel;

        public DiscardPile(CardSprite sprite)
        {
            discardPile = new List<CardSprite>();
            discardPanel = sprite;
        }

        public List<CardSprite> DiscardList { get { return discardPile; } }
        public int Count { get { return DiscardList.Count; } }

        public CardSprite this[int i]
        {
            get { return DiscardList[i]; }
        }

        public CardSprite GetDiscardSprite()
        {
            return discardPanel;
        }

        public void Remove(CardSprite card)
        {
            discardPile.Remove(card);
        }

        public void DrawDiscardPile(SpriteBatch spriteBatch)
        {
            discardPanel.DrawSprite(spriteBatch);

            if (discardPile.Count >= 3)
                spriteBatch.Draw(discardPile[discardPile.Count - 3].Texture,
                    new Vector2(discardPanel.Position.X, discardPanel.Position.Y + discardPanel.Texture.Height / 2),
                    null, Color.White, 0f,Vector2.Zero,1f,SpriteEffects.None, 0.95f);
            if (discardPile.Count >= 2)
                spriteBatch.Draw(discardPile[discardPile.Count - 2].Texture,
                    new Vector2(discardPanel.Position.X, discardPanel.Position.Y + discardPanel.Texture.Height / 4),
                     null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            if (discardPile.Count >= 1)
                spriteBatch.Draw(discardPile[discardPile.Count - 1].Texture, discardPanel.Position,
                    null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.85f);
        }
    }
}