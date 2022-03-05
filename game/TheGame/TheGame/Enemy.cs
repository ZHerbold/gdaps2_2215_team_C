﻿using System;
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
        private float XSpeed = 1.7f;
        private float YSpeed = 1.7f;
        private Player player;
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
        public void Draw(SpriteBatch spriteBatch)
        {
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
            X += XSpeed;
            Y += YSpeed;
            float distanceX = player.X - (X - 30);
            float distanceY = player.Y - (Y + frameHeight/2);
            if (distanceX > 0 && XSpeed < 0)
            {
                XSpeed *= -1;
            }
            if (distanceX < 0 && XSpeed > 0)
            {
                XSpeed *= -1;
            }
            if (distanceY > 0 && YSpeed < 0)
            {
                YSpeed *= -1;
            }
            if (distanceY < 0 && YSpeed > 0)
            {
                YSpeed *= -1;
            }
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
            // Give the player gold
            // Remove the enemy from the game
        }
    }
}
