using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core.Properties;

namespace VRBuilder.Randomization.Demo
{
    /// <summary>
    /// Handles two buttons to increase and decrease the pressure while they are being touched.
    /// </summary>
    public class PressureControl : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer increaseButton, decreaseButton;

        [SerializeField]
        private float increaseSpeed = 1f;

        private ISnappableProperty snappableProperty;

        private NumberDataProperty pressureData;

        private bool isTouchingIncrease;
        private bool isTouchingDecrease;

        public event EventHandler PressureChanged;

        private void Start()
        {
            snappableProperty = GetComponent<ISnappableProperty>();

            snappableProperty.Snapped += OnObjectSnapped;
            snappableProperty.Unsnapped += OnObjectUnsnapped;
        }

        public void OnIncreaseTouched(XRBaseInteractor interactor)
        {
            if (interactor is XRSocketInteractor)
            {
                return;
            }

            if (isTouchingDecrease)
            {
                OnDecreaseUntouched(interactor);
            }

            isTouchingIncrease = true;
            increaseButton.material.color = pressureData == null ? Color.red : Color.green;
        }

        public void OnDecreaseTouched(XRBaseInteractor interactor)
        {
            if (interactor is XRSocketInteractor)
            {
                return;
            }

            if (isTouchingIncrease)
            {
                OnIncreaseUntouched(interactor);
            }

            isTouchingDecrease = true;
            decreaseButton.material.color = pressureData == null ? Color.red : Color.green;
        }

        public void OnIncreaseUntouched(XRBaseInteractor interactor)
        {
            isTouchingIncrease = false;
            increaseButton.material.color = Color.white;
        }

        public void OnDecreaseUntouched(XRBaseInteractor interactor)
        {
            isTouchingDecrease = false;
            decreaseButton.material.color = Color.white;
        }

        private void OnObjectSnapped(object sender, EventArgs e)
        {
            pressureData = snappableProperty.SnappedZone.SceneObject.GameObject.GetComponent<NumberDataProperty>();
        }

        private void OnObjectUnsnapped(object sender, EventArgs e)
        {
            pressureData = null;
        }

        private void Update()
        {
            if (pressureData == null)
            {
                return;
            }

            if (isTouchingIncrease && !isTouchingDecrease)
            {
                pressureData.SetValue(pressureData.GetValue() + increaseSpeed * Time.deltaTime);
                PressureChanged?.Invoke(this, EventArgs.Empty);
            }

            if (!isTouchingIncrease && isTouchingDecrease)
            {
                pressureData.SetValue(pressureData.GetValue() - increaseSpeed * Time.deltaTime);
                PressureChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}