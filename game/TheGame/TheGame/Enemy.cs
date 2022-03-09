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
        FaceUp,
        FaceDown,
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight,
        Attack,
        Chase
    }

    class Enemy : GameObject
    {
        // FOR THE SKELETON WE ONLY HAVE 1 TYPE OF ENEMY

        // Fields???
        EnemyState state = EnemyState.FaceLeft;
        private Player player;                  // Used for chasing player and giving gold on death
        private float distanceX;
        private float distanceY;
        private Vector2 distance;
        private Vector2 direction;
        private bool isDead;
        

        // Frame dimentions
        private int frameWidth = 50;
        private int frameHeight = 128;
        

        // Animation
        //int frame;
        //double timeCounter;
        //double fps;
        //double timePerFrame;
        //
        //// Constants for animation
        //const int WalkCountFrame = 6;
        //const int EnemyOffsetX = 

        // Properties
        public bool IsDead
        {
            get { return isDead; }
        }

        // Constructor
        public Enemy(int health, Vector2 position, Texture2D image, Player player) : base(health, position, image)
        {            
            this.player = player;
        }

        // Methods
        //override method to call when update is called in game1
        //PUT CODE YOU WANT TO CALL DURING THE GAME HERE
        public override void Update(GameTime gameTime)
        {
            Chase();
            switch (state)
            {
                case EnemyState.FaceUp:
                    break;
                case EnemyState.FaceDown:
                    break;
                case EnemyState.FaceLeft:
                    break;
                case EnemyState.FaceRight:
                    break;
                case EnemyState.WalkLeft:
                    break;
                case EnemyState.WalkRight:
                    break;
                case EnemyState.Attack:
                    break;
                default:
                    break;
            }
        }

        //we'll eventually need drawing for the enemy to call in Game1
        public void Draw(SpriteBatch spriteBatch, SpriteFont debug)
        {
            spriteBatch.DrawString(debug, String.Format(
                "X distance: {0}\n" +
                "Y distance: {1}\n"
                , distanceX, distanceY)
                , new Vector2(10, 10), Color.White);
            switch (state)
            {
                case EnemyState.FaceUp:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
                case EnemyState.FaceDown:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
                case EnemyState.FaceLeft:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
                case EnemyState.FaceRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
                case EnemyState.WalkLeft:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
                case EnemyState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
                default:
                    break;
            }
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
            
            float followDistance = 25f;     // How close the enemy will get to the player before stopping
            float speed = 2.3f;             // Speed the enemy moves towards the player

            // get distance between enemy and player
            distance = player.Position - Position;
            
            // get the angle and direction between the enemy and player
            float angle = MathF.Atan2(distance.Y, distance.X);
            direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            
            //Get the current distance between the player and enemy
            float currentDistance = Vector2.Distance(position, player.Position);
            
            // Move the enemy towards the player
            if (currentDistance > followDistance)
            {
                //float t = MathHelper.Min(MathF.Abs(currentDistance - followDistance), speed);
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

        public void DrawWalking(SpriteEffects flipSprite, SpriteBatch sprite)
        {
            sprite.Draw(image,                                  //the actual image
            position,                                           //where it is
            new Rectangle(0, 0, frameWidth, frameHeight),       //cropping the image to a certain size and place (USE THIS FOR ANIMATION WITH THE SPRITE SHEET)
            Color.White,                                        //Color
            0,                                                  //amount of rotation (we dont need most likely
            Vector2.Zero,                                       //axis on which it rotates (""      "")
            2f,                                                 //Scale of the image. its kinda blurry, so if anyone knows how to fix it, be my guest.
            flipSprite,                                         //sprite effect
            0);                                                 //layer, make sure it is above the background
        }

        public void Die()
        {
            isDead = true;
            player.Gold += 5;
            // Remove the enemy from the game
        }
    }
}
