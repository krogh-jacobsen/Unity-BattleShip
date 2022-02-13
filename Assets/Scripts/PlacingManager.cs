using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlacingManager : MonoBehaviour
{

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

    public List<ShipsToPlace> shipList = new List<ShipsToPlace>();
    int currentShip = 0;
        
    RaycastHit raycastHit;      // called hit in tutorial
    Vector3 raycastHitPointPosition;    // called hitpoint in tutorial

    void Start()
    {
        UpdateAmountText();
        // Deactivate them all
        ActivateShipGhost(-1);
        // Only activate the currentship
        //ActivateShipGhost(currentShip);
        isInPlacingMode = false;
    }

    
    void Update()
    {
        if(isInPlacingMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, layerToCheck))
            {
                // Ensure the tile belongs to player and not opponent

            }
            raycastHitPointPosition = raycastHit.point;
        }

        // Placing our ship
        if(Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceShip();
        }

        // Rotating our ship
        if(Input.GetMouseButtonDown(1))
        {
            RotateShipGhost();
        }
        // Place ghost
        PlaceGhost();
    }

    void ActivateShipGhost(int index)
    {
        // If our index is negative we want to exit
        if(index != -1)
        {
            // Check if its already active in the hierachy so we dont duplicate it?
            if(shipList[index].shipGhost.activeInHierarchy)
            {
                return; 
            }
        }

        // Deactivate all ghosts
        for (int i = 0; i < shipList.Count; i++)
        {
            shipList[i].shipGhost.SetActive(false);
        }

        if(index == -1)
        {
            return;
        }

        //Activate needed ghost
        shipList[index].shipGhost.SetActive(true);
    }

    void PlaceGhost()
    {
        if(isInPlacingMode)
        {
            canPlace = CheckForOtherShips();
            Vector3 shipGhostPosition = new Vector3(Mathf.Round(raycastHitPointPosition.x), 0.5f, Mathf.Round(raycastHitPointPosition.z));
            shipList[currentShip].shipGhost.transform.position = shipGhostPosition;
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
        shipList[currentShip].shipGhost.transform.localEulerAngles += new Vector3(0, 90f, 0);
    }

    private bool CheckForOtherShips()
    {
        // Check for the ghost child cubes 
        foreach (Transform child  in shipList[currentShip].shipGhost.transform)
        {
            GhostBehavior ghost = child.GetComponent<GhostBehavior>();
            if(!ghost.OverTile())
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

    void PlaceShip()
    {
        if(isInPlacingMode)
        { 
            // Get the wholenumber position of the mouse click raycast
            Vector3 position = new Vector3(Mathf.Round(raycastHitPointPosition.x), 0, Mathf.Round(raycastHitPointPosition.z));
            // Get rotation of current ghost ship
            Quaternion quaternion = shipList[currentShip].shipGhost.transform.rotation;

            GameObject newShip = Instantiate(shipList[currentShip].shipPrefab, position, quaternion);
            Debug.Log("Placing ship");

            // Updating grid
            GameManager.instance.UpdateGrid(shipList[currentShip].shipGhost.transform, newShip.GetComponent<ShipBehavior>(), newShip);
        
            // Increment the number of ships placed
            shipList[currentShip].placedAmount++;
        }
        // Deactivate the placing mode
        isInPlacingMode = false;

        // Deactivate all ghosts
        ActivateShipGhost(-1);

        // Update the count with right number
        UpdateAmountText();

        // Check if all ships have been placed


    }

    // Buttons
    public void PlaceShipButton(int index)
    {
        // We need to check if all the ships have been placed first
        if(CheckIfAllShipsArePlaced(index))
        {
            Debug.Log("You have placed all the ships allready. No more to place");
            return;
        }
        else
        {
            Debug.Log("We still got something left to place");
        }

        // We activate ship ghost
        currentShip = index;
        ActivateShipGhost(currentShip);
        // We enter the placing mode
        isInPlacingMode = true;
    }

    // Function  to check if all the ships have been placed
    bool CheckIfAllShipsArePlaced(int index)
    {
        // If the number of placed ships is equal to the number we placed its true otherwise false
        return shipList[index].placedAmount >= shipList[index].amountToPlace;
    }

    void UpdateAmountText()
    {
        // TODO: Refactor to foreach since its a list
        for (int i = 0; i < shipList.Count; i++)
        {
            shipList[i].amountText.text = (shipList[i].amountToPlace - shipList[i].placedAmount).ToString();
        }

    }
}
