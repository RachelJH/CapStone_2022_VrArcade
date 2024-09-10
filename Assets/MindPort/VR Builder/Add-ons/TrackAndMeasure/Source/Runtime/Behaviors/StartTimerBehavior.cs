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
    /// A behavior that starts a <see cref="TimerProperty"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    public class StartTimerBehavior : Behavior<StartTimerBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="StartTimerBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Start Timer")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Timer")]
            public ScenePropertyReference<ITimerProperty> TimerProperty { get; set; }

            [DataMember]
            [DisplayName("Is Countdown")]
            public bool IsCountdown { get; set; }

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
                Data.TimerProperty.Value.IsCountdown = Data.IsCountdown;
                Data.TimerProperty.Value.StartTimer();
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        public StartTimerBehavior() : this("", false)
        {
        }

        public StartTimerBehavior(string propertyName, bool isCountdown, string name = "Start Timer")
        {
            Data.TimerProperty = new ScenePropertyReference<ITimerProperty>(propertyName);
            Data.IsCountdown = isCountdown;
        }

        public StartTimerBehavior(ITimerProperty property, bool isCountdown, string name = "Start Timer") : this(ProcessReferenceUtils.GetNameFrom(property), isCountdown, name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
