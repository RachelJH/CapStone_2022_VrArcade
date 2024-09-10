using TMPro;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Displays a data property's value on a <see cref="TextMeshPro"/> component.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public abstract class DataPropertyDisplay<T> : MonoBehaviour
    {
        /// <summary>
        /// <see cref="IDataProperty{T}"/> to be displayed.
        /// </summary>
        public abstract IDataProperty<T> DataProperty { get; }       

        [SerializeField]   
        [TextArea]
        [Tooltip("These variables are supported with standard string formatting rules:\n" +
            "{0} - Property value\n" +
            "{1} - Property process object unique name\n" +
            "{2} - Property game object name")]
        protected string text = "{1}: {0}";

        protected TextMeshPro textMesh;

        /// <summary>
        /// Updates the displayed text.
        /// </summary>
        protected virtual void UpdateText()
        {
            textMesh.text = string.Format(text, DataProperty.GetValue(), DataProperty.SceneObject.UniqueName, DataProperty.SceneObject.GameObject.name);
        }

        protected virtual void OnEnable()
        {
            textMesh = GetComponent<TextMeshPro>();
            if(DataProperty != null)
            {
                DataProperty.OnValueChanged.AddListener(OnValueChanged);
                DataProperty.OnValueReset.AddListener(OnValueReset);
                UpdateText();
            }
        }

        protected virtual void OnDisable()
        {
            if (DataProperty != null)
            {
                DataProperty.OnValueChanged.RemoveListener(OnValueChanged);
                DataProperty.OnValueReset.RemoveListener(OnValueReset);
            }
        }

        private void OnValueReset()
        {
            UpdateText();
        }

        private void OnValueChanged(T value)
        {
            UpdateText();
        }
    }
}