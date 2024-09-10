using System;
using System.Collections;
using UnityEngine;
using VRBuilder.XRInteraction;

namespace VRBuilder.StatesAndData.DemoScene
{
    /// <summary>
    /// Component that manages the color mixer machine.
    /// </summary>
    public class ColorMixer : MonoBehaviour
    {
        [SerializeField]
        private Transform hatch, ballSpawnPoint;

        [SerializeField]
        private MeshRenderer button;

        [SerializeField]
        private MixerLight mixerLight;

        private MixerStateDataProperty dataProperty;

        private void Start()
        {
            dataProperty = GetComponent<MixerStateDataProperty>();

            // Register to data property event to know when state is changed through VR Builder.
            dataProperty.ValueChanged += OnStateChanged;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            // Check which is the current state and call the relevant method.
            switch (dataProperty.GetState())
            {
                case MixerState.Processing:
                    StartCoroutine(HandleProcessingState());
                    break;
                case MixerState.Finished:
                    StartCoroutine(HandleFinishedState());
                    break;
            }
        }

        private IEnumerator HandleProcessingState()
        {
            // The button turns to red while the machine is running.
            button.material.color = Color.red;

            // Hatch closing animation.
            float elapsedTime = 0;
            Quaternion startRotation = hatch.transform.localRotation;

            while (elapsedTime <= 1)
            {
                elapsedTime += Time.deltaTime;
                hatch.transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.identity, elapsedTime);
                yield return null;
            }

            hatch.transform.rotation = Quaternion.identity;

            yield return new WaitForSeconds(2f);

            // Set a new state in the data property.
            dataProperty.SetState(MixerState.Finished);
        }

        private IEnumerator HandleFinishedState()
        {
            // The button returns green once the machine is done.
            button.material.color = Color.green;

            // Hatch opening animation.
            float elapsedTime = 0;
            Quaternion startRotation = hatch.transform.localRotation;

            while (elapsedTime <= 1)
            {
                elapsedTime += Time.deltaTime;
                hatch.transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(-30, 0, 0), elapsedTime);
                yield return null;
            }

            hatch.transform.rotation = Quaternion.Euler(-30, 0, 0);

            // Spawn a sphere, and make it the selected color.
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = Vector3.one * 0.2f;
            sphere.GetComponent<MeshRenderer>().material.color = mixerLight.ColorFromDataProperties();
            sphere.transform.position = ballSpawnPoint.position;

            // Make the sphere a generic grabbable object, not related to the VR Builder process.
            sphere.AddComponent<InteractableObject>();

            // Set a new state in the data property.
            dataProperty.SetState(MixerState.Idle);
        }
    }
}