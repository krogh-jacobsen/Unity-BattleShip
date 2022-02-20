using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyBattleGame
{
    [System.Serializable]
    public class Player
    {
        #region Fields
        public enum PlayerType
        {
            Human,
            NPC
        }
        public PlayerType playerType;

        [Header("Game map")]
        public Tile[,] myGrid = new Tile[10, 10];
        public bool[,] revealedGrid = new bool[10,10];
        public Playfield playfield;

        [Header("Panels")]
        public GameObject placePanel;
        public GameObject shootPanel;
        public GameObject Winpanel;
        [Header("Camera position")]
        public GameObject camPosition;
        #endregion

        #region Methods
        public Player()
        {
            // Initializing all our tiles
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    OccupationType occupationType = OccupationType.Empty;
                    myGrid[x, z] = new Tile(occupationType, null);
                    revealedGrid[x, z] = false;
                }
            }
        }
        public List<GameObject> placedShipList = new List<GameObject>();
        #endregion
    }
}
