using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        // Fields
        private Player player;
        private int playerIHealth; // initial player health, raised via shop

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


            // Set the window size
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
