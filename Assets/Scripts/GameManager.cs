using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    //[System.Serializable]
    //public class Player
    //{
    //    public enum PlayerType
    //    {
    //        Human,
    //        NPC
    //    }
    //    public PlayerType playerType;
    //}



    int activePlayer;
    public Player[] players = new Player[2];

    private void AddShipToList(GameObject placedShip)
    {
        players[activePlayer].placedShipList.Add(placedShip);
    }

    public void UpdateGrid(Transform shipTransform, ShipBehavior shipBehavior, GameObject placedShip)
    {
        //
        foreach(Transform child in shipTransform)
        {
            TileInfo tileInfo = child.GetComponent<GhostBehavior>().GetTileInfo();
            players[activePlayer].myGrid[tileInfo.xPosition, tileInfo.zPosition] = new Tile(shipBehavior.type, shipBehavior);
        }

        AddShipToList(placedShip);
        DebugGrid();
    }

    public bool CheckIfOccupied(int xPositon, int zPosition)
    {
        return players[activePlayer].myGrid[xPositon, zPosition].IsOccupiedByShip();
    }


    void DebugGrid()
    {
        string seperatChars = "";
        int seperatorCount = 0;
        for (int x = 0; x < 10; x++)
        {
            seperatChars += "|";
            for (int y = 0; y < 10; y++)
            {
                string occupationTypeSymbol = "+";
                if (players[activePlayer].myGrid[x, y].occupationType == OccupationType.Carrier)
                {
                    occupationTypeSymbol = "C";
                }
                if (players[activePlayer].myGrid[x, y].occupationType == OccupationType.Battleship)
                {
                    occupationTypeSymbol = "B";
                }
                if (players[activePlayer].myGrid[x, y].occupationType == OccupationType.Submarine)
                {
                    occupationTypeSymbol = "S";
                }
                if (players[activePlayer].myGrid[x, y].occupationType == OccupationType.Destroyer)
                {
                    occupationTypeSymbol = "D";
                }
                if (players[activePlayer].myGrid[x, y].occupationType == OccupationType.Cruiser)
                {
                    occupationTypeSymbol = "R";
                }
                seperatChars += occupationTypeSymbol;
                seperatorCount = y % 10;
                if(seperatorCount == 9)
                {
                    seperatChars += "|";
                }
            }
            seperatChars += "\n";
        }
        print(seperatChars);
    }

    public void DeleteAllShipsFromList()
    {
        foreach(GameObject ship in players[activePlayer].placedShipList)
        {
            Destroy(ship);
        }
        players[activePlayer].placedShipList.Clear();

        // Reinit the grid
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Initializing all our tiles
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                OccupationType occupationType = OccupationType.Empty;
                players[activePlayer].myGrid[x, y] = new Tile(occupationType, null);
                players[activePlayer].revealedGrid[x, y] = false;
            }
        }
    }
}
