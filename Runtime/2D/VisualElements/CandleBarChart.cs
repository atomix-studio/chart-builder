using System.Collections.Generic;

namespace Atomix.ChartBuilder.VisualElements
{
    public struct CandleData
    {
        public double low;
        public double high;
        public double open;
        public double close;
        public double volume;
    }
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

        public CandleBarChart(List<CandleData> candles, float widthRatio = .8f)
        {
            onRefresh += () => InitRange_candles(candles);

            backgroundColor = _backgroundColor;

            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateCandleBars(meshGenerationContext, candles, widthRatio, true);
            };
        }

    }
}
