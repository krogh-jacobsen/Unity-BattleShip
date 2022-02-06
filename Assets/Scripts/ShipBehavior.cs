using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{
    public int shipLength;
    private int numberOfHits;

    private void Start()
    {
        numberOfHits = shipLength;
    }

    private bool IsSunk()
    {
        return shipLength <= 0;
        //if(shipLength <= 0)
        //{
        //    return true;
        //}
        //return false;
    }

    public bool IsHit()
    {
        return numberOfHits < shipLength && numberOfHits > 0;
    }

    public void TakeDamage()
    {
        numberOfHits--;
        if(IsSunk())
        {
            // Report the ship is sunk to the GameManager

            // Meshrenderer


        }
    }
     
}
