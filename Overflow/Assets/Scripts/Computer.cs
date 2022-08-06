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

    private readonly int MAX_SUIT_SIZE = 13; //The number of cards in one suit
    private readonly int MAX_RANK_VALUE = 13; //The highest value a card can have

    //TODO
    public override IEnumerator Play() {
        ChooseMove();

        yield return null;
    }

    private void ChooseMove() {
        snatchWeight = CalculateSnatchWeight();
        swapWeight = CalculateSwapWeight();
        stashWeight = CalculateStashWeight();
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
            float expectedGain = (score * (1 - fail)) + (-MAX_RANK_VALUE * fail);
            return score + expectedGain;
        }
        else {
            return Int32.MinValue;
        }
    }

    //Determines the weight for the stash action
    private float CalculateStashWeight() {
        if (game.stash.Count == 0) { //Stash is empty, computer can place a card in the stash
            //Formula: (player's score) + (expected gain from successfully stashing a card)
            float fail = ProbabilityStashFails(game.discard[game.discard.Count - 1].value);
            float expectedGain = 0.0f;
            if (ExistsGreater(game.discard[game.discard.Count - 1].value, hand)) {
                expectedGain = ((game.discard[game.discard.Count - 1].value + 1) * fail) + (-(game.discard[game.discard.Count - 1].value + 1) * (1 - fail));
            }
            else {
                expectedGain = (MAX_RANK_VALUE * (1 - fail)) + (-MAX_RANK_VALUE * fail);
            }
            return score + expectedGain;
        }
        else { //Stash is occupied, computer can steal from the stash
            float fail = ProbabilityStashFails(game.stashValue);
            float expectedGain = ((game.stashValue + 1) * (1 - fail)) + (-MAX_RANK_VALUE * fail);
            return score + expectedGain;
        }
    }

    //Calculates the probability that a spill will fail
    private float ProbabilitySpillFails(string suit) {
        float knownCount = memory.CountSuit(suit, game.sets, hand);
        float numerator = MAX_SUIT_SIZE - knownCount;
        float denominator = game.deck.Count;
        float probability = (numerator / denominator) + (numerator / (denominator - 1)) + (numerator / (denominator - 2));
        return (probability > 1) ? 1.0f : probability;
    }

    //Calculates the probability that a stash will result in a sticky card
    private float ProbabilityStashFails(int stashVal) {
        float knownCount = memory.CountRank(stashVal, game.sets, hand);
        float numerator = (stashVal * 4) - knownCount;
        float denominator = game.deck.Count + game.stash.Count;
        float probability = numerator / denominator;
        return (probability > 1) ? 1.0f : probability;
    }

    //Notifies player to add card to memory cache
    public void Notify(Card card) {
        memory.Add(card, game.sets, hand);
        memory.PrintCache(this.name + ": Noticed the (" + card.rank + card.suit + ") |");
    }
 }
