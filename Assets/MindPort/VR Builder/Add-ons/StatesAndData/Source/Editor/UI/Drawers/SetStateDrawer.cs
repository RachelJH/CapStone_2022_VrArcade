using System;
using UnityEngine;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Editor.UI;
using VRBuilder.Editor.UI.Drawers;
using VRBuilder.StatesAndData.Behaviors;
using VRBuilder.StatesAndData.Properties;

namespace VRBuilder.Editor.StatesAndData.UI.Drawers
{
    /// <summary>
    /// Custom drawer for <see cref="SetStateBehavior"/>.
    /// </summary>    
    [DefaultProcessDrawer(typeof(SetStateBehavior.EntityData))]
    public class SetStateDrawer : NameableDrawer
    {
        public override Rect Draw(Rect rect, object currentValue, Action<object> changeValueCallback, GUIContent label)
        {
            rect = base.Draw(rect, currentValue, changeValueCallback, label);

            float height = DrawLabel(rect, currentValue, changeValueCallback, label);

            height += EditorDrawingHelper.VerticalSpacing;

            Rect nextPosition = new Rect(rect.x, rect.y + height, rect.width, rect.height);

            SetStateBehavior.EntityData data = currentValue as SetStateBehavior.EntityData;

            nextPosition = DrawerLocator.GetDrawerForValue(data.DataProperty, typeof(ScenePropertyReference<IDataProperty<int>>)).Draw(nextPosition, data.DataProperty, (value) => UpdateDataProperty(value, data, changeValueCallback), "Data Property");
            height += nextPosition.height;
            height += EditorDrawingHelper.VerticalSpacing;
            nextPosition.y = rect.y + height;

            if(RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsName(data.DataProperty.UniqueName) && data.DataProperty.Value != null)            
            {
                StateDataPropertyBase stateData = (StateDataPropertyBase)data.DataProperty;
                Enum newValue = (Enum)Enum.ToObject(stateData.StateType, data.NewValue);

                nextPosition = DrawerLocator.GetDrawerForValue(newValue, stateData.StateType).Draw(nextPosition, newValue, (value) => UpdateState(value, data, changeValueCallback), "State");
                height += nextPosition.height;
                height += EditorDrawingHelper.VerticalSpacing;
                nextPosition.y = rect.y + height;
            }

            rect.height = height;
            return rect;
        }

        private void UpdateDataProperty(object value, SetStateBehavior.EntityData data, Action<object> changeValueCallback)
        {
            ScenePropertyReference<StateDataPropertyBase> newProperty = (ScenePropertyReference<StateDataPropertyBase>)value;
            ScenePropertyReference<StateDataPropertyBase> oldProperty = data.DataProperty;

            if (newProperty != oldProperty)
            {
                data.DataProperty = newProperty;
                changeValueCallback(data);
            }
        }

        private void UpdateState(object value, SetStateBehavior.EntityData data, Action<object> changeValueCallback)
        {
            int oldValue = data.NewValue;
            int newValue = (int)value;

            if (newValue != oldValue)
            {
                data.NewValue = newValue;
                changeValueCallback(data);
            }
        }
    }
}