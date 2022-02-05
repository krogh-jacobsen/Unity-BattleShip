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
}
