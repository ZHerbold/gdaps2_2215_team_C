using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheGame
{
    enum PlayerState
    {
        FaceUp,
        WalkUp,
        FaceDown,
        WalkDown,
        FaceLeft,
        WalkLeft,
        FaceRight,
        WalkRight
    }

    class Player : GameObject
    {
        // Fields--------------------------------------------------------------
        private int gold;
        PlayerState state;

        // Properties ---------------------------------------------------------
        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        // Constructor --------------------------------------------------------
        public Player(int health, Rectangle position, int gold, 
            PlayerState startingState) : 
            base(health, position)
        {
            this.gold = gold;
            this.state = startingState;
        }

        // Methods ------------------------------------------------------------
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
