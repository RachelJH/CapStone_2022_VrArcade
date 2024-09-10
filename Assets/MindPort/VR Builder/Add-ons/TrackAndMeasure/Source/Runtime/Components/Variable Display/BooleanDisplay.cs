using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Displays a <see cref="BooleanDataProperty"/> on a text mesh.
    /// </summary>
    public class BooleanDisplay : DataPropertyDisplay<bool>
    {
        [SerializeField]
        private string trueDisplayText = "Yes", falseDisplayText = "No";

        [SerializeField]
        private BooleanDataProperty dataProperty;

        /// <inheritdoc/>
        public override IDataProperty<bool> DataProperty { get { return dataProperty; } }

        /// <inheritdoc/>
        protected override void UpdateText()
        {
            textMesh.text = string.Format(text, DataProperty.GetValue() ? GetDisplayText(true) : GetDisplayText(false), DataProperty.SceneObject.UniqueName, DataProperty.SceneObject.GameObject.name);
        }

        private string GetDisplayText(bool value)
        {
            if (value && string.IsNullOrEmpty(trueDisplayText) == false)
            {
                return trueDisplayText;
            }
            else if (value == false && string.IsNullOrEmpty(falseDisplayText) == false) 
            {
                return falseDisplayText;
            }

            return value.ToString();
        }
    }
}