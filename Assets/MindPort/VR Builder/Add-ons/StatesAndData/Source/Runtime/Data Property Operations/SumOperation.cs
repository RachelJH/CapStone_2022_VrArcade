using VRBuilder.Core.ProcessUtils;

namespace VRBuilder.StatesAndData.ProcessUtils
{
    /// <summary>
    /// Sums left and right.
    /// </summary>
    public class SumOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            return leftOperand + rightOperand;
        }
    }
}