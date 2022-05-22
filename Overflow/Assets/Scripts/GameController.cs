using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    public List<Card> deck; //Holds all the cards in the deck pile
    public List<Card> discard; //Holds all the cards in the discard pile
    public List<Card> spill; //Holds all the cards in the spill pile
    public Card stash; //Holds the card in the stash pile

    //Fields to help build the card game objects
    public GameObject cardPrefab; //Template to build cards
    public Sprite[] cardFaces; //Holds the image for each card in the deck

    //Fields to help position cards
    public GameObject deckObj; //The game object representing the deck pile
    public GameObject discardObj; //The game object representing the discard pile
    public GameObject spillObj; //The game object representing the spill pile
    public GameObject stashObj; //The game object representing the stash pile
    
    // Start is called before the first frame update
    void Start()
    {
        PlayCards(); //Create and shuffle deck
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Prepares a shuffled deck of cards for the start of the game
    public void PlayCards() {
        this.deck = GenerateDeck();
        ShuffleDeck(deck);

        //Test that the deck is correct
        foreach (Card card in deck) {
            Debug.Log(card.rank + " | " + card.suit + " | " + card.value);
        }
        PlaceDeck();
    }

    //Creates all the card objects for the deck
    public List<Card> GenerateDeck() {
        List<Card> newDeck = new List<Card>();
        string[] suits = new string[] {"D", "C", "H", "S"}; //The suits of a deck of playing cards
        string[] ranks = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"}; //The ranks of a deck of playing cards
        for (int i = 0; i < suits.Length; i++) { //Each suit
            for (int j = 0; j < ranks.Length; j++) { //Each rank
                newDeck.Add(new Card(ranks[j], suits[i], j+1)); //Create card and add to deck
            }
        }
        return newDeck;
    } 

    //Shuffles cards in the deck
    public void ShuffleDeck(List<Card> list) {
        System.Random rand = new System.Random();
        for (int i = (list.Count - 1); i >= 0; i--) { //Touch each card in the deck and randomly swap with another card
            int j = rand.Next(list.Count);
            Card temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        } 
    }

    //Places the cards in the deck pile and instantiates game objects for each card
    public void PlaceDeck() {
        float zOffset = 0.0f;
        foreach(Card card in deck) {
            //Create a card object and set its name to match the rank and suit of a card in the deck
            GameObject newCard = Instantiate(cardPrefab, new Vector3(deckObj.transform.position.x, deckObj.transform.position.y, deckObj.transform.position.z - zOffset), Quaternion.identity); 
            newCard.name = card.rank + card.suit;

            //Have the card set its game object to the newly created game object
            card.SetObject();

            //Move each card down the z-axis to prevent cards from existing in the same spot
            zOffset += 0.03f;
        }
    }

    /*
    //Moves a card from one position to another position and updates the game decks accordingly
    public void MoveCard(GameObject origPos, GameObject newPos, List<Card> origDeck, List<Card> newDeck) {
        //The card has its position and rotation set to the game object that it is moving to
        origPos.transform.position = newPos.transform.position;
        origPos.transform.rotation = newPos.transform.rotation;

        //Move the card from its current deck to the new deck
        newDeck.Add(origDeck.RemoveAt(origDeck.Count - 1));
    }
    */

    //Debugging function to test synchronization between image and data of cards
    private void PrintList(List<Card> list) {
        foreach (Card card in list) {
            Debug.Log(card.rank + card.suit);
            Debug.Log(card.myObj.name);
            Debug.Log(card.myObj.GetComponent<CardDisplay>().card.rank);
            Debug.Log(card.myObj.GetComponent<CardDisplay>().card.suit);
        }
    }
}
