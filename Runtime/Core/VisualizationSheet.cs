using Atomix.ChartBuilder.Settings;
using Atomix.ChartBuilder.VisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder
{
    public class VisualizationSheet : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        private VisualElement _root;

        private static VisualizationSettings _visualizationSettings;
        public static VisualizationSettings visualizationSettings
        {
            get
            {
                if (_visualizationSettings == null)
                {
                    _visualizationSettings = Resources.Load<VisualizationSettings>(nameof(VisualizationSettings));
                }

                return _visualizationSettings;
            }
        }

        public void Awake()
        {
            if (_document == null)
                _document = GetComponent<UIDocument>();

            _root = _document.rootVisualElement;
            _document.panelSettings.targetTexture.Release();
            _document.panelSettings.targetTexture.Create();

            _root.Children().ToList().ForEach(t => t.Clear());
            _root.Clear();
        }

        
        /// <summary>
        /// Ajoute un parent vide, avec une dimension en pixels
        /// </summary>
        /// <param name="name"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public ChartBuilderElement AddContainer(string name, Color backgroundColor, Vector2Int dimensions, VisualElement container = null)
        {
            var parent = new ChartBuilderElement();
            parent.name = name;

            parent.SetRelativePosition();
            parent.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);

            parent.style.backgroundColor = backgroundColor;

            AddChart(parent, container);

            return parent;
        }

        public T AddChart<T>(T chart, VisualElement container) where T : ChartBuilderElement
        {
            if (container == null)
                _root.Add(chart);
            else container.Add(chart);

            chart.Refresh();
            return chart;
        }

        public AxisChart AddAxis(string name, Color backgroundColor, Vector2Int dimensions, VisualElement container = null)
        {
            var chart = new AxisChart();

            chart.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);
            chart.name = name;
            chart.style.backgroundColor = backgroundColor;

            return AddChart(chart, container);
        }

        public SimpleLineChart Add_SimpleLine(double[,] matrice, float lineWidth, Vector2Int dimensions, VisualElement container = null)
        {
            var chart = new SimpleLineChart(matrice);

            chart.SetLineWidth(lineWidth);
            chart.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);

            return AddChart(chart, container);
        }

        public SimpleLineChart Add_SimpleLine(double[] points, float lineWidth, Vector2Int dimensions, VisualElement container = null)
        {
            var chart = new SimpleLineChart(points);

            chart.SetLineWidth(lineWidth);
            chart.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);

            return AddChart(chart, container);
        }

        public SimpleLineChart Add_SimpleLine(Func<List<Vector2>> getPoints, float lineWidth, Vector2Int dimensions, VisualElement container = null)
        {
            var chart = new SimpleLineChart(getPoints);
            chart.SetLineWidth(lineWidth);
            chart.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);

            return AddChart(chart, container);
        }

        public SimpleLineChart Add_SimpleLine(Func<List<double>> getPoints, float lineWidth, Vector2Int dimensions, VisualElement container = null)
        {
            var chart = new SimpleLineChart(getPoints);
            chart.SetLineWidth(lineWidth);
            chart.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);

            return AddChart(chart, container);
        }

        public Scatter2DChart Add_Scatter(double[,] matrice, Vector2Int dimensions, VisualElement container = null)
        {
            var chart = new Scatter2DChart(() => matrice);
            chart.SetDimensions(container != null ? LengthUnit.Percent : LengthUnit.Pixel, dimensions.x, dimensions.y);
            return AddChart(chart, container);
        }
    }
}
