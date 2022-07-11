using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCache
{
    private Card[] cache; //Holds cards that will be used to decide moves

    //Constructor
    public MemoryCache(int size) {
        //Make a cache with a capacity of the inputted integer
        cache = new Card[size];

    }

    public bool Add(Card card, List<Card> gameState) {
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
            string dominantSuit = FindDominantSuit(gameState);
            int lowestIndex = FindLowestIndex(dominantSuit);

            int newCardWeight = (card.suit == dominantSuit ? 13 : 0) - card.value;
            int oldCardWeight = (cache[lowestIndex].suit == dominantSuit ? 13 : 0) - cache[lowestIndex].value;
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

    //Counts the number of cards in the cache that match the inputted suit
    private int CountSuit(string suit) {
        int count = 0;
        for (int i = 0; i < cache.Length; i++) {
            if (!(cache[i] is null) && cache[i].suit == suit) { //Found a card that matches in suit, increment count
                count++;
            }
        }
        return count;
    }

    //Determine which suit is the most identified in the game
    private string FindDominantSuit(List<Card> gameState) {
        int spadeCount = 0;
        int heartCount = 0;
        int clubCount = 0;
        int diamondCount = 0;

        //Count the suits of known cards in the game
        for (int i = 0; i < gameState.Count; i++) {
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
    private int FindLowestIndex(string dominantSuit) {
        int lowest = Int32.MaxValue;
        int index = 0;
        for (int i = 0; i < cache.Length; i++) {
            int weight = (cache[i].suit == dominantSuit ? 13 : 0) - cache[i].value;
            if (weight < lowest) {
                lowest = weight;
                index = i;
            }
        }
        return index;
    }

    //Debugging function used to print out the memory cache
    public void PrintCache() {
        Debug.Log("--------------------------------------");
        for (int i = 0; i < cache.Length; i++) {
            if (!(cache[i] is null)) {
                Debug.Log("Slot " + i + " holds the " + cache[i].rank + cache[i].suit);
            }
        }
        Debug.Log("--------------------------------------");
    }
}
