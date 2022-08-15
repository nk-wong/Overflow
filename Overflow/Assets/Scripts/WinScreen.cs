using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{
    public static bool isActive; //Indicates whether the win screen should be visible

    public GameObject winScreen; //The game object panel with UI
    public TextMeshProUGUI winnerLabel; //The label game object that shows which player won the game

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            GenerateWinScreen();
        }
    }

    //Create the win screen
    public void GenerateWinScreen() {
        //Observe the game
        GameController game = FindObjectOfType<GameController>();

        Player winner = null; //Player with the highest score
        int maxScore = 0; //The highest score

        //Find out which player has the highest score
        foreach (GameObject obj in game.playerObjs) {
            Player player = obj.GetComponent<Player>();
            if (player.score > maxScore) { //Found player with score higher than current max
                winner = player;
                maxScore = player.score;
            }
        }

        //Show the win screen and update label
        winScreen.SetActive(true);
        winnerLabel.text = winner.name + " Wins!";
    }

    //Restarts the game
    public void RestartGame() {
        isActive = false;
        winScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Redirects to main menu
    public void LoadMenu() {
        isActive = false;
        winScreen.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    //Quits the game
    public void QuitGame() {
        Application.Quit();
    }
    
}
