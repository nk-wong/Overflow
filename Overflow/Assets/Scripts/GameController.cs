using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the datatypes of a card
public struct Card {
    public string suit; //Holds the suit of the card
    public string rank; //Holds the pip of the card
    public int value; //Holds the numerical value of the card

    //Constructor
    public Card(string s, string r, int v) {
        this.suit = s;
        this.rank = r;
        this.value = v;
    }
    
    //Prints info about the card
    public void PrintCard() {
        Debug.Log(this.rank + " | " + this.suit + " | " + this.value);
    }
}

public class GameController : MonoBehaviour {
    
    public List<Card> deck;

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
        this.ShuffleDeck(deck);

        //Test that the deck is correct
        foreach (Card card in deck) {
            card.PrintCard();
        }
    }

    //Creates all the card objects for the deck
    public List<Card> GenerateDeck() {
        List<Card> newDeck = new List<Card>();
        string[] suits = new string[] {"D", "C", "H", "S"}; //The suits of a deck of playing cards
        string[] ranks = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"}; //The ranks of a deck of playing cards
        for (int i = 0; i < suits.Length; i++) { //Each suit
            for (int j = 0; j < ranks.Length; j++) { //Each rank
                newDeck.Add(new Card(suits[i], ranks[j], j+1)); //Create card and add to deck
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
}
