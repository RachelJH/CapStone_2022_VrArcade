using System.Globalization;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VRBuilder.Core;
using VRBuilder.Editor.UI.Graphics;
using VRBuilder.Editor.UndoRedo;
using VRBuilder.Randomization.Conditions;

namespace VRBuilder.Editor.Randomization.UI.Graphics
{
    /// <summary>
    /// Graphical representation of a random branch node.
    /// </summary>
    public class RandomBranchGraphNode : StepGraphNode
    {
        public RandomBranchGraphNode(IStep step) : base(step)
        {
        }

        /// <inheritdoc/>
        protected override void CreatePortWithUndo()
        {
            ITransition transition = EntityFactory.CreateTransition();
            transition.Data.Conditions.Add(new RandomlySelectedCondition());

            RevertableChangesHandler.Do(new ProcessCommand(
                () =>
                {
                    step.Data.Transitions.Data.Transitions.Add(transition);
                    AddTransitionPort();
                },
                () =>
                {
                    RemovePort(outputContainer[step.Data.Transitions.Data.Transitions.IndexOf(transition)] as Port);
                }
            ));
        }

        /// <inheritdoc/>
        public override Port AddTransitionPort(bool isDeletablePort = true, int index = -1)
        {
            Port port = base.AddTransitionPort(isDeletablePort, index);

            TextField weightField = new TextField();
            port.contentContainer.Insert(2, weightField);
            weightField.style.width = 40;

            int conditionIndex = outputContainer.IndexOf(port);
            RandomlySelectedCondition condition = step.Data.Transitions.Data.Transitions[conditionIndex].Data.Conditions.FirstOrDefault(c => c is RandomlySelectedCondition) as RandomlySelectedCondition;

            if (condition != null)
            {
                weightField.SetValueWithoutNotify(condition.Data.Weight.ToString());
                weightField.RegisterCallback<FocusOutEvent>(evt => OnWeightFieldChanged(evt, condition, weightField));
            }

            RefreshExpandedState();
            RefreshPorts();

            return port;
        }

        private void OnWeightFieldChanged(FocusOutEvent focusOutEvent, RandomlySelectedCondition condition, TextField textField)
        {
            float weight;
            string newValue = textField.text.Replace(',', '.');

            if (float.TryParse(newValue, NumberStyles.Number, CultureInfo.InvariantCulture, out weight))
            {
                weight = Mathf.Max(0f, weight);
                condition.Data.Weight = weight;
            }
            else
            {
                weight = condition.Data.Weight;
            }

            textField.SetValueWithoutNotify(weight.ToString());
        }
    }
}