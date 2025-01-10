using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Atomix.ChartBuilder.Settings
{
    [CreateAssetMenu(menuName = "ScriptableObjects/VisualizationSettigns")]
    public class VisualizationSettings : ScriptableObject
    {
        [SerializeField] private Gradient _warm_gradient;
        [SerializeField] private Gradient _cold_gradient;

        public Gradient warmGradient => _warm_gradient;
        public Gradient coldGradient => _cold_gradient;
    }
}
