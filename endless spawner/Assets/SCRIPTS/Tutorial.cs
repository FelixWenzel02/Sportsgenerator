using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Button goBackToMenu;
    void Start()
    {
        goBackToMenu.onClick.AddListener(GoBackToMenu);
    }

    public void GoBackToMenu() {
        SceneManager.LoadScene(0);
    }

}
