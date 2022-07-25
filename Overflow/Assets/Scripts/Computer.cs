using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Player
{
    private MemoryCache memory = new MemoryCache(15); //The cards in the discard pile that the computer remembers

    //TODO
    public override IEnumerator Play() {
        yield return null;
    }

    public override GameObject AddToHand(Card card) {
        //Find the index that the new card was added into
        int result = Add(card, hand);
        if (result < 0) { //No free space to add the card, output error
            Debug.Log(this.name + " does not have space in its hand to add the card(" + card.rank + card.suit + ")");
        }

        //Use the index to find the GameObject
        return this.transform.Find("Hand" + (result + 1)).gameObject;
    }

    public override void RemoveFromHand(Card card) {
        if (!Remove(card, hand)) { //Could not find the card, output error
            Debug.Log(this.name + " cannot remove the card(" + card.rank + card.suit + ") because it does not exist in the hand");
        }
    }

    public void Notify(Card card) {
        memory.Add(card, new List<Card>(hand));
        memory.PrintCache(this.name + ": Noticed the (" + card.rank + card.suit + ") |");
    }
 }
