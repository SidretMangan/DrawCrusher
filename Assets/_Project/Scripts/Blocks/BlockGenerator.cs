using ProceduralToolkit;

namespace DrawCrusher.BlockManagement
{
    public class BlockGenerator : GeneratorBase
    {
        public Blocker.Config config = new Blocker.Config();

        private Blocker blocker;

        private void Awake()
        {
            blocker = new Blocker();
            Generate();
            SetupPalette();
        }
        private void Generate()
        {
            GeneratePalette();

            config.gradient = ColorE.Gradient(GetMainColorHSV(), GetSecondaryColorHSV());

            blocker.Generate(config);
        }
    }
}
