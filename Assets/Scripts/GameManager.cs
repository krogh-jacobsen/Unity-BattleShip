using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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
    public int activePlayer;

    public Player[] players = new Player[2];

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
    public bool cameraIsMoving;
    public GameObject placingCanvas;

    bool isShooting;    // Protect for coroutine


    private void Awake()
    {
        instance = this;
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
                    players[activePlayer].placePanel.SetActive(false);
                    // Call the placing Manager
                    PlacingManager.instance.SetPlayFieldForPlayer(players[activePlayer].playfield, players[activePlayer].playerType.ToString());
                    StartCoroutine(MoveCamera(players[activePlayer].camPosition));
                    gameState = GameStates.Idle;
                }
                break;
            case GameStates.P2_Place_Ships:
                {
                    // Deactivate Panel
                    players[activePlayer].placePanel.SetActive(false);

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

    void HideAllPanels()
    {
        players[0].placePanel.SetActive(false);
        players[0].shootPanel.SetActive(false);

        players[1].placePanel.SetActive(false);
        players[1].shootPanel.SetActive(false);
    }

    // Place panel button P1
    public void P1PlaceShips()
    {
        gameState = GameStates.P1_Place_Ships;
    }
    // Place panel button P2
    public void P2PlaceShips()
    {
        gameState = GameStates.P2_Place_Ships;
    }

    // Ready button
    public void PlacingReady()
    {
        if(activePlayer == 0)
        {
            // Hide my ships
            HideAllMyShips();

            // Switch to the 2nd player
            SwitchPlayer();

            // Move the camera 2nd player
            StartCoroutine(MoveCamera(players[activePlayer].camPosition));
            //StartCoroutine(MoveCamera(players[1].camPosition));

            // Activate placing panel p2
            players[activePlayer].placePanel.SetActive(true);

            // Return
            return;
        }

        if (activePlayer == 1)
        {
            // Hide my ships
            HideAllMyShips();

            // Switch to the 1st player
            SwitchPlayer();

            // Move the camera to player 1
            StartCoroutine(MoveCamera(players[activePlayer].camPosition));
            //StartCoroutine(MoveCamera(battleCamPosition));

            // Activate shot panel p1
            players[activePlayer].shootPanel.SetActive(true);

            // Deactivate placing canvas
            placingCanvas.SetActive(false);

            // Game starts
            
        }
    }

    private void UnHideAllMyShips()
    {
        foreach (var ship in players[activePlayer].placedShipList)
        {
            ship.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void HideAllMyShips()
    {
        foreach(var ship in players[activePlayer].placedShipList)
        {
            ship.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void SwitchPlayer()
    {
        activePlayer++;
        
        // When ever activeplayer reach 2 it resets to zero
        activePlayer %= 2;
    }

    IEnumerator MoveCamera(GameObject cameraGameObject)
    {
        // Break out if the camera is already moving
        if(cameraIsMoving)
        {
            yield break;
        }
        // TODO: This could use some refactoring
        cameraIsMoving = true;

        float currentTime = 0;
        float duration = 0.5f;

        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        Vector3 toPosition = cameraGameObject.transform.position;
        Quaternion toRotation = cameraGameObject.transform.rotation;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // We use lerp to move smoothly between the points
            Camera.main.transform.position = Vector3.Lerp(startPosition, toPosition, currentTime / duration);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, toRotation, currentTime / duration);
            yield return null;
        }

        cameraIsMoving = false;
    }

    //------------------------------
    // Battle mode
    //------------------------------


    // Shoot panel button
    public void ShotButtonPressed()
    {
        // Make oyr own ships visible
        UnHideAllMyShips();
        // Update UI
        players[activePlayer].shootPanel.SetActive(false);
        // Change game state
        gameState = GameStates.Shooting;
    }

    // TODO: Rename function to something more appropiate
    int ReturnOpponent()
    {
        // TODO: Refactor this ugly asss code
        // Start by defining who I am
        int me = activePlayer;
        me++;
        me %= 2;
        int opponent = me;
        return opponent;
    }

    public void CheckShot(int x, int z, TileInfo tileInfo)
    {
        StartCoroutine(CheckCoordinate(x, z, tileInfo));
    }

    IEnumerator CheckCoordinate(int x, int z, TileInfo tileInfo)
    {
        if (isShooting)
        {
            yield break;
        }
        isShooting = true;

        int opponent = ReturnOpponent();

        // If tile is not opponent tile
        if(!players[opponent].playfield.RequestTile(tileInfo))
        {
            print("Dont shot your own tiles");
            isShooting = false;
            yield break;
        }

        // If shot this coordinate already
        if (players[opponent].revealedGrid[x,z] == true)
        {
            print("You have shot here already");
            isShooting = false;
            yield break;
        }

        // If this is occupied
        if(players[opponent].myGrid[x,z].IsOccupiedByShip())
        {
            // Do damage to ship
            bool sunk = players[opponent].myGrid[x, z].placedShipBehavior.TakeDamage();

            if(sunk)
            {
                // Remove the sunken ship from the list of remaining fleet
                players[opponent].placedShipList.Remove(players[opponent].myGrid[x, z].placedShipBehavior.gameObject);
            }

            // Highlight the title different
            tileInfo.ActivateHighlight(3, true);
        }
        else
        {
            // Not hit a ship
            tileInfo.ActivateHighlight(2, true);
        }
        
        // Reveal tile
        players[opponent].revealedGrid[x, z] = true;

        // Check for win condition (if there are no one left we have won)
        if(players[opponent].placedShipList.Count == 0)
        {
            print("You win");
            // Win logic
        }

        // Hide my ships

        // Switch player

        // Activate the correct panel

        // Gamestate to idle


        // Set flag 
        isShooting = false;
    }

}