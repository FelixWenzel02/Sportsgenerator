using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UniRx;

public class TileManager : MonoBehaviour
{

    public GameObject[] tilePrefabs;
    public Transform playerTransform;
    private float spawnZ = 40f;
    private float tileLength = 30f;
    private int tileAmount = 4;
    private int amnTiles = 7;
    System.Random rnd = new System.Random();
    public GameObject[] obstaclePrefabs;
    public BodySourceView bodySourceView;

    // Start is called before the first frame update
    void Start()
    {
        bodySourceView.jumped.Subscribe(jumped =>
        {
            Debug.Log("jumped");
            Jump();
        });

        bodySourceView.crouched.Subscribe(crouched =>
        {
            Debug.Log("crouched");
            Crouch();
        });
        
        playerTransform.position = new Vector3(0,1.14f,40f);
        for(int i = 0; i < amnTiles; i++)
        {
            spawn();
        }
    }

    private void Jump()
    {
        // TODO: Add jump implementation here
    }

    private void Crouch()
    {
        // TODO: Add jump implementation here
    }
    
    private void spawn(int prefabIndex = -1)
    {
        int num = rnd.Next(0, tilePrefabs.Length);
        GameObject go;
        go = Instantiate(tilePrefabs[num]);
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        if(num == 0 || num == 1 || num == 2)
        {
            int num2 = rnd.Next(0, obstaclePrefabs.Length);
            GameObject go2;
            go2 = Instantiate(obstaclePrefabs[num2]);
            go2.transform.SetParent(transform);
            go2.transform.position = Vector3.forward * spawnZ - new Vector3(0,0,0.5f);
        }
        else
        {

        }
        spawnZ += tileLength;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform.position.z > (spawnZ -amnTiles * tileLength))
        {
            spawn();
        }
    }
}
