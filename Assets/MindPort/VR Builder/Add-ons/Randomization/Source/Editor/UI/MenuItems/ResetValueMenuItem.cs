using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.Randomization.UI.Behaviors
{
    /// <inheritdoc />
    public class ResetValueMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Randomization/Reset Value";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new ResetValueBehavior();
        }
    }
}
