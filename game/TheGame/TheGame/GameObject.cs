using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheGame
{
    abstract class GameObject
    {
        // Fields -------------------------------------------------------------
        protected int health;
        protected bool active;
        protected Vector2 position;
        protected Texture2D image;

        // Properties ---------------------------------------------------------
        public int Health
        {
            get { return health; }
            set
            {
                // Only edit health if there is health remaining
                if (health > 0)
                {
                    health = value;

                    // Set minimum health to 0
                    if (health < 0)
                    {
                        health = 0;
                    }
                }
            }
        }

        //changed position to a float since having it as a rectange would be hindering since player has constant 
        //dimensions for frames to work (see PE mario walk if you dont understand)
        public float X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public float Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Origin
        {
            get {return new Vector2(image.Width / 2, image.Height / 2); }
        }
        public Texture2D Image
        {
            get { return image; }
        }        
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        // Constructor --------------------------------------------------------
        public GameObject(int health, Vector2 position, Texture2D image)
        {
            this.health = health;
            this.position = position;
            this.image = image;
            
            active = true;
        }

        // Methods ------------------------------------------------------------
        /// <summary>
        /// Checks if an enemy intersects the player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>

        //abstract update method for child classes to use
        public abstract void Update(GameTime gameTime);

        //draw method so the child classes can draw easily.
        

        /// <summary>
        /// Subtract an object's health when it is hit by an attack 
        /// </summary>
        /// <returns></returns>
        public void TakeDamage(int damage)
        {
            health -= damage;
            
            // FOR BOTH PLAYER & ENEMIES
            //      We may have a frame of animation dedicated to being hit and
            //      draw that here or call something that draws it instead?
        }
    }
}
