using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.Randomization.UI.Behaviors
{
    /// <inheritdoc />
    public class SetBooleanValueMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Randomization/Set Boolean";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new SetValueBehavior<bool>("Set Boolean");
        }
    }
}
