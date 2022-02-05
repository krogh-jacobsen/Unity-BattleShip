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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
