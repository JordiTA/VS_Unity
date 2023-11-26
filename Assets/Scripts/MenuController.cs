using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Levels To Load")]
    [SerializeField]
    private string newGameLevel;

    public void StartGameTutorial()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
