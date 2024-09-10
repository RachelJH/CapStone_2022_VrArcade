using TMPro;
using UnityEngine;

namespace VRBuilder.TrackAndMeasure.Components
{
    /// <summary>
    /// Makes the game object float up and destroy itself after a time. Configures a <see cref="TextMeshPro"/> on the same object.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class FloatingTextMesh : MonoBehaviour
    {
        [SerializeField]
        private float duration = 3f;

        [SerializeField]
        private float speed = 0.1f;

        [SerializeField]
        private AnimationCurve transparencyCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private float startTime;
        private TextMeshPro textMesh;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        public void Initialize(string text)
        {
            textMesh.text = text;
        }

        private void Start()
        {
            startTime = Time.time;
            transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(transform.position - Camera.main.transform.position).eulerAngles.y, 0);
        }

        void Update()
        {
            transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(transform.position - Camera.main.transform.position).eulerAngles.y, 0);
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);   
            float elapsedTime = Time.time - startTime;

            float textAlpha = 1 - transparencyCurve.Evaluate(elapsedTime / duration);

            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, textAlpha);

            if(elapsedTime > duration)
            {
                Destroy(gameObject);
            }
        }
    }
}