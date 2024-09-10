using VRBuilder.Core.Conditions;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.StatesAndData.Conditions;

namespace VRBuilder.Editor.StatesAndData.UI.Conditions
{
    /// <inheritdoc />
    public class CheckStateMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Check State";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new CheckStateCondition();
        }
    }
}
