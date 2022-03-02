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
        // Have a protected access modifier so that children may access
        protected int health;
        protected Rectangle position;

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

        public int X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public int Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

        // Constructor --------------------------------------------------------
        public GameObject(int health, Rectangle position)
        {
            this.health = health;
            this.position = position;
        }

        // Methods ------------------------------------------------------------
        public void Update()
        {

        }

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

        /// <summary>
        /// Creates projectile objects
        /// </summary>
        /// <returns></returns>
        public void Attack()
        {
            // FOR PLAYER (MELEE):
            //      Draw the attacks and immediately check for collisions.
            //      If there is a collision, call TakeDamage on the enemy
            //      that was hit.

            // For projectiles, create a Projectile object
        }
    }
}
