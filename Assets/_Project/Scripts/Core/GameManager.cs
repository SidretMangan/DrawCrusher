using System;
using UnityEngine;
using DrawCrusher.UI;

namespace DrawCrusher.Core
{
    public class GameManager : BaseSingleton<GameManager>
    {
        [Header("Connections")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private UIManager uiManager;
        #region GameManage
        [HideInInspector] public GameState State;
        public static event Action<GameState> OnGameStateChanged;
        private WaitForSeconds wait05 = new WaitForSeconds(0.5f);
        private WaitForSeconds wait1 = new WaitForSeconds(1f);
        private WaitForSeconds wait15 = new WaitForSeconds(1.5f);
        private WaitForSeconds wait2 = new WaitForSeconds(2f);
        private WaitForSeconds wait3 = new WaitForSeconds(3f);
        public int lastLevel;
        public bool endingLevel = false;
        private string levelText = "Level";
        private string tutorialShownText = "TutorialShown";
        #endregion
        private void Start()
        {
            //TODO first game state
        }
        public void UpdateGameState(GameState newState)
        {
            State = newState;
            //TODO after state set, switch(newState) here
            switch (newState)
            {
                case GameState.StartGame:
                    break;
                case GameState.PauseGame:
                    break;
                case GameState.EndGame:
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }

        private void HandleStartGame()
        {
            //TODO load playerprefs or SO or JSONUtility values to start from 
        }
        private void HandlePauseGame()
        {
            //TODO when application on pause invoke this one
        }
        private void HandleEndGame()
        {
            //TODO save playerprefs or SO or JSONUtility values to end 
        }
        public enum GameState
        {
            StartGame,
            PauseGame,
            EndGame
        }

    }
  


}
