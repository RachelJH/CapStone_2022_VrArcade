using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Displays a <see cref="TextDataProperty"/> on a text mesh.
    /// </summary>
    public class TextDisplay : DataPropertyDisplay<string>
    {
        [SerializeField]
        private TextDataProperty dataProperty;

        /// <inheritdoc/>
        public override IDataProperty<string> DataProperty { get { return dataProperty; } }
    }
}