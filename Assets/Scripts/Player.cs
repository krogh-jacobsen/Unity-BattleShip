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
}
