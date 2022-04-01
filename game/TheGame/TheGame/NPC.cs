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
        /*                  
        public void dialogeClass(SpriteBatch _spriteBatch, SpriteFont information, Player player, Rectangle rect)
        {
            // Box for text
            _spriteBatch.Draw(null, new Vector2(100, 100), rect, Color.Red, 0f, new Vector2(0, 0), 2f, SpriteEffects.None, 1);

            // Text
            _spriteBatch.DrawString(
                information,
                String.Format("Press 'H','M' or 'I' to buy upgrades.\n" +
                "Press 'ENTER' to exist the shop", player.Movement),
                new Vector2(30, 300),
                Color.White);
        }
        */
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
