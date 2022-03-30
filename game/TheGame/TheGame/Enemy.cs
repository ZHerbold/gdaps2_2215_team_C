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
        AttackLeft,
        AttackRight,
        DyingLeft,
        DyingRight
    }

    class Enemy : GameObject
    {
        // Fields
        EnemyState state;
        EnemyState previousState;
        private Player player;           // Used for chasing player
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
        private const int SpriteSheetWidth = 5;
        private const int EnemyOffsetX = (spriteSheetWidth / 6)*5;

        // Frame dimentions
        private const int frameWidth = spriteSheetWidth / 6;
        private const int frameHeight = spriteSheetHeight / 6;

        // Properties
        public bool IsDead
        {
            get { return isDead; }
        }
        public EnemyState State
        {
            get { return state; }
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

                if (frame > SpriteSheetWidth)     
                    frame = 1;

                timeCounter -= timePerFrame;     
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!isDead)
            {
                // If the skeleton is not moving, draw it standing
                if (currentDistance < followDistance &&
                    (player.Position.X < this.position.X))
                {
                    state = EnemyState.AttackLeft;
                }
                else if (currentDistance < followDistance &&
                    (player.Position.X > this.position.X))
                {
                    state = EnemyState.AttackRight;
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

            // Changes state after dying
            if (isDead)
            {
                // Die in the direction you were last facing
                if (state == EnemyState.AttackLeft ||
                    state == EnemyState.WalkLeft)
                {
                    frame = 0;
                    state = EnemyState.DyingLeft;
                }
                else if (state == EnemyState.AttackRight ||
                         state == EnemyState.WalkRight)
                {
                    frame = 0;
                    state = EnemyState.DyingRight;
                }

                // Delete at final frame of death animation
                if (frame == 3)
                {
                    active = false;
                }
            }

            // Change the frame to 0 whenever the
            // state switches from walk to attack
            if ((state == EnemyState.AttackLeft || 
                state == EnemyState.AttackRight) && 
                (previousState == EnemyState.WalkLeft || 
                previousState == EnemyState.WalkRight))
            {
                frame = 0;
            }
            previousState = state;

            // Movement logic
            switch (state)
            {
                case EnemyState.WalkLeft:
                case EnemyState.WalkRight:
                    Chase();
                    break;
                
                case EnemyState.AttackLeft:
                    if (frame == SpriteSheetWidth)
                    {
                        X += 60;
                        Chase();
                    }
                    break;

                case EnemyState.AttackRight:
                    if (frame == SpriteSheetWidth)
                    {
                        X -= 60;
                        Chase();
                    }
                    break;
            }
        }

        //we'll eventually need drawing for the enemy to call in Game1
        public void Draw(SpriteBatch spriteBatch)
        {
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

                case EnemyState.AttackLeft:
                    DrawAttack(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case EnemyState.AttackRight:
                    DrawAttack(SpriteEffects.None, spriteBatch);
                    break;

                case EnemyState.DyingLeft:
                    DrawDying(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case EnemyState.DyingRight:
                    DrawDying(SpriteEffects.None, spriteBatch);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Update gold, the active property
        /// </summary>
        public void Die()
        {
            isDead = true;
            player.Gold += 5;
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
        /// Draws the enemy's attack cycle
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sprite"></param>
        public void DrawAttack(SpriteEffects flipSprite, SpriteBatch sprite)
        {
            sprite.Draw(
                image,
                position,
                new Rectangle(
                    frameWidth * frame,
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
        /// Draws the enemy's death animation
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sprite"></param>
        public void DrawDying(SpriteEffects flipSprite, SpriteBatch sprite)
        {            
            sprite.Draw(
                image,
                position,
                new Rectangle(
                    frameWidth * frame,
                    frameHeight * 4,
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
        /// Gets the distance betweent the enemy and the player and goes towards the player
        /// </summary>
        public void Chase()
        {
            //FIXME: enemy goes to players back foot instead of middle of model
            followDistance = 60.0f;            // How close the enemy will get to the player before stopping
            float speed = 1.7f;             // Speed the enemy moves towards the player
            
            // get distance between enemy and player
            Vector2 newPos = new Vector2(X, Y+frameHeight/2);
            distance = player.Position - newPos;
            
            // get the angle and direction between the enemy and player
            float angle = MathF.Atan2(distance.Y, distance.X);
            direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            
            //Get the current distance between the player and enemy
            currentDistance = Vector2.Distance(player.Position, position);
            
            // Move the enemy towards the player
            if (currentDistance > followDistance)
            {
                float t = MathHelper.Min(MathF.Abs(currentDistance - followDistance), speed);
                Vector2 velocity = direction * speed;
                Position += velocity;
            }

            //float speed = 2.3f;
            //distanceX = player.X - X;
            //distanceY = player.Y - (Y + frameHeight / 2);
            //
            //    // Make the enemy move in the coresponding direction towards the player for both planes
            //    if (distanceX > 2)
            //    {
            //        //sameXPlane = true;
            //        X += speed;
            //    }
            //    if (distanceX < -2)
            //    {
            //        //YSpeed *= -1;
            //        //sameYPlane = false;
            //        X += speed * -1;
            //    }
            //    if (distanceY > 2)
            //    {
            //        //YSpeed *= -1;
            //        //sameYPlane = false;
            //        Y += speed;
            //    }
            //    if (distanceY < -2)
            //    {
            //        //sameYPlane = true;
            //        Y += speed * -1;
            //    }
        }
    }
}
