using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{

    private static GameObject winScreen; //The game object panel with UI
    private static TextMeshProUGUI winnerLabel; //The label game object that shows which player won the game

    //Initialize GUI for the win screen
    public static void Initialize() {
        //Initialize the win screen panel and win screen label
        winScreen = GameObject.Find("WinScreen");
        winnerLabel = GameObject.Find("WinnerLabel").GetComponent<TextMeshProUGUI>();

        //Make win screen hidden and disabled at the start of the game
        winScreen.SetActive(false);
    }

    //Create the win screen
    public static void GenerateWinScreen() {
        //Observe the game
        GameController game = FindObjectOfType<GameController>();

        Player winner = null; //Player with the highest score

        //Find out which player has the highest score
        for (int i = 0; i < game.playerObjs.Length; i++) {
            Player player = game.playerObjs[i].GetComponent<Player>();
            if (player.score == game.highestScore) { //Found player with highest score
                winner = player;
            }
        }

        //Show the win screen and update label
        winScreen.SetActive(true);
        winnerLabel.text = winner.name + " Wins!";
    }

    //Restarts the game
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Redirects to main menu
    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    //Quits the game
    public void QuitGame() {
        Application.Quit();
    }
    
}
