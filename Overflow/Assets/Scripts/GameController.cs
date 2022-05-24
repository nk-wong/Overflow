using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    public List<Card> deck { get; private set; } //Holds all the cards in the deck pile
    public List<Card> discard { get; private set; } //Holds all the cards in the discard pile
    public List<Card> spill { get; private set; } //Holds all the cards in the spill pile
    public Card stash { get; private set; } //Holds the card in the stash pile

    public List<Card> hands { get; private set; } //Holds all the cards in held by the players
    public List<Card> sets { get; private set; } //Holds all the cards that have been set

    //Fields to help build the card game objects
    public GameObject cardPrefab; //Template to build cards
    public Sprite[] cardFaces; //Holds the image for each card in the deck

    //Fields to help position cards
    public GameObject deckObj; //The game object representing the deck pile
    public GameObject discardObj; //The game object representing the discard pile
    public GameObject spillObj; //The game object representing the spill pile
    public GameObject stashObj; //The game object representing the stash pile
    public GameObject[] playerObjs; //The game objects representing the players

    //Game constants
    private readonly int NUM_PLAYERS = 4;
    
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
        StartCoroutine(DealDeck());
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

        //Generate the other decks
        discard = new List<Card>();
        spill = new List<Card>();
        hands = new List<Card>();
        sets = new List<Card>();

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

    //Deals the cards in the deck pile to each player
    public IEnumerator DealDeck() {
        float xOffset = 0.0f;
        float yOffset = 0.0f;
        float zOffset = 0.0f;
        for (int i = 0; i < NUM_PLAYERS; i++) { //Give cards to players
            for (int j = 0; j < NUM_PLAYERS; j++) { //Give one card to each player
                Card card = deck[deck.Count - 1]; //Take the top card
                Vector3 pos = playerObjs[j].transform.position; //Take player position
                switch (j+1) { //Adjust card position based on player position
                    case 1: //Player 1 position
                        pos.x += xOffset;
                        break;
                    case 2: //Player 2 position
                        pos.y -= yOffset;
                        break;
                    case 3: //Player 3 position
                        pos.x -= xOffset;
                        break;
                    case 4: //Player 4 position
                        pos.y += yOffset;
                        break;
                    default: //Error
                        Debug.Log("Unable to deal");
                        break;
                }
                pos.z -= zOffset;

                yield return new WaitForSeconds(0.05f); //Wait before placing a card
                card.myObj.transform.position = pos; //Set card position
                card.myObj.transform.rotation = playerObjs[j].transform.rotation; //Align card position to player rotation

                hands.Add(card); //Card is now in hand
                deck.RemoveAt(deck.Count - 1); //Card is no longer in the deck
            }
            xOffset += 1.0f;
            yOffset += 1.0f;
            zOffset += 1.0f;
        }
        //Place a starting card in the discard pile
        yield return new WaitForSeconds(0.05f);
        Card last = deck[deck.Count - 1];
        last.isFaceUp = true; //Flip the card over
        last.myObj.transform.position = discardObj.transform.position; //Card moves to the discard pile
        last.myObj.transform.rotation = discardObj.transform.rotation; //Align card with discard pile
        discard.Add(last); //Card is now in discard
        deck.RemoveAt(deck.Count - 1); //Card is no longer in deck
    }

    //Moves a card from one position to another position and updates the game decks accordingly
    private void MoveCard(Card card, GameObject newPos, List<Card> origDeck, List<Card> newDeck) {
        //The card has its position and rotation set to the game object that it is moving to
        card.myObj.transform.position = newPos.transform.position;
        card.myObj.transform.rotation = newPos.transform.rotation;

        //Move the card from its current deck to the new deck
        if (origDeck.Remove(card)) {
            Debug.Log("Remove successful for card: " + card.rank + card.suit);
            newDeck.Add(card);
        }
        else {
            Debug.Log("Remove unsuccessful for card: " + card.rank + card.suit);
        }
    }



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
