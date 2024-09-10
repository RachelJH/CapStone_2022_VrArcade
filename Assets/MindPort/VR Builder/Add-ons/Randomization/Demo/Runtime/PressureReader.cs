using System;
using System.Collections;
using UnityEngine;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core.Properties;

namespace VRBuilder.Randomization.Demo
{
    /// <summary>
    /// Reads a number data property on the snap zone it is snapped to, and moves the hand of the dial accordingly.
    /// </summary>
    public class PressureReader : MonoBehaviour
    {
        private ISnappableProperty snappableProperty;
        private PressureControl pressureControl;
        private float measuredPressure = -1f;

        [SerializeField]
        Transform handMin, handMax, hand;

        [SerializeField]
        float scaleMax = 70f;

        private void Start()
        {
            snappableProperty = GetComponent<ISnappableProperty>();
            pressureControl = GetComponent<PressureControl>();

            snappableProperty.Snapped += OnPressureChange;
            snappableProperty.Unsnapped += OnPressureChange;
            pressureControl.PressureChanged += OnPressureChange;

            ReadPressure();
        }

        private void OnPressureChange(object sender, EventArgs e)
        {
            ReadPressure();
        }

        private float MeasurePressure()
        {
            ISnapZoneProperty snappedZone = snappableProperty.SnappedZone;

            if (snappedZone != null)
            {
                NumberDataProperty pressureData = snappableProperty.SnappedZone.SceneObject.GameObject.GetComponent<NumberDataProperty>();

                if (pressureData != null)
                {
                    return pressureData.GetValue();
                }
            }

            return 0f;
        }

        private void ReadPressure()
        {
            float newPressure = MeasurePressure();
            if (newPressure != measuredPressure)
            {
                measuredPressure = newPressure;
                StopAllCoroutines();
                StartCoroutine(MovePressureHand(hand.transform.localRotation, GetHandRotation(measuredPressure)));
            }
        }

        private Quaternion GetHandRotation(float pressure)
        {
            return Quaternion.Slerp(handMin.localRotation, handMax.localRotation, pressure / scaleMax);
        }

        IEnumerator MovePressureHand(Quaternion currentRotation, Quaternion targetRotation)
        {
            float time = 0f;
            float animationTime = 0.5f;

            while (time < animationTime)
            {
                time += Time.deltaTime;
                Mathf.Clamp(time, 0f, animationTime);
                hand.localRotation = Quaternion.Slerp(currentRotation, targetRotation, time / animationTime);
                yield return null;
            }
        }
    }
}