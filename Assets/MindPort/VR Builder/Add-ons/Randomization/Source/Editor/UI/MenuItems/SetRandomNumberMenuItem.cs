using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.Randomization.Behaviors;

namespace VRBuilder.Editor.Randomization.UI.Behaviors
{
    /// <inheritdoc />
    public class SetRandomNumberMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Randomization/Set Random Number";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new SetRandomNumberBehavior();
        }
    }
}
