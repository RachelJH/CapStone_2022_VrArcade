using VRBuilder.Core.Conditions;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.StatesAndData.UI.Conditions
{
    /// <inheritdoc />
    public class CompareBooleansMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Compare Booleans";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new CompareValuesCondition<bool>("Compare Booleans");
        }
    }
}
