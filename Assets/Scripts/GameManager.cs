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
    public GameObject battleCameraPosition;
    public bool cameraIsMoving;
    public GameObject placingCanvas;

    
    bool isShooting;    // Protect for coroutine
    public GameObject rocketPrefab;
    float rocketAmplitude = 3f;
    float currentLerpTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Hide all panels
        HideAllPanels();

        // Hide win panels
        players[0].Winpanel.SetActive(false);
        players[1].Winpanel.SetActive(false);

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
                    // Move camera to battle mode
                    StartCoroutine(MoveCamera(battleCameraPosition));
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

    // TODO: fix the debugging log
    void DebugGrid()
    {
        string seperatorChar = "";
        int seperatorCount = 0;
        for (int x = 0; x < 10; x++)
        {
            seperatorChar += "|";
            for (int z = 0; z < 10; z++)
            {
                string occupationTypeSymbol = "+";
                if (players[activePlayer].myGrid[x, z].occupationType == OccupationType.Carrier)
                {
                    occupationTypeSymbol = "C";
                }
                if (players[activePlayer].myGrid[x, z].occupationType == OccupationType.Battleship)
                {
                    occupationTypeSymbol = "B";
                }
                if (players[activePlayer].myGrid[x, z].occupationType == OccupationType.Submarine)
                {
                    occupationTypeSymbol = "S";
                }
                if (players[activePlayer].myGrid[x, z].occupationType == OccupationType.Destroyer)
                {
                    occupationTypeSymbol = "D";
                }
                if (players[activePlayer].myGrid[x, z].occupationType == OccupationType.Cruiser)
                {
                    occupationTypeSymbol = "R";
                }
                seperatorChar += occupationTypeSymbol;
                seperatorCount = z % 10;
                if(seperatorCount == 9)
                {
                    seperatorChar += "|";
                }
            }
            seperatorChar += "\n";
        }
        print(seperatorChar);
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

            // Check if the player 2 is NPC
            if(players[activePlayer].playerType == Player.PlayerType.NPC)
            {
                gameState = GameStates.P2_Place_Ships;
                StartCoroutine(MoveCamera(battleCameraPosition));
                return;
            }

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

        // Shoot rocket
        Vector3 startPositionRocket = Vector3.zero;
        Vector3 targetDestination = tileInfo.gameObject.transform.position;

        // instantiate the rocket
        GameObject rocket = Instantiate(rocketPrefab, startPositionRocket, Quaternion.identity);

        // Send the rocket towards target
        while(MoveInArcToTile(startPositionRocket, targetDestination, 0.5f, rocket)) 
        {
            yield return null;
        }

        // We remove it when we are done with it
        Destroy(rocket);

        // We reset the timer
        currentLerpTime = 0;

        // If this is occupied
        if (players[opponent].myGrid[x,z].IsOccupiedByShip())
        {
            // Do damage to ship
            bool sunk = players[opponent].myGrid[x, z].placedShipBehavior.TakeDamage();

            if(sunk)
            {
                // Remove the sunken ship from the list of remaining fleet
                players[opponent].placedShipList.Remove(players[opponent].myGrid[x, z].placedShipBehavior.gameObject);
            }

            // Highlight the title different
            // TODO: Add sound effect and particles for hit
            tileInfo.ActivateHighlight(3, true);
        }
        else
        {
            // Not hit a ship (hit water)
            // TODO: Add sound effect and particles for water shot
            tileInfo.ActivateHighlight(2, true);
        }
        
        // Reveal tile
        players[opponent].revealedGrid[x, z] = true;

        // Check for win condition (if there are no one left we have won)
        if(players[opponent].placedShipList.Count == 0)
        {
            print("You win");
            // Win logic
            // Hide win panels
            players[activePlayer].Winpanel.SetActive(true);
            yield break;
        }
        yield return new WaitForSeconds(1f);

        // Hide my ships
        HideAllMyShips();

        // Switch player 
        SwitchPlayer();

        // If we are in player vs AI mode
        if(players[activePlayer].playerType == Player.PlayerType.NPC)
        {
            isShooting = false;
            gameState = GameStates.Idle;
            NPCShot();
            yield break;
        }

        // Activate the correct panel
        players[activePlayer].shootPanel.SetActive(true);

        // Gamestate to idle
        gameState = GameStates.Idle;

        // Set flag 
        isShooting = false;
    }

    // TODO: Move this out to seperate rocket class
    bool MoveInArcToTile(Vector3 startPosition, Vector3 targetPosition, float speed, GameObject rocket)
    {
        currentLerpTime += speed * Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPosition, targetPosition, currentLerpTime);
        
        // Move in sinus wave like curve
        myPosition.y = rocketAmplitude * Mathf.Sin(Mathf.Clamp01(currentLerpTime) * (float)Math.PI);
        // Ensure the point of the rocket is always facing forward
        rocket.transform.LookAt(myPosition);
        
        // If the target position matches where the rockets position
        return targetPosition != (rocket.transform.position = Vector3.Lerp(rocket.transform.position, myPosition, currentLerpTime));
    }


    //------------------------------
    // NPC Battle mode
    //------------------------------

    void NPCShot()
    {
        int index = 0;
        int x = 0;
        int z = 0;
        TileInfo tileInfo;

        int opponent = ReturnOpponent();

        //------------------------------
        // Partially revealed ships
        //------------------------------
        // List containing an array with the coordinates of our partially revealed tiles
        List<int[]> partiallyRevealedTiles = new List<int[]>();

        // Loopthrough all of our grid to examine what we hit and havent yet
        // TODO: 10 == gridsize but shouldnt that be a constant?
        for (int playGridX = 0; playGridX < 10; playGridX++)
        {
            for (int playGridY = 0; playGridY < 10; playGridY++)
            {
                // Did it get hit already
                if(players[opponent].revealedGrid[playGridX, playGridY])
                {
                    // Is the tile already occupied by a ship?
                    if(players[opponent].myGrid[playGridX, playGridY].IsOccupiedByShip())
                    {
                        // Ok there is a ship here but the whole ship hasn't been destroyed already?
                        if(players[opponent].myGrid[playGridX, playGridY].placedShipBehavior.IsHit())
                        {
                            // Add to the list of tiles with ships hit but not fully destroyed yet
                            partiallyRevealedTiles.Add(new int[2] { playGridX, playGridY });
                        }
                    }
                }
            }
        }

        // Store all the neigbour tiles to the partially sunk ships
        List<int[]> neighbourList = new List<int[]>();
        if(partiallyRevealedTiles.Count > 0)
        {
            for (int i = 0; i < partiallyRevealedTiles.Count; i++)
            {
                neighbourList.AddRange(GetNeighbours(partiallyRevealedTiles[i]));
            }

            // Section 62 NPC3
            index = UnityEngine.Random.Range(0, neighbourList.Count);

            x = neighbourList[index][0];
            z = neighbourList[index][1];

            tileInfo = players[opponent].playfield.GetTileInfo(x,z);

            CheckShot(x, z, tileInfo);
            return;
        }

        //------------------------------
        // Shot any another random
        //------------------------------
        // List of tiles we already shot
        List<int[]> randomShootList = new List<int[]>();

        // Loop through the grid to see which ones we haven't revealed/shot at yet
        for (int playGridX = 0; playGridX < 10; playGridX++)
        {
            for (int playGridY = 0; playGridY < 10; playGridY++)
            {
                // We haven't shot this one yet aka not revealed
                if(players[opponent].revealedGrid[playGridX,playGridY] != true)
                {
                    // Add it to our list of tiles to shot
                    randomShootList.Add(new int[2] { playGridX, playGridY });
                }
            }
        }
        // Lets pick a random tile from the list
        index = UnityEngine.Random.Range(0, randomShootList.Count);

        x = randomShootList[index][0];
        z = randomShootList[index][1];

        tileInfo = players[opponent].playfield.GetTileInfo(x, z);

        CheckShot(x, z, tileInfo);

    }

    private List<int[]> GetNeighbours(int[] originalCoordinates)
    {
        List<int[]> neighbours = new List<int[]>();
        
        // Find all surrounding neighbour tiles so in a 3x3 grid around the original coordinates
        // We want to start left of original coordinate so our starting point is -1 and end point +1
        // TODO: Refactor to just add the tiles we need rather than do this overcomplicated loop
        for (int neighbourX = -1; neighbourX < 1; neighbourX++)
        {
            for (int neighbourZ = -1; neighbourZ < 1; neighbourZ++)
            {
                // Ignore diagonals and the center
                if(neighbourX == 0 && neighbourZ == 0)
                {
                    // Skip to the next part of the loop
                    continue;
                }
                // Top left diagonal is skipped too
                if (neighbourX == -1 && neighbourZ == 1)
                {
                    // Skip to the next part of the loop
                    continue;
                }
                // Top right diagonal is skipped too
                if (neighbourX == 1 && neighbourZ == 1)
                {
                    // Skip to the next part of the loop
                    continue;
                }
                // Bottom left diagonal is skipped too
                if (neighbourX == -1 && neighbourZ == -1)
                {
                    // Skip to the next part of the loop
                    continue;
                }
                // Bottom right diagonal is skipped too
                if (neighbourX == 1 && neighbourZ == -1)
                {
                    // Skip to the next part of the loop
                    continue;
                }

                // TODO: What happened here?
                int checkX = originalCoordinates[0] + neighbourX;
                int checkZ = originalCoordinates[1] + neighbourZ;

                // Check if we are inside the our 10x10 grid and has not been revealed
                if(checkX >= 0 && checkX < 10 && checkZ >= 0 && checkZ < 10 && players[ReturnOpponent()].revealedGrid[checkX,checkZ] == false)
                {
                    neighbours.Add(new int[2] { checkX, checkZ });
                }
            }
        }
        return neighbours;
    }


}