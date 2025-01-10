using Atomix.ChartBuilder.Math;
using System;
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
            var parent = _visualizationSheet.AddContainer("c1", Color.green, dimension);
            parent.SetPadding(10, 10, 10, 10);

            // on ajoute un graphe qui contient seulement les axis
            var axis = _visualizationSheet.AddAxis("a1", new Color(0, 0, 0, 0), new Vector2Int(100, 100), parent);
            axis.backgroundColor = new Color(.9f, .9f, .9f, 1);

            var lineGraph = _visualizationSheet.Add_SimpleLine(points, 3, new Vector2Int(100, 100), axis);
            lineGraph.backgroundColor = new Color(0, 0, 0, 0);
            // permet d s'afficher par dessus les AXIS
            //lineGraph.backgroundColor = new Color(1, 1, 0, 1);
            lineGraph.SetPadding(10, 10, 10, 10);
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

            line.DrawBottomLeftGraduation();
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

        private void Test_Scatter(int pCount = 100, int X = 50, int Y = 50)
        {
            _visualizationSheet.Awake();

            var points = new double[pCount, 2];

            for (int i = 0; i < pCount; ++i)
            {
                points[i, 0] = RandomHelpers.Shared.Range(-X, X);
                points[i, 1] = RandomHelpers.Shared.Range(-Y, Y);
            }

            var parent = _visualizationSheet.AddContainer("c0", Color.black, new Vector2Int(500, 500));
            parent.SetPadding(5, 5, 5, 5);
            var scatter = _visualizationSheet.Add_Scatter(points, new Vector2Int(100, 100), parent);
            parent.SetTitle("Scatter Graph");
            scatter.SetPadding(25, 25, 25, 25);
            scatter.gridDelta = new Vector2Double(X * 2 / 10, X * 2 / 10);
            scatter.gridColor = new Color(.9f, .9f, .9f, 1);
            scatter.DrawAutomaticGrid();
        }

        private void Test_Texts()
        {
            var doc = GetComponent<UIDocument>();

            var root = doc.rootVisualElement;
            root.Clear();
            doc.panelSettings.targetTexture.Release();
            doc.panelSettings.targetTexture.Create();

            // Create the label
            var label = new Label("Hello, World!");
            label.style.position = Position.Absolute; // Use absolute positioning
            label.style.top = 0; // Align to the top
            label.style.left = new StyleLength(50);  // Add bas
            label.style.color = Color.white;

            /*
                        var parent = _visualizationSheet.AddContainer("c0", Color.white, new Vector2Int(500, 500));
                        parent.SetPadding(10, 10, 10, 10);
                        parent.SetTitle("Scatter Graph");*/

            root.Add(label);
            root.MarkDirtyRepaint();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("1"))
            {
                Test_SimpleLine();
            }

            if (GUILayout.Button("2"))
            {
                Test_SimpleLine2();
            }

            if (GUILayout.Button("3"))
            {
                Test_SimpleLine3();
            }

            if (GUILayout.Button("4"))
            {
                Test_Scatter();
            }
            
            if (GUILayout.Button("5"))
            {
                Test_Texts();
            }
        }
    }
}
