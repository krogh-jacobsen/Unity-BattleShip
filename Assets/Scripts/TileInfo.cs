using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour
{
    public int xPosition;
    public int zPosition;

    bool hasBeenShot;

    public SpriteRenderer sprite;
    public Sprite[] tileHighLights = new Sprite[4];
    // 0 = FRAME, 1 = CROSSHAIR, 2 = WATER, 3 = HIT

    // TODO: should we refactor the passing index vs. having an enum to speak to the tiles?
    public void ActivateHighlight(int index, bool _hasBeenShot)
    {
        sprite.sprite = tileHighLights[index];

        // Color the sprite

        hasBeenShot = _hasBeenShot;
    }

    public void SetTileInfo(int _xPos, int _zPos)
    {
        xPosition = _xPos;
        zPosition = _zPos;
    }

    public void OnMouseOver()
    {
        if(GameManager.instance.gameState == GameManager.GameStates.Shooting)
        {
            if(!hasBeenShot)
            { 
                ActivateHighlight(1, false);
            }
            if(Input.GetMouseButtonDown(0))
            {
                // Game manager to check this coordinate
                GameManager.instance.CheckCoordinate(xPosition, zPosition, this);
            }
        }
    }

    void OnMouseExit()
    {
        if(!hasBeenShot)
        {
            ActivateHighlight(0, false);
        }
        
    }
}
