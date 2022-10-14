using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GSL.InputCustomGUIBehaviour
{
    [RequireComponent(typeof(Image))]
    public abstract class CustomGUIBehaviour : MonoBehaviour,IPointerUpHandler,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
    {
        protected abstract void ClickAction();
        protected abstract void Init();

        public Color normalColor = Color.white;
        public Color ClickColor = Color.black;
        public Color HoverColor = Color.white;

        Image TargetGraphic;
        [Header("Sprites")]
        public Sprite HoverSprite;
        [HideInInspector]
        public Sprite DefaultSprite;

        private void Awake()
        {
            TargetGraphic=GetComponent<Image>();
            DefaultSprite = TargetGraphic.sprite;
            if (HoverSprite == null) HoverSprite = DefaultSprite;
            NormalState();
            Init();
        }
        protected virtual void HoverState()
        {
            transform.localScale = Vector3.one * 1.1f;
            TargetGraphic.color = HoverColor;
            TargetGraphic.sprite = DefaultSprite;
        }
        protected virtual void ClickState()
        {
            transform.localScale = Vector3.one;
            TargetGraphic.color = ClickColor;
            TargetGraphic.sprite = HoverSprite;
        }
        protected virtual void NormalState()
        {
            transform.localScale = Vector3.one;
            TargetGraphic.color = normalColor;
            TargetGraphic.sprite = DefaultSprite;
        }
        //Interfaces implementations
        //You can easily add all sorts of extra functionality in these methods
        public void OnPointerClick(PointerEventData eventData)
        {
            ClickAction();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HoverState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            NormalState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ClickState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HoverState();
        }
    }
}
