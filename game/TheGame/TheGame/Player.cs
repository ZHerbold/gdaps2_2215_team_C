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
        AttackRight,
        AttackLeft,
        Roll
    }

    class Player : GameObject
    {
        // Fields--------------------------------------------------------------
        private int gold;
        private int score;
        private bool ability;
        private Dictionary<int, string> abilityDict;
        private int typeOfAbility;
        PlayerState state;
        private KeyboardState prevKBstate;
        private MouseState prevMState;

        //The dimensions for each frame;
        private const int FrameWidth = 100;
        private const int FrameHeight = 55;

        private int movement = 5;

        // Animation
        private const int SpriteSheetWidth = 7;
        private const int AttackFrameCount = 7;
        private const int PlayerWalkOffsetY = 55;
        private const int PlayerAttackOffsetY = 165;

        private int frame;
        private double timeCounter;
        private double fps;
        private double timePerFrame;

        private bool iFrame;
        private Color color;

        // Properties ---------------------------------------------------------
        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        public int Frame
        {
            get { return frame; }
        }

        public bool IFrame 
        { 
            get { return iFrame; } 
            set { iFrame = value; }
        }

        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public bool Ability
        {
            get { return ability; }
            set { ability = value; }
        }

        // Can be modified at the shop
        public int Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        public int TypeOfAbility
        {
            get { return typeOfAbility; }
            set { typeOfAbility = value; }
        }

        // Constructor --------------------------------------------------------
        public Player(int health, Vector2 position, Texture2D image, int gold, 
            PlayerState startingState) : 
            base(health, position, image)
        {

            abilityDict = new Dictionary<int, string>();
            // Abilities
            abilityDict.Add(1, "dodge");
            abilityDict.Add(0, "potion");
            abilityDict.Add(2, "shield");
            ability = false;

            this.gold = gold;
            
            this.state = startingState;

            iFrame = false;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
        }

        // Methods ------------------------------------------------------------
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
            // Clicking while attacking does nothing
            if (
                (
                (prevMState.LeftButton != ButtonState.Pressed && mState.LeftButton == ButtonState.Pressed) 
                
                ||                

                (kbState.IsKeyDown(Keys.K) && !prevKBstate.IsKeyDown(Keys.K)
                ) 
                
                &&

                (state != PlayerState.AttackRight && 
                state != PlayerState.AttackLeft)))
            {
                // set initial animation marker
                frame = 0;

                // Attack in a direction dependent on previous state
                // ---- RIGHT ATTACK ----
                if (state == PlayerState.FaceRight ||
                    state == PlayerState.WalkRight)
                {
                    state = PlayerState.AttackRight;
                }

                // ---- LEFT ATTACK ----
                else if (state == PlayerState.FaceLeft ||
                         state == PlayerState.WalkLeft)
                {
                    state = PlayerState.AttackLeft;
                }
            }

            // Must finish an attack before moving again
            if (state != PlayerState.AttackRight && 
                state != PlayerState.AttackLeft)
            {
                // ---- MOVE RIGHT ----
                if (kbState.IsKeyDown(Keys.D))
                {
                    State = PlayerState.WalkRight;
                    position.X += movement; //movement in the direction specified
                    if (position.X > 1100)
                        position.X = 1100;
                }
                // ---- MOVE LEFT ----
                else if (kbState.IsKeyDown(Keys.A))
                {
                    State = PlayerState.WalkLeft;
                    position.X -= movement;
                    if (position.X < -75)
                        position.X = -75;
                }
                // ---- MOVE UP ----
                if (kbState.IsKeyDown(Keys.W))
                {
                    position.Y -= movement;
                    if (position.Y < 0)
                        position.Y = 0;

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
                    if (position.Y > 575)
                        position.Y = 575;

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

            // End attack animations on the last frame of them
            switch (state)
            {
                case PlayerState.AttackRight:

                    // Stop attack animations
                    if (frame == AttackFrameCount)
                    {
                        state = PlayerState.FaceRight;
                    }
                    break;

                case PlayerState.AttackLeft:

                    // Stop attack animations
                    if (frame == AttackFrameCount)
                    {
                        state = PlayerState.FaceLeft;
                    }
                    break;
            }

            // Visual representation of active iFrames
            if (iFrame)
            {
                color = Color.Red;
            }
            else
            {
                color = Color.White;
            }

            // Update previous mouse/keyboard states
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

                if (frame > SpriteSheetWidth)
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

                case PlayerState.AttackRight:
                    DrawAttack(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.AttackLeft:
                    DrawAttack(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;
            }
        }

        public void DrawIdle(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image,                               
                position,
                new Rectangle(
                    frame * FrameWidth,
                    0,
                    FrameWidth,
                    FrameHeight),
                color,                                       
                0,                          
                Vector2.Zero, 
                2.5f,
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
                    color,
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
                    color,
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
                color,
                0,
                Vector2.Zero,
                2.5f,
                flipSprite,
                0);
        }
    }
}
