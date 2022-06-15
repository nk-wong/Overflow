using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{

    protected GameController game; //Observes the game
    protected Card[] hand = new Card[4]; //The player's hand cards
    protected Card[] set = new Card[5]; //The player's point cards

    public abstract void MakeMove();

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<GameController>(); //Initialize the observer state
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Adds the inputted card to the player's hand
    public int AddToHand(Card card) {
        for (int i = 0; i < hand.Length; i++) { //Find an open spot in the hand
            if (hand[i] is null) { //Open spot found
                hand[i] = card;
                return 0;
            }
        }

        //No free space to add the card, output error
        Debug.Log(this.name + " does not have space in its hand to add the card(" + card.rank + card.suit + ")");
        return -1;
    }

    //Removes the specified card from the player's hand
    private int RemoveFromHand(Card card) {
        for (int i = 0; i < hand.Length; i++) { //Find the inputted card
            if (hand[i] == card) { //Card has been found in hand
                hand[i] = null;
                return 0;
            }
        }

        //Could not find the card, output error
        Debug.Log(this.name + " cannot remove the card(" + card.rank + card.suit + ") because it does not exist in the hand");
        return -1;
    }

    //Debugging function to identify which cards are in a player's hand
    public void PrintHand() {
        for (int i = 0; i < hand.Length; i++) {
            Debug.Log(this.name + " has card(" + hand[i].rank + hand[i].suit + ")");
        }
    }
}
