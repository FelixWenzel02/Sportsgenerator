using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
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
    private List<GameObject> activeTiles;
    private List<GameObject> activeObstacles;
    public BodySourceView bodySourceView;
    
    public Play player;
    //private bool isGrounded = true;

    private bool currentlyRight;
    private bool currentlyLeft;

    [SerializeField] 
    private Animator animator;

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
                Debug.Log("reactive left step");
                MoveLeft();
            }
        });
        
        bodySourceView.movedRight.Subscribe(movedRight =>
        {   
            if(movedRight) {
                Debug.Log("reactive right step");
                MoveRight();
            }
        });
        
        bodySourceView.movedMiddle.Subscribe(movedMiddle =>
        {
            if(movedMiddle) {
                Debug.Log("reactive middle step");
                MoveMiddle();
            }
        });
        
        playerTransform.position = new Vector3(0,1.1f,60f);
        for(int i = 0; i < amnTiles-3; i++)
        {
            spawn(0);
        }
        spawn();
        spawn();
    }

    private void Jump()
    {
        player.jump = true;
        Invoke("JumpReset", 1);
        animator.SetTrigger("Jump");
        playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*10);
    }

    private void JumpReset()
    {
        player.jump = false;
    }

    private void Crouch()
    {
        player.slide = true;
        animator.SetTrigger("Crouch");
        Invoke("SlideReset", 1);
        
    }

    private void SlideReset()
    {
        player.slide = false;
    }

    private void MoveLeft()
    {
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity += Vector3.left * 6.5f; 
        //playerTransform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left*4);
        currentlyRight = false;
        currentlyLeft = true;
    }

    private void MoveRight()
    {
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerTransform.gameObject.GetComponent<Rigidbody>().velocity += Vector3.right * 6.5f; 
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
            num = rnd.Next(1, tilePrefabs.Length);
        }
        
        GameObject go;
        go = Instantiate(tilePrefabs[num]);
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        activeTiles.Add(go);
        if(num != 0 && num != 5 && num != 6 && num != 7 )
        {
            int num2 = rnd.Next(0, obstaclePrefabs.Length);
            GameObject go2;
            go2 = Instantiate(obstaclePrefabs[num2]);
            go2.transform.SetParent(transform);
            go2.transform.position = Vector3.forward * spawnZ - new Vector3(0,0,0.5f);
            activeTiles.Add(go2);
        }
        else
        {

        }
        spawnZ += tileLength;
    }

    private void deleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private void deleteObstacle()
    {
        Destroy(obstaclePrefabs[0]);
        activeObstacles.RemoveAt(0);
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
            deleteTile();
            if(playerTransform.position.z > activeObstacles[0].transform.position.z + spawnZ)
            {
                deleteObstacle();
            }
        }

    }
}
