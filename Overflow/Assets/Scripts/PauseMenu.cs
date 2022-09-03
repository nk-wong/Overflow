using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; //The game object that holds the pause menu UI

    //Pauses the execution of the game and shows pause menu
    public void PauseGame() {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    //Resumes the execution of the game and disables pause menu
    public void ResumeGame() {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    //Loads the main menu
    public void LoadMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    //Quits the game
    public void QuitGame() {
        Application.Quit();
    }
}
