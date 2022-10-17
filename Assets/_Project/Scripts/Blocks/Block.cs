using System;
using UnityEngine;

namespace DrawCrusher.BlockManagement
{
    public class Block : MonoBehaviour
    {
        public event Action blockOnHit = () => { };

        public MeshRenderer meshRenderer;
        public void InvokeHit()
        {
            blockOnHit();
        }
    }
}
