using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour
{

    public void PlayGame() {
        SceneManager.LoadScene("Game");
    }

    public void LoadRules() {
        SceneManager.LoadScene("Rules");
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        Application.Quit();
    }
  
}
