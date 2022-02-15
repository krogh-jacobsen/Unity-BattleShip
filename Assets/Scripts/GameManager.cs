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

    // State machines
    public enum GameStates
    {
        P1_Place_Ships,
        P2_Place_Ships,
        Shooting,
        Idle
    }
    public GameStates gameState;
    public GameObject battleCamPosition;

    int activePlayer;
    public Player[] players = new Player[2];

    private void AddShipToList(GameObject placedShip)
    {
        players[activePlayer].placedShipList.Add(placedShip);
    }

    private void Start()
    {
        // Hide all panels
        HideAllPanels();

        // Active place the panel from the first player
        players[activePlayer].placePanel.SetActive(true);
        gameState = GameStates.Idle;

        // Move the camera to the field
    }


    //------------------------------
    // Preparing a Battle
    //------------------------------

    private void Update()
    {
        switch (gameState)
        {
            case GameStates.P1_Place_Ships:
                {
                    // Deactivate Panel

                    // Call the placing Manager
                    PlacingManager.instance.SetPlayFieldForPlayer(players[activePlayer].playfield, players[activePlayer].playerType.ToString());
                    gameState = GameStates.Idle;
                }
                break;
            case GameStates.P2_Place_Ships:
                {
                    PlacingManager.instance.SetPlayFieldForPlayer(players[activePlayer].playfield, players[activePlayer].playerType.ToString());
                    gameState = GameStates.Idle;
                }
                break;
            case GameStates.Shooting:
                {
                    // Battlemode
                    if(players[activePlayer].playerType == Player.PlayerType.NPC)
                    {
                        // NPC turn
                    }

                }
                break;
            case GameStates.Idle:
                {
                    // Waiting here

                }
                break;
        }

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

    void HideAllPanels()
    {
        players[0].placePanel.SetActive(false);
        players[0].shootPanel.SetActive(false);

        players[1].placePanel.SetActive(false);
        players[1].shootPanel.SetActive(false);
    }

}