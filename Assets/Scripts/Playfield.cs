using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    public bool fill;

    public GameObject tilePrefab;

    List<GameObject> tileList = new List<GameObject>();

    private void Start()
    {
        SpawnPlayerField();
    }

    // private void OnDrawGizmos()

    private void SpawnPlayerField()
    {
        // Defensive prog to avoid duplication
        if(tilePrefab != null && fill)
        {
            // Delete existing tiles
            for (int i = 0; i < tileList.Count; i++)
            {
                DestroyImmediate(tileList[i]);
            }
            tileList.Clear();

            // Create our 10x10 map of tiles
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    Vector3 position = new Vector3(transform.position.x + x, 0, transform.position.z + z);
                    // Instantiate and parent it in hierachy
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    // Initiate the location info of the new spawned tile
                    tile.GetComponent<TileInfo>().SetTileInfo(x, z);
                    // Add to our list of tile
                    tileList.Add(tile);
                }
            }
        }

    }
}
