using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheGame
{
    class Room
    {
        // Fields
        private int waveCount;
        private int currentWave;
        private Texture2D texture;
        private bool allDead;
        private string roomType;
        private Vector2 backgroundPos = new Vector2(4, -25); //constant

        // properties
        public int WaveCount { get { return waveCount; } }

        public int CurrentWave { set { currentWave = value; } }

        public bool AllDead { get { return allDead; } set { allDead = value; } }

        public string RoomType { get { return roomType; } }

        // Constructor
        public Room(int waveCount, Texture2D texture, string roomType)
        {
            currentWave = 0;
            this.waveCount = waveCount;
            this.texture = texture;
            if (roomType == "exit" || roomType == "shop" || roomType == "start")
                allDead = true;
            else
                allDead = false;
            this.roomType = roomType;
        }

        // Methods
        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                                    texture,
                                    backgroundPos,
                                    null,
                                    Color.White,
                                    0f,
                                    new Vector2(0, 0),
                                    1.99f,
                                    SpriteEffects.None,
                                    0);

        }

    }
}
