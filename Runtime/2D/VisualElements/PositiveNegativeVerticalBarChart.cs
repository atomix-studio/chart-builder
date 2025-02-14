namespace Atomix.ChartBuilder.VisualElements
{
    public class PositiveNegativeVerticalBarChart : ChartBaseElement
    {
        public PositiveNegativeVerticalBarChart(double[,] pointXY, float spacing = 0)
        {
            onRefresh += () => InitRange_pointsXY(pointXY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GeneratePositiveNegativeVerticalBars(meshGenerationContext, pointXY, spacing, true);
            };
        }
    }
}
