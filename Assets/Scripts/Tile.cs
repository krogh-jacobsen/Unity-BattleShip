using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyBattleGame
{
    public enum OccupationType
    {
        Empty,
        Cruiser,
        Destroyer,
        Submarine,
        Battleship,
        Carrier,
        Hit,
        Miss
    }

    public class Tile
    {
        public OccupationType occupationType;
        public ShipBehavior placedShipBehavior;

        // Constructor
        public Tile(OccupationType _occupationType, ShipBehavior _placedShipBehavior)
        {
            occupationType = _occupationType;
            placedShipBehavior = _placedShipBehavior;
        }
        public bool IsOccupiedByShip()
        {
            return occupationType == OccupationType.Battleship ||
                occupationType == OccupationType.Carrier ||
                occupationType == OccupationType.Submarine ||
                occupationType == OccupationType.Destroyer ||
                occupationType == OccupationType.Cruiser;
        }
    }
}
