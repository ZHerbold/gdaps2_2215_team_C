using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheGame
{
    class Projectile
    {
        // Fields
        private Texture2D sprite;
        private Rectangle position;
        
        // Get-only for checking collisions
        public Rectangle Position { get { return position; } }

        // Constructor
        public Projectile(Texture2D sprite, Rectangle position)
        {
            this.sprite = sprite;
            this.position = position;
        }
    }
}
