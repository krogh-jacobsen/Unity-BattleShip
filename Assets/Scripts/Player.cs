using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public enum PlayerType
    {
        Human,
        NPC
    }
    public PlayerType playerType;
    public Tile[,] myGrid = new Tile[10, 10];
    public bool[,] revealedGrid = new bool[10,10];
    public Playfield playfield;
    public LayerMask layerMaskToPlaceOn;

    // Panels to show and hide ships

    // Constructor
    public Player()
    {
        // Initializing all our tiles
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                OccupationType occupationType = OccupationType.Empty;
                myGrid[x, y] = new Tile(occupationType, null);
                revealedGrid[x, y] = false;
            }
        }
    }
    public List<GameObject> placedShipList = new List<GameObject>();
}
