using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Randomization.Behaviors
{
    /// <summary>
    /// A behavior that sets a boolean data property to a random value.
    /// </summary>
    [DataContract(IsReference = true)]
    public class SetRandomBooleanBehavior : Behavior<SetRandomBooleanBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetRandomBooleanBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Set Random Boolean")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Data Property")]
            public ScenePropertyReference<IDataProperty<bool>> DataProperty { get; set; }

            [DataMember]
            [DisplayName("Probability to be True")]
            [UsesSpecificProcessDrawer("NormalizedFloatDrawer")]
            public float TrueProbability { get; set; }

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
                float randomValue = Random.value;
                Data.DataProperty.Value.SetValue(randomValue <= Data.TrueProbability && randomValue != 0f);
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        [JsonConstructor]
        public SetRandomBooleanBehavior() : this("")
        {
        }

        public SetRandomBooleanBehavior(string propertyName, float trueProbability = 0.5f, string name = "Set Random Boolean")
        {
            Data.DataProperty = new ScenePropertyReference<IDataProperty<bool>>(propertyName);
            Data.TrueProbability = trueProbability;
            Data.Name = name;
        }

        public SetRandomBooleanBehavior(IDataProperty<bool> property, float trueProbability, string name = "Set Random Boolean") : this(ProcessReferenceUtils.GetNameFrom(property), trueProbability, name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
