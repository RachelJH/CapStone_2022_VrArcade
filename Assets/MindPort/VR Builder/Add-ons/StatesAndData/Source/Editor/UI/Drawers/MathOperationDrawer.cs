using System;
using UnityEngine;
using VRBuilder.Core.ProcessUtils;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Editor.UI;
using VRBuilder.Editor.UI.Drawers;
using VRBuilder.StatesAndData.Behaviors;
using VRBuilder.StatesAndData.ProcessUtils;

namespace VRBuilder.Editor.StatesAndData.UI.Drawers
{
    /// <summary>
    /// Custom drawer for <see cref="MathOperationBehavior"/>.
    /// </summary>    
    [DefaultProcessDrawer(typeof(MathOperationBehavior.EntityData))]
    internal class MathOperationDrawer : NameableDrawer
    {
        private enum Operator
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max,
        }

        /// <inheritdoc/>
        public override Rect Draw(Rect rect, object currentValue, Action<object> changeValueCallback, GUIContent label)
        {
            rect = base.Draw(rect, currentValue, changeValueCallback, label);
            float height = rect.height;
            height += EditorDrawingHelper.VerticalSpacing;

            Rect nextPosition = new Rect(rect.x, rect.y + height, rect.width, rect.height);

            MathOperationBehavior.EntityData data = currentValue as MathOperationBehavior.EntityData;
            
            nextPosition = DrawerLocator.GetDrawerForValue(data.ModifiedProperty, typeof(ScenePropertyReference<IDataProperty<float>>)).Draw(nextPosition, data.ModifiedProperty, (value) => UpdateLeftOperand(value, data, changeValueCallback), "Left Operand");
            height += nextPosition.height;
            height += EditorDrawingHelper.VerticalSpacing;
            nextPosition.y = rect.y + height;

            Operator currentOperator = GetCurrentOperator(data);
            nextPosition = DrawerLocator.GetDrawerForValue(currentOperator, typeof(Operator)).Draw(nextPosition, currentOperator, (value) => UpdateOperator(value, data, changeValueCallback), "Operator");            
            height += nextPosition.height;
            height += EditorDrawingHelper.VerticalSpacing;
            nextPosition.y = rect.y + height;

            ProcessVariable<float> right = new ProcessVariable<float>(data.ModifierConst, data.ModifierProperty.UniqueName, data.IsModifierConst);

            nextPosition = DrawerLocator.GetDrawerForValue(right, typeof(ProcessVariable<float>)).Draw(nextPosition, right, (value) => UpdateRightOperand(value, data, changeValueCallback), "Right Operand");
            height += nextPosition.height;
            nextPosition.y = rect.y + height;

            rect.height = height;
            return rect;
        }

        private void UpdateLeftOperand(object value, MathOperationBehavior.EntityData data, Action<object> changeValueCallback)
        {
            ScenePropertyReference<IDataProperty<float>> newProperty = (ScenePropertyReference<IDataProperty<float>>)value;
            ScenePropertyReference<IDataProperty<float>> oldProperty = data.ModifiedProperty;

            if(newProperty != oldProperty)
            {
                data.ModifiedProperty = newProperty;
                changeValueCallback(data);
            }
        }

        private void UpdateRightOperand(object value, MathOperationBehavior.EntityData data, Action<object> changeValueCallback)
        {
            ProcessVariable<float> newOperand = (ProcessVariable<float>)value;
            ProcessVariable<float> oldOperand = new ProcessVariable<float>(data.ModifierConst, data.ModifierProperty.UniqueName, data.IsModifierConst);

            bool valueChanged = false;

            if (newOperand.PropertyReference != oldOperand.PropertyReference)
            {
                data.ModifierProperty = newOperand.PropertyReference;
                valueChanged = true;
            }

            if (newOperand.ConstValue.Equals(oldOperand.ConstValue) == false)
            {
                data.ModifierConst = newOperand.ConstValue;
                valueChanged = true;
            }

            if (newOperand.IsConst != oldOperand.IsConst)
            {
                data.IsModifierConst = newOperand.IsConst;
                valueChanged = true;
            }

            if (valueChanged)
            {
                changeValueCallback(data);
            }
        }

        private Operator GetCurrentOperator(MathOperationBehavior.EntityData data)
        {
            Operator currentOperator = Operator.Add;

            if (data.Operation.GetType() == typeof(SumOperation))
            {
                currentOperator = Operator.Add;
            }

            if (data.Operation.GetType() == typeof(SubtractOperation))
            {
                currentOperator = Operator.Subtract;
            }

            if (data.Operation.GetType() == typeof(MultiplyOperation))
            {
                currentOperator = Operator.Multiply;
            }

            if (data.Operation.GetType() == typeof(DivideOperation))
            {
                currentOperator = Operator.Divide;
            }

            if (data.Operation.GetType() == typeof(MaxOperation))
            {
                currentOperator = Operator.Max;
            }

            if (data.Operation.GetType() == typeof(MinOperation))
            {
                currentOperator = Operator.Min;
            }

            return currentOperator;
        }

        private void UpdateOperator(object value, MathOperationBehavior.EntityData data, Action<object> changeValueCallback)
        {
            Operator newOperator = (Operator)value;
            Operator oldOperator = GetCurrentOperator(data);

            if (newOperator != oldOperator)
            {
                switch(newOperator)
                {
                    case Operator.Add:
                        data.Operation = new SumOperation();
                        break;
                    case Operator.Subtract:
                        data.Operation = new SubtractOperation();
                        break;
                    case Operator.Multiply:
                        data.Operation = new MultiplyOperation();
                        break;
                    case Operator.Divide:
                        data.Operation = new DivideOperation();
                        break;
                    case Operator.Min:
                        data.Operation = new MinOperation();
                        break;
                    case Operator.Max:
                        data.Operation = new MaxOperation();
                        break;
                }
                
                changeValueCallback(data);
            }
        }
    }
}