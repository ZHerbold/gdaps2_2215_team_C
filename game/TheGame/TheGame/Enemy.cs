using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public Enemy(int health, Vector2 position, Texture2D image) : base(health, position, image)
        {
        }

        // Methods
        //override method to call when update is called in game1
        //PUT CODE YOU WANT TO CALL DURING THE GAME HERE
        public override void Update(GameTime gameTime)
        {

        }

        //we'll eventually need drawing for the enemy to call in Game1
        public void Draw(SpriteBatch spriteBatch)
        {

        }


            public void Die()
        {
            // Give the player gold
            // Remove the enemy from the game
        }
    }
}
