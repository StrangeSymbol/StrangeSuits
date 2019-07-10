using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;

namespace StrangeSuits
{
    enum MenuStage : byte { VeryEasy = 1, Easy, Medium, Hard, VeryHard, MainMenu, Play, GameRules, RockPaperScissors,
    LAN, Settings, RockPaperScissorsPlaced, RockPaperScissorsWait, InGame, Connected, Connect, SplashScreen, Online,
    TwoPlayer, WaitToPlay,}

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Menu : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        SSGame game;

        Texture2D splashScreen;
        Texture2D menu;
        Texture2D title;
        Texture2D developed;
        Texture2D rules;

        SpriteFont spriteFont;

        MenuStage menuStage;
        MenuStage selectedStage;

        SSButton[] playButtons;
        SSButton[] mainButtons;
        SSButton[] rocPapSciBtns;
        SSButton[] gameRulesBtns;
        SSButton[] settingsBtns;
        SSButton[] twoPlayerBtns;
        SSButton backButton;
        double elapsedTime;
        readonly float quarterScreenW;
        readonly float quarterScreenH;
        int playerOne = -1;
        int playerTwo = -1;

        public Menu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 675;
            Window.Title = "Strange Suits";
            this.IsMouseVisible = true;
            
            quarterScreenW = graphics.PreferredBackBufferWidth/4.0f;
            quarterScreenH = graphics.PreferredBackBufferHeight/4.0f;
            SSEngine.QuarterSceenWidth = quarterScreenW;
            SSEngine.QuarterSceenHeight = quarterScreenH;
            menuStage = MenuStage.SplashScreen;
            mainButtons = new SSButton[3];
            playButtons = new SSButton[7];
            rocPapSciBtns = new SSButton[4];
            gameRulesBtns = new SSButton[1];
            settingsBtns = new SSButton[1];
            twoPlayerBtns = new SSButton[3];
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //transition = new Transition(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, SSButton.ButtonWait);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (DateTime.Now.Month == 3) // Check that it is March.
                splashScreen = Content.Load<Texture2D>(@"SplashScreens\SplashScreenMarch");
            else if (DateTime.Now.Month == 10) // Check that it is October.
                splashScreen = Content.Load<Texture2D>(@"SplashScreens\SplashScreenOctober");
            else if (DateTime.Now.Month == 12) // Check that it is December.
                splashScreen = Content.Load<Texture2D>(@"SplashScreens\SplashScreenDecember");
            else
                splashScreen = Content.Load<Texture2D>(@"SplashScreens\SplashScreen");
            menu = Content.Load<Texture2D>(@"Menu\mainmenu");
            title = Content.Load<Texture2D>(@"Menu\Title");
            developed = Content.Load<Texture2D>(@"Menu\BlendTextDeveStrange");
            rules = Content.Load<Texture2D>(@"Menu\Rules");

            spriteFont = Content.Load<SpriteFont>(@"Fonts\StrangeFont");

