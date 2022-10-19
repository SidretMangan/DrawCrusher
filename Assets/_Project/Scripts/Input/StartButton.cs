using DrawCrusher.Core;
namespace DrawCrusher.UIInput
{
    public class StartButton : ButtonUI
    {
        public override void ClickDown()
        {
            base.ClickDown();
            GameManager.instance.UpdateGameState(GameManager.GameState.StartGame);
        }
    }
}
