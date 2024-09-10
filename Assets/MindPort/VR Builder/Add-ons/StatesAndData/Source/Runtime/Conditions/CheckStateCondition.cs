using System.Runtime.Serialization;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.ProcessUtils;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.StatesAndData.Properties;

namespace VRBuilder.StatesAndData.Conditions
{
    /// <summary>
    /// A condition that compares a <see cref="StateDataProperty{T}"/> to a specified value and completes when the comparison returns true.
    /// </summary>
    [DataContract(IsReference = true)]
    public class CheckStateCondition : Condition<CheckStateCondition.EntityData>
    {
        /// <summary>
        /// The data for a <see cref="CheckStateCondition"/>
        /// </summary>
        [DisplayName("Check State")]
        public class EntityData : IConditionData
        {
            [DataMember]
            [HideInProcessInspector]
            public ScenePropertyReference<StateDataPropertyBase> DataProperty { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public IOperationCommand<int, bool> Operation { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public int CompareValue { get; set; }

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
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                int left = Data.DataProperty.Value.GetValue();
                int right = Data.CompareValue;

                return Data.Operation.Execute(left, right);
            }
        }

        public CheckStateCondition() : this("", default, new EqualToOperation<int>())
        {
        }

        public CheckStateCondition(StateDataPropertyBase dataProperty, int compareValue, IOperationCommand<int, bool> operation, string name = "Check State") :
            this(ProcessReferenceUtils.GetNameFrom(dataProperty), compareValue, operation, name)
        {
        }

        public CheckStateCondition(string dataProperty, int compareValue, IOperationCommand<int, bool> operation, string name = "Check State")
        {
            Data.DataProperty = new ScenePropertyReference<StateDataPropertyBase>(dataProperty);
            Data.CompareValue = compareValue;
            Data.Operation = operation;
            Data.Name = name;
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }
    }
}