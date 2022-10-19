using UnityEngine;
using UnityEngine.UI;
using DrawCrusher.Database;
using TMPro;
using UniRx;
using DG.Tweening;
using DrawCrusher.UIInput;

namespace DrawCrusher.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] public IntVariable moneyVariable;
        [SerializeField] private TextMeshProUGUI moneyValueText;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image loadingScreenImage;
        [SerializeField] private GameObject moneyPanel;
        [SerializeField] private RectTransform moneyPanelRectTransform;
        [SerializeField] private GameObject drawPhysicsBoundsPanel;
        [SerializeField] private RectTransform startButtonRectTransform;
        [SerializeField] private ButtonUI startButtonButtonUI;
        [SerializeField] private RectTransform backButtonRectTransform;
        [SerializeField] private ButtonUI backButtonButtonUI;

        private void Start()
        {
            moneyValueText.SetText(moneyVariable.Value.ToString());
            moneyVariable.ObserveEveryValueChanged(x => x.Value).Subscribe(x => moneyValueText.SetText(moneyVariable.Value.ToString()));
        }
        private void ChangeStatusPanel(GameObject gameObject,bool status)
        {
            gameObject.SetActive(status);
        }
        public void StartButtonOn(bool status)
        {
            if (status)
            {
                ChangeStatusPanel(startButtonRectTransform.gameObject, true);
                startButtonRectTransform.DOAnchorPosX(0f, 2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    startButtonButtonUI.GetInteractable();
                });
            }
            else
            {
                startButtonRectTransform.DOAnchorPosX(1500f, 2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    ChangeStatusPanel(startButtonRectTransform.gameObject, false);
                    BackButtonOn(true);
                    startButtonRectTransform.anchoredPosition = new Vector2(-1500f, 0f);
                });
            }
        }
        public void LoadingScreenOn(bool status)
        {
            if (status)
            {
                ChangeStatusPanel(loadingScreen, true);
                loadingScreenImage.DOFade(1f, 1f);
            }
            else
            {
                loadingScreenImage.DOFade(0f, 1f).OnComplete(()=> { ChangeStatusPanel(loadingScreen, false); });
            }
        }
        public void StartGameStateUIBegin()
        {
            MoneyPanelOn(true);
            DrawPhysicsBoundsPanelOn(true);
            BackButtonOn(true);
        }
        public void EndGameStateUIBegin()
        {
            DrawPhysicsBoundsPanelOn(false);
            BackButtonOn(false);
            MoneyPanelOn(false);
        }
        private void MoneyPanelOn(bool status)
        {
            if (status)
            {
                ChangeStatusPanel(moneyPanel, true);
                moneyPanelRectTransform.DOAnchorPosX(-121f,1f);
            }
            else
            {
                moneyPanelRectTransform.DOAnchorPosX(121f, 1f).OnComplete(() => { ChangeStatusPanel(moneyPanel, false);});
            }
        }
        private void DrawPhysicsBoundsPanelOn(bool status)
        {
            if (status)
            {
                ChangeStatusPanel(drawPhysicsBoundsPanel, true);
            }
            else
            {
                ChangeStatusPanel(drawPhysicsBoundsPanel, false);
            }
        }

        private void BackButtonOn(bool status)
        {
            if (status)
            {
                ChangeStatusPanel(backButtonRectTransform.gameObject, true);
                backButtonRectTransform.DOAnchorPosX(117f, 1f).OnComplete(() =>
                {
                    backButtonButtonUI.GetInteractable();
                });
            }
            else
            {
                backButtonRectTransform.DOAnchorPosX(-117f, 1f).OnComplete(() =>
                {
                    ChangeStatusPanel(backButtonRectTransform.gameObject, false);
                });
            }
        }
    }
}