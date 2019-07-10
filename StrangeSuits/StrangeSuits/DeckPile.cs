using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Lidgren.Network;

namespace StrangeSuits
{
    struct DeckPile
    {
        List<CardSprite> deckPile;
        SoundEffect drawEffect;
        SoundEffect shuffleEffect;
        SoundEffect setEffect;
        double elapsedTime;
        bool isPlayer1;
        bool shuffleMove;
        byte numShuffles;

        public DeckPile(CardSprite deck, CardSprite cardBack, SoundEffect shuffleEffect,
            SoundEffect drawEffect, SoundEffect setEffect)
        {
            deckPile = new List<CardSprite>() { deck, cardBack.Clone(deck.Position),cardBack.Clone(deck.Position)};
            this.drawEffect = drawEffect;
            this.shuffleEffect = shuffleEffect;
            this.setEffect = setEffect;
            elapsedTime = 0.0;
            isPlayer1 = false;
            this.shuffleMove = false;
            this.numShuffles = 0;
        }

        public CardSprite RetrieveCard(int card)
        {
            if (card >= 3 && card < 55)
                return deckPile[card];
            else
                throw new IndexOutOfRangeException("Card index has to be between 3 and 54");
        }

        public List<CardSprite> Deck { get { return deckPile; } }

        public void ShuffleDeck()
        {
            if ((SSEngine.Player1.Value && SSEngine.VersionStage != MenuStage.LAN && SSEngine.VersionStage != MenuStage.Online) ||
                 (SSEngine.VersionStage == MenuStage.LAN && !SSEngine.LANClient.HasMessage()) || 
                 (SSEngine.VersionStage == MenuStage.Online && SSEngine.Peer.PeekMessage() == null))
            {
                Random genDeck = new Random();
                bool[] check52 = new bool[52];
                for (int count = 0; count < 52; count++)
                {
                    bool hasCard = false;
                    int suitInt = 0;
                    int rankInt = 0;
                    int deckInt = 0;
                    while (!hasCard)
                    {
                        deckInt = genDeck.Next(52);
                        rankInt = deckInt % 13;
                        suitInt = deckInt / 13;
                        if (!check52[deckInt])
                            hasCard = true;
                    }
                    check52[deckInt] = true;
                    deckPile.Add(SSEngine.DeckSprites[(RankValue)rankInt + "" + (Suit)suitInt]);
                }
                if (SSEngine.VersionStage == MenuStage.LAN)
                {
                    byte[] data = new byte[2 * (deckPile.Count - 3) + 1];
                    data[0] = (byte)NetGame.Shuffle;
                    for (int i = 3, j = 1; j < data.Length - 1; i++, j += 2)
                    {
                        data[j] = (byte)deckPile[i].Rank;
                        data[j + 1] = (byte)deckPile[i].CardSuit;
                    }
                    SSEngine.LANClient.Send(data);
                }
                else if (SSEngine.VersionStage == MenuStage.Online)
                {
                    NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                    msg.Write((byte)NetGame.Shuffle);
                    msg.Write((byte)(2 * (deckPile.Count - 3)));
                    byte[] data = new byte[2 * (deckPile.Count - 3)];
                    for (int i = 3, j = 0; j < data.Length - 1; i++, j += 2)
                    {
                        data[j] = (byte)deckPile[i].Rank;
                        data[j + 1] = (byte)deckPile[i].CardSuit;
                    }
                    msg.Write(data);
                    SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public bool ShuffleDeck(DiscardPile discardPile)
        {
            if (deckPile.Count == 3 && (SSEngine.VersionStage != MenuStage.LAN || SSEngine.VersionStage != MenuStage.Online ||
                SSEngine.Player1 != SSEngine.Player1First))
            {
                Random random = new Random();
                List<CardSprite> shuffledDeck = new List<CardSprite>();
                
                while (discardPile.Count > 1)
                {
                    CardSprite card = discardPile[random.Next(0, discardPile.Count - 1)];
                    discardPile.Remove(card);
                    shuffledDeck.Add(card);
                }

                deckPile.AddRange(shuffledDeck);

                if (SSEngine.VersionStage == MenuStage.LAN)
                {
                    byte[] data = new byte[2 * (deckPile.Count - 3) + 1];
                    data[0] = (byte)NetGame.Shuffle;
                    for (int i = 3, j = 1; j < data.Length - 1; i++, j += 2)
                    {
                        data[j] = (byte)deckPile[i].Rank;
                        data[j + 1] = (byte)deckPile[i].CardSuit;
                    }
                    SSEngine.LANClient.Send(data);
                }
                else if (SSEngine.VersionStage == MenuStage.Online)
                {
                    NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                    msg.Write((byte)NetGame.Shuffle);
                    msg.Write((byte)(2 * (deckPile.Count - 3)));
                    byte[] data = new byte[2 * (deckPile.Count - 3)];
                    for (int i = 3, j = 0; j < data.Length - 1; i++, j += 2)
                    {
                        data[j] = (byte)deckPile[i].Rank;
                        data[j + 1] = (byte)deckPile[i].CardSuit;
                    }
                    msg.Write(data);
                    SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                }
                return true;
            }
            return false;
        }

        public bool UpdateDeckPile(GameTime gameTime, IHand hand)
        {
            if (hand is PlayerHand)
                isPlayer1 = true;
            else if (hand is AIHand)
                isPlayer1 = false;
            if (!SSEngine.IsACardMoving)
            {
                SSEngine.IsACardMoving = true;
                deckPile[2].IsCovered = false;
                deckPile[2].IsMoving = true;
                deckPile[2].CourseToCard(hand.GetNextHandPosition());
                elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                drawEffect.Play();
            }
            else if (deckPile[2].Time >= gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
                deckPile[2].Move(gameTime, deckPile[0].Position, hand.GetNextHandPosition());
            else if (deckPile[2].Time < gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
            {
                hand.AddCardToHand(deckPile[deckPile.Count - 1].Clone());
                deckPile.RemoveAt(deckPile.Count - 1);
                deckPile[2].Position = deckPile[0].Position;
                deckPile[2].Destination = deckPile[2].CollisionRectangle;
                SSEngine.IsACardMoving = false;
                deckPile[2].IsMoving = false;
                elapsedTime = 0.0;
                setEffect.Play();
                return true;
            }
            return false;
        }

        public bool UpdateDeckPile(GameTime gameTime, MouseState mouse, PlayerHand hand)
        {
            isPlayer1 = true;
            if (!SSEngine.IsACardMoving)
            {
                if (mouse.LeftButton == ButtonState.Pressed &&
                    deckPile[0].CollisionRectangle.Contains(mouse.X, mouse.Y))
                {
                    SSEngine.IsACardMoving = true;
                    deckPile[2].IsMoving = true;
                    deckPile[2].CourseToCard(hand.GetNextHandPosition());
                    elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                    drawEffect.Play();
                    if (SSEngine.VersionStage == MenuStage.LAN)
                        SSEngine.LANClient.Send((byte)NetGame.Draw);
                    else if (SSEngine.VersionStage == MenuStage.Online)
                    {
                        NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                        msg.Write((byte)NetGame.Draw);
                        SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
            else if (deckPile[2].Time >= gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
                deckPile[2].Move(gameTime, deckPile[0].Position, hand.GetNextHandPosition());
            else if (deckPile[2].Time < gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
            {
                hand.AddCardToHand(deckPile[deckPile.Count - 1].Clone());
                deckPile.RemoveAt(deckPile.Count - 1);
                deckPile[2].Position = deckPile[0].Position;
                deckPile[2].Destination = deckPile[2].CollisionRectangle;
                SSEngine.IsACardMoving = false;
                deckPile[2].IsMoving = false;
                elapsedTime = 0.0;
                setEffect.Play();
                return true;
            }
            return false;
        }

        public bool UpdateShuffleDeck(GameTime gameTime)
        {
            if (!SSEngine.IsACardMoving)
            {
                SSEngine.IsACardMoving = true;
                deckPile[2].IsMoving = true;
                if (!shuffleMove)
                {
                    if (this.isPlayer1)
                        deckPile[2].CourseToCard(deckPile[0].Position + new Vector2(deckPile[0].Texture.Width, 0));
                    else
                        deckPile[2].CourseToCard(deckPile[0].Position + new Vector2(-deckPile[0].Texture.Width, 0));
                }
                else
                {
                    deckPile[2].CourseToCard(deckPile[0].Position);
                }
                elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;

                if (numShuffles == 1)
                    shuffleEffect.Play();
            }
            else if (deckPile[2].Time >= gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
                deckPile[2].Move(gameTime);
            else if (deckPile[2].Time < gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
            {
                SSEngine.IsACardMoving = false;
                deckPile[2].IsMoving = false;
                elapsedTime = 0.0;
                if (!shuffleMove)
                {
                    if (this.isPlayer1)
                        deckPile[2].Position = deckPile[0].Position + new Vector2(deckPile[0].Texture.Width, 0);
                    else
                        deckPile[2].Position = deckPile[0].Position + new Vector2(-deckPile[0].Texture.Width, 0);
                    shuffleMove = true;
                }
                else
                {
                    deckPile[2].Position = deckPile[0].Position;
                    shuffleMove = false;
                    if (++numShuffles == 3)
                    {
                        numShuffles = 0;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UpdateDeckPile(GameTime gameTime, DiscardPile discardPile)
        {
            if (!SSEngine.IsACardMoving)
            {
                SSEngine.IsACardMoving = true;
                deckPile[2].IsCovered = false;
                deckPile[2].IsMoving = true;
                deckPile[2].CourseToCard(discardPile.GetDiscardSprite());
                elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                drawEffect.Play();
            }
            else if (deckPile[2].Time >= gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
                deckPile[2].Move(gameTime, deckPile[0].Position, discardPile.GetDiscardSprite().Position);
            else if (deckPile[2].Time < gameTime.TotalGameTime.TotalMilliseconds - elapsedTime && deckPile[2].IsMoving)
            {
                discardPile.DiscardList.Add(RetrieveCard(3));
                Deck.RemoveAt(3);
                deckPile[2].Position = deckPile[0].Position;
                deckPile[2].Destination = deckPile[2].CollisionRectangle;
                SSEngine.IsACardMoving = false;
                deckPile[2].IsMoving = false;
                elapsedTime = 0.0;
                setEffect.Play();
                return true;
            }
            return false;
        }

        public bool AddingCardsToHand(GameTime gameTime, DiscardPile discardPile, IHand hand)
        {
            ShuffleDeck(discardPile);
            return UpdateDeckPile(gameTime, hand);
        }

        public void DrawDeckPile(SpriteBatch spriteBatch)
        {
            Texture2D texture = null;

            if (deckPile[2].IsMoving && isPlayer1)
                texture = deckPile[deckPile.Count - 1].Texture;

            deckPile[0].DrawSprite(spriteBatch);

            if (deckPile.Count == 4)
                deckPile[2].DrawSprite(spriteBatch, texture, 0.95f);
            else if (deckPile.Count >= 5)
            {
                deckPile[1].DrawSprite(spriteBatch, null, 0.95f);
                deckPile[2].DrawSprite(spriteBatch, texture, 0.9f);
            }
        }

        public void DrawShuffleDeck(SpriteBatch spriteBatch)
        {
            deckPile[0].DrawSprite(spriteBatch);

            if (!shuffleMove)
            {
                deckPile[1].DrawSprite(spriteBatch, null, 0.9f);
                deckPile[2].DrawSprite(spriteBatch, null, 0.95f);
            }
            else
            {
                deckPile[1].DrawSprite(spriteBatch, null, 0.85f);
                deckPile[2].DrawSprite(spriteBatch, null, 0.9f);
            }
        }
    }
}