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
    //private bool isGrounded = true;

    public Vector3 posRight;
    public Vector3 posLift;
    public Vector3 posMiddle;

    // Start is called before the first frame update
    void Start()
    {
        bodySourceView.jumped.Subscribe(jumped =>
        {
            Debug.Log("jumped");
            Jump(jumped);
        });

        bodySourceView.crouched.Subscribe(crouched =>
        {
            Debug.Log("crouched");
            Crouch(crouched);
        });
        
        bodySourceView.movedLeft.Subscribe(movedLeft =>
        {
            Debug.Log("movedLeft");
            MoveLeft(movedLeft);
        });
        
        bodySourceView.movedRight.Subscribe(movedRight =>
        {
            Debug.Log("movedRight");
            MoveRight(movedRight);
        });
        
        bodySourceView.movedMiddle.Subscribe(movedMiddle =>
        {
            Debug.Log("movedMiddle");
            MoveMiddle(movedMiddle);
        });
        
        playerTransform.position = new Vector3(0,1.14f,40f);
        for(int i = 0; i < amnTiles; i++)
        {
            spawn();
        }
    }

    private void Jump(bool jumped)
    {
        for(int i = 0; i <50; i++)
        {
            playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y + 0.10f, playerTransform.position.z);
            StartCoroutine(WaitShort());
        }

        StartCoroutine(WaitLong());
        
        for(int i = 0; i <50; i++)
        {
            playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y - 0.10f, playerTransform.position.z);
            StartCoroutine(WaitShort());
        }
    }

    private void Crouch(bool crouched)
    {
        for(int i = 0; i <100; i++)
        {
            playerTransform.Rotate(-1.8f,0,0);
            StartCoroutine(WaitShort());
        }

        StartCoroutine(WaitLong());
        for(int i = 0; i <100; i++)
        {
            playerTransform.Rotate(1.8f,0,0);
            StartCoroutine(WaitShort());
        }
    }

    private void MoveLeft(bool movedLeft)
    {
        //TODO calculate how much we have to go left
        
        for(int i = 0; i <150; i++)
        {
            playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y - 0.10f, playerTransform.position.z);
            StartCoroutine(WaitShort());
        }
    }

    private void MoveRight(bool movedRight)
    {
        //TODO calculate how much we have to go right
        for(int i = 0; i <150; i++)
        {
            playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y + 0.10f, playerTransform.position.z);
            StartCoroutine(WaitShort());
        }
    }

    private void MoveMiddle(bool movedMiddle)
    {
        //TODO calculate how much we have to go to middle
        if (playerTransform.position.x < posMiddle.x)
        {
            for(int i = 0; i <150; i++)
            {
                playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y + 0.10f, playerTransform.position.z);
                StartCoroutine(WaitShort());
            }
        }
        else
        {
            for(int i = 0; i <150; i++)
            {
                playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y - 0.10f, playerTransform.position.z);
                StartCoroutine(WaitShort());
            }
        }
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
    
    private IEnumerator WaitShort()
    {
        yield return new WaitForSeconds(0.004f);
    }
    private IEnumerator WaitLong()
    {
        yield return new WaitForSeconds(0.50f);
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
