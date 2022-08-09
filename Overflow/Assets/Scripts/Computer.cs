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

        ChooseCard();

        yield return null;
    }

    //Calculates the heuristic of each move to find out which move to make
    private void ChooseMove() {
        //Find out the strength of each move
        snatchWeight = CalculateSnatchWeight();
        swapWeight = CalculateSwapWeight();
        stashWeight = CalculateStashWeight();
        spillWeight = CalculateSpillWeight();

        Debug.Log(this.name + "|" + snatchWeight + "|" + swapWeight + "|" + stashWeight + "|" + spillWeight + "|");

        //Get the strongest move
        float max = Math.Max(Math.Max(Math.Max(snatchWeight, swapWeight), stashWeight), spillWeight);

        if ((SetCount() == set.Length - 1) && (max - game.highestScore) < 0) { //If computer cannot win from strongest move, take the least risky move
            //Find out the least risky move
            snatchWeight = (snatchWeight == Int32.MinValue) ? snatchWeight : -(snatchWeight - game.highestScore);
            swapWeight = (swapWeight == Int32.MinValue) ? swapWeight : -(swapWeight - game.highestScore);
            stashWeight = -(stashWeight - game.highestScore);
            spillWeight = -(spillWeight - game.highestScore);

            //Get the weakest move
            max = Math.Max(Math.Max(Math.Max(snatchWeight, swapWeight), stashWeight), spillWeight);

            Debug.Log(this.name + "|" + snatchWeight + "|" + swapWeight + "|" + stashWeight + "|" + spillWeight + "|");
        }

        //Determine move to make
        float tolerance = 0.0001f;
        if (Math.Abs(max - snatchWeight) <= tolerance) { //Snatch
            selectedMove = Move.SNATCH;
        }
        else if (Math.Abs(max - swapWeight) <= tolerance) { //Swap
            selectedMove = Move.SWAP;
        }
        else if (Math.Abs(max - stashWeight) <= tolerance) { //Stash
            selectedMove = Move.STASH;
        }
        else if (Math.Abs(max - spillWeight) <= tolerance) { //Spill
            selectedMove = Move.SPILL;
        }
        else { //Undefined
            Debug.Log(this.name + " was unable to choose a move");
            selectedMove = Move.UNDEFINED;
        }
        Debug.Log(this.name + " has decided to " + selectedMove);
    }

    //Selects the appropriate card based on the selected move
    private void ChooseCard() {
        switch(selectedMove) {
            case Move.SNATCH: //Select the first card that matches in value with top of discard
                for (int i = 0; i < hand.Length; i++) {
                    if (hand[i].value == game.discard[game.discard.Count - 1].value) {
                        selectedCard = hand[i];
                    }
                }
                break;
            case Move.SWAP: //Select the first random card that matches in color with top of discard
                do {
                    System.Random rand = new System.Random();
                    int i = rand.Next(hand.Length);
                    selectedCard = hand[i];
                } while (selectedCard.isRed != game.discard[game.discard.Count - 1].isRed);
                break;
            case Move.STASH: //If hand has a card higher in value than discard, choose the highest value card, else choose the lowest value card
                if (ExistsGreater(game.discard[game.discard.Count - 1].value, hand)) {
                    selectedCard = HighestCard(hand);
                }
                else {
                    selectedCard = LowestCard(hand);
                }
                break;
            case Move.SPILL: //Select the card with the highest value
                selectedCard = HighestCard(hand);
                break;
            default:
                Debug.Log(this.name + " could not select a card to player for move " + selectedMove);
                break;
        }
        Debug.Log("Selected card: " + selectedCard.rank + selectedCard.suit);
    }

    //Determines the weight for the snatch action
    private float CalculateSnatchWeight() {
        if (Exists(game.discard[game.discard.Count - 1].value, hand)) { //Computer has a card that matches in rank with top of discard
            //Snatch weight is the player's score
            return score;
        }
        else {
            //Snatch is not possible
            return Int32.MinValue;
        }
    }

    //Determines the weight for the swap action
    private float CalculateSwapWeight() {
        if (Exists(game.discard[game.discard.Count - 1].isRed, hand)) { //Computer has a card that matches in color with top of discard
            //Swap weight is the outcome of an opposing player's spill 
            float fail = ProbabilitySpillFails(game.discard[game.discard.Count - 1].suit);
            float expectedGain = (score * (1 - fail)) + (-MAX_RANK_VALUE * fail);
            return score + expectedGain;
        }
        else {
            //Swap is not possible
            return Int32.MinValue;
        }
    }

    //Determines the weight for the stash action
    private float CalculateStashWeight() {
        if (game.stash.Count == 0) { //Stash is empty, computer can place a card in the stash
            float fail = ProbabilityStashFails(game.discard[game.discard.Count - 1].value);
            float expectedGain = 0.0f;
            if (ExistsGreater(game.discard[game.discard.Count - 1].value, hand)) {
                //Stash weight is the possibility of tricking opposing players that a sticky card has been stashed
                expectedGain = ((game.discard[game.discard.Count - 1].value + 1) * fail) + (-(game.discard[game.discard.Count - 1].value + 1) * (1 - fail));
            }
            else {
                //Stash weight is the possibility of tricking opposing players that a non-sticky card has been stashed
                expectedGain = (MAX_RANK_VALUE * (1 - fail)) + (-MAX_RANK_VALUE * fail);
            }
            return score + expectedGain;
        }
        else { //Stash is occupied, computer can steal from the stash
            //Stash weight is the possibility of stealing a non-sticky card
            float fail = ProbabilityStashFails(game.stashValue);
            float expectedGain = ((game.stashValue + 1) * (1 - fail)) + (-MAX_RANK_VALUE * fail);
            return score + expectedGain;
        }
    }

    //Determines the weight for the spill action
    private float CalculateSpillWeight() {
        //Spill weight is the possibility of performing a successful spill
        float fail = ProbabilitySpillFails(game.discard[game.discard.Count - 1].suit);
        float expectedGain = (HighestCard(hand).value * (1 - fail)) + (-score * fail);
        return score + expectedGain;
    }

    //Calculates the probability that a spill will fail
    private float ProbabilitySpillFails(string suit) {
        float knownCount = memory.CountSuit(suit, game.sets, hand); //Cards that match in suit with discard out of the deck
        float numerator = MAX_SUIT_SIZE - knownCount; //Cards that match in suit with discard still in the deck
        float denominator = game.deck.Count; //Total amount of cards in deck
        float probability = (numerator / denominator) + (numerator / (denominator - 1)) + (numerator / (denominator - 2)); //Probability is the chance of drawing a card that matches in suit with top of discard within 3 draws
        return (probability > 1) ? 1.0f : probability;
    }

    //Calculates the probability that a stash will result in a sticky card
    private float ProbabilityStashFails(int stashVal) {
        float knownCount = memory.CountRank(stashVal, game.sets, hand); //Cards that have a value less than or equal to the stash value out of the deck
        float numerator = (stashVal * 4) - knownCount; //Cards that have a value less than or equal to the stash value still in the deck
        float denominator = game.deck.Count + game.stash.Count; //Total amount of cards in deck and stash
        float probability = numerator / denominator; //Probability is the chance of stashing or stealing a card that is lower than the stash value
        return (probability > 1) ? 1.0f : probability;
    }

    //Notifies player to add card to memory cache
    public void Notify(Card card) {
        memory.Add(card, game.sets, hand);
        memory.PrintCache(this.name + ": Noticed the (" + card.rank + card.suit + ") |");
    }
 }
