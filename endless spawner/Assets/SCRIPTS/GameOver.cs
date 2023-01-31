using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    [SerializeField] private Button mainMenu;

    void Start()
    {
        mainMenu.onClick.AddListener(BackToMainMenu);
    }

    private void BackToMainMenu() {
        SceneManager.LoadScene(0);
    }
   
}
