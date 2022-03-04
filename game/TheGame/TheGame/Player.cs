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
        WalkUp,
        WalkDown,
        Attack
    }

    class Player : GameObject
    {
        // Fields--------------------------------------------------------------
        private int gold;
        PlayerState state;
        private bool faceRight;
        private KeyboardState prevKBstate;

        //The dimensions for each frame;
        private const int frameWidth = 100;
        private const int frameHeight = 55;

        private const int movement = 4;

        // Properties ---------------------------------------------------------
        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        // Constructor --------------------------------------------------------
        public Player(int health, Vector2 position, Texture2D image, int gold, 
            PlayerState startingState) : 
            base(health, position, image)
        {
            this.gold = gold;
            this.state = startingState;
            faceRight = true;

        }



        // Methods ------------------------------------------------------------
        //override method to call when update is called in game1
        //PUT CODE YOU WANT TO CALL DURING THE GAME HERE
        public override void Update(GameTime gameTime)
        {
            //gets a keyboard state for controls
            KeyboardState kbState = Keyboard.GetState();

            //Put FSM Logic for stats here HERE
            /* template
             if (kbState.IsKeyDown(Keys.))
                    {
                        State = PlayerState.;
                    }
            */
            switch(State)
            {
                case PlayerState.FaceRight:
                    faceRight = true;   //for sprite flipping purposes
                    if(kbState.IsKeyDown(Keys.D))
                    {
                        State = PlayerState.WalkRight;
                    }
                    if(kbState.IsKeyDown(Keys.A))
                    {
                        State = PlayerState.FaceLeft;
                    }
                    if (kbState.IsKeyDown(Keys.W))
                    {
                        State = PlayerState.WalkUp;
                    }
                    if (kbState.IsKeyDown(Keys.S))
                    {
                        State = PlayerState.WalkDown;
                    }
                    break;
                case PlayerState.FaceLeft:
                    faceRight = false;  //for sprite flipping purposes
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        State = PlayerState.FaceRight;
                    }
                    if (kbState.IsKeyDown(Keys.A))
                    {
                        State = PlayerState.WalkLeft;
                    }
                    if (kbState.IsKeyDown(Keys.W))
                    {
                        State = PlayerState.WalkUp;
                    }
                    if (kbState.IsKeyDown(Keys.S))
                    {
                        State = PlayerState.WalkDown;
                    }
                    break;
                case PlayerState.WalkRight:
                    position.X += movement; //movement in the direction specified

                    //adds diagonal movement and prevents "moon walking"
                    if (kbState.IsKeyDown(Keys.W))
                    {
                        position.Y -= movement;
                    }
                    if (kbState.IsKeyDown(Keys.S))
                    {
                        position.Y += movement;
                    }

                    if (kbState.IsKeyUp(Keys.D) && prevKBstate.IsKeyDown(Keys.D))
                    {
                        State = PlayerState.FaceRight;
                    }
                    break;
                case PlayerState.WalkLeft:
                    position.X -= movement; //movement in the direction specified

                    //adds diagonal movement and prevents "moon walking"
                    if (kbState.IsKeyDown(Keys.W))
                    {
                        position.Y -= movement;
                    }
                    if (kbState.IsKeyDown(Keys.S))
                    {
                        position.Y += movement;
                    }

                    if (kbState.IsKeyUp(Keys.A) && prevKBstate.IsKeyDown(Keys.A))
                    {
                        State = PlayerState.FaceLeft;
                    }
                    break;
                case PlayerState.WalkUp:
                    position.Y -= movement; //movement in the direction specified

                    //adds diagonal movement and prevents "moon walking"
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        faceRight = true;
                        position.X += movement;
                    }
                    if (kbState.IsKeyDown(Keys.A))
                    {
                        faceRight = false;
                        position.X -= movement;
                    }

                    //checks to see if both buttons are not pressed
                    if ((kbState.IsKeyUp(Keys.A) && prevKBstate.IsKeyDown(Keys.A)) || (kbState.IsKeyUp(Keys.W) && prevKBstate.IsKeyDown(Keys.W)) && !faceRight)
                    {
                        State = PlayerState.FaceLeft;
                    }
                    else if ((kbState.IsKeyUp(Keys.D) && prevKBstate.IsKeyDown(Keys.D)) || (kbState.IsKeyUp(Keys.W) && prevKBstate.IsKeyDown(Keys.W)) && faceRight)
                    {
                        State = PlayerState.FaceRight;
                    }
                    break;
                case PlayerState.WalkDown:
                    position.Y += movement; //movement in the direction specified

                    //adds diagonal movement and prevents "moon walking"
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        faceRight = true;
                        position.X += movement;
                    }
                    if (kbState.IsKeyDown(Keys.A))
                    {
                        faceRight = false;
                        position.X -= movement;
                    }

                    //checks to see if both buttons are not pressed
                    if ((kbState.IsKeyUp(Keys.A) && prevKBstate.IsKeyDown(Keys.A)) || (kbState.IsKeyUp(Keys.S) && prevKBstate.IsKeyDown(Keys.S)) && !faceRight)
                    {
                        State = PlayerState.FaceLeft;
                    }
                    else if ((kbState.IsKeyUp(Keys.D) && prevKBstate.IsKeyDown(Keys.D)) || (kbState.IsKeyUp(Keys.S) && prevKBstate.IsKeyDown(Keys.S)) && faceRight)
                    {
                        State = PlayerState.FaceRight;
                    }
                    break;
                case PlayerState.Attack:    //Cant get here yet.
                    break;
            }



            prevKBstate = kbState;
        }

        //calls during draw method in Game1
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
                case PlayerState.WalkUp:
                    if(faceRight) //uses this to keep the orrientation it last had
                    {
                        DrawWalking(SpriteEffects.None, spriteBatch);
                    }
                    else
                    {
                        DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    }
                    break;
                case PlayerState.WalkDown:
                    if (faceRight)  //uses this to keep the orrientation it last had
                    {
                        DrawWalking(SpriteEffects.None, spriteBatch);
                    }
                    else
                    {
                        DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    }
                    break;
            }
        }

        public void DrawIdle(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image,                                 //the actual image
                position,                                           //where it is
                new Rectangle(0, 0, frameWidth, frameHeight),       //cropping the image to a certain size and place (USE THIS FOR ANIMATION WITH THE SPRITE SHEET)
                Color.White,                                        //Color
                0,                                                  //amount of rotation (we dont need most likely
                Vector2.Zero,                                       //axis on which it rotates (""      "")
                2.5f,                                               //Scale of the image. its kinda blurry, so if anyone knows how to fix it, be my guest.
                flipSprite,                                         //sprite effect
                0);                                                 //layer, make sure it is above the background
        }

        public void DrawWalking(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            //place holder for now, until animations are in place.
            spriteBatch.Draw(image, position, new Rectangle(0, 0, frameWidth, frameHeight), Color.White, 0, Vector2.Zero, 2.5f, flipSprite, 0);
        }

        public void DrawAttack(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {

        }

        // TakeDamage override that draws depleted heart or etc.
        // ONE FOR EACH ATTACK TYPE, only 2 for now
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
        }
    }
}
