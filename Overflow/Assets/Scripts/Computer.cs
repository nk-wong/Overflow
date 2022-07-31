using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Player
{
    private MemoryCache memory = new MemoryCache(11); //The cards in the discard pile that the computer remembers

    //TODO
    public override IEnumerator Play() {
        yield return null;
    }

    public void Notify(Card card) {
        memory.Add(card, game.sets, hand);
        memory.PrintCache(this.name + ": Noticed the (" + card.rank + card.suit + ") |");
    }
 }
