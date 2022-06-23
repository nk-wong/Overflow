using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{

    protected GameController game; //Observes the game
    protected Card[] hand = new Card[4]; //The player's hand cards
    protected Card[] set = new Card[5]; //The player's point cards

    public abstract IEnumerator Play(); //Runs through the player's turn
    public abstract void AddToHand(Card card); //Adds the inputted card to the player's hand
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

    //Adds an inputted card to an empty spot in a Card array
    protected bool Add(Card card, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (type[i] is null) { //Empty spot found
                type[i] = card;
                return true;
            }
        }

        return false;
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

    //Debugging function to identify which cards are in a player's hand
    public void PrintHand() {
        for (int i = 0; i < hand.Length; i++) {
            Debug.Log(this.name + " has card(" + hand[i].rank + hand[i].suit + ")");
        }
    }
}
