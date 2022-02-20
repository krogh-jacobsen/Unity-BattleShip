using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NavyBattleGame
{

    public class PlacingManager : MonoBehaviour
    {
        // TODO: Refactor the singleton here
        public static PlacingManager instance;

        public bool isInPlacingMode; //canPlace
        bool canPlace;  // Free to place

        Playfield playfield;
        public LayerMask layerToCheck;


        [System.Serializable]
        public class ShipsToPlace
        {
            public GameObject shipGhost;
            public GameObject shipPrefab;
            public int amountToPlace;
            public int placedAmount = 0;
            public TextMeshProUGUI amountText;

        }

        public Button readyButton;

        public List<ShipsToPlace> shipList = new List<ShipsToPlace>();
        int currentShipType = 0;

        RaycastHit raycastHit;      // called hit in tutorial
        Vector3 raycastHitPointPosition;    // called hitpoint in tutorial

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            UpdateAmountText();
            // Deactivate them all
            ActivateShipGhost(-1);
            // Only activate the currentship
            //ActivateShipGhost(currentShip);
            isInPlacingMode = false;
        }

        public void SetPlayFieldForPlayer(Playfield _playfield, string playerType)
        {
            playfield = _playfield;
            // Initialize the readybutton
            readyButton.interactable = false;

            ClearAllShips();

            // NPC
            if (playerType == "NPC")
            {

                // Auto placement for CPU
                // TODO: Fix the autoplacing and then enable this
                // AutoPlaceShips();

                // Update Game Manager that the turn is complete
                // TODO: Fix the autoplacing and then enable this
                // GameManager.instance.PlacingReady();
            }
        }

        void Update()
        {
            if (isInPlacingMode)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, layerToCheck))
                {
                    // Ensure the tile belongs to player and not opponent
                    if (!playfield.RequestTile(raycastHit.collider.GetComponent<TileInfo>()))
                    {
                        // Return to restrict us from not being able to place or rotate our ship
                        return;
                    }

                }
                raycastHitPointPosition = raycastHit.point;
            }

            // Placing our ship
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceShip();
            }

            // Rotating our ship
            if (Input.GetMouseButtonDown(1))
            {
                RotateShipGhost();
            }
            // Place ghost
            PlaceGhost();
        }

        void ActivateShipGhost(int index)
        {
            // If our index is negative we want to exit
            if (index != -1)
            {
                // Check if its already active in the hierachy so we dont duplicate it?
                if (shipList[index].shipGhost.activeInHierarchy)
                {
                    return;
                }
            }

            // Deactivate all ghosts
            for (int i = 0; i < shipList.Count; i++)
            {
                shipList[i].shipGhost.SetActive(false);
            }

            if (index == -1)
            {
                return;
            }

            //Activate needed ghost
            shipList[index].shipGhost.SetActive(true);
        }

        void PlaceGhost()
        {
            if (isInPlacingMode)
            {
                canPlace = CheckForOtherShips();
                Vector3 shipGhostPosition = new Vector3(Mathf.Round(raycastHitPointPosition.x), 0.5f, Mathf.Round(raycastHitPointPosition.z));
                shipList[currentShipType].shipGhost.transform.position = shipGhostPosition;
                //Debug.Log(shipGhostPosition);
            }
            else
            {
                // Deactivate all ghosts
                //Debug.Log("Deactivate all ghots");
                ActivateShipGhost(-1);
            }
        }

        void RotateShipGhost()
        {
            shipList[currentShipType].shipGhost.transform.localEulerAngles += new Vector3(0, 90f, 0);
        }

        private bool CheckForOtherShips()
        {
            // Check for the ghost child cubes 
            foreach (Transform child in shipList[currentShipType].shipGhost.transform)
            {
                GhostBehavior ghost = child.GetComponent<GhostBehavior>();
                if (!ghost.OverTile())
                {
                    // Turn into red
                    child.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 125);
                    return false;
                }
                // Turn back to white
                child.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 100);
            }
            return true;
        }
        // Overloading method where we dont need to do anything but check its valid
        private bool CheckForOtherShips(Transform transform)
        {
            // Check for the ghost child cubes 
            foreach (Transform child in transform)
            {
                GhostBehavior ghost = child.GetComponent<GhostBehavior>();
                if (!ghost.OverTile())
                {
                    return false;
                }
            }
            return true;
        }


        void PlaceShip()
        {
            if (isInPlacingMode)
            {
                // Get the wholenumber position of the mouse click raycast
                Vector3 position = new Vector3(Mathf.Round(raycastHitPointPosition.x), 0, Mathf.Round(raycastHitPointPosition.z));
                // Get rotation of current ghost ship
                Quaternion quaternion = shipList[currentShipType].shipGhost.transform.rotation;

                GameObject newShip = Instantiate(shipList[currentShipType].shipPrefab, position, quaternion);
                Debug.Log("Placing ship");

                // Updating grid
                GameManager.instance.UpdateGrid(shipList[currentShipType].shipGhost.transform, newShip.GetComponent<ShipBehavior>(), newShip);

                // Increment the number of ships placed
                shipList[currentShipType].placedAmount++;
            }
            // Deactivate the placing mode
            isInPlacingMode = false;

            // Deactivate all ghosts
            ActivateShipGhost(-1);

            // Update the count with right number
            UpdateAmountText();

            // Check if all ships have been placed
            CheckIfAllShipsArePlaced();

        }

        #region buttons
        // Buttons
        public void PlaceShipButton(int index)
        {
            // We need to check if all the ships have been placed first
            if (CheckIfAllShipsArePlaced(index))
            {
                Debug.Log("You have placed all the ships allready. No more to place");
                return;
            }
            else
            {
                Debug.Log("We still got something left to place");
            }

            // We activate ship ghost
            currentShipType = index;
            ActivateShipGhost(currentShipType);
            // We enter the placing mode
            isInPlacingMode = true;
        }

        // Function  to check if that specific shiptype have been placed
        bool CheckIfAllShipsArePlaced(int index)
        {
            // If the number of placed ships is equal to the number we placed its true otherwise false
            return shipList[index].placedAmount >= shipList[index].amountToPlace;
        }

        // Function  to check if ALL the ships have been placed
        bool CheckIfAllShipsArePlaced()
        {
            foreach (var ship in shipList)
            {
                // check if we placed the correct number of ships
                if (ship.placedAmount != ship.amountToPlace)
                {
                    return false;
                }
            }
            readyButton.interactable = true;
            return true;
        }

        void UpdateAmountText()
        {
            // TODO: Refactor to foreach since its a list
            for (int i = 0; i < shipList.Count; i++)
            {
                shipList[i].amountText.text = (shipList[i].amountToPlace - shipList[i].placedAmount).ToString();
            }

        }

        // Function to be clear button
        public void ClearAllShips()
        {
            GameManager.instance.DeleteAllShipsFromList();
            foreach (var ship in shipList)
            {
                ship.placedAmount = 0;
            }
            UpdateAmountText();
            // Disable ready button
            readyButton.interactable = false;
        }

        public void AutoPlaceShips()
        {
            // Clear all ships first
            //ClearAllShips();
            GameManager.instance.DeleteAllShipsFromList();

            // Creating a flag for our loop
            bool positionFound = false;

            // Loop through all the ships in our list
            for (int i = 0; i < shipList.Count; i++)
            {
                Debug.Log(shipList[i].ToString());
                // Loop thorugh every shipamount for each type of ship to place
                for (int j = 0; j < shipList[i].amountToPlace; j++)
                {
                    // To avoid the situation where we dont have any left
                    if (shipList[i].amountToPlace <= 0)
                    {
                        Debug.LogError("There is no or negative shipamount in the placing manager");
                        return;
                    }
                    positionFound = false;
                    // Defensive code stopper
                    //int defensiveCount = 0;

                    // While no position fond
                    //while (!positionFound || defensiveCount > 1000)

                    while (!positionFound)
                    {
                        //defensiveCount++;

                        // Stay here until we find a suitable position
                        currentShipType = i;

                        // Pick a random position
                        int randomXPosition = UnityEngine.Random.Range(0, 10);
                        int randomZPosition = UnityEngine.Random.Range(0, 10);

                        Debug.Log("Random position: " + randomXPosition + "|" + randomZPosition);

                        // Create a ghost
                        GameObject temporaryGhost = Instantiate(shipList[currentShipType].shipGhost);
                        temporaryGhost.SetActive(true);

                        // Set Ghost playfield??? Maybe not needed

                        // Set position of the ghost ship
                        temporaryGhost.transform.position = new Vector3(
                            playfield.transform.position.x + randomXPosition,
                            0,
                            playfield.transform.position.z + randomZPosition);

                        // We define the 4 potential/acceptable rotations
                        Vector3[] possibleRotations = {
                        new Vector3(0, 0, 0),
                        new Vector3(0, 90, 0),
                        new Vector3(0, 180, 0),
                        new Vector3(0, 270, 9)
                    };

                        // Check for all rotations
                        for (int rotationOption = 0; rotationOption < possibleRotations.Length; rotationOption++)
                        {
                            List<int> indexList = new List<int> { 0, 1, 2, 3 };
                            int randomRotation = indexList[UnityEngine.Random.Range(0, indexList.Count)];

                            // Rotate
                            temporaryGhost.transform.rotation = Quaternion.Euler(possibleRotations[randomRotation]);


                            // Check the location works or is overlapping
                            if (CheckForOtherShips(temporaryGhost.transform))
                            {
                                Debug.Log("CheckForOtherShips -> No ships here so spawning it ");
                                PlaceAutoShip(temporaryGhost);
                                positionFound = true;
                            }
                            // No position from all posibble locations
                            else
                            {
                                Debug.Log("CheckForOtherShips -> Ships here so removing position ");
                                //Destroy(temporaryGhost);
                                indexList.Remove(rotationOption);
                            }
                        }

                    }

                }
            }
            //readyButton.interactable = true;
            CheckIfAllShipsArePlaced();

        }


        public void PlaceAutoShip(GameObject temporaryGO)
        {
            Debug.Log("PlaceAutoShip");
            GameObject newShip = Instantiate(shipList[currentShipType].shipPrefab, temporaryGO.transform.position, temporaryGO.transform.rotation);
            GameManager.instance.UpdateGrid(temporaryGO.transform, newShip.GetComponent<ShipBehavior>(), newShip);
            shipList[currentShipType].placedAmount++;

            // remove the temporary go
            Destroy(temporaryGO);
            // Update UI
            UpdateAmountText();
        }

        #endregion
    }

}