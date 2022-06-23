using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Player
{
    //TODO
    public override IEnumerator Play() {
        yield return null;
    }

    public override void AddToHand(Card card) {
        if (!Add(card, hand)) { //No free space to add the card, output error
            Debug.Log(this.name + " does not have space in its hand to add the card(" + card.rank + card.suit + ")");
        }
    }

    public override void RemoveFromHand(Card card) {
        if (!Remove(card, hand)) { //Could not find the card, output error
            Debug.Log(this.name + " cannot remove the card(" + card.rank + card.suit + ") because it does not exist in the hand");
        }
    }
}
