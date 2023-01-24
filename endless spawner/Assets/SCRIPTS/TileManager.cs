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
            Crouch(crouched);
        });
        
        bodySourceView.movedLeft.Subscribe(movedLeft =>
        {
            MoveLeft(movedLeft);
        });
        
        bodySourceView.movedRight.Subscribe(movedRight =>
        {
            MoveRight(movedRight);
        });
        
        bodySourceView.movedMiddle.Subscribe(movedMiddle =>
        {
            MoveMiddle(movedMiddle);
        });
        
        playerTransform.position = new Vector3(0,2f,60f);
        for(int i = 0; i < amnTiles; i++)
        {
            spawn(0);
        }
    }

    private void Jump(bool jumped)
    {
        playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*5);
    }

    private void Crouch(bool crouched)
    {
        // TODO: check for crouch
    }

    private void MoveLeft(bool movedLeft)
    {
        Debug.Log("moved middle");
        playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right);
    }

    private void MoveRight(bool movedRight)
    {
        Debug.Log("moved right");
        playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left);
    }

    private void MoveMiddle(bool movedMiddle)
    {
        if(playerTransform.position.x > 0)
            playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left);
        else
            playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right);
    }
    
    private void spawn(int prefabIndex = -1)
    {
        int num = 0;
        if (prefabIndex != 0)
        {
            num = rnd.Next(0, tilePrefabs.Length);
        }
        
        GameObject go;
        Debug.Log($"num={num}");
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
