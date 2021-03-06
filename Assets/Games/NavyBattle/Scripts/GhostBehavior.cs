using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyBattleGame
{
    public class GhostBehavior : MonoBehaviour
    {
        #region Fields
        public LayerMask layerToCheck;
        private RaycastHit raycastHit;      // Named hit in tutorial
        private TileInfo tileInfo;          // Named info in tutorial
        private Playfield playfield;
        #endregion

        #region Methods
        public void SetPlayfield(Playfield _playfield)
        {
            playfield = _playfield;
        }

        
        // Check if the ghost is above a tile
        public bool OverTile()
        {
            // Get the info of the tile we are above
            tileInfo = GetTileInfo();

            // If this it not null it means we found a tile and so we return true
            // Also we want to check its not occupied by another ship
            if (tileInfo != null && !GameManager.instance.CheckIfOccupied(tileInfo.xPosition, tileInfo.zPosition)) // Lets double check if occupied
            {
                //Debug.Log("We are over a tile");
                return true;
            }

            // Reset it to null
            tileInfo = null;

            // No tile found and so we return false
            //Debug.Log("We are NOT over a tile");
            return false;
        }

        public TileInfo GetTileInfo()
        {
            // Shot downwards with ray to detect which tile the ghost is on top of
            Ray ray = new Ray(transform.position, -transform.up);

            // If we hit a tile we want to return that Tileinfo
            if (Physics.Raycast(ray, out raycastHit, 1f, layerToCheck))
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.red);
                // Returning the tile which is just below the ghost
                return raycastHit.collider.GetComponent<TileInfo>();
            }

            return null;
        }
        #endregion
    }
}