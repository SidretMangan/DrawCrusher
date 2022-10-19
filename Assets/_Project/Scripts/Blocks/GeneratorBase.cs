using ProceduralToolkit;
using System.Collections.Generic;
using UnityEngine;

namespace DrawCrusher.BlockManagement
{
    public class GeneratorBase : MonoBehaviour
    {
        private Palette currentPalette = new Palette();
        private Palette targetPalette = new Palette();

        protected void GeneratePalette()
        {
            List<ColorHSV> palette = RandomE.TetradicPalette(0.25f, 0.7f);
            targetPalette.mainColor = palette[0].WithSV(0.8f, 0.6f);
            targetPalette.secondaryColor = palette[1].WithSV(0.8f, 0.6f);
        }

        protected ColorHSV GetMainColorHSV()
        {
            return targetPalette.mainColor;
        }

        protected ColorHSV GetSecondaryColorHSV()
        {
            return targetPalette.secondaryColor;
        }

        protected void SetupPalette()
        {
            currentPalette.mainColor = targetPalette.mainColor;
            currentPalette.secondaryColor = targetPalette.secondaryColor;
        }

        private class Palette
        {
            public ColorHSV mainColor;
            public ColorHSV secondaryColor;
        }
    }
}
