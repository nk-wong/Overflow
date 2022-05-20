using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{

    public Card card; //The card data object that matches with the display
    public Sprite cardFace; //The image of the card when face-up
    public Sprite cardBack; //The image of the card when face-down
    private SpriteRenderer spriteRenderer;
    private GameController game;

    // Start is called before the first frame update
    void Start()
    {
        FindCard();
    }

    // Update is called once per frame
    void Update()
    {
        //Check the face-up status to load the correct card image
        if (this.card.isFaceUp == true) {
            spriteRenderer.sprite = cardFace;
        }
        else {
            spriteRenderer.sprite = cardBack;
        }
    }

    //Find the corresponding card object that matches the display to sync the data
    private void FindCard() {
        //Retrieve the deck from the game controller
        game = FindObjectOfType<GameController>();
        List<Card> deck = game.GenerateDeck();
        for (int i = 0; i < deck.Count; i++) { //Iterate through the deck and find the card that matches in suit and rank
            if (this.name == (deck[i].suit + deck[i].rank)) {
                this.card = deck[i];
                this.cardFace = game.cardFaces[i];
                break;
            }
        }
        //Prepare the sprite renderer
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
