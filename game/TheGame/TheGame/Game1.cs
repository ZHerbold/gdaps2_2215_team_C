﻿using Microsoft.Xna.Framework;
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
            EndlessWave,
            DialogueBox, // For NPC
            Shop,        // For NPC
            Victory
        }

        enum LevelState
        {
            level1,
            level2,
        }
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Fields
        // Fields -----------------------------------------------------------

        //Player Fields
        private Player player;
        private const int playerIHealth = 3;      // initial player health, raised via shop
        private int maxHealth;
        private Texture2D playerImage;
        private Vector2 playerPos;
        private Rectangle playerHitbox;
        private List<Rectangle> enemyHitbox;
        private Rectangle rectangle;
        private double timeCounter;
        private double endIFrame = 1.0;

        //Enemy Fields
        private Enemy enemy;
        private Texture2D enemyImage;
        private Vector2 enemyPos;
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
        private LevelState currentLevelState;  
        private KeyboardState currentKbState;
        private KeyboardState previousKbState;
        private Texture2D heart;
        private SpriteFont goldText;
        private SpriteFont information;

        // Constants
        private const int enemyIHealth = 1; // initial enemy health        
        private const int windowWidth = 1280;
        private const int windowHeight = 720;

        // Costs of Upgrades
        private const int healthCost = 10;
        private const int moveCost = 20;
        private const int invulCost = 30;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region Initialize
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize list of enemies, set wave count to 1 and enemy health to 1
            enemies = new List<Enemy>();
            currentWave = 1;
            rng = new Random();
            currentState = GameState.MainMenu;
            enemyHitbox = new List<Rectangle>();
            nextWave = true;
            maxHealth = playerIHealth;

            // Set the window size
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.ApplyChanges();
            enemyHealth = 1;

            base.Initialize();
        }
        #endregion

        #region Load Content
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            currentLevelState = LevelState.level1;

            //Player Content
            playerImage = Content.Load<Texture2D>("Game Stuff");
            playerPos = new Vector2(
                (GraphicsDevice.Viewport.Width / 2) - 125, 
                (GraphicsDevice.Viewport.Height / 2) - 95);

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
                0, 
                PlayerState.FaceRight);

            enemy = new Enemy(enemyIHealth, enemyPos, enemyImage, player);

            heart = Content.Load<Texture2D>("newHeart");
            goldText = Content.Load<SpriteFont>("gold");
            information = Content.Load<SpriteFont>("information");

            //Debug font
            debug = Content.Load<SpriteFont>("Debug");
        }
        #endregion

        #region Update
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
                    else if (SingleKeyPress(Keys.X, currentKbState))
                    {
                        currentState = GameState.Shop;
                    }

                    break;

                // Settings (Not created yet)
                case GameState.Settings:
                    break;

                // Gameover GameState
                case GameState.GameOver:

                    currentKbState = Keyboard.GetState();

                    // For testing purposes, may remove later
                    if (SingleKeyPress(Keys.X, currentKbState))
                    {
                        currentState = GameState.Shop;
                    }

                    //if (currentKbState.IsKeyDown(Keys.Enter))
                    //{
                    //    playerIHealth = 3;
                    //    playerPos = 
                    //    currentState = GameState.MainMenu;
                    //}

                    break;

                // Endless Wave Gamestate
                case GameState.EndlessWave:

                    switch(currentLevelState)
                    {
                        case LevelState.level1:
                            {


                                if(currentWave > 5)
                                {
                                    currentWave = 0;
                                    currentLevelState = LevelState.level2;
                                }

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
                                    // Construct the hitbox based on the direction it's walking
                                    if (enemies[i].State == EnemyState.WalkRight ||
                                        enemies[i].State == EnemyState.AttackRight)
                                    {
                                        enemyHitbox[i] =
                                          new Rectangle(
                                              (int)enemies[i].Position.X + 50,
                                              (int)enemies[i].Position.Y + 70,
                                              EnemyFrameWidth / 2,
                                              EnemyFrameHeight + 30);
                                    }
                                    else if (enemies[i].State == EnemyState.WalkLeft ||
                                        enemies[i].State == EnemyState.AttackLeft)
                                    {
                                        enemyHitbox[i] =
                                          new Rectangle(
                                              (int)enemies[i].Position.X + 140,
                                              (int)enemies[i].Position.Y + 70,
                                              EnemyFrameWidth / 2,
                                              EnemyFrameHeight + 30);
                                    }

                                    // Removes hitbox if enemies are dead
                                    else
                                    {
                                        enemyHitbox[i] =
                                          new Rectangle(
                                              (int)enemies[i].Position.X + 140,
                                              (int)enemies[i].Position.Y + 70,
                                              0,
                                              0);
                                    }
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

                                // Manage enemy attacks
                                for (int i = 0; i < enemyHitbox.Count; i++)
                                {
                                    switch (enemies[i].State)
                                    {
                                        case EnemyState.AttackLeft:
                                        case EnemyState.AttackRight:

                                            // Check for collisions when attacking
                                            // Only active during certain frames
                                            if (enemies[i].Frame == 4)
                                            {
                                                Attack(i);
                                            }

                                            break;
                                    }
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
                                if (player.Health <= 0)
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

                                // Update iFrame if active
                                // After a set period, iFrame is set to false
                                if (player.IFrame)
                                {
                                    timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                                    if (timeCounter >= endIFrame)
                                    {
                                        player.IFrame = false;
                                    }
                                }

                                break;
                            }
                            break;

                        case LevelState.level2:
                            {


                                if (currentWave > 7)
                                {
                                    currentWave = 0;
                                    currentState = GameState.Victory;
                                }

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
                                    // Construct the hitbox based on the direction it's walking
                                    if (enemies[i].State == EnemyState.WalkRight ||
                                        enemies[i].State == EnemyState.AttackRight)
                                    {
                                        enemyHitbox[i] =
                                          new Rectangle(
                                              (int)enemies[i].Position.X + 50,
                                              (int)enemies[i].Position.Y + 70,
                                              EnemyFrameWidth / 2,
                                              EnemyFrameHeight + 30);
                                    }
                                    else if (enemies[i].State == EnemyState.WalkLeft ||
                                        enemies[i].State == EnemyState.AttackLeft)
                                    {
                                        enemyHitbox[i] =
                                          new Rectangle(
                                              (int)enemies[i].Position.X + 140,
                                              (int)enemies[i].Position.Y + 70,
                                              EnemyFrameWidth / 2,
                                              EnemyFrameHeight + 30);
                                    }

                                    // Removes hitbox if enemies are dead
                                    else
                                    {
                                        enemyHitbox[i] =
                                          new Rectangle(
                                              (int)enemies[i].Position.X + 140,
                                              (int)enemies[i].Position.Y + 70,
                                              0,
                                              0);
                                    }
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

                                // Manage enemy attacks
                                for (int i = 0; i < enemyHitbox.Count; i++)
                                {
                                    switch (enemies[i].State)
                                    {
                                        case EnemyState.AttackLeft:
                                        case EnemyState.AttackRight:

                                            // Check for collisions when attacking
                                            // Only active during certain frames
                                            if (enemies[i].Frame == 4)
                                            {
                                                Attack(i);
                                            }

                                            break;
                                    }
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
                                if (player.Health <= 0)
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

                                // Update iFrame if active
                                // After a set period, iFrame is set to false
                                if (player.IFrame)
                                {
                                    timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                                    if (timeCounter >= endIFrame)
                                    {
                                        player.IFrame = false;
                                    }
                                }

                                break;
                            }
                            break;
                    }
                    break;

                // Dialogue Box Gamestate (Not coded yet)
                case GameState.DialogueBox:
                    break;

                // ----- Purchase Weapons/Upgrades -----
                case GameState.Shop:

                    currentKbState = Keyboard.GetState();

                    // Return to game from shop
                    if (SingleKeyPress(Keys.Enter, currentKbState))
                    {
                        // Apply health changes
                        player.Health = maxHealth;

                        // Change state
                        currentState = GameState.EndlessWave;
                    }

                    // Purchase Extra Health
                    if (player.Gold >= healthCost)
                    {
                        if (SingleKeyPress(Keys.H, currentKbState))
                        {
                            maxHealth++;
                            player.Gold -= healthCost;
                        }
                    }

                    // Purchase Extra Movement Speed
                    if (player.Gold >= moveCost)
                    {
                        if (SingleKeyPress(Keys.M, currentKbState))
                        {
                            player.Movement++;
                            player.Gold -= moveCost;
                        }
                    }

                    // Purchase Extra iFrames
                    if (player.Gold >= invulCost)
                    {
                        if (SingleKeyPress(Keys.I, currentKbState))
                        {
                            endIFrame += 0.2;
                            player.Gold -= invulCost;
                        }
                    }

                    // free gold (for testing)
                    if (SingleKeyPress(Keys.P, currentKbState))
                    {
                        player.Gold++;
                    }

                    break;

                case GameState.Victory:

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
        #endregion

        #region Draw
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
                        "Press 'X' to access the shop\n" +
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

                    switch(currentLevelState)
                    {
                        case LevelState.level1:
                            {
                                //Draw stuff for the background
                                _spriteBatch.Draw(
                                    background,
                                    backgroundPos,
                                    null,
                                    Color.MistyRose,
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

                                // visualize hitboxs
                                //_spriteBatch.Draw(heart, playerHitbox, Color.Red);
                                //
                                //for (int i = 0; i < enemyHitbox.Count; i++)
                                //{
                                //    _spriteBatch.Draw(heart, enemyHitbox[i], Color.Red);
                                //}

                                //drawing for the enemy
                                foreach (Enemy e in enemies)
                                {
                                    if (e.Active)
                                    {
                                        e.Draw(_spriteBatch);
                                    }
                                }

                                if (player.Health > 0)
                                {
                                    for (int i = 0; i < player.Health; i++)
                                    {
                                        _spriteBatch.Draw(
                                        heart,
                                        new Vector2(i * 40, 10),
                                        new Rectangle(0, 0, 32, 32),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1.5f,
                                        SpriteEffects.None,
                                        0);
                                    }
                                }

                                if (enemies.Count > 0)
                                {
                                    _spriteBatch.DrawString(
                                        information,
                                        String.Format("Gold: {0}", player.Gold),
                                        new Vector2(10, 50),
                                        Color.White);
                                }

                            }
                            break;

                        case LevelState.level2:
                            {
                                //Draw stuff for the background
                                _spriteBatch.Draw(
                                    background,
                                    backgroundPos,
                                    null,
                                    Color.SkyBlue,
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

                                // visualize hitboxs
                                //_spriteBatch.Draw(heart, playerHitbox, Color.Red);
                                //
                                //for (int i = 0; i < enemyHitbox.Count; i++)
                                //{
                                //    _spriteBatch.Draw(heart, enemyHitbox[i], Color.Red);
                                //}

                                //drawing for the enemy
                                foreach (Enemy e in enemies)
                                {
                                    if (e.Active)
                                    {
                                        e.Draw(_spriteBatch);
                                    }
                                }

                                if (player.Health > 0)
                                {
                                    for (int i = 0; i < player.Health; i++)
                                    {
                                        _spriteBatch.Draw(
                                        heart,
                                        new Vector2(i * 40, 10),
                                        new Rectangle(0, 0, 32, 32),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1.5f,
                                        SpriteEffects.None,
                                        0);
                                    }
                                }

                                if (enemies.Count > 0)
                                {
                                    _spriteBatch.DrawString(
                                        information,
                                        String.Format("Gold: {0}", player.Gold),
                                        new Vector2(10, 50),
                                        Color.White);
                                }

                            }
                            break;
                    }

                    
                    break;

                case GameState.DialogueBox:
                    break;

                case GameState.Shop:

                    // Display hearts, updates as upgrades are purchased
                    for (int i = 0; i < maxHealth; i++)
                    {
                        _spriteBatch.Draw(
                            heart,
                            new Vector2((i * 40) + 30, 150),
                            new Rectangle(0, 0, 32, 32),
                            Color.White,
                            0,
                            Vector2.Zero,
                            1.5f,
                            SpriteEffects.None,
                            0);
                    }

                    // Display current gold
                    _spriteBatch.DrawString(
                        goldText, 
                        String.Format("Gold: {0}", player.Gold), 
                        new Vector2(30, 60), 
                        Color.Gold);

                    // Display current movement speed
                    _spriteBatch.DrawString(
                        information,
                        String.Format("Movement Speed - {0}", player.Movement),
                        new Vector2(30, 200),
                        Color.White);

                    // Display current invulnerability
                    _spriteBatch.DrawString(
                        information,
                        String.Format("Invulnerability - {0}", endIFrame),
                        new Vector2(30, 220),
                        Color.White);

                    // Display price of more health
                    _spriteBatch.DrawString(
                        goldText,
                        String.Format("(h)  +HEALTH [cost: {0}]", healthCost),
                        new Vector2(70, windowHeight/2),
                        Color.BlanchedAlmond);

                    // Display price of more movement speed
                    _spriteBatch.DrawString(
                        goldText,
                        String.Format("(m)  +MOVE [cost: {0}]", moveCost),
                        new Vector2(70, (windowHeight / 2) + 100),
                        Color.BlanchedAlmond);

                    // Display price of more invuln frames
                    _spriteBatch.DrawString(
                        goldText,
                        String.Format("(i)  +INVUL [cost: {0}]", invulCost),
                        new Vector2(70, (windowHeight / 2) + 200),
                        Color.BlanchedAlmond);
                    break;

                default:
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Methods
        // The Next Wave method
        public void NextWave()
        {
            currentWave++;

            enemyHealth = currentWave * currentWave;

            // Checks to see if there are any enemies within the enemies list
            if (enemies != null)
            {
                enemies.Clear();
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
            // Check which way the player is facing, and exted the hitbox in that direction
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

        /// <summary>
        /// Check for collisions between the player and an enemy from the list
        /// </summary>
        /// <param name="e"></param>
        private void Attack(int i)
        {
            // Check which way the enemy is facing
            // and extend the hitbox in that direction
            if (enemies[i].State == EnemyState.AttackRight)
            {
                enemyHitbox[i] = new Rectangle(
                        (int)player.Position.X + 90,
                        (int)player.Position.Y + 30,
                        160,
                        110);
            }
            else
            {
                enemyHitbox[i] = new Rectangle(
                        (int)player.Position.X + 10,
                        (int)player.Position.Y + 30,
                        160,
                        110);
            }

            // Check for collisions only if the enemy is active
            if (enemies[i].Active)
            {
                // If the player and enemy collide return true
                if (enemyHitbox[i].Intersects(playerHitbox))
                {
                    if (!player.IFrame)
                    {
                        player.Health--;
                        player.IFrame = true;
                        timeCounter = 0;
                    }
                }
            }
        }

        // Collision method
        //public void CheckCollision()
        //{
        //    for (int i = 0; i < enemies.Count; i++)
        //    {
        //        // If enemy hits player, knock the enemy back
        //        //Vector2.Distance(enemies[i].Position, player.Position) < 25  -  leave this here for backup use
        //        if (rectangle.Intersects(enemyHitbox[i]))
        //        {
        //            playerIHealth--;
        //            for (int j = 0; j < 50; j++)
        //            {
        //                enemies[i].Position += new Vector2(1, 1);
        //            }

        //            //enemies[i].Die();
        //            //enemies.RemoveAt(i);
        //            //enemyPositions.RemoveAt(i);
        //        }
        //    }
        //}

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
        #endregion
    }
}
