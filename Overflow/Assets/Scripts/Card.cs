using System.Collections;
using System.Collections.Generic;

public class Card
{

    public string suit; //Holds the suit of the card
    public string rank; //Holds the pip of the card
    public int value; //Holds the numerical value of the card
    public bool isFaceUp; //Holds whether the card is face-up or face-down

    //Constructor
    public Card(string s, string r, int v) {
        this.suit = s;
        this.rank = r;
        this.value = v;
        this.isFaceUp = false;
    }

}
