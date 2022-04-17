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
        private int enemyCount;
        private Texture2D texture;
        private bool isActive;

        // properties
        public int EnemyCount { get { return enemyCount; } }

        public bool IsActive { get { return isActive; } }

        // Constructor
        public Room(int enemyCount, Texture2D texture)
        {
            this.enemyCount = enemyCount;
            this.texture = texture;
            isActive = false;
        }

        // Methods
        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw()
        {

        }

    }
}
