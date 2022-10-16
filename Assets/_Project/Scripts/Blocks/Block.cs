using System;
using UnityEngine;

namespace DrawCrusher.BlockManagement
{
    public class Block : MonoBehaviour
    {
        public event Action onHit = () => { };

        public MeshRenderer meshRenderer;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            onHit();
        }
    }
}
