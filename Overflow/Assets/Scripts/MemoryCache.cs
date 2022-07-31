using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCache
{
    private Card[] cache; //Holds cards that will be used to decide moves

    private readonly int MAX_CACHE_SIZE = 52; //The max number of cards that a cache can hold

    //Constructor
    public MemoryCache(int size) {
        if (!(size > MAX_CACHE_SIZE)) {
            //Make a cache with a capacity of the inputted integer
            cache = new Card[size];
        }
        else {
            Debug.Log("A memory cache of size " + size + " cannot be created because it exceeds the limit of " + MAX_CACHE_SIZE);
        }
    }

    public bool Add(Card card, List<Card> gameState, Card[] hand) {
        if (!IsFull()) { //Cache still has space, add in empty spot
            for (int i = 0; i < cache.Length; i++) {
                if (cache[i] is null) { //Open spot found
                    cache[i] = card;
                    return true;
                }
            }

            //Failed to find spot to add card
            return false;
        }
        else {
            string dominantSuit = FindDominantSuit(gameState, hand);
            int lowestIndex = FindLowestIndex(dominantSuit, gameState, hand);

            int newCardWeight = card.suit == dominantSuit ? MAX_CACHE_SIZE : (CountSuit(card.suit, gameState, hand) - card.value);
            int oldCardWeight = cache[lowestIndex].suit == dominantSuit ? MAX_CACHE_SIZE : (CountSuit(cache[lowestIndex].suit, gameState, hand) - cache[lowestIndex].value);
            if (newCardWeight > oldCardWeight) {
                cache[lowestIndex] = card;
                return true;
            }
            return false;
        }
    }

    //Determine whether all spots in the cache are occupied by a card
    private bool IsFull() {
        for (int i = 0; i < cache.Length; i++) {
            if (cache[i] is null) { //Empty slot found, cache not full yet
                return false;
            }
        }
        return true;
    }

    //Empties the cache of all cards
    public void Clear() {
        for (int i = 0; i < cache.Length; i++) {
            cache[i] = null;
        }
    }

    //Counts the number of cards in the cache and game that match the inputted suit
    public int CountSuit(string suit, List<Card> gameState, Card[] hand) {
        int count = 0;
        for (int i = 0; i < gameState.Count; i++) {
            if (gameState[i].isFaceUp && gameState[i].suit == suit) { //Found a card in the game that matches in suit, increment count
                count++;
            }
        }
        for (int i = 0; i < cache.Length; i++) {
            if (!(cache[i] is null) && cache[i].suit == suit) { //Found a card in the cache that matches in suit, increment count
                count++;
            }
        }
        for (int i = 0; i < hand.Length; i++) {
            if (!(hand[i] is null) && hand[i].suit == suit) { //Found a card in the hand that matches in suit, increment count
                count++;
            }
        }
        return count;
    }

    //Determine which suit is the most identified in the game
    private string FindDominantSuit(List<Card> gameState, Card[] hand) {
        int spadeCount = 0;
        int heartCount = 0;
        int clubCount = 0;
        int diamondCount = 0;

        //Count the suits of known cards in the game
        for (int i = 0; i < gameState.Count; i++) {
            if (gameState[i].isFaceUp) {
                switch (gameState[i].suit) {
                    case "S":
                        spadeCount++;
                        break;
                    case "H":
                        heartCount++;
                        break;
                    case "C":
                        clubCount++;
                        break;
                    case "D":
                        diamondCount++;
                        break;
                    default:
                        break;
                }
            }
        }

        //Count the suits of remembered cards in the cache
        for (int i = 0; i < cache.Length; i++) {
            switch (cache[i].suit) {
                case "S":
                    spadeCount++;
                    break;
                case "H":
                    heartCount++;
                    break;
                case "C":
                    clubCount++;
                    break;
                case "D":
                    diamondCount++;
                    break;
                default:
                    break;
            }
        }

        //Count the suits of cards in the hand
        for (int i = 0; i < hand.Length; i++) {
            if (!(hand[i] is null)) {
                switch (hand[i].suit) {
                    case "S":
                    spadeCount++;
                    break;
                case "H":
                    heartCount++;
                    break;
                case "C":
                    clubCount++;
                    break;
                case "D":
                    diamondCount++;
                    break;
                default:
                    break;
                }
            }
        }

        //Determine which suit had the largest count
        int max = Math.Max(Math.Max(Math.Max(spadeCount, heartCount), clubCount), diamondCount);
        if (max == 0) { return ""; } //No dominant suit
        else if (max == spadeCount) { return "S"; } //Spade dominant
        else if (max == heartCount) { return "H"; } //Heart dominant
        else if (max == clubCount) { return "C"; } //Club dominant
        else if (max == diamondCount) { return "D"; } //Diamond dominant
        else { return null; } //Undefined
    }

    //Determine which index in the cache holds the least valuable card
    private int FindLowestIndex(string dominantSuit, List<Card> gameState, Card[] hand) {
        int lowest = Int32.MaxValue;
        int index = 0;
        for (int i = 0; i < cache.Length; i++) {
            int weight = cache[i].suit == dominantSuit ? MAX_CACHE_SIZE : (CountSuit(cache[i].suit, gameState, hand) - cache[i].value);
            if (weight < lowest) {
                lowest = weight;
                index = i;
            }
        }
        return index;
    }

    //Debugging function used to print out the memory cache
    public void PrintCache(string result) {
        for (int i = 0; i < cache.Length; i++) {
            if (!(cache[i] is null)) {
                result = result + cache[i].rank + cache[i].suit + "|";
            }
        }
        Debug.Log(result);
    }
}
