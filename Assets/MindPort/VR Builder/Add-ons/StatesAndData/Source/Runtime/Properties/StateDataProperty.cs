using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRBuilder.StatesAndData.Properties
{
    /// <summary>
    /// Template class for state data properties. Override with a non-generic class to create usable state data properties.
    /// </summary>    

    public abstract class StateDataProperty<T> : StateDataPropertyBase where T : Enum
    {
        [Header("Settings")]
        [SerializeField]
        private T defaultValue;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<T> valueChanged = new UnityEvent<T>();

        [SerializeField]
        private UnityEvent valueReset = new UnityEvent();

        private UnityEvent<int> intValueChanged = new UnityEvent<int>();

        /// <inheritdoc/>
        public override UnityEvent<int> OnValueChanged => intValueChanged;

        /// <inheritdoc/>
        public override UnityEvent OnValueReset => valueReset;

        /// <inheritdoc/>
        public override int DefaultValue => Convert.ToInt32(defaultValue);

        /// <inheritdoc/>
        public override Type StateType => typeof(T);

        protected void Start()
        {
            intValueChanged.AddListener(EmitValueChanged);
        }

        private void EmitValueChanged(int value)
        {
            valueChanged?.Invoke((T)Enum.ToObject(StateType, value));
        }

        /// <summary>
        /// Sets the current state to the specified value.
        /// </summary>
        public void SetState(T state)
        {
            SetValue(Convert.ToInt32(state));
        }

        /// <summary>
        /// Returns the current state.
        /// </summary>       
        public T GetState()
        {
            return (T)Enum.ToObject(StateType, GetValue());
        }

        protected override string ValueToString(int value)
        {
            return ((T)Enum.ToObject(StateType,value)).ToString();
        }
    }
}