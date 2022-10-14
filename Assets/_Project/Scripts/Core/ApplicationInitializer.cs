using UnityEngine;
using DG.Tweening;

namespace DrawCrusher.Core
{
    public class ApplicationInitializer : MonoBehaviour
    {
        private void Awake()
        {
            DOTween.Init();
            DOTween.SetTweensCapacity(500, 50);
            Application.targetFrameRate = 30;
        }
    }
}
  