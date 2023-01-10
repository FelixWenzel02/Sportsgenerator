using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    private Transform playerTransform;
    private float spawnZ = -6f;
    private float tileLength = 10f;
    private int tileAmount = 7;
    private int amnTiles = 15;
    System.Random rnd = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < amnTiles; i++)
        {
            spawn();
        }
    }

    private void spawn(int prefabIndex = -1)
    {
        GameObject go;
        go = Instantiate(obstaclePrefabs[rnd.Next(0, obstaclePrefabs.Length)]);
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += tileLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.z > (spawnZ - amnTiles * tileLength))
        {
            spawn();
        }
    }
}
