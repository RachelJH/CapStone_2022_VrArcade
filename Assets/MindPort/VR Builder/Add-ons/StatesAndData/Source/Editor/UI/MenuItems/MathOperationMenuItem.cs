using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.StatesAndData.Behaviors;

namespace VRBuilder.Editor.StatesAndData.UI.Behaviors
{
    /// <inheritdoc />
    public class MathOperationMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Math Operation";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new MathOperationBehavior(); 
        }
    }
}
