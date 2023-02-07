using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highscoreText;

    public float highScoreValue;

    private void Awake() {
        DontDestroyOnLoad(this);
        highScoreValue = PlayerPrefs.GetFloat("highScore");
        highscoreText.text = $"HighScore: {Mathf.Ceil(highScoreValue)}";
    }
    
}
