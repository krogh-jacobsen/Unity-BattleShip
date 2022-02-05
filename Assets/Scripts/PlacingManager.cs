using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingManager : MonoBehaviour
{

    public bool isInPlacingMode;
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
    int currentShip;

    RaycastHit raycastHit;      // called hit in tutorial
    Vector3 raycastHitPointPosition;    // called hitpoint in tutorial

    void Start()
    {
        
    }

    // Update is called once per frame
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

        }

        // Place ghost
    }

    void ActivateShipGhost(int index)
    {
        if(index != -1)
        {
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
        }
    }

    private bool CheckForOtherShips()
    {
        foreach (Transform child  in shipList[currentShip].shipGhost.transform)
        {
            GhostBehavior ghost = child.GetComponent<GhostBehavior>();
            if(!ghost.OverTile())
            {
                // Turn into read
                child.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 125);
                return false;
            }
            // Turn back to white
            child.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 100);
        }
        return true; 
    }
}
