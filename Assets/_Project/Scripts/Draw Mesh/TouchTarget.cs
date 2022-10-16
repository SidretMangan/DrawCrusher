using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawCrusher.DrawingField
{
    public class TouchTarget : MonoBehaviour
    {
        private DrawSettings drawSettings;
        private CircleCollider2D circleCollider2D;
        private TargetJoint2D targetJoint2D;
        private float scaleChange = 1.2f;

        public void Init(DrawSettings drawSettings)
        {
            this.drawSettings = drawSettings;
            gameObject.layer = drawSettings.drawingMeshLayer;
            circleCollider2D = GetComponent<CircleCollider2D>();
            targetJoint2D = GetComponent<TargetJoint2D>();
            circleCollider2D.isTrigger = true;
            transform.localScale = drawSettings.endWidth * scaleChange * Vector3.one;
        }

        /// <summary>
        /// Set target point
        /// </summary>
        public void SetTarget(Vector2 targetPos)
        {
            targetJoint2D.target = targetPos;
        }

        /// <summary>
        /// Set whether the touch target is a trigger
        /// </summary>
        public void SetTrigger(bool isTrigger)
        {
            circleCollider2D.isTrigger = isTrigger;
        }

        /// <summary>
        /// Determine if a can't draw object is hit
        /// </summary>
        public bool HitCantDrawObject()
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, drawSettings.cantDrawLayers);
            if (raycastHit2D.collider != null)
            {
                return true;
            }
            return false;
        }
    }
}