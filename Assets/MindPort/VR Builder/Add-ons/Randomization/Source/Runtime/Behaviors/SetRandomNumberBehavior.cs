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
    public class SetRandomNumberBehavior : Behavior<SetRandomNumberBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetRandomNumberBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Set Random Number")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Data Property")]
            public ScenePropertyReference<IDataProperty<float>> DataProperty { get; set; }

            [DataMember]
            [DisplayName("Min Value")]
            public float MinValue { get; set; }

            [DataMember]
            [DisplayName("Max Value")]
            public float MaxValue { get; set; }

            [DataMember]
            [DisplayName("Randomize Integer")]
            public bool RandomizeInteger { get; set; }

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
                float randomValue;

                if(Data.RandomizeInteger)
                {
                    randomValue = Random.Range((int)Mathf.Floor(Data.MinValue), (int)Mathf.Floor(Data.MaxValue) + 1);
                }
                else
                {                    
                    randomValue = Random.Range(Data.MinValue, Data.MaxValue);
                }

                Data.DataProperty.Value.SetValue(randomValue);
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        [JsonConstructor]
        public SetRandomNumberBehavior() : this("", 0f, 0f, false)
        {
        }

        public SetRandomNumberBehavior(string propertyName, float minValue, float maxValue, bool randomizeInteger, string name = "Set Random Number")
        {
            Data.DataProperty = new ScenePropertyReference<IDataProperty<float>>(propertyName);
            Data.MinValue = minValue;
            Data.MaxValue = maxValue;
            Data.RandomizeInteger = randomizeInteger;
            Data.Name = name;            
        }

        public SetRandomNumberBehavior(IDataProperty<float> property, float minValue, float maxValue, bool randomizeInteger, string name = "Set Random Number") : this(ProcessReferenceUtils.GetNameFrom(property), minValue, maxValue, randomizeInteger, name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
