using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Player
{
    // Update is called once per frame
    void Update() {
        for (int i = 0; i < hand.Length; i++) {
            if (!(hand[i] is null)) {
                hand[i].isFaceUp = true;
            }
        }
    }

    //TODO
    public override void MakeMove() {
        
    }




}
