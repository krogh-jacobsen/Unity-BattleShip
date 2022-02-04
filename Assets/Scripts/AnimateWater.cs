using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWater : MonoBehaviour
{

    public float scrollSpeed = 0.5f;

    float offset;
    public bool U = false;
    public bool V = false;

    Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset = Time.time * scrollSpeed % 1;
        if(U & V)
        {
            material.mainTextureOffset = new Vector2(offset, offset);
        }
        else if (U)
        {
            material.mainTextureOffset = new Vector2(offset, 0);
        }
        else if (V)
        {
            material.mainTextureOffset = new Vector2(0, offset);
        }


    }
}
