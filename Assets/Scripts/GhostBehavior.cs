using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehavior : MonoBehaviour
{
    public LayerMask layerToCheck;
    RaycastHit raycastHit;      // Named hit in tutorial
    TileInfo tileInfo;          // Named info in tutorial

    Playfield playfield;

    public void SetPlayfield(Playfield _playfield)
    {
        playfield = _playfield;
    }

    public bool OverTile()
    {
        tileInfo = GetTileInfo();

        if(tileInfo != null)
        {

        }

        return false;
    }

    private TileInfo GetTileInfo()
    {
        // Shot downwards with ray to detect which tile the ghost is on top of
        Ray ray = new Ray(transform.position, -transform.up);
        
        if(Physics.Raycast(ray, out raycastHit, 1f, layerToCheck))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            // Returning the tile which is just below the ghost
            return raycastHit.collider.GetComponent<TileInfo>();
        }

        return null;
    }
}
