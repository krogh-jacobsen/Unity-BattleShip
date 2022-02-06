using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public int amountToPlace = 1;
        [HideInInspector] public int placedAmount = 0;
    }

    public List<ShipsToPlace> shipList = new List<ShipsToPlace>();
    int currentShip = 4;

    RaycastHit raycastHit;      // called hit in tutorial
    Vector3 raycastHitPointPosition;    // called hitpoint in tutorial

    void Start()
    {
        // Deactivate them all
        ActivateShipGhost(-1);
        // Only activate the currentship
        ActivateShipGhost(currentShip);
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
            Debug.Log("Deactivate all ghots");
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
}
