using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheGame
{
    // Works while we only have 1 enemy type
    enum EnemyState
    {
        FaceUp,
        FaceDown,
        FaceLeft,
        FaceRight
    }

    class Enemy : GameObject
    {
        // FOR THE SKELETON WE ONLY HAVE 1 TYPE OF ENEMY
        
        // Fields???

        // Constructor
        public Enemy(int health, Rectangle position) : base(health, position)
        {
        }

        // Methods
        public void Die()
        {
            // Give the player gold
            // Remove the enemy from the game
        }
    }
}
