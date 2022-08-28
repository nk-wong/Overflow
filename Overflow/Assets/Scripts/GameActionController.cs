using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameActionController : MonoBehaviour
{
    private GameController game; //Connect to current game session
    private Player currentPlayer; //The player currently using the action controller

    [SerializeField] private Button snatchButton;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button stashButton;
    [SerializeField] private Button spillButton;

    // Start is called before the first frame update
    void Start()
    {
        //Find game session
        game = FindObjectOfType<GameController>();

        //Make all buttons disabled at the start of the game
        snatchButton.interactable = false;
        swapButton.interactable = false;
        stashButton.interactable = false;
        spillButton.interactable = false;
    }

    //Enables buttons on the UI based on the state of the game
    public void EnableInput(Player player) {
        //Enable input for player
        this.currentPlayer = player;

        if (player.selectedMove != Move.SPILL || game.spill.Count == 0) { //Player can choose from all moves if not chain spilling
            snatchButton.interactable = player.Exists(game.discard[game.discard.Count - 1].value, player.hand) ? true : false;
            swapButton.interactable = player.Exists(game.discard[game.discard.Count - 1].isRed, player.hand) ? true : false ;
            stashButton.interactable = true;
            spillButton.interactable = true;
        }
        else { //Player is currently chain spilling
            spillButton.interactable = true;
        }
    }

    //Disables all buttons on the UI
    public void DisableInput() {
        //Disable input for player
        this.currentPlayer = null;

        snatchButton.interactable = false;
        swapButton.interactable = false;
        stashButton.interactable = false;
        spillButton.interactable = false;
    }

    //Chooses snatch for player
    public void ChooseSnatch() {
        this.currentPlayer.selectedMove = Move.SNATCH;
    }

    //Chooses swap for player
    public void ChooseSwap() {
        this.currentPlayer.selectedMove = Move.SWAP;
    }

    //Chooses stash for player
    public void ChooseStash() {
        this.currentPlayer.selectedMove = Move.STASH;
    }

    //Chooses spill for player
    public void ChooseSpill() {
        this.currentPlayer.selectedMove = Move.SPILL;
    }


}
