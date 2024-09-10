using System.Collections;
using System.Runtime.Serialization;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.ProcessUtils;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.StatesAndData.ProcessUtils;

namespace VRBuilder.StatesAndData.Behaviors
{
    /// <summary>
    /// A behavior that performs an operation on a <see cref="NumberDataProperty"/> and sets it to the new value.
    /// </summary>
    [DataContract(IsReference = true)]
    public class MathOperationBehavior : Behavior<MathOperationBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="MathOperationBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Math Operation")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [HideInProcessInspector]
            [UsesSpecificProcessDrawer("ValuePropertyDrawer")]
            public ScenePropertyReference<IDataProperty<float>> ModifiedProperty { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public IOperationCommand<float, float> Operation { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public ScenePropertyReference<IDataProperty<float>> ModifierProperty { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public float ModifierConst { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public bool IsModifierConst { get; set; }

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
                float modifierValue = Data.IsModifierConst ? Data.ModifierConst : Data.ModifierProperty.Value.GetValue();

                Data.ModifiedProperty.Value.SetValue(Data.Operation.Execute(Data.ModifiedProperty.Value.GetValue(), modifierValue));
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        public MathOperationBehavior() : this("", "", 0f, true, new SumOperation())
        {
        }

        public MathOperationBehavior(string modifiedPropertyName, string modifierPropertyName, float modifierValue, bool isModifierConst, IOperationCommand<float, float> operation, string name = "Math Operation")
        {
            Data.ModifiedProperty = new ScenePropertyReference<IDataProperty<float>>(modifiedPropertyName);
            Data.ModifierProperty = new ScenePropertyReference<IDataProperty<float>>(modifierPropertyName);
            Data.ModifierConst = modifierValue;           
            Data.IsModifierConst = isModifierConst;
            Data.Operation = operation;
            Data.Name = name;
        }

        public MathOperationBehavior(IDataProperty<float> modifiedProperty, IDataProperty<float> modifierProperty, float value, bool isModifierConst, IOperationCommand<float, float> operation, string name = "Math Operation") : 
            this(ProcessReferenceUtils.GetNameFrom(modifiedProperty), ProcessReferenceUtils.GetNameFrom(modifierProperty), value, isModifierConst, operation, name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
