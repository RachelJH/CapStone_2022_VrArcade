using System.Collections;
using System.Runtime.Serialization;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.TrackAndMeasure.Properties;

namespace VRBuilder.TrackAndMeasure.Behaviors
{
    /// <summary>
    /// A behavior that stops a <see cref="TimerProperty"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    public class StopTimerBehavior : Behavior<StopTimerBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="StopTimerBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Stop Timer")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Timer")]
            public ScenePropertyReference<ITimerProperty> TimerProperty { get; set; }

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
                Data.TimerProperty.Value.StopTimer();
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        public StopTimerBehavior() : this("")
        {
        }

        public StopTimerBehavior(string propertyName, string name = "Stop Timer")
        {
            Data.TimerProperty = new ScenePropertyReference<ITimerProperty>(propertyName);
        }

        public StopTimerBehavior(ITimerProperty property, string name = "Stop Timer") : this(ProcessReferenceUtils.GetNameFrom(property), name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
