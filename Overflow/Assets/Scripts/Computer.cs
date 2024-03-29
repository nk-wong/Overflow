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

    private bool isHighTrick; //Determines whether the computer should trick high or low

    private readonly float TOLERANCE = 0.0001f; //The tolerance for float comparison

    private readonly int MAX_SUIT_SIZE = 13; //The number of cards in one suit
    private readonly int MAX_RANK_VALUE = 13; //The highest value a card can have
    private readonly float CHAIN_SPILL_THRESHOLD = 0.8f; //The threshold that determines whether computer should continue spilling

    public override IEnumerator Play() {
        //Reset selections before making move or continue with chain spill
        selectedMove = (selectedMove == Move.SPILL) ? Move.SPILL : Move.UNDEFINED;
        selectedCard = null;

        //Simulate thinking time for computer
        System.Random rand = new System.Random();
        float waitTime = (float)rand.NextDouble() + 1.5f;
        yield return new WaitForSeconds(waitTime);

        ChooseMove();

        ChooseCard();

        yield return MakeMove();
    }

    //Calculates the heuristic of each move to find out which move to make
    private void ChooseMove() {
        if (selectedMove != Move.SPILL || game.spill.Count == 0) { //Choose move
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
                snatchWeight = (snatchWeight == Int32.MinValue) ? snatchWeight : Int32.MaxValue;
                swapWeight = (swapWeight == Int32.MinValue) ? swapWeight : Int32.MaxValue;
                stashWeight = -(stashWeight - game.highestScore);
                spillWeight = -(spillWeight - game.highestScore);

                //Get the weakest move
                max = Math.Max(Math.Max(Math.Max(snatchWeight, swapWeight), stashWeight), spillWeight);

                Debug.Log(this.name + "|" + snatchWeight + "|" + swapWeight + "|" + stashWeight + "|" + spillWeight + "|");
            }

            //Determine move to make
            if (Math.Abs(max - snatchWeight) <= TOLERANCE) { //Snatch
                selectedMove = Move.SNATCH;
            }
            else if (Math.Abs(max - swapWeight) <= TOLERANCE) { //Swap
                selectedMove = Move.SWAP;
            }
            else if (Math.Abs(max - stashWeight) <= TOLERANCE) { //Stash
                selectedMove = Move.STASH;
            }
            else if (Math.Abs(max - spillWeight) <= TOLERANCE) { //Spill
                selectedMove = Move.SPILL;
            }
            else { //Undefined
                Debug.Log(this.name + " was unable to choose a move");
                selectedMove = Move.UNDEFINED;
            }
        }
        else { //Choose to continue spill
            //Find out if spill odds surpass threshold
            float fail = ProbabilitySpillFails(game.discard[game.discard.Count - 1].suit);
            float success = 1.0f - fail;

            Debug.Log(this.name + "| (FAIL) " + fail + "| (SUCCESS) " + success + "|");

            if (success >= CHAIN_SPILL_THRESHOLD) { //Continue spill
                selectedMove = Move.SPILL;
            }
            else { //End turn
                selectedMove = Move.END;
            }
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
            case Move.STASH: //If computer can trick on high card, choose highest card, else the lowest card
                if (isHighTrick && ExistsGreater(game.discard[game.discard.Count - 1].value, hand)) { //Found card for high trick
                    selectedCard = HighestCard(hand);
                }
                else if (!isHighTrick && ExistsLesser(game.discard[game.discard.Count - 1].value, hand)) { //Found card for low trick
                    selectedCard = LowestCard(hand);
                }
                else { //No suitable card for trick, pick randomly
                    System.Random rand = new System.Random();
                    int i = rand.Next(hand.Length);
                    selectedCard = hand[i];
                }
                break;
            case Move.SPILL: //Select the card with the highest value
                selectedCard = HighestCard(hand);
                break;
            case Move.END: //Select the first card in hand
                selectedCard = hand[0];
                break;
            default: //Undefined
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
            float trickHigh = 0.0f;
            float trickLow = 0.0f;

            trickHigh = ExistsGreater(game.discard[game.discard.Count - 1].value, hand) ? ((game.discard[game.discard.Count - 1].value + 1) * fail) + (-(game.discard[game.discard.Count - 1].value + 1) * (1 - fail)) : Int32.MinValue;
            trickLow = ExistsLesser(game.discard[game.discard.Count - 1].value, hand) ? (((set.Length - (SetCount() + StickyCount())) * -MAX_RANK_VALUE) * (1 - fail)) + ((((set.Length / 2) - (SetCount() + StickyCount())) * MAX_RANK_VALUE) * fail) : Int32.MinValue;

            //Stash weight is the trick with higher possibility of succeeding
            float expectedGain = Math.Max(trickHigh, trickLow);

            //Set trick flag
            isHighTrick = (Math.Abs(expectedGain - trickHigh) <= TOLERANCE) ? true : false;

            return score + expectedGain;
        }
        else { //Stash is occupied, computer can steal from the stash
            //Stash weight is the possibility of stealing a non-sticky card
            float fail = ProbabilityStashFails(game.stashValue);
            float expectedGain = ((game.stashValue + 1) * (1 - fail)) + ((((set.Length / 2) - (SetCount() + StickyCount())) * MAX_RANK_VALUE) * fail);
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

    //Notifies player to clear the memory cache
    public void Notify() {
        memory.Clear();
        memory.PrintCache(this.name + ": Cleared memory");
    }
 }
