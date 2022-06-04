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
    public GameObject locationPrefab; //Template to build temporary locations
    public Sprite[] cardFaces; //Holds the image for each card in the deck

    //Fields to help position cards
    public GameObject deckObj; //The game object representing the deck pile
    public GameObject discardObj; //The game object representing the discard pile
    public GameObject spillObj; //The game object representing the spill pile
    public GameObject stashObj; //The game object representing the stash pile
    public GameObject[] playerObjs; //The game objects representing the players

    //Game constants
    private readonly int NUM_PLAYERS = 4;
    private readonly float SPEED_CONSTANT = 100.0f;
    
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
            zOffset += 0.1f;
        }
    }

    //Deals the cards in the deck pile to each player
    public IEnumerator DealDeck() {
        for (int i = 0; i < NUM_PLAYERS; i++) { //Give cards to players
            for (int j = 0; j < NUM_PLAYERS; j++) { //Give one card to each player
                Card card = deck[deck.Count - 1]; //Take the top card
                GameObject obj = playerObjs[j].transform.GetChild(i).gameObject; //Get the player 
                if (j+1 == 1) { //If dealing to player 1, flip the card
                    card.isFaceUp = true;
                }
                yield return StartCoroutine(MoveCard(card, obj, hands));
            }
        }
        //Place a starting card in the discard pile
        Card last = deck[deck.Count - 1];
        last.isFaceUp = true; //Flip the card over
        yield return StartCoroutine(MoveCard(last, discardObj, discard));

        Card lose1 = hands[0];
        yield return StartCoroutine(Snatch(lose1));

        Card lose2 = hands[0];
        yield return StartCoroutine(Snatch(lose2));

        Card lose3 = hands[0];
        yield return StartCoroutine(Snatch(lose3));

        Card lose4 = hands[0];
        yield return StartCoroutine(Snatch(lose4));
    }

    //Moves a card from one position to another position and updates the game decks accordingly
    private IEnumerator MoveCard(Card card, GameObject newPos, List<Card> newDeck) {
        List<Card> origDeck = FindPile(card); //Find the deck that the card currently resides in
        origDeck.Remove(card); //Remove the card from that deck
        newDeck.Add(card); //Add the card to the new deck

        AlignPile(deck, deckObj); //Make sure the deck is showing the correct top card
        AlignPile(discard, discardObj); //Make sure the discard is showing the correct top card
        AlignPile(spill, spillObj); //Make sure the spill is showing the correct top card

        //Set card rotation and position
        card.myObj.transform.rotation = newPos.transform.rotation;
        while (Vector3.Distance(card.myObj.transform.position, newPos.transform.position) != 0.0f) {
            card.myObj.transform.position = Vector3.MoveTowards(card.myObj.transform.position, newPos.transform.position, SPEED_CONSTANT * Time.deltaTime);
            yield return null;
        }
    }

    //Finds the pile that the card is currently residing in
    private List<Card> FindPile(Card card) {
        if (deck.Contains(card)) {
            return deck;
        }
        else if (discard.Contains(card)) {
            return discard;
        }
        else if (spill.Contains(card)) {
            return spill;
        }
        else if (hands.Contains(card)) {
            return hands;
        }
        else if (sets.Contains(card)) {
            return sets;
        }
        else {
            return null;
        }
    }

    //Ensures that the piles' cards are stacked correctly
    private void AlignPile(List<Card> pile, GameObject pos) {
        float zOffset = 0.0f;
        for (int i = 0; i < pile.Count; i++) {
            pile[i].myObj.transform.position = new Vector3(pos.transform.position.x, pos.transform.position.y,  pos.transform.position.z - zOffset);
            zOffset += 0.1f;
        }
    }

    //Move a card from hand to discard, take a new card from the top of the deck
    public IEnumerator Snatch(Card handCard) {
        //Save the position of the hand card
        GameObject temp = Instantiate(locationPrefab, new Vector3(handCard.myObj.transform.position.x, handCard.myObj.transform.position.y, handCard.myObj.transform.position.z), handCard.myObj.transform.rotation);

        //Move the hand card to the discard pile
        handCard.isFaceUp = true;
        yield return StartCoroutine(MoveCard(handCard, discardObj, discard));

        //Move the card on the top of the deck to the player's hand
        Card gain = deck[deck.Count - 1];
        yield return StartCoroutine(MoveCard(gain, temp, hands));

        //Remove the temp position
        Destroy(temp);
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
