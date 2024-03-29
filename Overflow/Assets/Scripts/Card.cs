using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string rank { get; private set; } //Holds the number of the card
    public string suit { get; private set; } //Holds the suit of the card
    public int value { get; private set; } //Holds the numerical value of the card
    public bool isFaceUp { get; set; } //Holds whether the card is face-up or face-down
    public bool isRed { get; private set; } //Holds whether the color of the card is red

    public GameObject myObj; //Holds the visual object that the card data is attached to

    //Constructor
    public Card(string r, string s, int v) {
        this.rank = r;
        this.suit = s;
        this.value = v;
        this.isFaceUp = false;

        if (this.suit == "H" || this.suit == "D") {
            this.isRed = true;
        }
        else {
            this.isRed = false;
        }
    }

    //Defines how two Cards are the same
    public override bool Equals(object obj) {
        if (obj.GetType() != this.GetType()) { //Check if the objects are the same type before comparing
            return false;
        }

        Card other = (Card)obj;
        if (other.suit == this.suit && other.rank == this.rank) {
            return true;
        }
        return false;
    }

    public static bool operator ==(Card c1, Card c2) {
        if (c1.suit == c2.suit && c1.rank == c2.rank) {
            return true;
        }
        return false;
    }

    public static bool operator !=(Card c1, Card c2) {
        if (c1.suit != c2.suit || c1.rank != c2.rank) {
            return true;
        }
        return false;
    }

    //Match the card object to a game object that matches in rank and suit
    public void SetObject() {
        myObj = GameObject.Find(this.rank + this.suit);
        myObj.GetComponent<CardDisplay>().FindCard(); //Have game object find data
        myObj.GetComponent<CardDisplay>().FindFaceCard(); //Have game object find card face
        //Debug.Log(myObj.name);
    }

}
