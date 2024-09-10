using VRBuilder.Core.ProcessUtils;

namespace VRBuilder.StatesAndData.ProcessUtils
{
    /// <summary>
    /// Subtracts right from left.
    /// </summary>
    public class SubtractOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            return leftOperand - rightOperand;
        }
    }
}