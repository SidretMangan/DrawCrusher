using UnityEngine;

namespace DrawCrusher.DrawingField
{
    /// <summary>
    /// After the specified time, it will disappear automatically; 
    /// freezing does not count as time
    /// </summary>
    public class DelayDestroy : MonoBehaviour
    {
        private float survivalTimer;
        private float survivalTime;

        /// <summary>
        /// Set survival time
        /// </summary>
        public void SetSurvivalTime(float survivalTime)
        {
            this.survivalTime = survivalTime;
            survivalTimer = 0;
        }

        private void Update()
        {
            survivalTimer += Time.deltaTime;
            if (survivalTimer >= survivalTime)
            {
                Destroy(gameObject);
            }
        }
    }
}