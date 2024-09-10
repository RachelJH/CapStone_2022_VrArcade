using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Displays a <see cref="NumberDataProperty"/> on a text mesh.
    /// </summary>
    public class NumberDisplay : DataPropertyDisplay<float>
    {
        [SerializeField]
        private NumberDataProperty dataProperty;

        /// <inheritdoc/>
        public override IDataProperty<float> DataProperty { get { return dataProperty; } }
    }
}