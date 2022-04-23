using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheGame
{
    public class Game1 : Game
    {
        enum GameState
        {
            MainMenu,
            GameOver,
            EndlessWave,
            Shop,        // For NPC
        }

        /*
        enum LevelStateaaaaaaaa
        {
            level1,
            level2
        }
        */

        enum DialogeState
        {
            fail,
            success,
            normal
        }
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Fields
        // Fields -----------------------------------------------------------

        //Player Fields
        private Player player;
        private int playerIHealth = 3;      // initial player health, raised via shop
        private int maxHealth;
        private Texture2D playerImage;
        private Vector2 playerPos;
        private Rectangle playerHitbox;
        private List<Rectangle> enemyHitbox;
        private Rectangle rectangle;
        private double timeCounter;
        private double endIFrame = 1.0;
        private int mapX;
        private int mapY;


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
        
        //private Texture2D background;
        //private Vector2 backgroundPos;

        private Texture2D startingRoom;
        private Texture2D room1;
        private Texture2D room2;
        private Texture2D room3;
        private Texture2D room4;
        private Texture2D shopRoom;
        private Texture2D endRoom;  //not implemented yet. do we want a hybring.
        private Texture2D forcefields;

        private Texture2D deadEnd;
        private Texture2D clearFloor;
        private Texture2D menuButton;

        private int area;
        private List<Texture2D> roomList;

        private Map map;
        private float speed;

        //Debug
        private SpriteFont debug;

        //Game info
        private bool nextWave;
        private int currentWave;
        private Random rng;
        private GameState currentState;
        //private LevelState currentLevelState;  
        private KeyboardState currentKbState;
        private KeyboardState previousKbState;
        private Texture2D heart;
        private Texture2D shrine;
        private SpriteFont goldText;
        private SpriteFont information;
        private SpriteFont displayMessage;
        private SpriteFont shopMessage;
        private Texture2D rect;
        float timer;
        private int difficulty; //IMPORTANT;

        // Constants
        private const int enemyIHealth = 1; // initial enemy health        
        private const int windowWidth = 1280;
        private const int windowHeight = 720;

        // Costs of Upgrades
        private int healthCost = 15;
        private int moveCost = 20;
        private int invulCost = 30;
        private const int costRaise = 5;

        // Cost of Skills
        private const int potionCost = 30;
        private const int dodgeCost = 45;
        private const int shieldCost = 60;

        // File IO
        private const string fileName = "savedata.txt";

        // Game Over Details
        private int level;

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
            rng = new Random();
            currentState = GameState.MainMenu;
            enemyHitbox = new List<Rectangle>();
            nextWave = true;
            maxHealth = playerIHealth;

            // Set the window size
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.ApplyChanges();

            currentWave = 0;
            enemyHealth = 1;
            timer = 0;
            level = 1;

            base.Initialize();
        }
        #endregion

        #region Load Content
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Player Content
            playerImage = Content.Load<Texture2D>("Game Stuff");
            playerPos = new Vector2(
                (GraphicsDevice.Viewport.Width / 2) - 125, 
                (GraphicsDevice.Viewport.Height / 2) - 95);

            //Enemy Content
            enemyImage = Content.Load<Texture2D>("Melee Enemy");
            enemyPos = new Vector2(900, 300);

            //Background Content
            //background = Content.Load<Texture2D>("Background With Portal");
            //backgroundPos = new Vector2(0, -17);
            startingRoom = Content.Load<Texture2D>("Teleporter Room 1");
            room1 = Content.Load<Texture2D>("Room 2");
            room2 = Content.Load<Texture2D>("Room 3");
            room3 = Content.Load<Texture2D>("Room 4");
            room4 = Content.Load<Texture2D>("Room 5");
            shopRoom = Content.Load<Texture2D>("Store Template");
            endRoom = Content.Load<Texture2D>("Exit");
            forcefields = Content.Load<Texture2D>("Forcefield");

            deadEnd = Content.Load<Texture2D>("Deadend");
            clearFloor = Content.Load<Texture2D>("Clearfloor");
            menuButton = Content.Load<Texture2D>("Menu Button");

            roomList = new List<Texture2D>();
            roomList.Add(startingRoom);
            roomList.Add(room1);
            roomList.Add(room2);
            roomList.Add(room4);
            roomList.Add(room3); //most common room
            roomList.Add(shopRoom);
            roomList.Add(endRoom);

            area = 3;

            map = new Map(area, roomList, difficulty);
            mapX = 1;
            mapY = 1;

            speed = 1.7f;

            player = new Player(
                playerIHealth, 
                playerPos, 
                playerImage, 
                0, 
                PlayerState.FaceRight);

            enemy = new Enemy(enemyIHealth, enemyPos, enemyImage, player, speed);

            heart = Content.Load<Texture2D>("newHeart");
            goldText = Content.Load<SpriteFont>("gold");
            information = Content.Load<SpriteFont>("information");
            displayMessage = Content.Load<SpriteFont>("display");
            shopMessage = Content.Load<SpriteFont>("shopDialogue");

            // NPC
            shrine = Content.Load<Texture2D>("Shrine");
            rectangle = new Rectangle(500,500,100,100);
            rect = new Texture2D(_graphics.GraphicsDevice, 300, 300);

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

                    // Game Loop
                    if (SingleKeyPress(Keys.Enter, currentKbState))
                    {
                        SoftReset();
                        LevelReset();

                        currentState = GameState.EndlessWave;
                        //currentLevelState = LevelState.level1;
                    }                   

                    // Load Game
                    else if (SingleKeyPress(Keys.L, currentKbState))
                    {
                        LoadStats(fileName);
                    }

                    // Save Game
                    else if (SingleKeyPress(Keys.S, currentKbState))
                    {
                        SaveGame(fileName);
                    }

                    //// (FOR TESTING)
                    //else if (SingleKeyPress(Keys.P, currentKbState))
                    //{
                    //    currentState = GameState.Shop;                        
                    //}
                    //else if (SingleKeyPress(Keys.O, currentKbState))
                    //{
                    //    currentState = GameState.GameOver;
                    //}

                    break;

                // Gameover GameState
                case GameState.GameOver:

                    currentKbState = Keyboard.GetState();
                    
                    if (SingleKeyPress(Keys.Enter, currentKbState))
                    {
                        SoftReset();
                        LevelReset();
                        currentState = GameState.MainMenu;
                    }

                    break;

                // Endless Wave Gamestate
                case GameState.EndlessWave:

                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    currentKbState = Keyboard.GetState();

                     // Shop
                    if (SingleKeyPress(Keys.X, currentKbState) && map.CurrentRoom.RoomType == "shop")
                    {
                        timer = 0;
                        currentState = GameState.Shop;
                    }
                    //next level
                    else if (SingleKeyPress(Keys.X, currentKbState) && map.CurrentRoom.RoomType == "exit" && map.UnvSquares <= 0)
                    {
                        NextLevel();
                    }

                    //// (FOR TESTING)
                    //else if (SingleKeyPress(Keys.O, currentKbState))
                    //{
                    //    currentState = GameState.GameOver;
                    //}

                    // Construct player hitbox
                    playerHitbox = new Rectangle(
                        (int)player.Position.X + 90,
                        (int)player.Position.Y + 30,
                        80,
                        110);

                    // Construct enemy hitboxes
                    // Only run after the level text has gone away
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

                    //calls enemy update
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].UpdateAnimation(gameTime);
                        enemies[i].Update(gameTime);
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

                    map.Update(gameTime, player.Position);

                    // checks to see if the players health is zero
                    if (player.Health <= 0)
                    {
                        currentState = GameState.GameOver;
                    }

                    // Update nextWave bool based on the
                    // active property of enemies
                    if(!map.CurrentRoom.AllDead)
                    {
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
                    }
                    

                    // If there aren't any more active enemies
                    // progress to next wave
                    if (nextWave && !map.CurrentRoom.AllDead)
                    {
                        if(currentWave < map.CurrentRoom.WaveCount)
                        {
                            NextWave();
                            nextWave = false;
                        }
                        else
                        {
                            map.CurrentRoom.AllDead = true;
                            map.UnvSquares--;
                            difficulty++;
                            map.Diff = difficulty;
                            currentWave = 0;
                            nextWave = false;
                        }
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

                    /*
                    if (currentWave == map.CurrentRoom.WaveCount + 1)
                    {
                        map.CurrentRoom.AllDead = true;
                        difficulty++;
                        map.Diff = difficulty;
                    }
                    */
                    
                    //checks if the player is at any of the exits

                    //shrine collision, do the same below but testing for the shrine.
                    if(map.CurrentRoom.RoomType == "shop")
                    {
                        if(true)
                        {

                        }
                    }

                    if(map.CurrentRoom.AllDead)
                    {
                        if (player.Position.X >= 1100 && (player.Position.Y > 250 && player.Position.Y < 290))
                        {
                            NextRoom();
                        }
                        else if (player.Position.X <= -75 && (player.Position.Y > 250 && player.Position.Y < 290))
                        {
                            NextRoom();
                        }
                        else if (player.Position.Y <= 0 && (player.Position.X > 490 && player.Position.X < 530))
                        {
                            NextRoom();
                        }
                        else if (player.Position.Y >= 575 && (player.Position.X > 490 && player.Position.X < 530))
                        {
                            NextRoom();
                        }
                    }
                    
                    

                    /*
                    switch (currentLevelState)
                    {
                        // LEVEL 1 ----------------------------------------------------------------------------------------------
                        case LevelState.level1:
                            {
                                if (currentWave > 5)
                                {
                                    currentWave = 0;
                                    currentLevelState = LevelState.level2;
                                    timer = 0;
                                }
                                break;
                            }
                        

                        // LEVEL 2 ----------------------------------------------------------------------------------------------
                        case LevelState.level2:
                            {
                                if (currentWave > 7)
                                {
                                    currentWave = 0;
                                    currentState = GameState.MainMenu;
                                }
                                break;
                            }                            
                    }
                    */
                    break;

                // ----- Purchase Weapons/Upgrades -----
                case GameState.Shop:

                    currentKbState = Keyboard.GetState();

                    // Return to game from shop
                    if (SingleKeyPress(Keys.Space, currentKbState))
                    {
                        // Change state
                        SoftReset();
                        currentState = GameState.EndlessWave;
                        //currentLevelState = LevelState.level1;
                    }

                    // Go to Main Menu
                    if (SingleKeyPress(Keys.Enter, currentKbState))
                    {
                        currentState = GameState.MainMenu;
                    }

                    //// Free Gold (FOR TESTING)
                    //if (SingleKeyPress(Keys.P, currentKbState))
                    //{
                    //    player.Gold += 100;
                    //}

                    // Purchase Extra Health
                    //if (player.Health < 6)
                    //{
                        if (player.Gold >= healthCost)
                        {
                            if (SingleKeyPress(Keys.H, currentKbState))
                            {
                                maxHealth++;
                                player.Health++;
                                player.Gold -= healthCost;
                                healthCost += costRaise;
                            }
                        }
                    //}
                    //// Purchase Health potion ability
                    //else
                    //{
                    //    if (player.Ability == false)
                    //    {
                    //        if (player.Gold >= potionCost)
                    //        {
                    //            if (SingleKeyPress(Keys.H, currentKbState))
                    //            {
                    //                player.Ability = true;
                    //                player.TypeOfAbility = 0;
                    //            }
                    //        }
                    //    }
                    //}

                    // Purchase Extra Movement Speed
                    //if (player.Movement <= 9)
                    //{
                        if (player.Gold >= moveCost)
                        {
                            if (SingleKeyPress(Keys.M, currentKbState))
                            {
                                player.Movement++;
                                player.Gold -= moveCost;
                                moveCost += costRaise;
                            }
                        }
                    //}
                    //// Purchase Dodge Ability
                    //else
                    //{
                    //    if (player.Ability == false)
                    //    {
                    //        if (player.Gold >= dodgeCost)
                    //        {
                    //            if (SingleKeyPress(Keys.M, currentKbState))
                    //            {
                    //                player.Ability = true;
                    //                player.TypeOfAbility = 1;
                    //            }
                    //        }
                    //    }
                    //}

                    // Purchase Extra iFrames
                    //if (endIFrame <= 3)
                    //{
                        if (player.Gold >= invulCost)
                        {
                            if (SingleKeyPress(Keys.I, currentKbState))
                            {
                                endIFrame += 0.2;
                                player.Gold -= invulCost;
                                invulCost += costRaise;
                            }
                        }
                    //}
                    //// Purchase Shield Ability
                    //else
                    //{
                    //    if (player.Ability == false)
                    //    {
                    //        if (player.Gold >= shieldCost)
                    //        {
                    //            if (SingleKeyPress(Keys.I, currentKbState))
                    //            {
                    //                player.Ability = true;
                    //                player.TypeOfAbility = 2;
                    //            }
                    //        }
                    //    }
                    //}
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
                        "Welcome to MOURNBLADE\n" +
                        "Press 'Enter' to play\n" +
                        "Press 'L' to load or 'S' to save\n" +
                        "use 'wasd' to move and 'mouse1' or 'K' to attack"),
                        new Vector2(500, 500),
                        Color.White);
                    break;

                // --- GAME OVER ---
                case GameState.GameOver:

                    _spriteBatch.DrawString(
                        information,
                        String.Format("" +
                        "GAME OVER\n" +
                        "Press 'ENTER' to return to the main menu"),
                        new Vector2(500, 500),
                        Color.White);

                    // Display Score
                    _spriteBatch.DrawString(
                        goldText, 
                        String.Format("You made it to level - {0}" +
                                      "\n     Score: {1}", 
                                      level, player.Score), 
                        new Vector2(300, 300), Color.Gold);

                    break;

                // --- GAME LOOP ---
                case GameState.EndlessWave:                  

                    map.Draw(_spriteBatch, displayMessage);
                    if (!map.CurrentRoom.AllDead)
                    {
                        _spriteBatch.Draw(forcefields, new Vector2(4, -25), null, Color.White, 0f, new Vector2(0, 0), 1.99f, SpriteEffects.None, 0f);
                    }

                    if (map.CurrentRoom.RoomType == "shop")
                    {
                        _spriteBatch.Draw(shrine, new Vector2(450, 170), null, Color.White, 0f, new Vector2(0, 0), 1.99f, SpriteEffects.None, 0f);
                    }

                    player.Draw(_spriteBatch);

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
                            1);
                        }
                    }                    

                    if (currentState == GameState.EndlessWave)
                    {
                        _spriteBatch.DrawString(
                            information,
                            String.Format("Gold: {0}", player.Gold),
                            new Vector2(10, 50),
                            Color.White);
                        _spriteBatch.DrawString(
                            information,
                            String.Format("Wave count: {0}\nMax Waves: {1}", currentWave, map.CurrentRoom.WaveCount),
                            new Vector2(10, 90),
                            Color.White);
                        _spriteBatch.DrawString(
                            information,
                            String.Format("Pos: {0},{1}", player.Position.X, player.Position.Y),
                            new Vector2(10, 70),
                            Color.White);
                        _spriteBatch.DrawString(
                            information,
                            String.Format("Room Pos: {0},{1}", map.X, map.Y),
                            new Vector2(10, 130),
                            Color.White);
                        _spriteBatch.DrawString(
                            information,
                            String.Format("Program Pos: {0},{1}", mapX, mapY),
                            new Vector2(10, 150),
                            Color.White);
                        _spriteBatch.DrawString(
                           information,
                           String.Format("Num Tiles: {0}", map.NumTiles),
                           new Vector2(10, 170),
                           Color.White);
                        _spriteBatch.DrawString(
                           information,
                           String.Format("Difficulty: {0}", map.Diff),
                           new Vector2(10, 190),
                           Color.White);
                        _spriteBatch.DrawString(
                          information,
                          String.Format("Deadend: {0}", map.DeadEnd),
                          new Vector2(10, 210),
                          Color.White);
                        _spriteBatch.DrawString(
                          information,
                          String.Format("Rooms left: {0}", map.UnvSquares),
                          new Vector2(10, 230),
                          Color.White);
                        _spriteBatch.DrawString(
                          information,
                          String.Format("Level: {0}", level),
                          new Vector2(10, 250),
                          Color.White);
                    }

                    //maybe make the menu buttons bob up and down?
                    if (map.DeadEnd)
                    {
                        _spriteBatch.Draw(deadEnd, new Vector2(440, 20), null, Color.White, 0f, new Vector2(0, 0), 3f, SpriteEffects.None, 0f);
                    }

                    if(map.CurrentRoom.RoomType == "shop")
                    {
                        _spriteBatch.Draw(menuButton, new Vector2(600, 200), null, Color.White, 0f, new Vector2(0, 0), 2f, SpriteEffects.None, 0f);
                    }

                    if(map.UnvSquares > 0 && map.CurrentRoom.RoomType == "exit")
                    {
                        _spriteBatch.Draw(clearFloor, new Vector2(420, 600), null, Color.White, 0f, new Vector2(0, 0), 3f, SpriteEffects.None, 0f);
                    }
                    else if (map.CurrentRoom.RoomType == "exit")
                    {
                        _spriteBatch.Draw(menuButton, new Vector2(605, 330), null, Color.White, 0f, new Vector2(0, 0), 2f, SpriteEffects.None, 0f);
                    }

                    // Prints a giant message indicating which level the player is on
                    if (map.CurrentRoom.RoomType == "start")
                    {
                        LevelAnnouncer();
                    }

                    break;

                    /*
                    switch (currentLevelState)
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
                                    0);

                                //drawing for the player
                                player.Draw(_spriteBatch);

                                if (timer < 3f)
                                {
                                    _spriteBatch.DrawString(
                                        debug,
                                        String.Format("LEVEL 1"),
                                        new Vector2(
                                            _graphics.GraphicsDevice.Viewport.Width/2 - 250,
                                            _graphics.GraphicsDevice.Viewport.Height/2 - 50),
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

                                //drawing for the player
                                player.Draw(_spriteBatch);

                                // Display level for 3 seconds
                                if (timer < 3f)
                                {
                                    _spriteBatch.DrawString(
                                        debug,
                                        String.Format("LEVEL 2"),
                                        new Vector2(
                                            _graphics.GraphicsDevice.Viewport.Width / 2 - 250,
                                            _graphics.GraphicsDevice.Viewport.Height / 2 - 50),
                                        Color.White);
                                }
                            }
                            break;
                    }

                    
                    break;
                    */

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
                        String.Format("Invulnerability - {0:F1}", endIFrame),
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

                    // Displays the controls/actions and how to exist the shop.
                    _spriteBatch.DrawString(
                        information,
                        String.Format("Press 'H','M' or 'I' to buy upgrades.\n" +
                        "Press 'SPACE' to return to the game\n" + 
                        "Press 'ENTER' to QUIT to the Main Menu", 
                        player.Movement),
                        new Vector2(30, 280),
                        Color.White);

                    // Dialoge box - For Sprite 4
                    /*
                    _spriteBatch.Draw(
                        rect, 
                        new Vector2(300, 200), 
                        rectangle, 
                        Color.Red, 
                        0f, 
                        new Vector2(0, 0), 
                        4f, 
                        SpriteEffects.None, 
                        1);
                    */

                    // Displays NPC in the shop
                    _spriteBatch.Draw(
                            shrine,
                            new Vector2(700, 150),
                            new Rectangle(0, 0, 400, 400),
                            Color.White,
                            0,
                            Vector2.Zero,
                            3f,
                            SpriteEffects.None,
                            0);

                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Health restored message
                    if (timer < 5f)
                    {
                        _spriteBatch.DrawString(
                            shopMessage,
                            String.Format("You feel a calming presence\n" +
                                          "     as your life is restored"),
                            new Vector2(745, 140),
                            Color.LightBlue);
                    }

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
                            rng.Next(-75, 1100), 
                            rng.Next(0, 575)), 
                        enemyImage, 
                        player, speed));

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

        public void NextRoom()
        {
            

            if (player.X >= 1100 && (player.Y > 250 && player.Y < 290) && mapX < map.NumTiles - 1)
            {
                mapX++;
                map.MoveRoom(player.Position);
                player.X = -74;
            }
            else if (player.X <= -75 && (player.Y > 250 && player.Y < 290) && mapX > 0)
            {
                mapX--;
                map.MoveRoom(player.Position);
                player.X = 1099;
            }
            else if (player.Y <= 0 && (player.X > 490 && player.X < 530) && mapY > 0)
            {
                mapY--;
                map.MoveRoom(player.Position);
                player.Y = 574;
            }
            else if (player.Y >= 575 && (player.X > 490 && player.X < 530) && mapY < map.NumTiles - 1)
            {
                mapY++;
                map.MoveRoom(player.Position);
                player.Y = 1;
            }
            
            nextWave = true;
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

        /// <summary>
        /// File IO readline, reads in player health, move, and invuln stats
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadStats(string fileName)
        {
            StreamReader reader = null;
            string line;
            string[] stats = new string[4];

            try
            {
                reader = new StreamReader(fileName);

                while ((line = reader.ReadLine()) != null)
                {
                    stats = line.Split('|');
                }

                // Assign read in values to stats
                maxHealth = int.Parse(stats[0]);
                player.Health = int.Parse(stats[0]);
                player.Movement = int.Parse(stats[1]);
                endIFrame = double.Parse(stats[2]);
                player.Gold = int.Parse(stats[3]);
            }

            // Writes exceptions to the Output window
            catch (IOException ioe)
            {
                System.Diagnostics.Debug.WriteLine(
                    "IO Error: " + ioe.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "General Error: " + ex.Message);
            }

            // Close the reader
            if (reader != null)
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Saves the player's current stats to a save file
        /// </summary>
        private void SaveGame(string fileName)
        {
            // Open the writer
            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(fileName);

                // Write the current stats to the file
                writer.WriteLine(
                    "{0}|{1}|{2}|{3}", 
                    maxHealth, 
                    player.Movement, 
                    endIFrame,
                    player.Gold);
            }

            // Writes exceptions to the Output window
            catch (IOException ioe)
            {
                System.Diagnostics.Debug.WriteLine(
                    "IO Error: " + ioe.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "General Error: " + ex.Message);
            }

            // Close the writer
            if (writer != null)
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Resets the game pieces when switching between game states
        /// </summary>
        private void SoftReset()
        {
            // Clears current wave
            nextWave = true;
            enemies.Clear();
            enemyHitbox.Clear();

            // Apply health changes
            player.Health = maxHealth;

            // Reset player location and wave
            player.IFrame = false;
            timer = 0;
            currentWave = 0;
        }

        /// <summary>
        /// Resets map position, difficulty, and enemy speed
        /// </summary>
        private void LevelReset()
        {
            // Resets Game Over Details
            player.Score = 0;
            level = 1;

            player.Position = playerPos;

            speed = 1.7f;

            mapX = 1;
            mapY = 1;
            area = 3;
            difficulty = 0;
            map.Diff = 0;
            map.SetUpMap(area, roomList);
        }

        private void NextLevel()
        {
            // Increase the level counter
            level++;

            // Update
            player.Health = maxHealth;
            speed += 0.7f;
            timer = 0;

            // Reset the map
            mapX = 1;
            mapY = 1;
            map.SetUpMap(area, roomList);
            player.Position = new Vector2(
                (GraphicsDevice.Viewport.Width / 2) - 125,
                (GraphicsDevice.Viewport.Height / 2) - 95);
        }

        private void LevelAnnouncer()
        {
            if (timer < 3f)
            {
                _spriteBatch.DrawString(
                    debug,
                    String.Format("LEVEL {0}", level),
                    new Vector2(
                        _graphics.GraphicsDevice.Viewport.Width / 2 - 250,
                        _graphics.GraphicsDevice.Viewport.Height / 2 - 50),
                    Color.White);
            }
        }

        #endregion
    }
}
