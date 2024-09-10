using System;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.StatesAndData.DemoScene
{
    /// <summary>
    /// Component that matches the light on the machine to the currently mixed color.
    /// </summary>
    public class MixerLight : MonoBehaviour
    {
        [SerializeField]
        private NumberDataProperty red, green, blue;
        private MeshRenderer meshRenderer;

        // The maximum intensity value a single color can have. "7" means each color will have 8 shades (0-7). Not many, but we want each bottle to make a visible difference when added to the mix.
        private const int maximumIntensity = 7;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();

            // Register ValueChanged on all properties.
            red.ValueChanged += OnValueChanged;
            green.ValueChanged += OnValueChanged;
            blue.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            // Since the value of a property has changed, update the color of the light.
            meshRenderer.material.color = ColorFromDataProperties();
        }

        // Convert from our data property values to a 32 bit color. We are making this function public so we can access it from elsewhere.
        public Color ColorFromDataProperties()
        {
            return new Color(red.GetValue() / maximumIntensity, green.GetValue() / maximumIntensity, blue.GetValue() / maximumIntensity);
        }
    }
}