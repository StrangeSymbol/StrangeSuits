using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Lidgren.Network;

namespace StrangeSuits
{
    enum GameStage : byte { End, Start, Run, Draw, Drawing, DrawingCard, DeckToDiscard, EightAbility, Shuffle, ReShuffle, }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SSGame : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        Vector2 deckPosition;

        DiscardPile discardPile;
        DeckPile deckPile;
        PlayerHand playerCards1;
        AIHand playerCards2;

        SSButton backButton;
        Button endTurnButton;
        SelectPanel panel;

        SpriteFont font;

        bool done;
        string cardAbility;
        int numSameRank;
        int numDraws;
        int numHaveSet;
        int numTotalSets;
        int selectedIndex;
        int numExtraDraws;
        Suit? changedSuit = null;
        GameStage gameState;
        GameStage nextState;
        
        public SSGame(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            SSEngine.DeckSprites = new Dictionary<string, CardSprite>();
            gameState = GameStage.Shuffle;
            nextState = GameStage.Start;
            numDraws = 20;
            numHaveSet = 0;
            selectedIndex = -1;
            numExtraDraws = 1;
            cardAbility = String.Empty;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            SSEngine.DeckSprites["AceClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/AceClub"), RankValue.Ace, Suit.Club);
            SSEngine.DeckSprites["AceDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/AceDiamond"), RankValue.Ace, Suit.Diamond);
            SSEngine.DeckSprites["AceHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/AceHearts"), RankValue.Ace, Suit.Hearts);
            SSEngine.DeckSprites["AceSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/AceSpades"), RankValue.Ace, Suit.Spades);
            SSEngine.DeckSprites["DeuceClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/DeuceClub"), RankValue.Deuce, Suit.Club);
            SSEngine.DeckSprites["DeuceDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/DeuceDiamond"), RankValue.Deuce, Suit.Diamond);
            SSEngine.DeckSprites["DeuceHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/DeuceHearts"), RankValue.Deuce, Suit.Hearts);
            SSEngine.DeckSprites["DeuceSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/DeuceSpades"), RankValue.Deuce, Suit.Spades);
            SSEngine.DeckSprites["ThreeClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/ThreeClub"), RankValue.Three, Suit.Club);
            SSEngine.DeckSprites["ThreeDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/ThreeDiamond"), RankValue.Three, Suit.Diamond);
            SSEngine.DeckSprites["ThreeHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/ThreeHearts"), RankValue.Three, Suit.Hearts);
            SSEngine.DeckSprites["ThreeSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/ThreeSpades"), RankValue.Three, Suit.Spades);
            SSEngine.DeckSprites["FourClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FourClub"), RankValue.Four, Suit.Club);
            SSEngine.DeckSprites["FourDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FourDiamond"), RankValue.Four, Suit.Diamond);
            SSEngine.DeckSprites["FourHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FourHearts"), RankValue.Four, Suit.Hearts);
            SSEngine.DeckSprites["FourSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FourSpades"), RankValue.Four, Suit.Spades);
            SSEngine.DeckSprites["FiveClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FiveClub"), RankValue.Five, Suit.Club);
            SSEngine.DeckSprites["FiveDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FiveDiamond"), RankValue.Five, Suit.Diamond);
            SSEngine.DeckSprites["FiveHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FiveHearts"), RankValue.Five, Suit.Hearts);
            SSEngine.DeckSprites["FiveSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/FiveSpades"), RankValue.Five, Suit.Spades);
            SSEngine.DeckSprites["SixClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SixClub"), RankValue.Six, Suit.Club);
            SSEngine.DeckSprites["SixDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SixDiamond"), RankValue.Six, Suit.Diamond);
            SSEngine.DeckSprites["SixHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SixHearts"), RankValue.Six, Suit.Hearts);
            SSEngine.DeckSprites["SixSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SixSpades"), RankValue.Six, Suit.Spades);
            SSEngine.DeckSprites["SevenClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SevenClub"), RankValue.Seven, Suit.Club);
            SSEngine.DeckSprites["SevenDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SevenDiamond"), RankValue.Seven, Suit.Diamond);
            SSEngine.DeckSprites["SevenHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SevenHearts"), RankValue.Seven, Suit.Hearts);
            SSEngine.DeckSprites["SevenSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/SevenSpades"), RankValue.Seven, Suit.Spades);
            SSEngine.DeckSprites["EightClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/EightClub"), RankValue.Eight, Suit.Club);
            SSEngine.DeckSprites["EightDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/EightDiamond"), RankValue.Eight, Suit.Diamond);
            SSEngine.DeckSprites["EightHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/EightHearts"), RankValue.Eight, Suit.Hearts);
            SSEngine.DeckSprites["EightSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/EightSpades"), RankValue.Eight, Suit.Spades);
            SSEngine.DeckSprites["NineClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/NineClub"), RankValue.Nine, Suit.Club);
            SSEngine.DeckSprites["NineDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/NineDiamond"), RankValue.Nine, Suit.Diamond);
            SSEngine.DeckSprites["NineHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/NineHearts"), RankValue.Nine, Suit.Hearts);
            SSEngine.DeckSprites["NineSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/NineSpades"), RankValue.Nine, Suit.Spades);
            SSEngine.DeckSprites["TenClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/TenClub"), RankValue.Ten, Suit.Club);
            SSEngine.DeckSprites["TenDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/TenDiamond"), RankValue.Ten, Suit.Diamond);
            SSEngine.DeckSprites["TenHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/TenHearts"), RankValue.Ten, Suit.Hearts);
            SSEngine.DeckSprites["TenSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/TenSpades"), RankValue.Ten, Suit.Spades);
            SSEngine.DeckSprites["JackClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/JackClub"), RankValue.Jack, Suit.Club);
            SSEngine.DeckSprites["JackDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/JackDiamond"), RankValue.Jack, Suit.Diamond);
            SSEngine.DeckSprites["JackHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/JackHearts"), RankValue.Jack, Suit.Hearts);
            SSEngine.DeckSprites["JackSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/JackSpades"), RankValue.Jack, Suit.Spades);
            SSEngine.DeckSprites["QueenClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/QueenClub"), RankValue.Queen, Suit.Club);
            SSEngine.DeckSprites["QueenDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/QueenDiamond"), RankValue.Queen, Suit.Diamond);
            SSEngine.DeckSprites["QueenHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/QueenHearts"), RankValue.Queen, Suit.Hearts);
            SSEngine.DeckSprites["QueenSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/QueenSpades"), RankValue.Queen, Suit.Spades);
            SSEngine.DeckSprites["KingClub"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/KingClub"), RankValue.King, Suit.Club);
            SSEngine.DeckSprites["KingDiamond"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/KingDiamond"), RankValue.King, Suit.Diamond);
            SSEngine.DeckSprites["KingHearts"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/KingHearts"), RankValue.King, Suit.Hearts);
            SSEngine.DeckSprites["KingSpades"] = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/KingSpades"), RankValue.King, Suit.Spades);

            CardSprite cardBack = new CardSprite(Game.Content.Load<Texture2D>(@"DeckOfCards/CardBack"));
            Vector2 discardPosition = new Vector2(3 * SSEngine.QuarterSceenWidth - 2 * cardBack.Texture.Width,
                    2 * SSEngine.QuarterSceenHeight - cardBack.Texture.Height / 2);
            CardSprite discardpileSprite = new CardSprite(Game.Content.Load<Texture2D>("DiscardPile"), discardPosition);
            discardPile = new DiscardPile(discardpileSprite);
            deckPosition = new Vector2(3 * SSEngine.QuarterSceenWidth - discardpileSprite.Texture.Width / 2,
                    2 * SSEngine.QuarterSceenHeight - discardpileSprite.Texture.Height / 2);
            CardSprite deckSprite = new CardSprite(Game.Content.Load<Texture2D>("DeckPile"), deckPosition);
            SoundEffect setSound = Game.Content.Load<SoundEffect>(@"Audio\CardSet");
            deckPile = new DeckPile(deckSprite, cardBack, Game.Content.Load<SoundEffect>(@"Audio\CardShuffle"),
                Game.Content.Load<SoundEffect>(@"Audio\DrawCard"), setSound);
            deckPile.ShuffleDeck();
            playerCards1 = new PlayerHand(Game.Content.Load<Texture2D>(@"Hand\ArrowButtonL"),
                Game.Content.Load<Texture2D>(@"Hand\ArrowButtonCoverL"), Game.Content.Load<Texture2D>(@"Hand\ArrowButtonR"),
                Game.Content.Load<Texture2D>(@"Hand\ArrowButtonCoverR"), cardBack.Texture, setSound);
            playerCards2 = new AIHand(cardBack.Texture, setSound);
            font = Game.Content.Load<SpriteFont>(@"Fonts\GameFont");
            Texture2D overlay = Game.Content.Load<Texture2D>(@"Buttons\backBtnCover");
            backButton = new SSButton(Game.Content.Load<Texture2D>(@"Buttons\SuitBackButton"), new Vector2(20f, 20f), overlay, MenuStage.Play);
            overlay = Game.Content.Load<Texture2D>(@"Buttons\ButtonCover");
            endTurnButton = new Button(Game.Content.Load<Texture2D>(@"Buttons\SuitEndTurnButton"),
                new Vector2(3 * SSEngine.QuarterSceenWidth + discardpileSprite.Texture.Width,
                2 * SSEngine.QuarterSceenHeight - overlay.Height / 2), overlay);
            panel = new SelectPanel(Game.Content, "Change To");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            if (gameState != GameStage.Shuffle && deckPile.ShuffleDeck(discardPile))
            {
                nextState = gameState;
                gameState = GameStage.ReShuffle;
            }

            switch (gameState)
            {
                case GameStage.ReShuffle:
                case GameStage.Shuffle:
                    if (deckPile.UpdateShuffleDeck(gameTime))
                    {
                        gameState = nextState;
                    }
                    break;
                case GameStage.Start:
                    if (numDraws > 0)
                    {
                        if (done)
                        {
                            done = false;
                            SSEngine.Player1First = !SSEngine.Player1First;
                            numDraws--;
                        }
                        else
                            gameState = GameStage.Drawing;
                    }
                    else
                    {
                        AIOpponent.Init(SSEngine.VersionStage); 
                        gameState = GameStage.DeckToDiscard;
                    }
                    break;
                case GameStage.DeckToDiscard:
                    bool finished;
                    finished = deckPile.UpdateDeckPile(gameTime, discardPile);
                    if (finished)
                    {
                        changedSuit = discardPile[0].CardSuit;
                        gameState = GameStage.Run;
                        if (discardPile[0].Rank == RankValue.Three || discardPile[0].Rank == RankValue.Four
                            || discardPile[0].Rank == RankValue.Deuce || discardPile[0] == SSEngine.DeckSprites["QueenSpades"]
                            || discardPile[0].Rank == RankValue.Eight)
                        {
                            SSEngine.Player1First = !SSEngine.Player1First;
                            cardAblities(discardPile[0]);
                            numExtraDraws = numTotalSets;
                        }
                        else
                        {
                            numTotalSets = 1;
                            cardAblities(discardPile[0]);
                        }
                    }
                    break;
                case GameStage.Drawing:
                    if (SSEngine.Player1First == SSEngine.Player1)
                        done = deckPile.AddingCardsToHand(gameTime, discardPile, playerCards1);
                    else
                        done = deckPile.AddingCardsToHand(gameTime, discardPile, playerCards2);
                    if (done)
                        gameState = GameStage.Start;
                    break;
                case GameStage.DrawingCard:
                    if (deckPile.AddingCardsToHand(gameTime, discardPile, playerCards2))
                        gameState = GameStage.Run;
                    break;
                case GameStage.Draw:
                    if (SSEngine.Player1First == SSEngine.Player1)
                        done = deckPile.AddingCardsToHand(gameTime, discardPile, playerCards2);
                    else
                        done = deckPile.AddingCardsToHand(gameTime, discardPile, playerCards1);
                    if (done && --numDraws == 0)
                        gameState = GameStage.Run;
                    break;
                case GameStage.Run:
                    if ((playerCards1.Count == 0 || playerCards2.Count == 0) && numTotalSets == 0)
                    {
                        gameState = GameStage.End;
                        break;
                    }
      
                    if (numDraws == 0 && numTotalSets == 0)
                    {
                        numTotalSets = 1;
                        numExtraDraws = 1;
                        numHaveSet = 0;
                        cardAbility = String.Empty;
                        SSEngine.Player1First = !SSEngine.Player1First;
                        if (SSEngine.Player1SetThree == SSEngine.Player1First)
                            SSEngine.Player1SetThree = null;
                    }
                    bool activate = false;
                    if (SSEngine.Player1First == SSEngine.Player1)
                        activate = playerCards1.UpdateHand(gameTime, mouse, discardPile, changedSuit, ref cardAbility);
                    else if (SSEngine.VersionStage != MenuStage.LAN && SSEngine.VersionStage != MenuStage.Online)
                    {
                        if (selectedIndex == -1)
                        {
                            selectedIndex = AIOpponent.Opponent(playerCards2, discardPile, changedSuit);
                            if (numExtraDraws > 0 && selectedIndex == -1)
                                gameState = GameStage.DrawingCard;
                            if (selectedIndex == -1 && numExtraDraws == 0)
                                numTotalSets = 0;
                        }
                        if (selectedIndex != -1)
                            activate = playerCards2.UpdateHand(gameTime, selectedIndex, discardPile);
                        else if (numExtraDraws > 0)
                            numExtraDraws--;
                    }
                    else if (selectedIndex != -1)
                    {
                        activate = playerCards2.UpdateHand(gameTime, selectedIndex, discardPile);
                    }

                    if (activate)
                    {
                        if (numTotalSets > 0)
                            numTotalSets--;
                        numHaveSet++;
                        if (discardPile.Count > 1 && discardPile[discardPile.Count - 2].Rank == RankValue.Three 
                            && numExtraDraws == 1)
                            numExtraDraws++;
                        if (numExtraDraws > 0)
                            numExtraDraws--;
                        selectedIndex = -1;
                        ability(discardPile[discardPile.Count - 1]);
                    }
                    if (numExtraDraws > 0)
                    {
                        activate = false;
                        if (SSEngine.Player1First == SSEngine.Player1)
                        {
                            deckPile.ShuffleDeck(discardPile);
                            activate = deckPile.UpdateDeckPile(gameTime, mouse, playerCards1);
                        }
                        if (activate)
                            numExtraDraws--;
                    }
                    else
                    {
                        finished = endTurnButton.UpdateButton(mouse, gameTime);
                        if (finished)
                        {
                            numTotalSets = 0;
                            if (SSEngine.VersionStage == MenuStage.LAN)
                                SSEngine.LANClient.Send((byte)NetGame.EndTurn);
                            else if (SSEngine.VersionStage == MenuStage.Online)
                            {
                                NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                                msg.Write((byte)NetGame.EndTurn);
                                SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                            }
                        }
                    }
                    finished = backButton.UpdateButton(mouse, gameTime);
                    if (finished)
                    {
                        if (SSEngine.VersionStage == MenuStage.LAN)
                        {
                            SSEngine.LANClient.Send((byte)NetGame.Back);
                            SSEngine.LANClient = null;
                        }
                        else if (SSEngine.VersionStage == MenuStage.Online)
                        {
                            NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                            msg.Write((byte)NetGame.Back);
                            SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                            System.Threading.Thread.Sleep(1000);
                            SSEngine.Peer.Shutdown("Lost Connection.");
                            SSEngine.Peer = null;
                        }
                        SSEngine.VersionStage = MenuStage.Play;
                    }
                    break;
                case GameStage.EightAbility:
                    if (!SSEngine.Player1First && SSEngine.VersionStage != MenuStage.LAN && SSEngine.VersionStage != MenuStage.Online)
                    {
                        switch (SSEngine.VersionStage)
                        {
                            case MenuStage.VeryEasy:
                                AIOpponent.EightAbilityVE(ref changedSuit); break;
                            case MenuStage.Easy:
                            case MenuStage.Medium:
                                AIOpponent.EightAbilityEM(ref changedSuit, playerCards2); break;
                            case MenuStage.Hard:
                            case MenuStage.VeryHard:
                                AIOpponent.EightAbilityHVH(ref changedSuit, playerCards2); break;
                        }
                        if (numSameRank < 3)
                            gameState = GameStage.Run;
                        else
                            gameState = GameStage.Draw; 
                    }
                    else
                        eightAbilityP(gameTime, mouse);
                        break;
                case GameStage.End:
                    // Implement end game mechanics.
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            SSEngine.VersionStage = MenuStage.Play;
                            SSEngine.Disconnected = false;
                            SSEngine.LANClient = null;
                            SSEngine.Peer = null;
                        }
                        cardAbility = String.Empty;
                    break;
            }

            if (SSEngine.VersionStage == MenuStage.LAN && SSEngine.LANClient.HasMessage() &&
                !(deckPile.Deck.Count != 3 && (NetGame)SSEngine.LANClient.PeekMessage().Bytes[0] == NetGame.Shuffle) &&
                gameState != GameStage.ReShuffle)
            {
                Message msg = SSEngine.LANClient.ReceiveMessage();
                switch ((NetGame)msg.Bytes[0])
                {
                    case NetGame.SuitChanged:
                        if (msg.Bytes.Length == 3)
                            gameState = (GameStage)msg.Bytes[2];
                        else
                            gameState = GameStage.Run;
                        changedSuit = (Suit)msg.Bytes[1];
                        break;
                    case NetGame.Draw:
                        gameState = GameStage.DrawingCard;
                        break;
                    case NetGame.EndTurn:
                        numTotalSets = 0;
                        break;
                    case NetGame.Back:
                        gameState = GameStage.End;
                        SSEngine.Disconnected = true;
                        break;
                    case NetGame.Shuffle:
                        if (discardPile.Count > 1)
                            discardPile.DiscardList.RemoveRange(0, discardPile.Count - 1);
                        for (int j = 1; j < msg.Bytes.Length - 1; j += 2)
                            deckPile.Deck.Add(SSEngine.DeckSprites[(RankValue)msg.Bytes[j] + "" + (Suit)msg.Bytes[j + 1]]);
                        if (gameState != GameStage.Shuffle)
                        {
                            nextState = gameState;
                            gameState = GameStage.ReShuffle;
                        }
                        
                        break;
                    case NetGame.Set:
                        selectedIndex = msg.Bytes[1];
                        break;
                    default:
                        break;
                }
            }
            else if (SSEngine.VersionStage == MenuStage.Online)
            {
                NetIncomingMessage inc = SSEngine.Peer.PeekMessage();

                if (inc != null && !(deckPile.Deck.Count != 3 && (NetGame)inc.PeekByte() == NetGame.Shuffle) &&
                    gameState != GameStage.ReShuffle)
                {
                    inc = SSEngine.Peer.ReadMessage();
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            switch ((NetGame)inc.ReadByte())
                            {
                                case NetGame.SuitChanged:
                                    changedSuit = (Suit)inc.ReadByte();
                                    if (inc.LengthBytes == 1)
                                        gameState = (GameStage)inc.ReadByte();
                                    else
                                        gameState = GameStage.Run;
                                    break;
                                case NetGame.Draw:
                                    gameState = GameStage.DrawingCard;
                                    break;
                                case NetGame.EndTurn:
                                    numTotalSets = 0;
                                    break;
                                case NetGame.Back:
                                    gameState = GameStage.End;
                                    SSEngine.Disconnected = true;
                                    break;
                                case NetGame.Shuffle:
                                    if (discardPile.Count > 1)
                                        discardPile.DiscardList.RemoveRange(0, discardPile.Count - 1);
                                    byte numBytes = inc.ReadByte();
                                    byte[] data = inc.ReadBytes(numBytes);
                                    for (int j = 0; j < data.Length; j += 2)
                                        deckPile.Deck.Add(SSEngine.DeckSprites[(RankValue)data[j] + "" + (Suit)data[j + 1]]);
                                    if (gameState != GameStage.Shuffle)
                                    {
                                        nextState = gameState;
                                        gameState = GameStage.ReShuffle;
                                    }
                                    break;
                                case NetGame.Set:
                                    selectedIndex = inc.ReadByte();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            switch (inc.SenderConnection.Status)
                            {
                                case NetConnectionStatus.Disconnected:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            base.Update(gameTime);
        }

        private void eightAbilityP(GameTime gameTime, MouseState mouse)
        {
            Suit? suit = panel.UpdatePanel(gameTime);

            if (suit != null)
            {
                changedSuit = suit;
                if (numSameRank < 3)
                    gameState = GameStage.Run;
                else
                    gameState = GameStage.Draw;
                if (SSEngine.VersionStage == MenuStage.LAN)
                {
                    if (numSameRank < 3)
                        SSEngine.LANClient.Send((byte)NetGame.SuitChanged, (byte)suit.Value);
                    else
                        SSEngine.LANClient.Send((byte)NetGame.SuitChanged, (byte)suit.Value, (byte)GameStage.Draw);
                }
                else if (SSEngine.VersionStage == MenuStage.Online)
                {
                    if (numSameRank < 3)
                    {
                        NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                        msg.Write((byte)NetGame.SuitChanged);
                        msg.Write((byte)suit.Value);
                        SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                    }
                    else
                    {
                        NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                        msg.Write((byte)NetGame.SuitChanged);
                        msg.Write((byte)suit.Value);
                        msg.Write((byte)GameStage.Draw);
                        SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            backButton.Draw(spriteBatch);
            if (numExtraDraws == 0 && SSEngine.Player1First == SSEngine.Player1 && gameState == GameStage.Run)
                endTurnButton.Draw(spriteBatch);
            string text = "Suit: " + changedSuit.ToString();
            Vector2 position = new Vector2(SSEngine.QuarterSceenWidth / 2 - font.MeasureString(text).X / 2,
                    2 * SSEngine.QuarterSceenHeight - font.MeasureString(text).Y / 2);
            spriteBatch.DrawString(font, text, position, Color.Black);
            text = "Turn: " + (SSEngine.Player1First ? "Player 1" : "Player 2");
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y + font.MeasureString(text).Y), Color.Black);
            text = "Sets Left: " + numTotalSets;
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y + 2 * font.MeasureString(text).Y), Color.Black);
            if (cardAbility != String.Empty)
                spriteBatch.DrawString(font, cardAbility, new Vector2(position.X + 7 * font.MeasureString(text).X / 5,
                    position.Y), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            discardPile.DrawDiscardPile(spriteBatch);
            if (gameState != GameStage.Shuffle && gameState != GameStage.ReShuffle)
            {
                deckPile.DrawDeckPile(spriteBatch);
                text = (deckPile.Deck.Count - 3).ToString();
                spriteBatch.DrawString(font, text,
                    new Vector2(deckPile.Deck[0].Position.X + deckPile.Deck[0].Texture.Width / 2 - font.MeasureString(text).X / 2,
                    deckPile.Deck[0].Position.Y + deckPile.Deck[0].Texture.Height / 2 - font.MeasureString(text).Y / 2),
                    Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.75f);
            }
            else
                deckPile.DrawShuffleDeck(spriteBatch);
            playerCards1.DrawHand(spriteBatch);
            playerCards2.DrawHand(spriteBatch);
            if (gameState == GameStage.EightAbility && SSEngine.Player1First == SSEngine.Player1)
                panel.DrawPanel(spriteBatch); 
            else if (gameState == GameStage.End)
            {
                text = "The Winner is: " + (playerCards1.Count <= 0 || SSEngine.Disconnected ? (SSEngine.Player1 == true
                    ? "Player1" : "Player2") : (SSEngine.Player1 == true ? "Player2" : "Player1")) + "\n(Press Enter)";
                spriteBatch.DrawString(font, text, new Vector2(2 * SSEngine.QuarterSceenWidth -
                    2 * font.MeasureString(text).X / 3, 2 * SSEngine.QuarterSceenHeight - font.MeasureString(text).Y / 2),
                    Color.Black);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        private void cardAblities(CardSprite cardPrint)
        {
            if (cardPrint.Rank == RankValue.Ace)
            {
                rankRules();
                if (cardPrint.CardSuit == Suit.Club)
                    changedSuit = Suit.Spades;
                else if (cardPrint.CardSuit == Suit.Spades)
                    changedSuit = Suit.Club;
                else if (cardPrint.CardSuit == Suit.Hearts)
                    changedSuit = Suit.Diamond;
                else if (cardPrint.CardSuit == Suit.Diamond)
                    changedSuit = Suit.Hearts;
            }
            else if (cardPrint.Rank == RankValue.Deuce)
            {
                if (discardPile.DiscardList.Count > 1)
                {
                    if (discardPile[discardPile.Count - 2] == SSEngine.DeckSprites["QueenSpades"] &&
                        cardPrint.CardSuit == Suit.Spades)
                        rankSpade2Rules();
                    else if (discardPile[discardPile.Count - 2].Rank != RankValue.Three)
                        rank2Rules();
                    else if (discardPile[discardPile.Count - 2].Rank == RankValue.Three
                        && SSEngine.Player1First != SSEngine.Player1SetThree)
                    {
                        rank2Rules();
                        SSEngine.Player1SetThree = null;
                    }

                    // Your deuce card ablity is negated if none of these statements are true.
                }
                else
                    rank2Rules();
            }

            else if (cardPrint.Rank == RankValue.Three)
            {
                if (discardPile.Count > 3 && discardPile[discardPile.Count - 3].Rank == RankValue.Three &&
                        discardPile[discardPile.Count - 4].Rank == RankValue.Three)
                {
                    numTotalSets++;
                    numExtraDraws++;
                }
                else if (discardPile.Count > 2 && discardPile[discardPile.Count - 2].Rank == RankValue.Three &&
                        discardPile[discardPile.Count - 3].Rank == RankValue.Three)
                {
                    numTotalSets++;
                    numExtraDraws++;
                }
                else if (discardPile.Count > 1 && discardPile[discardPile.Count - 1].Rank == RankValue.Three &&
                        discardPile[discardPile.Count - 2].Rank == RankValue.Three)
                {
                    numTotalSets++;
                    numExtraDraws++;
                }
                else
                {
                    numTotalSets += 2;
                    numExtraDraws += 2;
                }
                SSEngine.Player1SetThree = SSEngine.Player1First;
            }
            else if (cardPrint.Rank == RankValue.Four)
            {
                if (discardPile.Count > 1)
                {
                    if (discardPile[discardPile.Count - 2].Rank != RankValue.Three)
                    {
                        numTotalSets++;
                        numExtraDraws++;
                    }
                    else if (discardPile[discardPile.Count - 2].Rank == RankValue.Three
                        && SSEngine.Player1First != SSEngine.Player1SetThree)
                    {
                        numTotalSets++;
                        numExtraDraws++;
                        SSEngine.Player1SetThree = null;
                    }
                    // If none of these statements are true Card 4 ability negated.
                }
                else
                {
                    numTotalSets++;
                    numExtraDraws++;
                }
            }
            else if (cardPrint.Rank == RankValue.Five)
                rankRules();
            else if (cardPrint.Rank == RankValue.Six)
                rankRules();
            else if (cardPrint.Rank == RankValue.Seven)
                rankRules();
            else if (cardPrint.Rank == RankValue.Eight)
            {
                rankRules();
                gameState = GameStage.EightAbility;
            }
            else if (cardPrint.Rank == RankValue.Nine)
                rankRules();
            else if (cardPrint.Rank == RankValue.Ten)
                rankRules();
            else if (cardPrint.Rank == RankValue.Jack)
                rankRules();
            else if (cardPrint.Rank == RankValue.Queen)
            {
                rankRules();
                bool queenNeg = false;
                if (discardPile.Count > 1 && discardPile[discardPile.Count - 2].Rank == RankValue.Three &&
                        cardPrint.CardSuit == Suit.Spades && SSEngine.Player1First == SSEngine.Player1SetThree)
                {
                    // The Queen Of Spades is negated.
                    queenNeg = true;
                    SSEngine.Player1SetThree = null;
                }
                if (cardPrint.CardSuit == Suit.Spades && !queenNeg)
                {
                    numDraws = 5;
                    gameState = GameStage.Draw;
                }
            }
            else if (cardPrint.Rank == RankValue.King)
                rankRules();
        }

        private void ability(CardSprite card)
        {
            if (numHaveSet == 1 && card.Rank == RankValue.Three &&
                    discardPile[discardPile.Count - 2].Rank == RankValue.Three)
            {
                changedSuit = card.CardSuit;
                cardAblities(card);
                numTotalSets++;
                numExtraDraws++;
                return;
            }

            if (card.Rank == discardPile[discardPile.Count - 2].Rank)
            {
                if (discardPile.Count > 3 &&
                    card.Rank == discardPile[discardPile.Count - 4].Rank &&
                    card.Rank == discardPile[discardPile.Count - 3].Rank)
                    numSameRank = 3;
                else if (discardPile.Count > 2 &&
                    card.Rank == discardPile[discardPile.Count - 3].Rank)
                    numSameRank = 2;
                else
                    numSameRank = 1;

                changedSuit = card.CardSuit;
                cardAblities(card);
            }

            else if (card.CardSuit == changedSuit ||
                card.Rank == RankValue.Eight)
            {
                numSameRank = 0;
                cardAblities(card);
            }
        }
       
        private void rankSpade2Rules()
        {
            numDraws = 7;
            gameState = GameStage.Draw;
        }

        private void rankRules()
        {
            numSameRank++;
            if (numSameRank == 3)
            {
                numDraws = 2;
                gameState = GameStage.Draw;
            }
            else if (numSameRank == 4)
            {
                numDraws = 4;
                gameState = GameStage.Draw;
            }
        }

        private void rank2Rules()
        {
            numSameRank++;
            numDraws = 2 * numSameRank;
            gameState = GameStage.Draw;
        }
    }
}