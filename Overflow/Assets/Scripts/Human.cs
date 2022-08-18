using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Player
{
     
    private bool move = false; //Detemines whether the player can choose a move
    private bool select = false; //Determines whether the player can select a card
    private bool chain = false; //Determines whether the player can choose to chain spill

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
        if (chain) { //Chain spill has been unlocked
            if (Input.GetKeyDown(KeyCode.R)) {
                Debug.Log(this.name + " has decided to spill");
                selectedMove = Move.SPILL;
            }
            if (Input.GetKeyDown(KeyCode.T)) {
                Debug.Log(this.name + " has decided to end");
                selectedMove = Move.END;
            }
        }
        //TEMPORARY

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
        //Reset selections before making move or continue with chain spill
        selectedMove = (selectedMove == Move.SPILL) ? Move.SPILL : Move.UNDEFINED;
        selectedCard = null;

        //Determine which moves the player can make
        EnableInput();

        //Player selects a move
        if (selectedMove != Move.SPILL) { //Player only chooses a move if the player is not chain spilling
            yield return ChooseMove();
        }
        else { //Decide if player should continue chain spills
            yield return ChooseContinue();
        }

        //Player selects a card
        if (selectedMove != Move.END && (selectedMove != Move.STASH || game.stash.Count == 0)) { //Player only chooses a card when not ending turn or stealing
            yield return ChooseCard();
        }
        
        //Player performs the move
        yield return MakeMove();

        //Player's turn has ended
        DisableInput();
    }

    //Chooses the move that the player will make on the turn
    private IEnumerator ChooseMove() {
        //Allow player to select a move
        move = true;
        //Wait until a move has been selected
        while (selectedMove == Move.UNDEFINED) {
            yield return null;
        }
        //Move has been selected, disable the player's ability to select another move
        move = false;
    }

    //Chooses the card that the player will play on the turn
    private IEnumerator ChooseCard() {
        //Allow player to select a card
        select = true;
        //Wait until the selected card matches the rules of the move
        while (!IsValidSelection(selectedMove, selectedCard)) {
            yield return null;
        }
        //Card has been selected, disable the player's ability to select another card
        select = false;
    }

    //Chooses whether the player will continue to chain spill
    private IEnumerator ChooseContinue() {
        //Player has not decided move yet
        selectedMove = Move.UNDEFINED;

        //Allow player to choose whether to continue spill
        chain = true;
        //Wait until a move has been selected
        while (selectedMove == Move.UNDEFINED) {
            yield return null;
        }
        //Decision has been made
        chain = false;
    }

    //Enables move buttons based on the state of the game
    private void EnableInput() {

    }

    //Resets the selected card and move for the next turn
    private void DisableInput() {

    }

    public override GameObject AddToHand(Card card) {
        //Find the index that the new card was added into
        int result = Add(card, hand);
        if (result >= 0) { //Check that adding the card was possible
            card.isFaceUp = true; //See the added card
            card.myObj.AddComponent<BoxCollider2D>(); //Enable interaction with the added card
        }
        else { //No free space to add the card, output error
            Debug.Log(this.name + " does not have space in its hand to add the card(" + card.rank + card.suit + ")");
        }

        //Use the index to find the GameObject
        return this.transform.Find("Hand" + (result + 1)).gameObject;
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
