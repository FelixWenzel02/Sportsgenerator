using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitButton;

    private void Awake() 
    {
        startButton.onClick.AddListener(delegate {OnButtonClick(0);});
        tutorialButton.onClick.AddListener(delegate {OnButtonClick(1);});
        exitButton.onClick.AddListener(delegate {OnButtonClick(2);});
    }

    private void OnButtonClick(int index) 
    {
        if(index == 0)
        {
            SceneManager.LoadScene(1);
            return;
        }
        if(index == 1) 
        {
            SceneManager.LoadScene(3);
            return;
        }
        if(index == 2)
        {
            Application.Quit();
        }
    } 

}
