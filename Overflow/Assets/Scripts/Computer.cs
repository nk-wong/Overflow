using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Player
{
    private MemoryCache memory = new MemoryCache(11); //The cards in the discard pile that the computer remembers

    private float snatchWeight; //Heuristic for choosing the snatch move
    private float swapWeight; //Heuristic for choosing the swap move
    private float stashWeight; //Heuristic for choosing the stash move
    private float spillWeight; //Heuristic for choosing the spill move

    private readonly int SUIT_SIZE = 13; //The number of cards in one suit

    //TODO
    public override IEnumerator Play() {
        ChooseMove();

        yield return null;
    }

    private void ChooseMove() {
        snatchWeight = CalculateSnatchWeight();
        swapWeight = CalculateSwapWeight();
        stashWeight = 0; //TODO
        spillWeight = 0; //TODO

        Debug.Log(snatchWeight);
        Debug.Log(swapWeight);
        Debug.Log(stashWeight);
        Debug.Log(spillWeight);
    }

    //Determines the weight for the snatch action
    private float CalculateSnatchWeight() {
        if (Exists(game.discard[game.discard.Count - 1].value, hand)) { //Computer has a card that matches in rank with top of discard
            //Formula: player's score
            return score;
        }
        else {
            return Int32.MinValue;
        }
    }

    //Determines the weight for the swap action
    private float CalculateSwapWeight() {
        if (Exists(game.discard[game.discard.Count - 1].isRed, hand)) { //Computer has a card that matches in color with top of discard
            //Formula: (player's score) + (expected gain from another player's spill)
            float fail = ProbabilitySpillFails(game.discard[game.discard.Count - 1].suit);
            float expectedGain = (score * fail) + (-13 * (1 - fail));
            return score + expectedGain;
        }
        else {
            return Int32.MinValue;
        }
    }

    //Calculates the probability that a spill will fail based on the top of the discard
    private float ProbabilitySpillFails(string suit) {
        float knownCount = memory.CountSuit(suit, game.sets, hand);
        float numerator = SUIT_SIZE - knownCount;
        float denominator = game.deck.Count;
        float probability = (numerator / denominator) + (numerator / (denominator - 1)) + (numerator / (denominator - 2));
        return (probability > 1) ? 1.0f : probability;
    }

    //Notifies player to add card to memory cache
    public void Notify(Card card) {
        memory.Add(card, game.sets, hand);
        memory.PrintCache(this.name + ": Noticed the (" + card.rank + card.suit + ") |");
    }
 }
