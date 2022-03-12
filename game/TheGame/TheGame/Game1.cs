using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private int playerIHealth;      // initial player health, raised via shop
        private Texture2D playerImage;
        private Vector2 playerPos;
        private int gold;

        //Enemy Fields
        private Enemy enemy;
        private Texture2D enemyImage;
        private Vector2 enemyPos;
        private List<Enemy> enemies;
        private int enemyHealth;

        //Background fields;
        private Texture2D background;
        private Vector2 backgroundPos;

        //Debug
        private SpriteFont debug;

        //Game info
        private int currentWave;

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
            currentWave = 1;

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

            //Debug font
            debug = Content.Load<SpriteFont>("Debug");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            enemy.UpdateAnimation(gameTime);
            player.UpdateAnimation(gameTime);

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



            //calls player update.
            player.Update(gameTime);

            //calls enemy update
            enemy.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

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

            //drawing for the player
            player.Draw(_spriteBatch);

            //drawing for the enemy
            enemy.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void NextWave()
        {
            enemyHealth = currentWave * currentWave;
            if (enemies != null)
            {
                enemies.Clear();
            }
            for (int i = 0; i < currentWave; i++)
            {
                // FIXME: add enemy position
                enemies.Add(new Enemy(enemyHealth,enemyPos ,enemyImage, player));
            }
        }
    }
}
