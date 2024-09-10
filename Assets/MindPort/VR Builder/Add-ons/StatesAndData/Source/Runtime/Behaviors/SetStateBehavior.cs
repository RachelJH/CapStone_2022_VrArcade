using System;
using System.Collections;
using System.Runtime.Serialization;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.StatesAndData.Properties;

namespace VRBuilder.StatesAndData.Behaviors
{
    /// <summary>
    /// Behavior that sets a <see cref="StateDataProperty{T}"/> to a specific value.
    /// </summary>
    public class SetStateBehavior : Behavior<SetStateBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetValueBehavior{T}"/> behavior data.
        /// </summary>
        [DisplayName("Set State")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [HideInProcessInspector]
            public ScenePropertyReference<StateDataPropertyBase> DataProperty { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public int NewValue { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            public string Name { get; set; }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            /// <inheritdoc />
            public override void End()
            {
                Data.DataProperty.Value.SetValue(Data.NewValue);
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        public SetStateBehavior() : this("", default)
        {
        }

        public SetStateBehavior(string propertyName, int value, string name = "Set State")
        {
            Data.DataProperty = new ScenePropertyReference<StateDataPropertyBase>(propertyName);
            Data.NewValue = value;
        }

        public SetStateBehavior(StateDataPropertyBase property, int value, string name = "Set State") : this(ProcessReferenceUtils.GetNameFrom(property), value, name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

    }
}