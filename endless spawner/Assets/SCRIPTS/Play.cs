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


    // this is used for checking whether the highscore has been reached
    public ScoreManager scoreManager;

    public TextMeshProUGUI scoreText;

    private void Awake() {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

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
            CheckAndSaveHighscore();
        }
    }

    // Checks if the highscore has been reached and saves it, so when starting the game anew it can be looked up
    private void CheckAndSaveHighscore() {
        if(scoreManager.highScoreValue < scoreValue) {
            // Highscore has been reached, so we could congratulate the player and set the new value
            scoreManager.highScoreValue = scoreValue;
            PlayerPrefs.SetFloat("highScore", scoreManager.highScoreValue);
            Debug.Log("Congratulations, you have unlocked the highscore!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Slide")
        {
            if(slide == false)
            {
                SceneManager.LoadScene(2);
                CheckAndSaveHighscore();
            }
        }
        else if(other.gameObject.tag == "Hindernisss")
        {
            if(jump == false)
            {
                SceneManager.LoadScene(2);
                CheckAndSaveHighscore();
            }
        }
    }
}
