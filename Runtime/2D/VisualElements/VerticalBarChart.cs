using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Atomix.ChartBuilder.VisualElements
{
    public class VerticalBarChart : ChartBaseElement
    {
        public VerticalBarChart(double[,] pointXY, float spacing = 0)
        {
            onRefresh += () => InitRange_pointsXY(pointXY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateVerticalBars(meshGenerationContext, pointXY, spacing, true);
            };
        } 
        
        public VerticalBarChart(Color color, double[,] pointXY, float spacing = 0)
        {
            onRefresh += () => InitRange_pointsXY(pointXY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateVerticalBarsWithColor(meshGenerationContext, color, pointXY, spacing, true);
            };
        }

        public VerticalBarChart(Dictionary<Color, double[,]> colored_pointXY, float spacing = 0)
        {
            onRefresh += () => InitRange_colorClassedPoints(colored_pointXY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateVerticalBarsWithColor(meshGenerationContext, colored_pointXY, spacing, true);
            };
        }
    }
}
