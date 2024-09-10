using System;
using UnityEngine;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.ProcessUtils;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Editor.UI;
using VRBuilder.Editor.UI.Drawers;
using VRBuilder.StatesAndData.Conditions;
using VRBuilder.StatesAndData.Properties;

namespace VRBuilder.Editor.StatesAndData.UI.Drawers
{
    /// <summary>
    /// Custom drawer for <see cref="CheckStateCondition"/>.
    /// </summary>    
    [DefaultProcessDrawer(typeof(CheckStateCondition.EntityData))]
    public class CheckStateDrawer : NameableDrawer
    {
        private enum Operator
        {
            EqualTo,
            NotEqualTo,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
        }

        public override Rect Draw(Rect rect, object currentValue, Action<object> changeValueCallback, GUIContent label)
        {
            rect = base.Draw(rect, currentValue, changeValueCallback, label);

            float height = DrawLabel(rect, currentValue, changeValueCallback, label);

            height += EditorDrawingHelper.VerticalSpacing;

            Rect nextPosition = new Rect(rect.x, rect.y + height, rect.width, rect.height);

            CheckStateCondition.EntityData data = currentValue as CheckStateCondition.EntityData;

            nextPosition = DrawerLocator.GetDrawerForValue(data.DataProperty, typeof(ScenePropertyReference<IDataProperty<int>>)).Draw(nextPosition, data.DataProperty, (value) => UpdateDataProperty(value, data, changeValueCallback), "Data Property");
            height += nextPosition.height;
            height += EditorDrawingHelper.VerticalSpacing;
            nextPosition.y = rect.y + height;

            Operator currentOperator = GetCurrentOperator(data);
            nextPosition = DrawerLocator.GetDrawerForValue(currentOperator, typeof(Operator)).Draw(nextPosition, currentOperator, (value) => UpdateOperator(value, data, changeValueCallback), "Operator"); height += nextPosition.height;
            height += EditorDrawingHelper.VerticalSpacing;
            nextPosition.y = rect.y + height;

            if (RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsName(data.DataProperty.UniqueName) && data.DataProperty.Value != null)
            {
                StateDataPropertyBase stateData = (StateDataPropertyBase)data.DataProperty;
                Enum compareValue = (Enum)Enum.ToObject(stateData.StateType, data.CompareValue);

                nextPosition = DrawerLocator.GetDrawerForValue(compareValue, stateData.StateType).Draw(nextPosition, compareValue, (value) => UpdateState(value, data, changeValueCallback), "State");
                height += nextPosition.height;
                height += EditorDrawingHelper.VerticalSpacing;
                nextPosition.y = rect.y + height;
            }

            rect.height = height;
            return rect;
        }

        private void UpdateDataProperty(object value, CheckStateCondition.EntityData data, Action<object> changeValueCallback)
        {
            ScenePropertyReference<StateDataPropertyBase> newProperty = (ScenePropertyReference<StateDataPropertyBase>)value;
            ScenePropertyReference<StateDataPropertyBase> oldProperty = data.DataProperty;

            if (newProperty != oldProperty)
            {
                data.DataProperty = newProperty;
                changeValueCallback(data);
            }
        }

        private void UpdateState(object value, CheckStateCondition.EntityData data, Action<object> changeValueCallback)
        {
            int oldValue = data.CompareValue;
            int newValue = (int)value;

            if (newValue != oldValue)
            {
                data.CompareValue = newValue;
                changeValueCallback(data);
            }
        }

        private void UpdateOperator(object value, CheckStateCondition.EntityData data, Action<object> changeValueCallback)
        {
            Operator newOperator = (Operator)value;
            Operator oldOperator = GetCurrentOperator(data);

            if (newOperator != oldOperator)
            {
                switch (newOperator)
                {
                    case Operator.EqualTo:
                        data.Operation = new EqualToOperation<int>();
                        break;
                    case Operator.NotEqualTo:
                        data.Operation = new NotEqualToOperation<int>();
                        break;
                    case Operator.GreaterThan:
                        data.Operation = new GreaterThanOperation<int>();
                        break;
                    case Operator.LessThan:
                        data.Operation = new LessThanOperation<int>();
                        break;
                    case Operator.GreaterThanOrEqual:
                        data.Operation = new GreaterOrEqualOperation<int>();
                        break;
                    case Operator.LessThanOrEqual:
                        data.Operation = new LessThanOrEqualOperation<int>();
                        break;
                }

                changeValueCallback(data);
            }
        }

        private Operator GetCurrentOperator(CheckStateCondition.EntityData data)
        {
            Operator currentOperator = Operator.EqualTo;

            if (data.Operation.GetType() == typeof(EqualToOperation<int>))
            {
                currentOperator = Operator.EqualTo;
            }
            else if (data.Operation.GetType() == typeof(NotEqualToOperation<int>))
            {
                currentOperator = Operator.NotEqualTo;
            }
            else if (data.Operation.GetType() == typeof(GreaterThanOperation<int>))
            {
                currentOperator = Operator.GreaterThan;
            }
            else if (data.Operation.GetType() == typeof(LessThanOperation<int>))
            {
                currentOperator = Operator.LessThan;
            }
            else if (data.Operation.GetType() == typeof(GreaterOrEqualOperation<int>))
            {
                currentOperator = Operator.GreaterThanOrEqual;
            }
            else if (data.Operation.GetType() == typeof(LessThanOrEqualOperation<int>))
            {
                currentOperator = Operator.LessThanOrEqual;
            }

            return currentOperator;
        }

    }
}