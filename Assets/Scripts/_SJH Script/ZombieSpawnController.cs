using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnController : MonoBehaviour
{
    public GameObject[] zombies;
    public float spawnRangeX = 50f;
    public float spawnPosZ = -50f;
    public float startDelay = 2;
    public float spawnInterval = 2f;

    void Start()
    {
        InvokeRepeating("SpawnRandomZombie", startDelay, spawnInterval);    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnRandomZombie();
        }
    }

    void SpawnRandomZombie()
    {
        int ZombieIndex = Random.Range(0, zombies.Length);
        Vector3 spawnPos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0, spawnPosZ);
        Instantiate(zombies[ZombieIndex], spawnPos, zombies[ZombieIndex].transform.rotation);
    }
}
