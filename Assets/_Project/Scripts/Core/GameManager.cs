using System;
using UnityEngine;
using DrawCrusher.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DrawCrusher.BlockManagement;

namespace DrawCrusher.Core
{
    public class GameManager : BaseSingleton<GameManager>
    {
        [Header("Connections")]
        [SerializeField] public UIManager uiManager;
        [SerializeField] private BlockGenerator blockGenerator;
        #region GameManage
        [HideInInspector] public GameState State;
        public static event Action<GameState> OnGameStateChanged;
        #endregion
        private async UniTaskVoid Start()
        {
            uiManager.LoadingScreenOn(true);
            await UniTask.Delay(2000);
            uiManager.StartButtonOn(true);
            uiManager.LoadingScreenOn(false);
        }
        public void UpdateGameState(GameState newState)
        {
            State = newState;
            switch (newState)
            {
                case GameState.StartGame:
                    HandleStartGame();
                    break;
                case GameState.EndGame:
                    HandleEndGame().Forget();
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }

        private void HandleStartGame()
        {
            uiManager.StartButtonOn(false);
            uiManager.StartGameStateUIBegin();
        }
        private async UniTaskVoid HandleEndGame()
        {
            uiManager.LoadingScreenOn(true);
            uiManager.EndGameStateUIBegin();
            await UniTask.Delay(2000);
            DOTween.KillAll();
            GC.Collect();
            await UniTask.Delay(1000);
            blockGenerator.Generate();
            await UniTask.Delay(1000);
            uiManager.LoadingScreenOn(false);
            uiManager.StartButtonOn(true);
        }
        public enum GameState
        {
            StartGame,
            EndGame
        }
    }
}
