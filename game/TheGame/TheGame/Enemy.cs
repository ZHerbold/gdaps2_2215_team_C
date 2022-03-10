using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheGame
{
    // Works while we only have 1 enemy type
    enum EnemyState
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight,
        Attack,
        Chase
    }

    class Enemy : GameObject
    {
        // Fields
        EnemyState state;
        private Player player;      // Used for chasing player
        private float distanceX;
        private float distanceY;
        private float followDistance;
        private float currentDistance;
        private Vector2 distance;
        private Vector2 direction;
        private bool isDead;        

        // Animation
        private int frame;
        private double timeCounter;
        private double fps;
        private double timePerFrame;

        // Constants for animation
        private const int spriteSheetWidth = 768;
        private const int spriteSheetHeight = 576;
        private const int WalkFrameCount = 5;
        private const int EnemyOffsetX = (spriteSheetWidth / 6)*5;

        // Frame dimentions
        private const int frameWidth = spriteSheetWidth / 6;
        private const int frameHeight = spriteSheetHeight / 6;

        // Properties
        public bool IsDead
        {
            get { return isDead; }
        }

        // Constructor
        public Enemy(int health, Vector2 position, Texture2D image, Player player) : base(health, position, image)
        {            
            this.player = player;

            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // The amount of time in a single walk image
        }

        // Methods
        //override method to call when update is called in game1
        //PUT CODE YOU WANT TO CALL DURING THE GAME HERE

        /// <summary>
        /// Update the skeleton's animations as needed
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Time passed
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            //      Adjust the frame to the next image
            //      Check the bounds - have we reached the end of walk cycle?
            //      Back to 1 (since 0 is the "standing" frame)
            //      Remove the time we "used" - don't reset to 0
            // This keeps the time passed 
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     

                if (frame > WalkFrameCount)     
                    frame = 1;

                timeCounter -= timePerFrame;     
            }
        }

        public override void Update(GameTime gameTime)
        {
            Chase();

            // If the skeleton is not moving, draw it standing
            if (currentDistance < followDistance && 
                (player.Position.X < this.position.X))
            {
                state = EnemyState.FaceLeft;
            }
            else if (currentDistance < followDistance &&
                (player.Position.X > this.position.X))
            {
                state = EnemyState.FaceRight;
            }

            // Change state depending on which direction the skeleton is facing
            else if (player.Position.X < this.position.X)
            {
                state = EnemyState.WalkLeft;
            }
            else if (player.Position.X > this.position.X)
            {
                state = EnemyState.WalkRight;
            }
        }

        //we'll eventually need drawing for the enemy to call in Game1
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.DrawString(debug, String.Format(
            //    "X distance: {0}\n" +
            //    "Y distance: {1}\n"
            //    , distanceX, distanceY)
            //    , new Vector2(10, 10), Color.White);

            switch (state)
            {
                case EnemyState.FaceLeft:
                    DrawStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case EnemyState.FaceRight:
                    DrawStanding(SpriteEffects.None, spriteBatch);
                    break;

                case EnemyState.WalkLeft:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case EnemyState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the enemy standing still
        /// </summary>
        public void DrawStanding(SpriteEffects flipSprite, SpriteBatch sprite)
        {
            sprite.Draw(
                image, 
                position, 
                new Rectangle(
                    0,
                    0,
                    frameWidth,
                    frameHeight), 
                Color.White, 
                0, 
                Vector2.Zero, 
                2f, 
                flipSprite, 
                0);
        }

        /// <summary>
        /// Draws the enemy's walk cycle
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sprite"></param>
        public void DrawWalking(SpriteEffects flipSprite, SpriteBatch sprite)
        {
            sprite.Draw(
                image,
                position,
                new Rectangle(
                    EnemyOffsetX,
                    (frame * frameHeight) + 1,
                    frameWidth,
                    frameHeight),       //cropping the image to a certain size and place (USE THIS FOR ANIMATION WITH THE SPRITE SHEET)
                Color.White,            //Color
                0,                      //amount of rotation (we dont need most likely
                Vector2.Zero,           //axis on which it rotates (""      "")
                2f,                     //Scale of the image. its kinda blurry, so if anyone knows how to fix it, be my guest.
                flipSprite,             //sprite effect
                0);                     //layer, make sure it is above the background
        }

        /// <summary>
        /// Gets the distance betweent the enemy and the player and goes towards the player
        /// </summary>
        public void Chase()
        {
            /*
             * OPTION 1: always follows player in a full 360 degree motion
             * OPTION 2: Follows player only in one X/Y plane at a time
            */

            // Get the distance between the player and enemy on both planes
            distanceX = player.X - (X - 30);
            distanceY = player.Y - (Y + frameHeight / 2);
            
            followDistance = 25f;     // How close the enemy will get to the player before stopping
            float speed = 2.3f;             // Speed the enemy moves towards the player

            // get distance between enemy and player
            distance = player.Position - Position;
            
            // get the angle and direction between the enemy and player
            float angle = MathF.Atan2(distance.Y, distance.X);
            direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            
            //Get the current distance between the player and enemy
            currentDistance = Vector2.Distance(position, player.Position);

            // Move the enemy towards the player
            if (currentDistance > followDistance)
            {
                float t = MathHelper.Min(MathF.Abs(currentDistance - followDistance), speed);
                Vector2 velocity = direction * speed;
                Position += velocity;
            }
            
            // Only move the enemy in one direction
            // FIXME: change y-speed variable name, when the distances are the same enemy moves diagonally
            /*
            float spewd = 2.3f;
            if (distanceX < -2)
            {
                speed *= -1;
            }
            if (distanceY < -2)
            {
                spewd *= -1;
            }
            if (Math.Abs(distanceX) >= Math.Abs(distanceY))
            {
                X += speed;
            }
            if (Math.Abs(distanceY) > Math.Abs(distanceX))
            {
                Y += spewd;
            }
            */

        }

        public void Die()
        {
            isDead = true;
            player.Gold += 5;
            // Remove the enemy from the game
        }
    }
}
