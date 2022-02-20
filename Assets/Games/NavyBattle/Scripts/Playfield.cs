using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyBattleGame
{
    public class Playfield : MonoBehaviour
    {
        #region Fields
        public bool fill;
        public GameObject tilePrefab;
        private List<GameObject> tileList = new List<GameObject>();
        private List<TileInfo> tileInfoList = new List<TileInfo>();
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            SpawnPlayerField();

            // Ensure we start fresh by removing old data
            tileList.Clear();
            tileInfoList.Clear();

            foreach (Transform tileTransform in transform)
            {
                if (tileTransform != transform)
                {
                    tileList.Add(tileTransform.gameObject);
                }
            }

            // Fill the tile infos
            foreach (GameObject tileGameObject in tileList)
            {
                tileInfoList.Add(tileGameObject.GetComponent<TileInfo>());
            }

        }

        private void OnDrawGizmos()
        {
            //SpawnPlayerField();
        }
        #endregion
        
        #region Methods
        public bool RequestTile(TileInfo _tileInfo)
        {
            // Will return either true or false 
            return tileInfoList.Contains(_tileInfo);
        }

        private void SpawnPlayerField()
        {
            // Defensive prog to avoid duplication
            if (tilePrefab != null && fill)
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

                        tile.name = x.ToString() + "|" + z.ToString();
                        // Add to our list of tile
                        tileList.Add(tile);
                    }
                }
            }

        }

        // Section 62 NPC3
        public TileInfo GetTileInfo(int x, int z)
        {
            for (int i = 0; i < tileInfoList.Count; i++)
            {
                if (tileInfoList[i].xPosition == x && tileInfoList[i].zPosition == z)
                {
                    return tileInfoList[i];
                }
            }
            return null;
        }
        #endregion
    }
}