using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.TrackAndMeasure.Behaviors;

namespace VRBuilder.Editor.TrackAndMeasure.UI.Behaviors
{
    /// <inheritdoc />
    public class SetBooleanValueMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Track and Measure/Set Boolean";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new SetValueBehavior<bool>("Set Boolean");
        }
    }
}
