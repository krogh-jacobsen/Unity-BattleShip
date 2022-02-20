using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyBattleGame
{
    public class AnimateWater : MonoBehaviour
    {
        #region Fields
        private float offset;
        private Material material;
        public float scrollSpeed = 0.5f;
        public bool U = false;
        public bool V = false;
        #endregion

        #region MonoBehaviour
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }
        
        void Update()
        {
            offset = Time.time * scrollSpeed % 1;
            if (U & V)
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
        #endregion
    }
}