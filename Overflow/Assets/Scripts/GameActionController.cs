using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameActionController : MonoBehaviour
{
    private static Player currentPlayer; //The player currently using the action controller

    private static Button snatchButton;
    private static Button swapButton;
    private static Button stashButton;
    private static Button spillButton;
    private static Button endButton;

    private static Color defaultColor;
    [SerializeField] private Color selectedColor;

    //Initialize all user input objects in the game
    public static void Initialize() {
        //Initialize buttons
        snatchButton = GameObject.Find("SnatchButton").GetComponent<Button>();
        swapButton = GameObject.Find("SwapButton").GetComponent<Button>();
        stashButton = GameObject.Find("StashButton").GetComponent<Button>();
        spillButton = GameObject.Find("SpillButton").GetComponent<Button>();
        endButton = GameObject.Find("EndButton").GetComponent<Button>();

        defaultColor = endButton.GetComponentInChildren<TextMeshProUGUI>().color;

        //Make all buttons disabled at the start of the game
        snatchButton.interactable = false;
        swapButton.interactable = false;
        stashButton.interactable = false;
        spillButton.interactable = false;
        endButton.interactable = false;
    }

    //Enables buttons on the UI based on the state of the game
    public static void EnableInput(Player player) {
        //Observe the game state
        GameController game = FindObjectOfType<GameController>();

        //Enable input for player
        currentPlayer = player;

        if (player.selectedMove != Move.SPILL || game.spill.Count == 0) { //Player can choose from all moves if not chain spilling
            snatchButton.interactable = player.Exists(game.discard[game.discard.Count - 1].value, player.hand) ? true : false;
            swapButton.interactable = player.Exists(game.discard[game.discard.Count - 1].isRed, player.hand) ? true : false ;
            stashButton.interactable = true;
            spillButton.interactable = true;
        }
        else { //Player is currently chain spilling
            spillButton.interactable = true;
            endButton.interactable = true;
        }
    }

    //Disables all buttons on the UI
    public static void DisableInput() {
        //Disable input for player
        currentPlayer = null;

        //Ensure all button text is set back to default
        DeselectButtons();

        snatchButton.interactable = false;
        swapButton.interactable = false;
        stashButton.interactable = false;
        spillButton.interactable = false;
        endButton.interactable = false;
    }

    //Chooses snatch for player
    public void ChooseSnatch() {
        currentPlayer.selectedMove = Move.SNATCH;
        Debug.Log(currentPlayer.name + " has decided to SNATCH");

        DeselectButtons();
        snatchButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    //Chooses swap for player
    public void ChooseSwap() {
        currentPlayer.selectedMove = Move.SWAP;
        Debug.Log(currentPlayer.name + " has decided to SWAP");

        DeselectButtons();
        swapButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    //Chooses stash for player
    public void ChooseStash() {
        currentPlayer.selectedMove = Move.STASH;
        Debug.Log(currentPlayer.name + " has decided to STASH");

        DeselectButtons();
        stashButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    //Chooses spill for player
    public void ChooseSpill() {
        currentPlayer.selectedMove = Move.SPILL;
        Debug.Log(currentPlayer.name + " has decided to SPILL");

        DeselectButtons();
        spillButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    //Chooses end for player
    public void ChooseEnd() {
        currentPlayer.selectedMove = Move.END;
        Debug.Log(currentPlayer.name + " has decided to END");

        DeselectButtons();
        endButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    private static void DeselectButtons() {
        snatchButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        swapButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        stashButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        spillButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        endButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
    }

}
