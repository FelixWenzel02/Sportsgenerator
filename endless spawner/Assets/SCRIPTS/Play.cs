using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{

    public bool slide = false;
    public bool jump = false;
    private float scoreValue;

    public TextMeshProUGUI scoreText;

    private void Start() {
        scoreText.gameObject.SetActive(true);    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        scoreText.text = $"{Mathf.Ceil(scoreValue)}";
        scoreValue += 0.1f;
        gameObject.transform.Translate(new Vector3(0, 0, 0.2f));
        if(transform.position.y < -10) 
        {
            // Load Death Scene (= 2)
            SceneManager.LoadScene(2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Slide")
        {
            if(slide == false)
            {
                SceneManager.LoadScene(2);
            }
        }
        else if(other.gameObject.tag == "Hindernisss")
        {
            if(jump == false)
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}
