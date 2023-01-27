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
    private bool jump = false;
    private bool slide = false;
    //private bool isGrounded = true;

    private bool currentlyRight;
    private bool currentlyLeft;

    void Start()
    {
        bodySourceView.jumped.Subscribe(jumped =>
        {
            if(jumped) {
                Jump();
            }
        });

        bodySourceView.crouched.Subscribe(crouched =>
        {
            if(crouched) {
                Crouch();
            }
        });
        
        bodySourceView.movedLeft.Subscribe(movedLeft =>
        {
            if(movedLeft) {
                MoveLeft();
            }
        });
        
        bodySourceView.movedRight.Subscribe(movedRight =>
        {   
            if(movedRight) {
                MoveRight();
            }
        });
        
        bodySourceView.movedMiddle.Subscribe(movedMiddle =>
        {
            if(movedMiddle) {
                MoveMiddle();
            }
        });
        
        playerTransform.position = new Vector3(0,1.1f,60f);
        for(int i = 0; i < amnTiles; i++)
        {
            spawn(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag == "Slide")
        {
            if(slide == false)
            {
                //Die
            }
        }
        else if(other.gameObject.tag == "Hinderniss")
        {
            if(jump == false)
            {
                //Die
            }
        }
    }

    private void Jump()
    {
        jump = true;
        Invoke("JumpReset", 1);
        playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*5);
    }

    private void JumpReset()
    {
        jump = false;
    }

    private void Crouch()
    {
        slide = true;
        Invoke("SlideReset", 1);
        // TODO: check for crouch
    }

    private void SlideReset()
    {
        slide = false;
    }

    private void MoveLeft()
    {
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.right * 6; 
        //playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left*4);
        currentlyRight = false;
        currentlyLeft = true;
    }

    private void MoveRight()
    {
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.left * 6; 
        //playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right*4);
        currentlyRight = true;
        currentlyLeft = false;
    }

    private void MoveMiddle()
    {
        //playerTransform.position = new Vector3(0, 1.1f, playerTransform.position.z);

        if (currentlyRight)
        {
            MoveLeft();
        }
        else
        {
            if (currentlyLeft)
            {
                MoveRight();
            }
            else
            {
                playerTransform.position = new Vector3(0, 1.1f, playerTransform.position.z);
            }
        }
        
        /*
        if(playerTransform.position.x > 2f)
            playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left*3);
        else
            playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right*3);
         */
    }
    
    private void spawn(int prefabIndex = -1)
    {
        int num = 0;
        if (prefabIndex == -1)
        {
            num = rnd.Next(0, tilePrefabs.Length);
        }
        
        GameObject go;
        go = Instantiate(tilePrefabs[num]);
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        if(num != 5 && num != 6 && num != 7 )
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
