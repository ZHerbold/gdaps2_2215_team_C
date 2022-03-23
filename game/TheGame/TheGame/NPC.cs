using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheGame
{
    class NPC: Object
    {
        // Animation fields
        private const int IdleFrameCount = 7;
        private int frame;
        private double fps;
        private double timePerFrame;
        private double timeCounter;

        // Constructor --------------------------------------------------------
        public NPC(Vector2 position, Texture image)
        {
            this.fps = 10;
            this.timePerFrame = 1.0 / fps;
        }

        // Dialoge ------------------------------------------------------------
        public void dialogeClass()
        {

        }

        // Animation Updater --------------------------------------------------
        public void UpdateAnimation(GameTime gameTime)
        {
            // Time passed
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeCounter >= timePerFrame)
            {
                frame += 1;

                if (frame > IdleFrameCount)
                    frame = 0;

                timeCounter -= timePerFrame;
            }
        }

        // Draw ---------------------------------------------------------------
        public void Draw(SpriteBatch spriteBatch)
        {
            // add code to make the NPC be able to be animated
        }
    }
}
