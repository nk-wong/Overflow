using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the different types of moves that a player can make
public enum Move {
    UNDEFINED,
    SNATCH,
    SWAP,
    STASH,
    SPILL,
    END
};

public abstract class Player : MonoBehaviour
{

    protected GameController game; //Observes the game

    public Card[] hand = new Card[4]; //The player's hand cards
    public Card[] set = new Card[5]; //The player's point cards

    public int score { get; private set; } //The player's score

    public Move selectedMove = Move.UNDEFINED; //The move selected by the player
    public Card selectedCard; //The card selected by the player

    public abstract IEnumerator Play(); //Runs through the player's turn

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

    //Returns true if a card in the Card array matches in suit with the input
    protected bool Exists(string suit, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && suit == type[i].suit) { //Found card that matches in suit
                return true;
            }
        }

        return false;
    }

    //Returns true if a card in the Card array matches in rank with the input
    protected bool Exists(int rank, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && rank == type[i].value) { //Found card that matches in rank
                return true;
            }
        }

        return false;
    }

    //Returns true if a card in the Card array matches in color with the input
    protected bool Exists(bool color, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && type[i].isRed == color) { //Found card that matches in color
                return true;
            }
        }

        return false;
    }

    //Returns true if a card in the Card array is greater than the input
    protected bool ExistsGreater(int rank, Card[] type) {
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && type[i].value > rank) { //Found card higher in value than input
                return true;
            }
        }

        return false;
    }

    //Returns the card with the highest value in a Card array
    protected Card HighestCard(Card[] type) {
        int index = 0;
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && type[i].value > type[index].value) {
                index = i;
            }
        }
        return type[index];
    }

    //Returns the card with the lowest value in the Card array
    protected Card LowestCard(Card[] type) {
        int index = 0;
        for (int i = 0; i < type.Length; i++) {
            if (!(type[i] is null) && type[i].value < type[index].value) {
                index = i;
            }
        }
        return type[index];
    }

    //Adds the inputted card to the player's hand
    public virtual GameObject AddToHand(Card card) {
        //Find the index that the new card was added into
        int result = Add(card, hand);
        if (result < 0) { //No free space to add the card, output error
            Debug.Log(this.name + " does not have space in its hand to add the card(" + card.rank + card.suit + ")");
        }

        //Use the index to find the GameObject
        return this.transform.Find("Hand" + (result + 1)).gameObject;
    }

    //Removes the inputted card from the player's hand
    public virtual void RemoveFromHand(Card card) {
        if (!Remove(card, hand)) { //Could not find the card, output error
            Debug.Log(this.name + " cannot remove the card(" + card.rank + card.suit + ") because it does not exist in the hand");
        }
    }

    //Adds the inputted card to the player's set
    public GameObject AddToSet(Card card) {
        //Find the index that the new card was added into
        int result = Add(card, set);
        if (result < 0) { //No free space to add the card, output error
            Debug.Log(this.name + " does not have space in its set to add the card(" + card.rank + card.suit + ")");
        }

        //If the final card added to a set is sticky, player loses all their points
        score = (card.isFaceUp == false && SetCount() == set.Length) ? 0 : CalculateScore();
        Debug.Log(this.name + " score is " + score);

        //Use the index to find the GameObject
        return this.transform.Find("Set" + (result + 1)).gameObject;
    }

    //Removes the inputted card from the player's set
    public void RemoveFromSet(Card card) {
        if (!Remove(card, set)) { //Could not find the card, output error
            Debug.Log(this.name + " cannot remove the card(" + card.rank + card.suit + ") because it does not exist in the set");
        }
    }

    //Returns the number of cards that have been set by the player
    public int SetCount() {
        int count = 0;
        for (int i = 0; i < set.Length; i++) {
            if (!(set[i] is null)) { //Found a set card
                count++;
            }
        }
        return count;
    }

    //Determines the player's score based on the value of the cards that have been set
    protected int CalculateScore() {
        int sum = 0;
        for (int i = 0; i < set.Length; i++) {
            if (!(set[i] is null) && set[i].isFaceUp) {
                sum += set[i].value;
            }
        }
        return sum;
    }

    //Performs the selected move for the player and removes any necessary cards
    public IEnumerator MakeMove() {
        switch (selectedMove) {
            case Move.SNATCH:
                yield return game.Snatch(selectedCard, this);
                break;
            case Move.SWAP:
                yield return game.Swap(selectedCard, this);
                break;
            case Move.STASH:
                yield return game.Stash(selectedCard, this);
                break;
            case Move.SPILL:
                yield return game.Spill(selectedCard, this);
                break;
            default:
                Debug.Log(this.name + " could not find a move to perform");
                break;
        }
    }

    //Debugging function to identify which cards are in a player's hand
    public void PrintHand() {
        for (int i = 0; i < hand.Length; i++) {
            if (!(hand[i] is null)) {
                Debug.Log(this.name + " has card(" + hand[i].rank + hand[i].suit + ")");
            }
        }
    }

    //Debugging function to identify which cards are in a player's hand
    public void PrintSet() {
        for (int i = 0; i < set.Length; i++) {
            if (!(set[i] is null)) {
                Debug.Log(this.name + " has card(" + set[i].rank + set[i].suit + ")");
            }
        }
    }
}
