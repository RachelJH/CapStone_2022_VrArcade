using Newtonsoft.Json;
using System.Runtime.Serialization;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Conditions;

namespace VRBuilder.Randomization.Conditions
{
    /// <summary>
    /// Condition that can be triggered directly. Used internally.
    /// </summary>
    [DataContract(IsReference = true)]
    public class RandomlySelectedCondition : Condition<RandomlySelectedCondition.EntityData>
    {
        public bool IsSetToComplete { get; set; }

        /// <summary>
        /// The data for a <see cref="CheckStateCondition"/>
        /// </summary>
        [DisplayName("Randomly Selected")]
        public class EntityData : IConditionData
        {
            /// <summary>
            /// The higher the value, the more likely for this condition to be completed.
            /// </summary>
            [DataMember]
            public float Weight { get; set; }

            /// <inheritdoc />
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            [DataMember]
            [HideInProcessInspector]
            public string Name { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            private RandomlySelectedCondition outerCondition;

            public ActiveProcess(EntityData data, IEntity outer) : base(data, outer)
            {
                outerCondition = outer as RandomlySelectedCondition;
            }

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                if(outerCondition.IsSetToComplete)
                {
                    outerCondition.IsSetToComplete = false;
                    return true;
                }

                return false;
            }
        }

        [JsonConstructor]
        public RandomlySelectedCondition(float weight = 1f)
        {
            Data.Weight = weight;  
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data, this);
        }
    }
}