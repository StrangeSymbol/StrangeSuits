using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace StrangeSuits
{
    class AIHand : IEnumerable<CardSprite>, IHand
    {
        List<CardSprite> hand;
        SoundEffect setEffect;
        Texture2D cardBack;
        Vector2 cardInHandPos;
        double elaspedTime;
        readonly int totalMaxWidth;

        public AIHand(Texture2D cardBack, SoundEffect setEffect)
        {
            this.cardBack = cardBack;
            hand = new List<CardSprite>();
            this.setEffect = setEffect;
            elaspedTime = 0.0;
            totalMaxWidth = cardBack.Width * 10;
        }

        public int Count { get { return hand.Count; } }

        public bool UpdateHand(GameTime gameTime, int i, DiscardPile discardPile)
        {
            if (!SSEngine.IsACardMoving)
            {
                SSEngine.IsACardMoving = true;
                hand[i].IsMoving = true;
                hand[i].CourseToCard(discardPile.GetDiscardSprite());
                cardInHandPos = hand[i].Position;
                elaspedTime = gameTime.TotalGameTime.TotalMilliseconds;
                hand[i].IsFromFaceDownToFaceUp = false;
            }
            else if (hand[i].Time >= gameTime.TotalGameTime.TotalMilliseconds - elaspedTime && hand[i].IsMoving)
                hand[i].Move(gameTime, cardInHandPos, discardPile.GetDiscardSprite().Position);
            else if (hand[i].Time < gameTime.TotalGameTime.TotalMilliseconds - elaspedTime && hand[i].IsMoving)
            {
                discardPile.DiscardList.Add(hand[i].Clone(discardPile.GetDiscardSprite().Position));
                SSEngine.IsACardMoving = false;
                hand[i].IsMoving = false;
                elaspedTime = 0.0;
                RemoveCardFromHand(i);
                setEffect.Play();
                return true;
            }
            return false;
        }

        public CardSprite this[int i]
        {
            get { return this.hand[i]; }
        }

        public void AddCardToHand(CardSprite card)
        {
            hand.Add(card.Clone(GetNextHandPosition()));

            if (hand.Count > 10)
            {
                float ratio = totalMaxWidth / (float)hand.Count;
                for (int i = 0; i < hand.Count; i++)
                {
                    if (i > 0)
                        hand[i].Position = new Vector2(hand[i-1].Position.X - ratio, hand[i-1].Position.Y);
                }
            }
        }

        public void RemoveCardFromHand(int index)
        {
            List<CardSprite> cards;
            if (index != hand.Count - 1)
            {
                cards = hand.GetRange(index + 1, hand.Count - 1 - index);
                hand.RemoveRange(index, hand.Count - index);
                foreach (CardSprite card in cards)
                    AddCardToHand(card);
            }
            else if (index == hand.Count - 1)
                hand.RemoveAt(index);
        }

        public Vector2 GetNextHandPosition()
        {
            if (hand.Count == 0)
                return new Vector2(3 * SSEngine.QuarterSceenWidth,
                    SSEngine.QuarterSceenHeight - cardBack.Height / 2);
            else if (hand.Count < 10)
                return new Vector2(hand[hand.Count - 1].Position.X - cardBack.Width,
                    hand[hand.Count - 1].Position.Y);
            else
                return new Vector2(hand[hand.Count - 1].Position.X - totalMaxWidth / (float)(hand.Count + 1),
                    hand[hand.Count - 1].Position.Y);
        }

        public void EmptyHand()
        {
            hand.Clear();
        }

        public List<byte> GetSuitCounts()
        {
            List<byte> lst = new List<byte>();
            lst.Add((byte)hand.FindAll(c => c.CardSuit == Suit.Club).Count);
            lst.Add((byte)hand.FindAll(c => c.CardSuit == Suit.Diamond).Count);
            lst.Add((byte)hand.FindAll(c => c.CardSuit == Suit.Hearts).Count);
            lst.Add((byte)hand.FindAll(c => c.CardSuit == Suit.Spades).Count);
            return lst;
        }

        public int Find(Predicate<CardSprite> match)
        {
            if (hand.Exists(match))
                return hand.FindIndex(match);
            else
                return -1;
        }

        public int FindCount(Predicate<CardSprite> match)
        {
            return hand.FindAll(match).Count;
        }

        public int[] FindAllIndex(Predicate<CardSprite> match)
        {
            List<int> lst = new List<int>();
            int startIndex = -1;
            while (startIndex < hand.Count - 1)
            {
                startIndex = hand.FindIndex(startIndex + 1, match);
                if (startIndex == -1)
                    break;
                else
                    lst.Add(startIndex);
            }
            return lst.ToArray();
        }

        public List<byte> GetSelectSuitCounts(Predicate<CardSprite> match)
        {
            Suit[] suits = findAllSuits(match);
            List<byte> lst = new List<byte>();
            foreach (Suit suit in suits)
                lst.Add((byte)FindCount(c => c.CardSuit == suit));
            return lst;
        }

        private Suit[] findAllSuits(Predicate<CardSprite> match)
        {
            int[] indexes = FindAllIndex(match);
            List<Suit> lst = new List<Suit>();
            foreach (int index in indexes)
                lst.Add(hand[index].CardSuit);
            return lst.ToArray();
        }

        IEnumerator<CardSprite> IEnumerable<CardSprite>.GetEnumerator()
        {
            foreach (CardSprite sprite in hand)
                yield return sprite;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void DrawHand(SpriteBatch spriteBatch)
        {
            float layer = 0.001f;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].IsMoving)
                    hand[i].DrawSprite(spriteBatch, cardBack);
                else
                    spriteBatch.Draw(cardBack, hand[i].Position, null,
                        Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.74f + i * layer);
            }
        }
    }
}