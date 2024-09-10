using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Utils.Logging;
using VRBuilder.Randomization.Conditions;

namespace VRBuilder.Randomization.Behaviors
{
    /// <summary>
    /// A behavior that completes a random <see cref="RandomlySelectedCondition"/> on the same step.
    /// </summary>
    [DataContract(IsReference = true)]
    public class SelectRandomTransitionBehavior : Behavior<SelectRandomTransitionBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SelectRandomTransitionBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Select Random Transition")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {          
            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            public string Name { get; set; }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data, IEntity outer) : base(data, outer)
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
                IStep step = null;
                IEntity parent = Outer.Parent;

                while (step == null && parent != null)
                {
                    if (parent as IStep != null)
                    {
                        step = parent as IStep;
                    }
                    else
                    {
                        parent = parent.Parent;
                    }
                }
                
                if (step != null)
                {
                    IEnumerable<RandomlySelectedCondition> conditions = step.Data.Transitions.Data.Transitions.SelectMany(t => t.Data.Conditions.Where(c => c is RandomlySelectedCondition).Select(c => c as RandomlySelectedCondition));
                    float totalWeights = 0f;

                    foreach (RandomlySelectedCondition condition in conditions)
                    {
                        totalWeights += condition.Data.Weight;
                    }

                    float random = Random.Range(0f, totalWeights);
                    float weightCounter = 0f;

                    foreach (RandomlySelectedCondition condition in conditions)
                    {
                        weightCounter += condition.Data.Weight;
                        if(weightCounter >= random)
                        {
                            if (LifeCycleLoggingConfig.Instance.LogSteps)
                            {
                                Debug.Log($"    <b>Random Branch</b> <i>'{step.Data.Name}'</i> selected transition to {GetTargetStepName(step, condition)}.\n");
                            }

                            condition.IsSetToComplete = true;
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log($"An error occurred with {Data.Name}. The behavior could not resolve.");
                }
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }

            private string GetTargetStepName (IStep step, ICondition condition)
            {
                ITransition transition = step.Data.Transitions.Data.Transitions.FirstOrDefault(t => t.Data.Conditions.Contains(condition));

                if(transition == null)
                {
                    Debug.LogError("Transition not found in step.");
                    return null;
                }

                return transition.Data.TargetStep != null ? $"<i>'{transition.Data.TargetStep.Data.Name}'</i>" : "end of Chapter";
            }
        }

        [JsonConstructor]
        public SelectRandomTransitionBehavior()
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data, this);
        }
    }
}

