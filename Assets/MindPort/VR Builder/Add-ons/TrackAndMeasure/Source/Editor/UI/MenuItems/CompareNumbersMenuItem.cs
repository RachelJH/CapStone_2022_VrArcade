using VRBuilder.Core.Conditions;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.TrackAndMeasure.UI.Conditions
{
    /// <inheritdoc />
    public class CompareNumbersMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Track and Measure/Compare Numbers";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new CompareValuesCondition<float>("Compare Numbers");
        }
    }
}
