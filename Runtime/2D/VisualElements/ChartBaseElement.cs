using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITF.Modules.UIComponent.ChartBuilder.VisualElements
{
    public abstract class ChartBaseElement : ChartBuilderElement
    {
        protected float _lineWidth;
        protected Color _strokeColor = Color.black;
        protected Color _backgroundColor = Color.white;

        public Color strokeColor { get { return _strokeColor; } set { _strokeColor = value; } }

        public Color backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; style.backgroundColor = new StyleColor(_backgroundColor); } }

        protected double x_min = 0;
        protected double x_max = 0;
        protected double y_min = 0;
        protected double y_max = 0;

        #region Drawing

        public ChartBaseElement SetLineWidth(float lineWidth)
        {
            _lineWidth = lineWidth;
            return this;
        }

        public Vector2 Plot(double x_normalized, double y_normalized)
        {
            var x = (float)(paddingLeft + x_normalized * real_width);
            var y = (float)(paddingBottom + (1 - y_normalized) * real_heigth);

            return new Vector2(x, y);
        }

        #endregion

        #region Orthonormal lines & Grid
        /// <summary>
        /// Affiche les lignes X, Y ancrées en bas à gauche du graphe
        /// </summary>
        public void DrawBottomLeftGraduation()
        {
            // Implement graduation drawing logic here
            generateVisualContent += DrawOrthonormalLines_BottomLeftAnchored;
            Refresh();
        }

        public void DrawAutomaticCenteredGraduation()
        {
            // TODO
            Refresh();
        }

        protected void DrawOrthonormalLines_BottomLeftAnchored(MeshGenerationContext ctx)
        {
            var painter2D = ctx.painter2D;
            painter2D.lineWidth = _lineWidth;

            painter2D.BeginPath();

            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(1, 0));

            painter2D.MoveTo(Plot(0, 0));
            painter2D.LineTo(Plot(0, 1));

            painter2D.Stroke();
        }

        #endregion


        public void Refresh()
        {
            MarkDirtyRepaint();
        }
    }
}
