using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Lidgren.Network;

namespace StrangeSuits
{
    class PlayerHand : IEnumerable<CardSprite>, IHand
    {
        List<CardSprite> hand;
        SoundEffect setEffect;
        Texture2D cardBack;
        Button leftButton;
        Button rightButton;
        Vector2[] positions;
        double elaspedTime;
        int start;
        int end;
        const int MAX_HAND = 10;

        public PlayerHand(Texture2D leftBtnSprite, Texture2D leftBtnSpriteO, Texture2D rightBtnSprite, Texture2D rightBtnSpriteO,
            Texture2D cardBack, SoundEffect setEffect)
        {
            this.cardBack = cardBack;
            hand = new List<CardSprite>();
            this.setEffect = setEffect;
            elaspedTime = 0.0;
            positions = new Vector2[MAX_HAND];
            for (int i = 0; i < positions.Length; i++)
            {
                if (i == 0)
                    positions[0] = new Vector2(SSEngine.QuarterSceenWidth - 3 * cardBack.Width / 2, 
                        3 * SSEngine.QuarterSceenHeight);
                else
                    positions[i] = new Vector2(positions[i - 1].X + cardBack.Width, positions[i - 1].Y);
            }
            this.leftButton = new Button(leftBtnSprite, new Vector2(positions[0].X - 2 * leftBtnSprite.Width, 
                positions[0].Y + cardBack.Width / 2 - leftBtnSprite.Width / 2), leftBtnSpriteO);
            this.rightButton = new Button(rightBtnSprite, new Vector2(positions[MAX_HAND-1].X + cardBack.Width + rightBtnSprite.Width,
                leftButton.Position.Y), rightBtnSpriteO);
        }

        public int Count { get { return hand.Count; } }

        public bool UpdateHand(GameTime gameTime, MouseState mouse, DiscardPile discardPile, Suit? changedSuit, ref string cardAbility)
        {
            for (int i = end - 1; i >= start; i--)
            {
                if (hand[i].CollisionRectangle.Contains(mouse.X, mouse.Y))
                    cardsAbility(hand[i], ref cardAbility);

                if (!SSEngine.IsACardMoving)
                {
                    if (mouse.LeftButton == ButtonState.Pressed &&
                    hand[i].CollisionRectangle.Contains(mouse.X, mouse.Y)
                    && handChecking(hand[i], discardPile, changedSuit, ref cardAbility))
                    {
                        SSEngine.IsACardMoving = true;
                        hand[i].IsMoving = true;
                        hand[i].CourseToCard(discardPile.GetDiscardSprite());
                        elaspedTime = gameTime.TotalGameTime.TotalMilliseconds;
                        cardAbility = String.Empty;
                        if (SSEngine.VersionStage == MenuStage.LAN)
                            SSEngine.LANClient.Send((byte)NetGame.Set, (byte)i);
                        else if (SSEngine.VersionStage == MenuStage.Online)
                        {
                            NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                            msg.Write((byte)NetGame.Set);
                            msg.Write((byte)i);
                            SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                        }
                    }

                    if (leftButton.UpdateButton(mouse, gameTime) && start != 0)
                    {
                        if (start > 0)
                        {
                            start--;
                            end--;
                            for (int k = start, j = 0; k < end; k++, j++)
                                hand[k].Position = positions[j];
                        }
                    }
                    else if (rightButton.UpdateButton(mouse, gameTime) && hand.Count > end)
                    {
                        if (hand.Count - end > 0)
                        {
                            start++;
                            end++;
                            for (int k = start, j = 0; k < end; k++, j++)
                                hand[k].Position = positions[j];
                        }
                    }
                }
                else if (hand[i].Time >= gameTime.TotalGameTime.TotalMilliseconds - elaspedTime && hand[i].IsMoving)
                    hand[i].Move(gameTime);
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
            }
            return false;
        }

        private bool handChecking(CardSprite card, DiscardPile discardPile, Suit? changedSuit, ref string cardAbility)
        {
            if (card.Rank != discardPile[discardPile.Count - 1].Rank && card.CardSuit != changedSuit &&
                card.Rank != RankValue.Eight)
            {
                cardAbility = "Wrong Card Selected.";
                return false;
            }
            else
                return true;
        }

        private void cardsAbility(CardSprite card, ref string cardAbility)
        {
            switch (card.Rank)
            {
                case RankValue.Ace:
                    cardAbility = "Ace: Change Suit Not Colour.\n(ex: heart to diamond).";
                    break;
                case RankValue.Deuce:
                    cardAbility = "Deuce: The opponent draws 2\ntimes the number of deuces set\n(ex: 2 for 1 set, 4 for 2 set).";
                    break;
                case RankValue.Three:
                    cardAbility = "Three: Gets to set two extra\ncards where first card effect\nnegated except " +
                    "for Ace's,\nEight's, and Three gets to set\nanother card.";
                    break;
                case RankValue.Four:
                    cardAbility = "Four: Gets to set an extra card.";
                    break;
                case RankValue.Five:
                    cardAbility = "Five: No Effect.";
                    break;
                case RankValue.Six:
                    cardAbility = "Six: No Effect.";
                    break;
                case RankValue.Seven:
                    cardAbility = "Seven: No Effect.";
                    break;
                case RankValue.Eight:
                    cardAbility = "Eight: Change to any suit.";
                    break;
                case RankValue.Nine:
                    cardAbility = "Nine: No Effect.";
                    break;
                case RankValue.Ten:
                    cardAbility = "Ten: No Effect.";
                    break;
                case RankValue.Jack:
                    cardAbility = "Jack: No Effect.";
                    break;
                case RankValue.Queen:
                    if (card.CardSuit == Suit.Spades)
                        cardAbility = "Queen: The opponent picks up 5\nand if 2 of spades is set after\nQueen of spades\nopponent " +
                            "picks up 7.";
                    else
                        cardAbility = "Queen: No Effect.";
                    break;
                case RankValue.King:
                    cardAbility = "King: No Effect.";
                    break;
            }
        }

        public CardSprite this[int i]
        {
            get { return this.hand[i]; }
        }

        public void AddCardToHand(CardSprite card)
        {
            hand.Add(card.Clone());
            if (end == hand.Count - 1 && hand.Count - 1 < MAX_HAND)
                end++;
            for (int i = start, j = 0; i < end; i++, j++)
                hand[i].Position = positions[j];
        }

        public void RemoveCardFromHand(int index)
        {
            if (index < hand.Count)
            {
                if (start > 0 && end == hand.Count)
                {
                    start--;
                    end--;
                }
                else if (end == hand.Count)
                    end--;

                hand.RemoveAt(index);
                for (int i = start, j = 0; i < end; i++, j++)
                    hand[i].Position = positions[j];
            }
            else
                throw new IndexOutOfRangeException("PlayerHand: No card in hand exist at index: " + index + ".");
        }

        public Vector2 GetNextHandPosition()
        {
            if (hand.Count < MAX_HAND)
                return positions[hand.Count];
            else
                return positions[MAX_HAND - 1];
        }

        public void EmptyHand()
        {
            hand.Clear();
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
            if (start != 0)
                leftButton.Draw(spriteBatch);
            if (end != hand.Count)
                rightButton.Draw(spriteBatch);

            for (int i = start; i < end; i++)
            {
                hand[i].DrawSprite(spriteBatch, null, 0.75f);
            }
        }
    }
}