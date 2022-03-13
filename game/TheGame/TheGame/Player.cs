using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheGame
{
    enum PlayerState
    {
        FaceRight,
        FaceLeft,
        WalkRight,
        WalkLeft,
        Attack
    }

    class Player : GameObject
    {
        // Fields--------------------------------------------------------------
        private int gold;
        PlayerState state;
        private KeyboardState prevKBstate;
        private MouseState prevMState;

        //The dimensions for each frame;
        private const int FrameWidth = 100;
        private const int FrameHeight = 55;

        private const int movement = 4;

        // Animation
        private const int WalkFrameCount = 7;
        private const int AttackFrameCount = 7;
        private const int PlayerWalkOffsetY = 55;
        private const int PlayerAttackOffsetY = 165;

        private int frame;
        private double timeCounter;
        private double fps;
        private double timePerFrame;

        // Properties ---------------------------------------------------------
        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }
        // Constructor --------------------------------------------------------
        public Player(int health, Vector2 position, Texture2D image, int gold, 
            PlayerState startingState) : 
            base(health, position, image)
        {
            this.gold = gold;
            this.state = startingState;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }

        // Methods ------------------------------------------------------------
        //override method to call when update is called in game1
        //PUT CODE YOU WANT TO CALL DURING THE GAME HERE

        /// <summary>
        /// Controls movement and FSM
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //gets a keyboard state for controls
            KeyboardState kbState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            //Put FSM Logic for stats here HERE
            /* template
             if (kbState.IsKeyDown(Keys.))
                    {
                        State = PlayerState.;
                    }
            */

            // Click to attack, holding m1 does nothing
            if (prevMState.LeftButton != ButtonState.Pressed && 
                mState.LeftButton == ButtonState.Pressed)
            {
                MeleeAttack();
            }

            // Must finish an attack before moving again
            if (state != PlayerState.Attack)
            {
                // ---- MOVE RIGHT ----
                if (kbState.IsKeyDown(Keys.D))
                {
                    State = PlayerState.WalkRight;
                    position.X += movement; //movement in the direction specified
                }
                // ---- MOVE LEFT ----
                else if (kbState.IsKeyDown(Keys.A))
                {
                    State = PlayerState.WalkLeft;
                    position.X -= movement;
                }
                // ---- MOVE UP ----
                if (kbState.IsKeyDown(Keys.W))
                {
                    position.Y -= movement;

                    // Determine which direction to face when walking
                    if (prevKBstate.IsKeyDown(Keys.D) ||
                        state == PlayerState.FaceRight)
                    {
                        State = PlayerState.WalkRight;
                    }
                    else if (prevKBstate.IsKeyDown(Keys.A) ||
                        state == PlayerState.FaceLeft)
                    {
                        State = PlayerState.WalkLeft;
                    }
                }
                // ---- MOVE DOWN ----
                else if (kbState.IsKeyDown(Keys.S))
                {
                    position.Y += movement;

                    // Determine which direction to face when walking
                    if (prevKBstate.IsKeyDown(Keys.D) ||
                        state == PlayerState.FaceRight)
                    {
                        State = PlayerState.WalkRight;
                    }
                    else if (prevKBstate.IsKeyDown(Keys.A) ||
                        state == PlayerState.FaceLeft)
                    {
                        State = PlayerState.WalkLeft;
                    }
                }

                // ---- FACE RIGHT ----
                if (prevKBstate.IsKeyDown(Keys.D) &&
                    !kbState.IsKeyDown(Keys.D))
                {
                    State = PlayerState.FaceRight;
                }

                // ---- FACE LEFT ----
                else if (prevKBstate.IsKeyDown(Keys.A) &&
                    !kbState.IsKeyDown(Keys.A))
                {
                    State = PlayerState.FaceLeft;
                }

                // ---- FACE RIGHT ---- (AFTER WALKING UP)
                else if (prevKBstate.IsKeyDown(Keys.W) &&
                    !kbState.IsKeyDown(Keys.W) &&
                    state == PlayerState.WalkRight)
                {
                    State = PlayerState.FaceRight;
                }

                // ---- FACE LEFT ---- (AFTER WALKING UP)
                else if (prevKBstate.IsKeyDown(Keys.W) &&
                    !kbState.IsKeyDown(Keys.W) &&
                    state == PlayerState.WalkLeft)
                {
                    State = PlayerState.FaceLeft;
                }

                // ---- FACE RIGHT ---- (AFTER WALKING DOWN)
                else if (prevKBstate.IsKeyDown(Keys.S) &&
                    !kbState.IsKeyDown(Keys.S) &&
                    state == PlayerState.WalkRight)
                {
                    State = PlayerState.FaceRight;
                }

                // ---- FACE LEFT ---- (AFTER WALKING DOWN)
                else if (prevKBstate.IsKeyDown(Keys.S) &&
                    !kbState.IsKeyDown(Keys.S) &&
                    state == PlayerState.WalkLeft)
                {
                    State = PlayerState.FaceLeft;
                }
            }            

            prevKBstate = kbState;
            prevMState = mState;
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            // Time passed
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeCounter >= timePerFrame)
            {
                frame += 1;

                if (frame > WalkFrameCount)
                    frame = 0;

                timeCounter -= timePerFrame;
            }
        }

        // Calls during draw method in Game1
        public void Draw(SpriteBatch spriteBatch)
        {            
            switch (State)
            {
                case PlayerState.FaceRight:
                    DrawIdle(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.FaceLeft:
                    DrawIdle(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.WalkLeft:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.Attack:
                    DrawAttack(SpriteEffects.None, spriteBatch);
                    break;
            }
        }

        public void DrawIdle(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image,                               
                position,  
                new Rectangle(
                    0, 
                    0, 
                    FrameWidth, 
                    FrameHeight),
                Color.White,                                       
                0,                          
                Vector2.Zero, 
                2.5f,           //Scale of the image. its kinda blurry, so if anyone knows how to fix it, be my guest.
                flipSprite,
                0);             //layer, make sure it is above the background
        }

        public void DrawWalking(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            if (frame < 8)
            {
                spriteBatch.Draw(
                    image,
                    position,
                    new Rectangle(
                        (frame * FrameWidth),
                        PlayerWalkOffsetY,
                        FrameWidth,
                        FrameHeight),
                    Color.White,
                    0,
                    Vector2.Zero,
                    2.5f,
                    flipSprite,
                    0);
            }
            else
            {
                spriteBatch.Draw(
                    image,
                    position,
                    new Rectangle(
                        (frame * FrameWidth),
                        0,
                        FrameWidth,
                        FrameHeight),
                    Color.White,
                    0,
                    Vector2.Zero,
                    2.5f,
                    flipSprite,
                    0);
            }
        }

        public void DrawAttack(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                image,
                position,
                new Rectangle(
                    (frame * FrameWidth) + 1,
                    PlayerAttackOffsetY,
                    FrameWidth,
                    FrameHeight),
                Color.White,
                0,
                Vector2.Zero,
                2.5f,
                flipSprite,
                0);
        }

        /// <summary>
        /// Simulate a ranged attack
        /// </summary>
        public void RangedAttack()
        {
            base.Attack();
        }

        /// <summary>
        /// Simulate a melee attack
        /// </summary>
        public void MeleeAttack()
        {
            base.Attack();
            state = PlayerState.Attack;
        }
    }
}
