using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.StatesAndData.UI.Behaviors
{
    /// <inheritdoc />
    public class SetNumberValueMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Set Number";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new SetValueBehavior<float>("Set Number");
        }
    }
}
