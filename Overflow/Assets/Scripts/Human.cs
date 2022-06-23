using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the different types of moves that a player can make
public enum Move {
    UNDEFINED,
    SNATCH,
    SWAP,
    STASH,
    SPILL
};

public class Human : Player
{
     
    private bool move = false; //Detemines whether the player can choose a move
    private bool select = false; //Determines whether the player can select a card
    private Move selectedMove = Move.UNDEFINED; //The move selected by the player
    private Card selectedCard; //The card selected by the player

    // Update is called once per frame
    void Update() {
        //TEMPORARY
        if (move) { //Move selection has been unlocked
            if (Input.GetKeyDown(KeyCode.Q)) {
                Debug.Log(this.name + " has decided to snatch");
                selectedMove = Move.SNATCH;
            }
            if (Input.GetKeyDown(KeyCode.W)) {
                Debug.Log(this.name + " has decided to swap");
                selectedMove = Move.SWAP;
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                Debug.Log(this.name + " has decided to stash");
                selectedMove = Move.STASH;
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                Debug.Log(this.name + " has decided to spill");
                selectedMove = Move.SPILL;
            }
        }

        if (select) { //Card selection has been unlocked
            if (Input.GetMouseButtonDown(0)) { //Look for a left click
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit && hit.collider.CompareTag("Card")) { //Clicked on a card
                    Debug.Log("Selected card: " + hit.transform.gameObject.name);
                    selectedCard = hit.transform.gameObject.GetComponent<CardDisplay>().card;
                }
            }
        }
    }

    public override IEnumerator Play() {
        //Determine which moves the player can make
        EnableInput();

        //Allow the player to select a move
        move = true;
        yield return ChooseMove();

        //Allow the player to select a card
        select = true;
        yield return ChooseCard();

        DisableInput();
    }

    private IEnumerator ChooseMove() {
        //Wait until a move has been selected
        while (selectedMove == Move.UNDEFINED) {
            yield return null;
        }
        //Move has been selected, disable the player's ability to select another move
        move = false;
    }

    private IEnumerator ChooseCard() {
        //Wait until the selected card matches the rules of the move
        while (!IsValidSelection(selectedMove, selectedCard)) {
            yield return null;
        }
        //Card has been selected, disable the player's ability to select another card
        select = false;
    }

    //Enables move buttons based on the state of the game
    private void EnableInput() {

    }

    //Resets the selected card and move for the next turn
    private void DisableInput() {
        selectedMove = Move.UNDEFINED;
        selectedCard = null;
    }

    public override void AddToHand(Card card) {
        if (Add(card, hand)) { //Check that adding the card is possible
            card.isFaceUp = true; //See the added card
            card.myObj.AddComponent<BoxCollider2D>(); //Enable interaction with the added card
        }
        else { //No free space to add the card, output error
            Debug.Log(this.name + " does not have space in its hand to add the card(" + card.rank + card.suit + ")");
        }
    }

    public override void RemoveFromHand(Card card) {
        if (Remove(card, hand)) { //Check that removing the card is possible
            Destroy(card.myObj.GetComponent<BoxCollider2D>()); //Disable interaction with the removed card
        }
        else { //Could not find the card, output error
            Debug.Log(this.name + " cannot remove the card(" + card.rank + card.suit + ") because it does not exist in the hand");
        }
    }

    //Determines whether the selected card can be played for the selected move
    private bool IsValidSelection(Move move, Card card) {
        if (card is null) { //Card has not been selected yet
            return false;
        }

        switch (move) {
            case Move.SNATCH:
                if (card.value == game.discard[game.discard.Count - 1].value) { //Selected card must match in rank with top of discard pile
                    return true;
                }
                else {
                    return false;
                }
            case Move.SWAP:
                if (card.isRed == game.discard[game.discard.Count - 1].isRed) { //Selected card must match in color with top of discard pile
                    return true;
                }
                else {
                    return false;
                }
            case Move.STASH:
                return true;
            case Move.SPILL:
                return true;
            default:
                return false;
        }
    }




}
