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
            DialogueBox, // For NPC
            Shop         // For NPC
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
        private Rectangle rectangle;

        //Enemy Fields
        private Enemy enemy;
        private Texture2D enemyImage;
        private Vector2 enemyPos;
        private List<Rectangle> enemyPositions;
        private List<Rectangle> visualHitboxes;
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
        private bool nextWave;
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
            nextWave = true;
            visualHitboxes = new List<Rectangle>();

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

                    // Visualized hitbox for player
                    rectangle = new Rectangle(
                        (int)player.Position.X + 90,
                        (int)player.Position.Y + 30,
                        80,
                        110);

                    // Construct player hitbox
                    playerHitbox = new Rectangle(
                        (int)player.Position.X + 90,
                        (int)player.Position.Y + 30,
                        80,
                        110);

                    

                    // Construct enemy hitboxes
                    for (int i = 0; i < enemyHitbox.Count; i++)
                    {
                        enemyHitbox[i] = 
                            new Rectangle(
                                (int)enemies[i].Position.X + 50, 
                                (int)enemies[i].Position.Y + 70,
                                EnemyFrameWidth/2, 
                                EnemyFrameHeight + 30);
                    }
                    
                    

                    // Manage player attacks
                    switch (player.State)
                    {
                        case PlayerState.AttackRight:
                        case PlayerState.AttackLeft:

                            // Check for collisions when attacking
                            // Only active during the swinging part of the animation
                            if (player.Frame > 1 && player.Frame < 6)
                            {
                                Attack();
                            }
                            break;
                    }

                    player.UpdateAnimation(gameTime);
                    player.Update(gameTime);

                    //calls enemy update
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].UpdateAnimation(gameTime);
                        enemies[i].Update(gameTime);
                    }

                    // checks to see if the players health is zero
                    if (playerIHealth <= 0)
                    {
                        currentState = GameState.GameOver;
                    }

                    // Update nextWave bool based on the
                    // active property of enemies
                    foreach (Enemy e in enemies)
                    {
                        if (e.Active)
                        {
                            nextWave = false;
                            break;
                        }
                        else
                        {
                            nextWave = true;
                        }
                    }

                    // If there aren't any more active enemies
                    // progress to next wave
                    if (nextWave)
                    {
                        NextWave();
                        nextWave = false;
                    }
                    //CheckCollision();


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
                // --- MAIN MENU ---
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

                // --- GAME OVER ---
                case GameState.GameOver:
                    _spriteBatch.DrawString(
                        information,
                        String.Format("" +
                        "GAME OVER\n" +
                        "Get better"),
                        new Vector2(500, 500),
                        Color.White);
                    break;

                // --- GAME LOOP ---
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
                        "Enemy distance: {0}", playerHitbox)
                        , new Vector2(10, 70), Color.White);
                    }                    

                    //drawing for the player
                    player.Draw(_spriteBatch);

                    // visual player hitbox
                    _spriteBatch.Draw(heart, playerHitbox, Color.Red);

                    for (int i = 0; i < enemyHitbox.Count; i++)
                    {
                        _spriteBatch.Draw(heart, enemyHitbox[i], Color.Red);
                    }

                    //drawing for the enemy
                    foreach (Enemy e in enemies)
                    {
                        if (e.Active)
                        {
                            e.Draw(_spriteBatch);
                        }
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
                        _spriteBatch.DrawString(
                            goldText, 
                            String.Format("Gold: {0}", enemyHitbox[0]), 
                            new Vector2(1, 50), 
                            Color.White);
                    }

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

                // Enemy Hitbox Locations
                    enemyHitbox.Add(
                        new Rectangle(
                            (int)enemies[i].Position.X, 
                            (int)enemies[i].Position.Y,

                // Enemy Hitbox Dimensions
                        EnemyFrameWidth, 
                        EnemyFrameHeight));
                
            }
        }

        /// <summary>
        /// Kill an enemy when your hitbox becomes active 
        /// and it intersects with theirs
        /// </summary>
        public void Attack()
        {
            

            if (player.State == PlayerState.AttackRight)
            {
                playerHitbox = new Rectangle(
                        (int)player.Position.X + 90,
                        (int)player.Position.Y + 30,
                        160,
                        110);
            }
            else
            {
                playerHitbox = new Rectangle(
                        (int)player.Position.X + 10,
                        (int)player.Position.Y + 30,
                        160,
                        110);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                // Check for collisions only if the enemy is active
                if (enemies[i].Active)
                {
                    // If the player and enemy collide return true
                    if (playerHitbox.Intersects(enemyHitbox[i]))
                    {
                        enemies[i].Die();
                    }
                }
            }
        }

        // Collision method by John
        public void CheckCollision()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                // If enemy hits player, knock the enemy back
                //Vector2.Distance(enemies[i].Position, player.Position) < 25  -  leave this here for backup use
                if (rectangle.Intersects(enemyHitbox[i]))
                {
                    playerIHealth--;
                    for (int j = 0; j < 50; j++)
                    {
                        enemies[i].Position += new Vector2(1, 1);
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
