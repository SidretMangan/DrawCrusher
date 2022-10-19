using DrawCrusher.Core;
namespace DrawCrusher.UIInput
{
    public class BackButton : ButtonUI
    {
        public override void ClickDown()
        {
            base.ClickDown();
            GameManager.instance.UpdateGameState(GameManager.GameState.EndGame);
        }
    }
}
