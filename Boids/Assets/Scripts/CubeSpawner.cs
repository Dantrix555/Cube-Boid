using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int prefabAmount;
    [SerializeField] private float spawnRadius;

    void Start()
    {
        SpawnCubes();
    }

    void SpawnCubes()
    {
        for (int i = 0; i < prefabAmount; i++)
        {
            Instantiate(cubePrefab, transform.position + Random.insideUnitSphere * spawnRadius, Random.rotation);
        }
    }
}