            loadMenuButtons();
        }

        private void loadMenuButtons()
        {
            Texture2D overlay = Content.Load<Texture2D>(@"Buttons\ButtonCover");
            loadMainButtons(overlay);
            loadPlayButtons(overlay);
            loadTwoPlayerButtons(overlay);
            overlay = Content.Load<Texture2D>(@"Buttons\RocPapSciButton");
            loadRocPapSciButtons(overlay);
            overlay = Content.Load<Texture2D>(@"Buttons\backBtnCover");
            backButton = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitBackButton"), new Vector2(20f, 20f), overlay, MenuStage.MainMenu);
            playButtons[6] = backButton;
            gameRulesBtns[0] = backButton;
            settingsBtns[0] = backButton;
            backButton = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitBackButton"), new Vector2(20f, 20f), overlay, MenuStage.Play);
            rocPapSciBtns[3] = backButton;
            twoPlayerBtns[2] = backButton;
        }

        private void loadMainButtons(Texture2D overlay)
        {
            float btnPosX = 2 * quarterScreenW - overlay.Width / 2f;
            float btnSpace = (quarterScreenH * 4 - quarterScreenH * 4 * .26f - 3f * overlay.Height) / 3;
            mainButtons[0] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitPlayButton"),
                new Vector2(btnPosX, quarterScreenH), overlay, MenuStage.Play);
            mainButtons[1] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitRulesButton"),
                new Vector2(btnPosX, mainButtons[0].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.GameRules);
            mainButtons[2] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitSettingsButton"),
                new Vector2(btnPosX, mainButtons[1].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.Settings);
        }

        private void loadPlayButtons(Texture2D overlay)
        {
            float btnPosX = 2 * quarterScreenW - overlay.Width / 2f;
            float btnSpace = (quarterScreenH * 4 - quarterScreenH * 4 * .55f - 3f * overlay.Height) / 3;
            playButtons[0] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitVeryEasyButton"),
                new Vector2(btnPosX, quarterScreenH * 4 * .1f), overlay, MenuStage.VeryEasy);
            playButtons[1] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitEasyButton"),
                new Vector2(btnPosX, playButtons[0].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.Easy);
            playButtons[2] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitMediumButton"),
                new Vector2(btnPosX, playButtons[1].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.Medium);
            playButtons[3] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitHardButton"),
                new Vector2(btnPosX, playButtons[2].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.Hard);
            playButtons[4] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitVeryHardButton"),
                new Vector2(btnPosX, playButtons[3].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.VeryHard);
            playButtons[5] = new SSButton(Content.Load<Texture2D>(@"Buttons\Suit2PlayerButton"),
                new Vector2(btnPosX, playButtons[4].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.TwoPlayer);
        }

        private void loadTwoPlayerButtons(Texture2D overlay)
        {
            float btnPosX = 2 * quarterScreenW - overlay.Width / 2f;
            float btnSpace = (quarterScreenH * 4 - quarterScreenH * 4 * .26f - 3f * overlay.Height) / 3;
            twoPlayerBtns[0] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitOnlineButton"),
                new Vector2(btnPosX, quarterScreenH), overlay, MenuStage.Online);
            twoPlayerBtns[1] = new SSButton(Content.Load<Texture2D>(@"Buttons\SuitLANButton"),
                new Vector2(btnPosX, twoPlayerBtns[0].Position.Y + overlay.Height + btnSpace), overlay, MenuStage.LAN);
        }

        private void loadRocPapSciButtons(Texture2D overlay)
        {
            float btnPosY = 2 * quarterScreenH - overlay.Height / 2;
            float btnSpace = (quarterScreenW * 4 - quarterScreenW * 4 * .25f - 3f * overlay.Width) / 3;
            rocPapSciBtns[0] = new SSButton(Content.Load<Texture2D>(@"Buttons\RockButton"),
                new Vector2(quarterScreenW * 4 * .15f, btnPosY), overlay, MenuStage.RockPaperScissorsPlaced);
            rocPapSciBtns[1] = new SSButton(Content.Load<Texture2D>(@"Buttons\PaperButton"),
                new Vector2(rocPapSciBtns[0].Position.X + overlay.Width + btnSpace, btnPosY), overlay, MenuStage.RockPaperScissorsPlaced);
            rocPapSciBtns[2] = new SSButton(Content.Load<Texture2D>(@"Buttons\ScissorsButton"),
               new Vector2(rocPapSciBtns[1].Position.X + overlay.Width + btnSpace, btnPosY), overlay, MenuStage.RockPaperScissorsPlaced);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            MouseState mouse = Mouse.GetState();
            switch (menuStage)
            {
                case MenuStage.SplashScreen:
                    if (gameTime.TotalGameTime.Seconds > 2)
                        menuStage = MenuStage.MainMenu;
                    break;
                case MenuStage.MainMenu:
                    UpdateButtons(mainButtons, gameTime, mouse); break;
                case MenuStage.Play:
                    UpdateButtons(playButtons, gameTime, mouse);
                    for (int i = 0; i < playButtons.Length - 1; i++)
                    {
                        if (playButtons[i].IsClicked && playButtons[i].Stage != MenuStage.TwoPlayer)
                        {
                            SSEngine.VersionStage = playButtons[i].Stage;
                            selectedStage = MenuStage.RockPaperScissors;
                            SSEngine.Player1 = true;
                        }
                    }
                    break;
                case MenuStage.TwoPlayer:
                    UpdateButtons(twoPlayerBtns, gameTime, mouse);
                    if (twoPlayerBtns[0].IsClicked) // Online Button.
                    {
                        SSEngine.Player1 = true;
                        SSEngine.VersionStage = twoPlayerBtns[0].Stage;
                        selectedStage = MenuStage.Connect;
                        if (SSEngine.Peer == null)
                        {
                            Client.MainClient();
                        }
                    }
                    else if (twoPlayerBtns[1].IsClicked) // LAN Button.
                    {
                        SSEngine.Player1 = true;
                        SSEngine.VersionStage = twoPlayerBtns[1].Stage;
                        selectedStage = MenuStage.Connect;
                        if (SSEngine.LANClient == null)
                        {
                            SSEngine.LANClient = new BroadcastClient();
                            if (SSEngine.LANClient.IsListening)
                                SSEngine.LANClient.Send((byte)MenuStage.Connect);
                        }
                    }
                    break;
                case MenuStage.Connect:
                    if (SSEngine.VersionStage == MenuStage.LAN && SSEngine.LANClient.HasMessage())
                    {
                        Message msg = SSEngine.LANClient.ReceiveMessage();
                        if (msg.Bytes[0] == (byte)MenuStage.Connect)
                        {
                            SSEngine.Player1 = true;
                            SSEngine.LANClient.Send((byte)MenuStage.Connected);
                            menuStage = MenuStage.Connected;
                        }
                        else if (msg.Bytes[0] == (byte)MenuStage.Connected)
                        {
                            SSEngine.LANClient.Send((byte)MenuStage.Connected);
                            SSEngine.Player1 = false;
                            menuStage = MenuStage.RockPaperScissors;
                        }
                    }
                    else if (SSEngine.VersionStage == MenuStage.Online && SSEngine.IsHost.HasValue)
                    {
                        if (Server.ConForm.cbClient.Checked && Server.ConForm.cbHost.Checked) // Is a host or client.
                        {
                            SSEngine.Player1 = SSEngine.IsHost.Value;
                            menuStage = MenuStage.RockPaperScissors;
                            System.Threading.Thread.Sleep(1000);
                            Server.ConForm.Close();
                        }
                    }
                    if (SSEngine.VersionStage == MenuStage.LAN && backButton.UpdateButton(mouse, gameTime))
                        menuStage = MenuStage.Play;
                    break;
                case MenuStage.Connected:
                    if (SSEngine.LANClient.HasMessage())
                    {
                        Message msg = SSEngine.LANClient.ReceiveMessage();
                        if (msg.Bytes[0] == (byte)MenuStage.Connected)
                            menuStage = MenuStage.RockPaperScissors;
                    }
                    break;
                case MenuStage.RockPaperScissors:
                    UpdateButtons(rocPapSciBtns, gameTime, mouse);
                    UpdateRockPaperScissors();
                    break;
                case MenuStage.RockPaperScissorsWait:
                    if (SSEngine.VersionStage == MenuStage.LAN && SSEngine.LANClient.HasMessage())
                    {
                        Message msg = SSEngine.LANClient.ReceiveMessage();
                        if (SSEngine.Player1.Value)
                            playerTwo = msg.Bytes[0];
                        else
                            playerOne = msg.Bytes[0];
                        RockPaperScissorsGame(playerOne, playerTwo);
                        menuStage = MenuStage.RockPaperScissorsPlaced;
                    }
                    else if (SSEngine.VersionStage == MenuStage.Online && SSEngine.Peer.PeekMessage() != null)
                    {
                        NetIncomingMessage msg = SSEngine.Peer.ReadMessage();
                        if (SSEngine.Player1.Value)
                            playerTwo = msg.ReadByte();
                        else
                            playerOne = msg.ReadByte();
                        RockPaperScissorsGame(playerOne, playerTwo);
                        menuStage = MenuStage.RockPaperScissorsPlaced;
                    }
                    if (backButton.UpdateButton(mouse, gameTime))
                        menuStage = MenuStage.Play;
                    break;
                case MenuStage.RockPaperScissorsPlaced:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        if (playerOne != playerTwo)
                        {
                            if (SSEngine.VersionStage == MenuStage.LAN && !SSEngine.LANClient.HasMessage())
                            {
                                menuStage = MenuStage.WaitToPlay;
                                SSEngine.LANClient.Send((byte)menuStage);
                            }
                            else if (SSEngine.VersionStage == MenuStage.LAN && SSEngine.LANClient.HasMessage())
                            {
                                Message msg = SSEngine.LANClient.ReceiveMessage();
                                if ((MenuStage)msg.Bytes[0] == MenuStage.WaitToPlay)
                                {
                                    SSEngine.LANClient.Send((byte)MenuStage.WaitToPlay);
                                    menuStage = MenuStage.InGame;
                                    game = new SSGame(this);
                                    Components.Add(game);
                                    game.Enabled = true;
                                    game.Visible = true;
                                } 
                            }
                            else if (SSEngine.VersionStage == MenuStage.Online && SSEngine.Peer.PeekMessage() == null)
                            {
                                menuStage = MenuStage.WaitToPlay;
                                NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                                msg.Write((byte)MenuStage.WaitToPlay);
                                SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                            }
                            else if (SSEngine.VersionStage == MenuStage.Online && SSEngine.Peer.PeekMessage() != null)
                            {
                                NetIncomingMessage inc = SSEngine.Peer.ReadMessage();
                                if ((MenuStage)inc.ReadByte() == MenuStage.WaitToPlay)
                                {
                                    NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                                    msg.Write((byte)MenuStage.WaitToPlay);
                                    SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                                    menuStage = MenuStage.InGame;
                                    game = new SSGame(this);
                                    Components.Add(game);
                                    game.Enabled = true;
                                    game.Visible = true;
                                }
                            }

                            if (SSEngine.VersionStage != MenuStage.LAN && SSEngine.VersionStage != MenuStage.Online)
                            {
                                menuStage = MenuStage.InGame;
                                game = new SSGame(this);
                                Components.Add(game);
                                game.Enabled = true;
                                game.Visible = true;
                            }
                        }
                        else
                            menuStage = MenuStage.RockPaperScissors;
                        playerOne = -1;
                        playerTwo = -1;
                    }
                    break;
                case MenuStage.WaitToPlay:
                    if (SSEngine.VersionStage == MenuStage.LAN && SSEngine.LANClient.HasMessage())
                    {
                        Message msg = SSEngine.LANClient.ReceiveMessage();
                        if ((MenuStage)msg.Bytes[0] == MenuStage.WaitToPlay)
                        {
                            menuStage = MenuStage.InGame;
                            game = new SSGame(this);
                            Components.Add(game);
                            game.Enabled = true;
                            game.Visible = true;
                        }
                    }
                    else if (SSEngine.VersionStage == MenuStage.Online && SSEngine.Peer.PeekMessage() != null)
                    {
                        NetIncomingMessage inc = SSEngine.Peer.ReadMessage();
                        if ((MenuStage)inc.ReadByte() == MenuStage.WaitToPlay)
                        {
                            NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                            msg.Write((byte)MenuStage.WaitToPlay);
                            SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                            menuStage = MenuStage.InGame;
                            game = new SSGame(this);
                            Components.Add(game);
                            game.Enabled = true;
                            game.Visible = true;
                        }
                    }
                    break;
                case MenuStage.InGame:
                    if (SSEngine.VersionStage == MenuStage.Play)
                    {
                        Components.Remove(game);
                        menuStage = MenuStage.Play;
                    }
                    break;
                case MenuStage.GameRules:
                    UpdateButtons(gameRulesBtns, gameTime, mouse); break;
                case MenuStage.Settings:
                    UpdateButtons(settingsBtns, gameTime, mouse); break;
            }

            base.Update(gameTime);
        }

        private void UpdateRockPaperScissors()
        {
            int player1Shot = -1;
            int player2Shot = -1;
            bool clicked = false;

            for (int i = 0; i < rocPapSciBtns.Length - 1; i++)
            {
                if (rocPapSciBtns[i].IsClicked)
                {
                    if (SSEngine.Player1.Value)
                        player1Shot = i;
                    else
                        player2Shot = i;
                    
                    if (playerOne == -1 && playerTwo == -1)
                        clicked = true;
                    break;
                }
            }
            if (SSEngine.VersionStage != MenuStage.LAN && SSEngine.VersionStage != MenuStage.Online && clicked)
            {
                Random random = new Random();
                int prob = random.Next(99);
                if (prob < 33)
                    player2Shot = 0;
                else if (prob < 66)
                    player2Shot = 1;
                else
                    player2Shot = 2;
                RockPaperScissorsGame(player1Shot, player2Shot);
                playerOne = player1Shot;
                playerTwo = player2Shot;
            }
            else if (SSEngine.VersionStage == MenuStage.LAN && clicked)
            {
                if (SSEngine.Player1.Value)
                {
                    playerOne = player1Shot;
                    SSEngine.LANClient.Send((byte)playerOne);
                }   
                else
                {
                    playerTwo = player2Shot;
                    SSEngine.LANClient.Send((byte)playerTwo);
                }
            }
            else if (SSEngine.VersionStage == MenuStage.Online && clicked)
            {
                if (SSEngine.Player1.Value)
                {
                    playerOne = player1Shot;
                    NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                    msg.Write((byte)playerOne);
                    SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                }
                else
                {
                    playerTwo = player2Shot;
                    NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                    msg.Write((byte)playerTwo);
                    SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                }
            }
            if (SSEngine.VersionStage == MenuStage.LAN || SSEngine.VersionStage == MenuStage.Online)
                selectedStage = MenuStage.RockPaperScissorsWait;
        }

        private void RockPaperScissorsGame(int player1, int player2)
        {
            if (player1 == 0 && player2 == 1)
                SSEngine.Player1First = false;
            else if (player1 == 0 && player2 == 2)
                SSEngine.Player1First = true;
            else if (player1 == 1 && player2 == 0)
                SSEngine.Player1First = true;
            else if (player1 == 1 && player2 == 2)
                SSEngine.Player1First = false;
            else if (player1 == 2 && player2 == 0)
                SSEngine.Player1First = false;
            else if (player1 == 2 && player2 == 1)
                SSEngine.Player1First = true;
        }
        
        private void UpdateButtons(SSButton[] sButtons, GameTime gameTime, MouseState mouse)
        {
            for (int i = 0; i < sButtons.Length; i++)
            {
                if (mouse.LeftButton == ButtonState.Pressed && sButtons[i].CollisionRectangle.Contains(mouse.X, mouse.Y))
                {
                    sButtons[i].IsClicked = true;
                    sButtons[i].IsCovered = true;
                    elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                    selectedStage = sButtons[i].Stage;
                    break;
                }
                else
                    sButtons[i].IsCovered = false;
            }
           
            if (elapsedTime != 0 && gameTime.TotalGameTime.TotalMilliseconds - elapsedTime >= 100f)
            {
                menuStage = selectedStage;
                foreach (SSButton btn in sButtons)
                    btn.IsClicked = false;
                elapsedTime = 0f;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(menu, 
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.Draw(title, new Vector2(Window.ClientBounds.Width / 2 - title.Width / 2, 0), Color.White);
            
            switch (menuStage)
            {
                case MenuStage.SplashScreen:
                    spriteBatch.Draw(splashScreen, new Rectangle(0, 0, 
                        (int)(SSEngine.QuarterSceenWidth * 4), (int)(SSEngine.QuarterSceenHeight * 4)), Color.White);
                    break;
                case MenuStage.MainMenu:
                    DrawButtons(spriteBatch, mainButtons);
                    spriteBatch.Draw(developed, new Vector2(0, 0), Color.White);
                    break;
                case MenuStage.Play:
                    DrawButtons(spriteBatch, playButtons); break;
                case MenuStage.TwoPlayer:
                    DrawButtons(spriteBatch, twoPlayerBtns); break;
                case MenuStage.Connect:
                    backButton.Draw(spriteBatch);
                    string wait = "Waiting";
                    spriteBatch.DrawString(spriteFont, wait,
                        new Vector2(2 * quarterScreenW - spriteFont.MeasureString(wait).X / 2,
                            2 * quarterScreenH - spriteFont.MeasureString(wait).Y / 2), Color.Black);
                    break;
                case MenuStage.RockPaperScissors:
                    string player;
                    Color color;
                    if (SSEngine.Player1.Value)
                    {
                        player = "Player1";
                        color = Color.Red;
                    }
                    else
                    {
                        player = "Player2";
                        color = Color.Blue;
                    }
                    DrawButtons(spriteBatch, rocPapSciBtns);
                    spriteBatch.DrawString(spriteFont, player,
                        new Vector2(2 * quarterScreenW - spriteFont.MeasureString(player).X / 2,
                            3 * quarterScreenH - spriteFont.MeasureString(player).Y / 2), color);
                    break;
                case MenuStage.RockPaperScissorsWait:
                    backButton.Draw(spriteBatch);
                    string waitShot = "Waiting For Shot";
                    spriteBatch.DrawString(spriteFont, waitShot,
                        new Vector2(2 * quarterScreenW - spriteFont.MeasureString(waitShot).X / 2,
                            2 * quarterScreenH - spriteFont.MeasureString(waitShot).Y / 2), Color.Black);
                    break;
                case MenuStage.RockPaperScissorsPlaced:
                    Texture2D player1 = GetRockPaperScissorsImage(playerOne);
                    Texture2D player2 = GetRockPaperScissorsImage(playerTwo);
                    if (player1 != null && player2 != null)
                    {
                        string text = "Player1";
                        Vector2 pos = new Vector2(quarterScreenW * 5 / 4 - spriteFont.MeasureString(text).X / 2,
                                quarterScreenH / 2 - spriteFont.MeasureString(text).Y / 2);
                        spriteBatch.DrawString(spriteFont, text, pos, Color.Red);
                        spriteBatch.Draw(player1, new Vector2(pos.X, pos.Y + quarterScreenH / 2), Color.White);
                        text = "Player2";
                        pos = new Vector2(pos.X + title.Width, pos.Y);
                        spriteBatch.DrawString(spriteFont, text, pos, Color.Blue);
                        spriteBatch.Draw(player2, new Vector2(pos.X, pos.Y + quarterScreenH / 2), Color.White);
                        if (playerOne == playerTwo)
                            text = "Draw, Press Enter to try again!";
                        else if (SSEngine.Player1First)
                            text = "Player1 goes first!\n(Press Enter to start the game).";
                        else
                            text = "Player2 goes first!\n(Press Enter to start the game).";
                        pos = new Vector2(quarterScreenW * 2 - spriteFont.MeasureString(text).X / 2, 2 * quarterScreenH);
                        spriteBatch.DrawString(spriteFont, text, pos, Color.Magenta);
                    }
                    else
                        DrawButtons(spriteBatch, rocPapSciBtns);
                    break;
                case MenuStage.WaitToPlay:
                    string waitToPlay = "Waiting For Other Player.";
                    spriteBatch.DrawString(spriteFont, waitToPlay,
                        new Vector2(2 * quarterScreenW - spriteFont.MeasureString(waitToPlay).X / 2,
                            2 * quarterScreenH - spriteFont.MeasureString(waitToPlay).Y / 2), Color.Black);
                    break;
                case MenuStage.InGame:
                    break;
                case MenuStage.GameRules:
                    spriteBatch.Draw(rules, new Vector2(2 * quarterScreenW - rules.Width / 2,
                        2 * quarterScreenH - rules.Height * 0.45f), Color.White);
                    DrawButtons(spriteBatch, gameRulesBtns); break;
                case MenuStage.Settings:
                    DrawButtons(spriteBatch, settingsBtns); break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Texture2D GetRockPaperScissorsImage(int player)
        {
            if (player == 0)
                return rocPapSciBtns[0].Texture;
            else if (player == 1)
                return rocPapSciBtns[1].Texture;
            else if (player == 2)
                return rocPapSciBtns[2].Texture;
            else
            {
                menuStage = MenuStage.RockPaperScissors;
                return null;
            }
        }

        private void DrawButtons(SpriteBatch spriteBatch, SSButton[] sButtons)
        {
            for (int i = 0; i < sButtons.Length; i++)
            {
                sButtons[i].Draw(spriteBatch);
            }
        }
        private void DrawWithoutButton(SpriteBatch spriteBatch, SSButton[] sButtons)
        {
            for (int i = 0; i < sButtons.Length - 1; i++)
            {
                sButtons[i].Draw(spriteBatch);
            }
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            if (SSEngine.VersionStage == MenuStage.LAN && menuStage == MenuStage.InGame)
                SSEngine.LANClient.Send((byte)NetGame.Back);
            else if (SSEngine.VersionStage == MenuStage.Online && menuStage == MenuStage.InGame)
            {
                NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
                msg.Write((byte)NetGame.Back);
                SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                System.Threading.Thread.Sleep(1000);
                SSEngine.Peer.Shutdown("Lost Connection.");
            }
            else if (SSEngine.VersionStage == MenuStage.Online)
                Server.Disconnect();

            base.OnExiting(sender, args);
        }
    }
}