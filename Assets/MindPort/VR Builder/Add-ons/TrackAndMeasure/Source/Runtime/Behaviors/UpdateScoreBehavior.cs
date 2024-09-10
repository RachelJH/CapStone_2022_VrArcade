using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.TrackAndMeasure.Properties;

namespace VRBuilder.TrackAndMeasure.Behaviors
{
    /// <summary>
    /// A behavior that updates a score and can provide feedback for it.
    /// </summary>
    [DataContract(IsReference = true)]
    public class UpdateScoreBehavior : Behavior<UpdateScoreBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="UpdateScoreBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Update Score")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Score data property")]
            public ScenePropertyReference<IDataProperty<float>> DataProperty { get; set; }

            [DataMember]
            [DisplayName("Score increase")]
            public float ValueDelta { get; set; }

            [DataMember]
            [DisplayName("Feedback property")]
            public ScenePropertyReference<IScoreFeedbackProperty> FeedbackProperty { get; set; }

            [DataMember]
            [DisplayName("Feedback position provider")]
            public SceneObjectReference FeedbackPositionProvider { get; set; }

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
                Data.DataProperty.Value.SetValue(Data.DataProperty.Value.GetValue() + Data.ValueDelta);

                if(Data.FeedbackProperty.Value == null)
                {
                    return;
                }

                Transform positionProvider = Data.FeedbackPositionProvider.Value != null ? Data.FeedbackPositionProvider.Value.GameObject.transform : Data.FeedbackProperty.Value.SceneObject.GameObject.transform;
                Data.FeedbackProperty.Value.TriggerFeedback(Data.ValueDelta, Data.DataProperty.Value.GetValue(), positionProvider);
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        public UpdateScoreBehavior() : this("", "", 0f, "")
        {
        }

        public UpdateScoreBehavior(string feedbackPropertyName, string dataPropertyName, float pointDelta, string positionProviderName, string name = "Update Score")
        {
            Data.FeedbackProperty = new ScenePropertyReference<IScoreFeedbackProperty>(feedbackPropertyName);
            Data.DataProperty = new ScenePropertyReference<IDataProperty<float>>(dataPropertyName);
            Data.ValueDelta = pointDelta;
            Data.FeedbackPositionProvider = new SceneObjectReference(positionProviderName);
        }

        public UpdateScoreBehavior(IScoreFeedbackProperty feedbackProperty, IDataProperty<float> dataProperty, float pointDelta, ISceneObject positionProvider, string name = "Update Score") : 
            this(ProcessReferenceUtils.GetNameFrom(feedbackProperty), ProcessReferenceUtils.GetNameFrom(dataProperty), pointDelta, ProcessReferenceUtils.GetNameFrom(positionProvider), name)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
