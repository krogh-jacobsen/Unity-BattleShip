using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour
{
    public int xPosition;
    public int yPosition;

    bool hasShot;

    public SpriteRenderer sprite;
    public Sprite[] tileHighLights = new Sprite[4];
    // 0 = FRAME, 1 = CROSSHAIR, 2 = WATER, 3 = HIT

    public void ActivateHighlight(int index )
    {
        sprite.sprite = tileHighLights[index];
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
