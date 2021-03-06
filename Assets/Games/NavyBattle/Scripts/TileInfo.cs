using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NavyBattleGame
{
    public class TileInfo : MonoBehaviour
    {
        #region Fields
        private bool hasBeenShot;
        
        public int xPosition;
        public int zPosition;

        public SpriteRenderer sprite;
        public Sprite[] tileHighLights = new Sprite[4];
        // 0 = FRAME, 1 = CROSSHAIR, 2 = WATER, 3 = HIT
        // TODO: should we refactor the passing index vs. having an enum to speak to the tiles?
        #endregion

        #region MonoBehaviour
        public void OnMouseOver()
        {
            if (GameManager.instance.gameState == GameState.GameStates.Shooting)
            {
                if (!hasBeenShot)
                {
                    ActivateHighlight(1, false);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    // Game manager to check this coordinate
                    GameManager.instance.CheckShot(xPosition, zPosition, this);
                }
            }
        }

        void OnMouseExit()
        {
            if (!hasBeenShot)
            {
                ActivateHighlight(0, false);
            }

        }
        #endregion
        #region Methods
        public void ActivateHighlight(int index, bool _hasBeenShot)
        {
            sprite.sprite = tileHighLights[index];

            // Color the sprite
            if (index == 2)
            {
                sprite.color = Color.blue;
            }
            // Hit ship (red)
            if (index == 3)
            {
                sprite.color = Color.red;
            }

            hasBeenShot = _hasBeenShot;
        }

        public void SetTileInfo(int _xPos, int _zPos)
        {
            xPosition = _xPos;
            zPosition = _zPos;
        }
        #endregion

    }
}