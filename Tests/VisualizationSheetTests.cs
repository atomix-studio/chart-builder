using Atomix.ChartBuilder.Math;
using Atomix.ChartBuilder.VisualElements;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Atomix.ChartBuilder
{
    [ExecuteInEditMode]
    public class VisualizationSheetTests : MonoBehaviour
    {
        [SerializeField] private VisualizationSheet _visualizationSheet;

        private void Reset()
        {
            _visualizationSheet = GetComponent<VisualizationSheet>();
        }

        private void Test_SimpleLine()
        {
            _visualizationSheet.Awake();
            var points = new double[100];

            for (int i = 0; i < 100; ++i)
            {
                points[i] = System.Math.Pow(i, 2);
            }

            var dimension = new Vector2Int(500, 500);

            // on crée une boite conteneur
            var parent = _visualizationSheet.AddContainer("c1", Color.black, dimension);
            parent.SetPadding(10, 10, 10, 10);

            var lineGraph = _visualizationSheet.Add_SimpleLine(points, 2, new Vector2Int(100, 100), parent);
            lineGraph.SetPadding(50, 50, 50, 50);
            lineGraph.gridSize = new Vector2Int(10, 10);
            lineGraph.DrawAutomaticGrid();

            lineGraph.Refresh();
        }

        private void Test_SimpleLine2()
        {
            _visualizationSheet.Awake();
            var points = new double[100];

            for (int i = 1; i < 100; ++i)
            {
                points[i] = 1f / System.Math.Pow(i, 2);
            }

            var parent = _visualizationSheet.AddContainer("c1", Color.green, new Vector2Int(500, 500));
            parent.SetPadding(10, 10, 10, 10);

            var line = _visualizationSheet.Add_SimpleLine(points, 2, new Vector2Int(100, 100), parent);

            line.DrawAxis();
        }

        private void Test_SimpleLine3()
        {
            _visualizationSheet.Awake();
            var points = new double[100];

            for (int i = 1; i < 100; ++i)
            {
                points[i] = i;
            }

            //var parent = _visualizationSheet.AddContainer("c1", Color.green, new Vector2Int(500, 500));

            _visualizationSheet.Add_SimpleLine(points, 2, new Vector2Int(500, 500), null);
        }

        private void Test_Scatter(int pCount = 100, int X = 100, int Y = 100)
        {
            _visualizationSheet.Awake();

            var points = new double[pCount, 2];

            for (int i = 0; i < pCount; ++i)
            {
                points[i, 0] = RandomHelpers.Shared.Range(-X, X);
                points[i, 1] = RandomHelpers.Shared.Range(-Y, Y);
            }

            var parent = _visualizationSheet.AddContainer("c0", Color.black, new Vector2Int(750, 750));
            parent.SetPadding(5, 5, 5, 5);
            var scatter = _visualizationSheet.Add_Scatter(points, new Vector2Int(100, 100), parent);
            scatter.SetPadding(50, 50, 50, 50);
            scatter.backgroundColor = Color.white;

            scatter.gridColor = new Color(.9f, .9f, .9f, .5f);

            scatter.DrawAutomaticGrid(12, "Densité des X", "Densité des Y");

            parent.SetTitle("Scatter Graph");
        }

        private void Test_ScatterFixedGrid(int pCount = 100, float X = 100, float Y = 100)
        {
            _visualizationSheet.Awake();

            var points = new double[pCount, 2];

            for (int i = 0; i < pCount; ++i)
            {
                points[i, 0] = RandomHelpers.Shared.Range(-X, X);
                points[i, 1] = RandomHelpers.Shared.Range(-Y, Y);
            }

            var parent = _visualizationSheet.AddContainer("c0", Color.black, new Vector2Int(750, 750));
            parent.SetPadding(5, 5, 5, 5);
            var scatter = _visualizationSheet.Add_Scatter(points, new Vector2Int(100, 100), parent);
            scatter.SetPadding(50, 50, 50, 50);
            scatter.backgroundColor = Color.white;
            scatter.gridSize = new Vector2Int(6, 6);
            scatter.gridColor = new Color(.9f, .9f, .9f, .5f);

            scatter.DrawAutomaticGrid(12, "Densité des X", "Densité des Y");

            parent.SetTitle("Scatter Graph");
        }

        private void Test_ScatterFixedGrid_Classes(int pCount = 500, float X = 100, float Y = 100)
        {
            _visualizationSheet.Awake();

            var dict = new Dictionary<Color, double[,]>();

            for (int j = 0; j < 3; ++j)
            {
                var points = new double[pCount / 3, 2];

                dict.Add(
                    new Color((float)RandomHelpers.Shared.Range(0.0, 1.0), (float)RandomHelpers.Shared.Range(0.0, 1.0), (float)RandomHelpers.Shared.Range(0.0, 1.0), 1),
                    points);

                for (int i = 0; i < pCount / 3; ++i)
                {

                    points[i, 0] = RandomHelpers.Shared.Range(-X, X);
                    points[i, 1] = RandomHelpers.Shared.Range(-Y, Y);
                }
            }

            var root = _visualizationSheet.AddPixelSizedContainer("root", new Vector2Int(800, 800), null);
            root.style.alignSelf = Align.Center;

            var parent = _visualizationSheet.AddContainer("c0", Color.black, new Vector2Int(100, 100), root);
            parent.SetPadding(5, 5, 5, 5);
            var scatter = _visualizationSheet.Add_Scatter(dict, new Vector2Int(100, 100), parent);
            scatter.SetPadding(50, 50, 50, 50);
            scatter.backgroundColor = Color.white;

            scatter.gridColor = new Color(.9f, .9f, .9f, .5f);

            scatter.DrawAutomaticGrid(12, "Densité des X", "Densité des Y");

            parent.SetTitle("Scatter Graph");
        }

        private void Test_ScatterFixedDelta_Values(int pCount = 500, int X = 100, int Y = 100)
        {
            _visualizationSheet.Awake();

            var dict = new Dictionary<double[,], double >();

            for (int j = 0; j < 3; ++j)
            {
                var points = new double[pCount / 3, 2];

                dict.Add(points,
                    (float)RandomHelpers.Shared.Range(0, X * Y));

                for (int i = 0; i < pCount / 3; ++i)
                {

                    points[i, 0] = RandomHelpers.Shared.Range(-X, X);
                    points[i, 1] = RandomHelpers.Shared.Range(-Y, Y);
                }
            }

            var root = _visualizationSheet.AddPixelSizedContainer("root", new Vector2Int(800, 800), null);
            root.style.alignSelf = Align.Center;

            var parent = _visualizationSheet.AddContainer("c0", Color.black, new Vector2Int(100, 100), root);
            parent.SetPadding(5, 5, 5, 5);
            var scatter = _visualizationSheet.Add_Scatter(dict, new Vector2Int(100, 100), parent);
            scatter.SetPadding(50, 50, 50, 50);
            scatter.backgroundColor = Color.white;

            scatter.gridColor = new Color(.9f, .9f, .9f, .5f);

            scatter.DrawAutomaticGrid(12, "Densité des X", "Densité des Y");

            parent.SetTitle("Scatter Graph");
        }

        private void OnGUI()
        {
            if (GUILayout.Button(nameof(Test_SimpleLine)))
            {
                Test_SimpleLine();
            }

            if (GUILayout.Button(nameof(Test_SimpleLine2)))
            {
                Test_SimpleLine2();
            }

            if (GUILayout.Button(nameof(Test_SimpleLine3)))
            {
                Test_SimpleLine3();
            }

            if (GUILayout.Button(nameof(Test_Scatter)))
            {
                Test_Scatter();
            }

            if (GUILayout.Button(nameof(Test_ScatterFixedGrid)))
            {
                Test_ScatterFixedGrid();
            }

            if (GUILayout.Button(nameof(Test_ScatterFixedGrid_Classes)))
            {
                Test_ScatterFixedGrid_Classes();
            }


            if (GUILayout.Button(nameof(Test_ScatterFixedGrid_Classes) + "_smallValues"))
            {
                Test_ScatterFixedGrid_Classes(35, 1, 1);
            }

            if (GUILayout.Button(nameof(Test_ScatterFixedDelta_Values)))
            {
                Test_ScatterFixedDelta_Values();
            }
        }
    }
}
