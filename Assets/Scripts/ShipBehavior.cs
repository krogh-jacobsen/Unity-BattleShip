using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyBattleGame
{
    public class ShipBehavior : MonoBehaviour
    {
        #region Fields
        private int numberOfHitpoints;
        public int shipLength;
        public OccupationType type;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            numberOfHitpoints = shipLength;
        }
        #endregion
        #region Methods
        private bool IsSunk()
        {
            return numberOfHitpoints <= 0;
            //if(shipLength <= 0)
            //{
            //    return true;
            //}
            //return false;
        }

        public bool IsHit()
        {
            return numberOfHitpoints < shipLength && numberOfHitpoints > 0;
        }

        public bool TakeDamage()
        {
            numberOfHitpoints--;
            if (IsSunk())
            {
                // Report the ship is sunk to the GameManager


                // Meshrenderer unhide the ship
                GetComponent<MeshRenderer>().enabled = true;
                return true;
            }
            return false;
        }
        #endregion
    }
}