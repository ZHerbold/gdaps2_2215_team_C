using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TheGame
{
    public class Game1 : Game
    {
        enum GameState
        {
            MainMenu,
            Settings,
            GameOver,
            EndlessWave, // Only Enless Mode for S2 Skeleton
            DialogueBox,
            Shop
        }
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Fields -----------------------------------------------------------
        
        //Player Fields
        private Player player;
        private int playerIHealth = 3;      // initial player health, raised via shop
        private Texture2D playerImage;
        private Vector2 playerPos;
        private int gold;
        private Rectangle playerHitbox;
        private List<Rectangle> enemyHitbox;
        private const int PlayerFrameWidth = 100;
        private const int PlayerFrameHeight = 55;

        //Enemy Fields
        private Enemy enemy;
        private Texture2D enemyImage;
        private Vector2 enemyPos;
        private List<Rectangle> enemyPositions;
        private List<Enemy> enemies;
        private int enemyHealth;
        private const int spriteSheetWidth = 768;
        private const int spriteSheetHeight = 576;
        private const int EnemyFrameWidth = spriteSheetWidth / 6;
        private const int EnemyFrameHeight = spriteSheetHeight / 6;

        //Background fields;
        private Texture2D background;
        private Vector2 backgroundPos;

        //Debug
        private SpriteFont debug;

        //Game info
        private int currentWave;
        private Random rng;
        private GameState currentState;
        private KeyboardState currentKbState;
        private KeyboardState previousKbState;
        private List<Texture2D> hearts;
        private Texture2D heart;
        private SpriteFont goldText;
        private SpriteFont information;

        // Constants
        private const int enemyIHealth = 1; // initial enemy health        
        private const int windowWidth = 1280;
        private const int windowHeight = 720;
        // If we have time, implement resolution choices in settings
        // But not for S2 skeleton

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize list of enemies, set wave count to 1 and enemy health to 1
            enemies = new List<Enemy>();
            enemyPositions = new List<Rectangle>();
            hearts = new List<Texture2D>();
            currentWave = 1;
            rng = new Random();
            currentState = GameState.MainMenu;
            enemyHitbox = new List<Rectangle>();
            

            // Set the window size
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.ApplyChanges();
            enemyHealth = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Player Content
            playerImage = Content.Load<Texture2D>("Game Stuff");
            playerPos = new Vector2(
                (GraphicsDevice.Viewport.Width / 2) - 125, 
                (GraphicsDevice.Viewport.Height / 2) - 95);
            gold = 0;

            //Enemy Content
            enemyImage = Content.Load<Texture2D>("Melee Enemy");
            enemyPos = new Vector2(900, 300);

            //Background Content
            background = Content.Load<Texture2D>("Background With Portal");
            backgroundPos = new Vector2(0, -17);

            player = new Player(
                playerIHealth, 
                playerPos, 
                playerImage, 
                gold, 
                PlayerState.FaceRight);

            enemy = new Enemy(enemyIHealth, enemyPos, enemyImage, player);

            //for (int i = 0; i < playerIHealth; i++)
            //{
            //    hearts.Add(Content.Load<Texture2D>("heart"));
            //}
            heart = Content.Load<Texture2D>("heart");
            goldText = Content.Load<SpriteFont>("gold");
            information = Content.Load<SpriteFont>("information");

            
            


            //Debug font
            debug = Content.Load<SpriteFont>("Debug");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            
            switch (currentState)
            {
                // Main Menu GameState
                case GameState.MainMenu:
                    currentKbState = Keyboard.GetState();
                    if (SingleKeyPress(Keys.Enter, currentKbState))
                    {
                        currentState = GameState.EndlessWave;
                    }
                    break;

                // Settings (Not created yet)
                case GameState.Settings:
                    break;

                // Gameover GameState
                case GameState.GameOver:
                    currentKbState = Keyboard.GetState();
                    break;

                // Endless Wave Gamestate
                case GameState.EndlessWave:
                    currentKbState = Keyboard.GetState();

                    // Enemy count
                    if (enemies.Count == 0)
                    {
                        NextWave();
                    }

                    playerHitbox = new Rectangle((int)player.Position.X + PlayerFrameWidth/2, (int)player.Position.Y + PlayerFrameHeight/2,
                    PlayerFrameWidth, PlayerFrameHeight);

                    for (int i = 0; i < enemyHitbox.Count; i++)
                    {
                        enemyHitbox[i] = new Rectangle((int)enemies[i].Position.X - EnemyFrameWidth / 2, (int)enemies[i].Position.Y - EnemyFrameHeight / 2,
                                            EnemyFrameWidth, EnemyFrameHeight);

                    }
                    

                    player.UpdateAnimation(gameTime);
                    //calls player update.
                    player.Update(gameTime);

                    //calls enemy update
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].UpdateAnimation(gameTime);
                        enemies[i].Update(gameTime);
                    }
                    CheckCollision();

                    // checks to see if the players health is zero
                    if (playerIHealth <= 0)
                    {
                        currentState = GameState.GameOver;
                    }
                    break;

                // Dialogue Box Gamestate (Not coded yet)
                case GameState.DialogueBox:
                    break;

                // Shop GameState (Not coded yet)
                case GameState.Shop:
                    break;

                default:
                    break;
            }

            

            //gets the keyboard state
            KeyboardState kbState = Keyboard.GetState();
            
            //Background movement code. we dont need it right now since the size is pretty good, but when we make the map bigger, we'll want it to 
            //move with the player.
            /*
            if (kbState.IsKeyDown(Keys.A))
            {
                backgroundPos.X+=3;
            }
            if (kbState.IsKeyDown(Keys.D))
            {
                backgroundPos.X-=3;
            }
            if (kbState.IsKeyDown(Keys.W))
            {
                backgroundPos.Y+=3;
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                backgroundPos.Y-=3;
            }
            */

            //Background code (since it probably doesnt need a class)

            previousKbState = currentKbState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.MainMenu:
                    _spriteBatch.DrawString(
                        information,
                        String.Format("" +
                        "Welcome to GAME TITLE\n" +
                        "Press 'Enter' to play\n" +
                        "use 'wasd' to move and 'mouse1' to attack"),
                        new Vector2(500, 500),
                        Color.White);
                    break;

                case GameState.Settings:
                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(
                        information,
                        String.Format("" +
                        "GAME OVER\n" +
                        "Get better"),
                        new Vector2(500, 500),
                        Color.White);
                    break;

                case GameState.EndlessWave:
                    //Draw stuff for the background
                    _spriteBatch.Draw(
                        background,
                        backgroundPos,
                        null,
                        Color.White,
                        0f,
                        new Vector2(0, 0),
                        2f,
                        SpriteEffects.None,
                        1);

                    if (enemies.Count != 0)
                    {
                        _spriteBatch.DrawString(
                        debug, String.Format("" +
                        "Enemy distance: {0}", enemies[0].Position)
                        , new Vector2(10, 70), Color.White);
                    }
                    

                    //drawing for the player
                    player.Draw(_spriteBatch);

                    //drawing for the enemy
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Draw(_spriteBatch);
                        _spriteBatch.Draw(enemyImage, enemyHitbox[i], Color.Red);
                    }

                    if (playerIHealth > 0)
                    {
                        for (int i = 0; i < playerIHealth; i++)
                        {
                            _spriteBatch.Draw(
                            heart,
                            new Vector2(i * 40, 10),
                            new Rectangle(0, 0, 16, 16),
                            Color.White,
                            0,
                            Vector2.Zero,
                            2,
                            SpriteEffects.None,
                            0);
                        }
                    }
                    if (enemies.Count > 0)
                    {
                        _spriteBatch.DrawString(goldText, String.Format("Gold: {0}", enemyHitbox[0]), new Vector2(1, 50), Color.White);
                    }
                    _spriteBatch.Draw(playerImage, playerHitbox, Color.Red);



                    break;

                case GameState.DialogueBox:
                    break;

                case GameState.Shop:
                    break;

                default:
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // The Next Wave method
        public void NextWave()
        {
            currentWave++;

            enemyHealth = currentWave * currentWave;
            // Checks to see if there are any enemies within the enemies list
            if (enemies != null)
            {
                enemies.Clear();
                enemyPositions.Clear();
                enemyHitbox.Clear();
            }
            // Creates the next wave of enemies
            for (int i = 0; i < currentWave; i++)
            {
                enemies.Add(
                    new Enemy(
                        enemyHealth, 
                        new Vector2(
                            rng.Next(50, windowWidth - 50), 
                            rng.Next(50, windowHeight - 50)), 
                        enemyImage, 
                        player));
                    enemyHitbox.Add(new Rectangle((int)enemies[i].Position.X - EnemyFrameWidth/2, (int)enemies[i].Position.Y - EnemyFrameHeight / 2,
                    EnemyFrameWidth, EnemyFrameHeight));
                
            }
        }

        // Collision method
        public void CheckCollision()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                // If enemy hits player, knock the enemy back
                //Vector2.Distance(enemies[i].Position, player.Position) < 25  -  leave this here for backup use
                if (playerHitbox.Intersects(enemies[i].Rectangle))
                {
                    //playerIHealth--;
                    for (int j = 0; j < 50; j++)
                    {
                        enemies[i].Position += new Vector2(1,1);
                    }
                    //enemies[i].Die();
                    //enemies.RemoveAt(i);
                    //enemyPositions.RemoveAt(i);
                }   
            }
            
        }

        // Single key press method for any key
        public bool SingleKeyPress(Keys key, KeyboardState currentKbState)
        {
            if (true)
            {
                if (currentKbState.IsKeyDown(key) && previousKbState.IsKeyUp(key))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
