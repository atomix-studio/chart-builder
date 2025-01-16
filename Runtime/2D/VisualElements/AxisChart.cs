using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    public class AxisChart : ChartBaseElement
    {
        public AxisChart()
        {
            generateVisualContent += DrawAxisCallback;
        }

        public override Vector2Double current_range_y { get; set; }
        public override Vector2Double current_range_x { get; set; }
    }
}
