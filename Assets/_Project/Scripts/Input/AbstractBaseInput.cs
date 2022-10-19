using UnityEngine;
using UnityEngine.EventSystems;

namespace DrawCrusher.UIInput
{
    public abstract class AbstractBaseInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}
