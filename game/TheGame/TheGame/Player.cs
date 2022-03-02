using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheGame
{
    enum PlayerState
    {
        FaceUp,
        FaceDown,
        FaceLeft,
        FaceRight
    }

    class Player : GameObject
    {
        // Fields
        private int gold;

        // Constructor
        public Player(int gold, int health, Rectangle position) : 
            base(health, position)
        {
            this.gold = gold;
        }

        // Methods
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
