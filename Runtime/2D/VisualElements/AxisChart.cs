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
            generateVisualContent += DrawOrthonormalLines_BottomLeftAnchored;
        }

        public AxisChart SetLineWidth(float lineWidth)
        {
            _lineWidth = lineWidth;
            return this;
        }
    }
}
