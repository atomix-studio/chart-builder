using Atomix.ChartBuilder.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder.VisualElements
{
    public class Scatter2DChart : ChartBaseElement
    {

        /// <summary>
        /// Gradient based scatter, with normalized position as gradient inputs
        /// </summary>
        /// <param name="points"></param>
        public Scatter2DChart(double[,] points)
        {
            onRefresh += () => InitRange_pointsXY(points);

            backgroundColor = _backgroundColor;
            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateGradientColoredScatter(meshGenerationContext, points, true);
            };


            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
        }

        /// <summary>
        /// Takes a dictionnary of points with a color for each collection of points
        /// </summary>
        /// <param name="classedPoints"></param>
        public Scatter2DChart(Dictionary<Color, double[,]> classedPoints)
        {
            onRefresh += () => InitRange_colorClassedPoints(classedPoints);

            backgroundColor = _backgroundColor;
            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateClassColoredScatter(meshGenerationContext, classedPoints, true);
            };

            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
            this.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                TryRemoveCurrentLabel();
            });
        }

        /// <summary>
        /// Takles a matrix and color each point with normalized Element value with VisualizationSheet.visualizationSettings.
        /// </summary>
        /// <param name="pointsValues"></param>
        public Scatter2DChart(Dictionary<double[,], double> pointsValues)
        {
            onRefresh += () => InitRange_pointsXYValues(pointsValues);

            backgroundColor = _backgroundColor;
            generateVisualContent += (meshGenerationContext) =>
            {
                GenerateValueDrivenGradientColoredScatter(meshGenerationContext, pointsValues, true);
            }; 

            this.RegisterCallback<MouseMoveEvent>(evt => OnHoverMove(evt), TrickleDown.TrickleDown);
            this.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                TryRemoveCurrentLabel();
                _is_computingHoverMove = false;
            });
        }

        private bool _is_computingHoverMove;

        private async void OnHoverMove(MouseMoveEvent mouseEnterEvent)
        {
            if (_is_computingHoverMove)
                return;

            _is_computingHoverMove = true;

            var local_mouse_pos = this.WorldToLocal(mouseEnterEvent.mousePosition);
            var dist = float.MaxValue;
            var pos_key = Vector2.zero;
            var pos_value = Vector2Double.zero;

            await Task.Run(() =>
            {
                foreach (var plot in _plottedPositions)
                {
                    var crt = (local_mouse_pos - plot.Key).magnitude;
                    if (crt < dist)
                    {
                        dist = crt;
                        pos_key = plot.Key;
                        pos_value = plot.Value;
                    }
                }
            });

            TryRemoveCurrentLabel();

            var label = new Label(pos_value.ToString());
            label.name = "CURRENT_HOVER";
            label.style.position = new StyleEnum<Position>(Position.Absolute);
            label.style.left = pos_key.x;
            label.style.top = pos_key.y;
            label.AddToClassList("hover-text");
            this.Add(label);

            this.MarkDirtyRepaint();

            _is_computingHoverMove = false;
        }

        private void TryRemoveCurrentLabel()
        {
            var crt_hover = this.Q<Label>("CURRENT_HOVER");
            if (crt_hover != null)
                this.Remove(crt_hover);
        }

    }
}
