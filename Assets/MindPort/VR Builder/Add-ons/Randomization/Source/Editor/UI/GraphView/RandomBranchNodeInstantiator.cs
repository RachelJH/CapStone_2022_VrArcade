using VRBuilder.Core;
using VRBuilder.Editor.UI.Graphics;
using VRBuilder.Randomization.Behaviors;
using VRBuilder.Randomization.Conditions;

namespace VRBuilder.Editor.Randomization.UI.Graphics
{
    /// <summary>
    /// Instantiator for a <see cref="RandomBranchGraphNode"/>.
    /// </summary>
    public class RandomBranchNodeInstantiator : IStepNodeInstantiator
    {
        /// <inheritdoc/>
        public string Name => "Random Branch";

        /// <inheritdoc/>
        public bool IsInNodeMenu => true;

        /// <inheritdoc/>
        public string StepType => "randomBranch";

        /// <inheritdoc/>
        public int Priority => 200;

        /// <inheritdoc/>
        public ProcessGraphNode InstantiateNode(IStep step)
        {
            return new RandomBranchGraphNode(step);
        }
    }
}