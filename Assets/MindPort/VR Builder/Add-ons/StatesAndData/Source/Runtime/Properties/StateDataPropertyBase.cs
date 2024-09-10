using System;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.StatesAndData.Properties
{
    /// <summary>
    /// Base class for state data properties.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class StateDataPropertyBase : DataProperty<int>
    {
        /// <summary>
        /// Returns the enum type for this state.
        /// </summary>
        public abstract Type StateType { get; }
    }
}
