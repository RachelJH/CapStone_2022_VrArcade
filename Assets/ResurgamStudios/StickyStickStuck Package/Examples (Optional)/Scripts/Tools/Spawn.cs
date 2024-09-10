/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for spawning objects.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StickyStickStuck
{
    public class Spawn : MonoBehaviour
    {
        #region Properties

        [SerializeField, Tooltip("Prefab gameobject to spawn.")]
        private GameObject spawnObject;
        public GameObject SpawnObject
        {
            get { return spawnObject; }
            set { spawnObject = value; }
        }

        [SerializeField, Tooltip("Spawn location transform.")]
        private Transform spawnLocation;
        public Transform SpawnLocation
        {
            get { return spawnLocation; }
            set { spawnLocation = value; }
        }

        [SerializeField, Tooltip("Max gameobjects the spawner can use.")]
        private int maxObjects = 25;
        public int MaxObjects
        {
            get { return maxObjects; }
            set { maxObjects = value; }
        }

        [SerializeField, Tooltip("The last spawned prefab."), Header("Spawned Objects:")]
        private GameObject lastSpawned;
        public GameObject LastSpawned
        {
            get { return lastSpawned; }
            set { lastSpawned = value; }
        }

        private List<GameObject> spawnedObjects;
        public List<GameObject> SpawnedObjects
        {
            get { return spawnedObjects; }
            set { spawnedObjects = value; }
        }

        private GameObject spawnObjectParent;
        private int index = 0;

        #endregion

        #region Unity Functions

        //Use this for initialization
        void Awake()
        {
            SetupSpawnPool();
        }

        #endregion

        #region Functions

        public void SetupSpawnPool()
        {
            if (spawnObject != null)
            {
                if (SpawnedObjects != null)
                {
                    for (int i = 0; i < SpawnedObjects.Count; i++)
                    {
                        Destroy(SpawnedObjects[i]);
                    }
                }
                SpawnedObjects = new List<GameObject>();

                spawnObjectParent = new GameObject(string.Format("Spawn-Pool: {0}", spawnObject.name));

                for (int i = 0; i < maxObjects; i++)
                {
                    GameObject spawnedObject = Instantiate(SpawnObject) as GameObject;

                    spawnedObject.transform.SetParent(spawnObjectParent.transform, false);

                    spawnedObject.SetActive(false);

                    SpawnedObjects.Add(spawnedObject);
                }
            }
        }

        public void Spawning(Vector3 force, Vector3 torque, float maxAngularVelocity = 0f)
        {
            if (index >= SpawnedObjects.Count)
                index = 0;

            if (index + 1 == SpawnedObjects.Count)
            {
                SpawnedObjects[0].SetActive(false);
            }
            else
            {
                SpawnedObjects[index + 1].SetActive(false);
            }

            SpawnedObjects[index].SetActive(true);
            
            LastSpawned = SpawnedObjects[index];

            LastSpawned.gameObject.transform.position = SpawnLocation.position;
            LastSpawned.gameObject.transform.rotation = SpawnLocation.rotation;

            if (LastSpawned.GetComponent<Rigidbody>() != null)
            {
                if (LastSpawned.GetComponent<Arrow>() != null)
                    LastSpawned.GetComponent<Arrow>().ResetInAir();

                LastSpawned.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                LastSpawned.GetComponent<Rigidbody>().velocity = Vector3.zero;

                LastSpawned.GetComponent<Rigidbody>().AddForce(force);
                LastSpawned.GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
                LastSpawned.GetComponent<Rigidbody>().AddTorque(torque);
            }
            if (LastSpawned.GetComponent<Rigidbody2D>() != null)
            {
                if (LastSpawned.GetComponent<Arrow2D>() != null)
                    LastSpawned.GetComponent<Arrow2D>().ResetInAir();

                LastSpawned.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                LastSpawned.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                LastSpawned.GetComponent<Rigidbody2D>().AddForce(force);
                LastSpawned.GetComponent<Rigidbody2D>().angularVelocity = maxAngularVelocity;
                LastSpawned.GetComponent<Rigidbody2D>().AddTorque(torque.x);
            }

            index++;

        }

        #endregion
    }
}