using System;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Displays a <see cref="NumberDataProperty"/> representing a value in seconds as time on a text mesh.
    /// </summary>
    public class TimeDisplay : DataPropertyDisplay<float>
    {
        [SerializeField]
        private NumberDataProperty dataProperty;

        /// <inheritdoc/>
        public override IDataProperty<float> DataProperty { get { return dataProperty; } }

        /// <inheritdoc/>
        protected override void UpdateText()
        {
            textMesh.text = string.Format(text, new TimeSpan(0, 0, 0, 0, (int)(DataProperty.GetValue() * 1000)), DataProperty.SceneObject.UniqueName, DataProperty.SceneObject.GameObject.name);
        }
    }
}