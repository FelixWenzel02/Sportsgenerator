using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    
    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Translate(new Vector3(0, 0, 0.1f));
    }
}
