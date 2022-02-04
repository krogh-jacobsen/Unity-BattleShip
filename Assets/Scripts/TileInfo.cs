using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour
{
    public int xPosition;
    public int zPosition;

    bool hasShot;

    public SpriteRenderer sprite;
    public Sprite[] tileHighLights = new Sprite[4];
    // 0 = FRAME, 1 = CROSSHAIR, 2 = WATER, 3 = HIT

    public void ActivateHighlight(int index )
    {
        sprite.sprite = tileHighLights[index];
    }

    public void SetTileInfo(int _xPos, int _zPos)
    {
        xPosition = _xPos;
        zPosition = _zPos;
    }

    public void OnMouseOver()
    {
        ActivateHighlight(1);
    }

    void OnMouseExit()
    {
        ActivateHighlight(0);
    }
}
