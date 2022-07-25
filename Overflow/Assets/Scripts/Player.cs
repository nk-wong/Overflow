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

public abstract class Player : MonoBehaviour
{

    protected GameController game; //Observes the game

    protected Card[] hand = new Card[4]; //The player's hand cards
    protected Card[] set = new Card[5]; //The player's point cards

    protected Move selectedMove = Move.UNDEFINED; //The move selected by the player
    protected Card selectedCard; //The card selected by the player

    public abstract IEnumerator Play(); //Runs through the player's turn
    public abstract GameObject AddToHand(Card card); //Adds the inputted card to the player's hand
    public abstract void RemoveFromHand(Card card); //Removes the inputted card from the player's hand

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the observer state
        game = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Adds an inputted card to an empty spot in a Card array and returns the index of the spot
    protected int Add(Card card, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (type[i] is null) { //Empty spot found
                type[i] = card;
                return i;
            }
        }

        return -1;
    }

    //Removes a specified card from the Card array
    protected bool Remove(Card card, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && type[i] == card) { //Specified card found
                type[i] = null;
                return true;
            }
        }

        return false;
    }

    //Performs the selected move for the player and removes any necessary cards
    public IEnumerator MakeMove() {
        switch (selectedMove) {
            case Move.SNATCH:
                RemoveFromHand(selectedCard);
                yield return game.Snatch(selectedCard, this);
                break;
            case Move.SWAP:
                RemoveFromHand(selectedCard);
                yield return game.Swap(selectedCard, this);
                break;
            case Move.STASH:
                RemoveFromHand(selectedCard);
                yield return game.Stash(selectedCard, this);
                break;
            case Move.SPILL:
                yield return game.Spill(this);
                break;
            default:
                Debug.Log(this.name + " could not find a move to perform");
                break;
        }
    }

    //Debugging function to identify which cards are in a player's hand
    public void PrintHand() {
        for (int i = 0; i < hand.Length; i++) {
            Debug.Log(this.name + " has card(" + hand[i].rank + hand[i].suit + ")");
        }
    }
}
