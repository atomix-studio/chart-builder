namespace Atomix.ChartBuilder.VisualElements
{
    public class CandleBarChart : ChartBaseElement
    {
        public CandleBarChart(double[,] pointXY, float widthRatio = .8f)
        {
            onRefresh += () => InitRange_pointsXYZWV(pointXY);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateCandleBars(meshGenerationContext, pointXY, widthRatio, true);
            };
        }
    }
}
