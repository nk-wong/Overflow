using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    public List<Card> deck { get; private set; } //Holds all the cards in the deck pile
    public List<Card> discard { get; private set; } //Holds all the cards in the discard pile
    public List<Card> spill { get; private set; } //Holds all the cards in the spill pile
    public List<Card> stash { get; private set; } //Holds the card in the stash pile

    public List<Card> hands { get; private set; } //Holds all the cards in held by the players
    public List<Card> sets { get; private set; } //Holds all the cards that have been set

    public int stashValue { get; private set; } //Holds the value of the top discard card when a player stashed
    public Player stashPlayer { get; private set; } //Holds the player who stashed the card which currently occupies the stash
    public int highestScore { get; private set; } //Holds the score that is the highest amongst all players
    public bool isGameOver { get; private set; } //Determines whether the game loop should continue

    //Fields to help build the card game objects
    public GameObject cardPrefab; //Template to build cards
    public Sprite[] cardFaces; //Holds the image for each card in the deck
    public GameObject scoreLabelPrefab; //Template to build score labels for scoreboard

    //Fields to help position cards
    public GameObject deckObj; //The game object representing the deck pile
    public GameObject discardObj; //The game object representing the discard pile
    public GameObject spillObj; //The game object representing the spill pile
    public GameObject stashObj; //The game object representing the stash pile
    public GameObject[] playerObjs; //The game objects representing the players

    //Game constants
    private readonly int NUM_PLAYERS = 4;
    private readonly float SPEED_CONSTANT = 100.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize other controllers
        GameActionController.Initialize();
        LabelController.Initialize(this.playerObjs, scoreLabelPrefab);
        WinScreen.Initialize();

        PlayCards(); //Create and shuffle deck
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Prepares a shuffled deck of cards for the start of the game
    public void PlayCards() {
        this.deck = GenerateDeck();
        ShuffleDeck(deck);

        //Test that the deck is correct
        foreach (Card card in deck) {
            Debug.Log(card.rank + " | " + card.suit + " | " + card.value);
        }

        PlaceDeck();
        StartCoroutine(DealDeck());
    }

    //Creates all the card objects for the deck
    public List<Card> GenerateDeck() {
        List<Card> newDeck = new List<Card>();
        string[] suits = new string[] {"D", "C", "H", "S"}; //The suits of a deck of playing cards
        string[] ranks = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"}; //The ranks of a deck of playing cards
        for (int i = 0; i < suits.Length; i++) { //Each suit
            for (int j = 0; j < ranks.Length; j++) { //Each rank
                newDeck.Add(new Card(ranks[j], suits[i], j+1)); //Create card and add to deck
            }
        }

        //Generate the other decks
        discard = new List<Card>();
        stash = new List<Card>();
        spill = new List<Card>();
        hands = new List<Card>();
        sets = new List<Card>();

        return newDeck;
    } 

    //Shuffles cards in the deck
    public void ShuffleDeck(List<Card> list) {
        System.Random rand = new System.Random();
        for (int i = (list.Count - 1); i >= 0; i--) { //Touch each card in the deck and randomly swap with another card
            int j = rand.Next(list.Count);
            Card temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        } 
    }

    //Places the cards in the deck pile and instantiates game objects for each card
    public void PlaceDeck() {
        float zOffset = 0.0f;
        foreach(Card card in deck) {
            //Create a card object and set its name to match the rank and suit of a card in the deck
            GameObject newCard = Instantiate(cardPrefab, new Vector3(deckObj.transform.position.x, deckObj.transform.position.y, deckObj.transform.position.z - zOffset), Quaternion.identity); 
            newCard.name = card.rank + card.suit;

            //Have the card set its game object to the newly created game object
            card.SetObject();

            //Move each card down the z-axis to prevent cards from existing in the same spot
            zOffset += 0.001f;
        }
    }

    //Deals the cards in the deck pile to each player
    public IEnumerator DealDeck() {
        for (int i = 0; i < NUM_PLAYERS; i++) { //Give cards to players
            for (int j = 0; j < NUM_PLAYERS; j++) { //Give one card to each player
                Card card = deck[deck.Count - 1]; //Take the top card
                GameObject obj = playerObjs[j].GetComponent<Player>().AddToHand(card); //Give the card to the player
                
                yield return MoveToHands(card, obj);
            }
        }
        //Place a starting card in the discard pile
        Card last = deck[deck.Count - 1];
        last.isFaceUp = true; //Flip the card over
        yield return MoveToDiscard(last);

        //Start game loop
        StartCoroutine(PlayGame());
    }

    //Starts the game loop
    private IEnumerator PlayGame() {
        uint index = 0;
        while (!isGameOver) {
            //Get player for turn
            Player player = playerObjs[index%NUM_PLAYERS].GetComponent<Player>();
            LabelController.HighlightCurrentPlayer((int)index%NUM_PLAYERS);

            //Card management
            yield return ResolveStashPile(player);
            LabelController.ChangeScoreLabels((int)index%NUM_PLAYERS, player.score);
            if (!DetermineWin(player)) { //Player does not need to go if game was won by resolved stash
                yield return player.Play();
                LabelController.ChangeScoreLabels((int)index%NUM_PLAYERS, player.score);
            }
            
            //Pile management
            yield return ReshuffleDeck();

            //Game management
            highestScore = DetermineHighestScore();
            isGameOver = DetermineWin(player);

            //Move on to the next player if a spill was not performed by the current player or the spill failed
            if (player.selectedMove != Move.SPILL || spill.Count == 0) {
                LabelController.UnhighlightCurrentPlayer((int)index%NUM_PLAYERS);
                index++;
            }
        }
        WinScreen.GenerateWinScreen();
    }

    //Finds the highest score amongst all players
    private int DetermineHighestScore() {
        int max = 0;
        for (int i = 0; i < playerObjs.Length; i++) {
            Player player = playerObjs[i].GetComponent<Player>();
            if (player.score > max) {
                max = player.score;
            }
        }
        return max;
    }

    //Returns true if a player has ended the game
    private bool DetermineWin(Player currentPlayer) {
        if (currentPlayer.SetCount() == currentPlayer.set.Length) { //Found player with set cards to end the game
            return true;
        }
        return false;
    }

    //Returns the stashed card to the player who stashed the card
    private IEnumerator ResolveStashPile(Player currentPlayer) {
        if (!(stashPlayer is null) && stashPlayer.name == currentPlayer.name) { //There exists a player who stashed a card which has not been stolen
            //The player will steal their own stashed card
            yield return Stash(null, currentPlayer);
        }
    }

    //Reshuffles the list of cards once the list has less than three cards remaining
    private IEnumerator ReshuffleDeck() {
        if (deck.Count <= 3) {
            foreach (GameObject obj in playerObjs) { //Clear the memory of each computer in the game
                Computer computer = obj.GetComponent<Computer>();
                if (!(computer is null)) {
                    computer.Notify();
                }
            }

            List<Card> temp = new List<Card>(deck); //Store any remaining cards in the deck
            deck.Clear(); //Clear out the data in the deck
            for (int i = discard.Count - 2; i >= 0; i--) { //Move cards, except for top of discard, from the discard to the deck
                Card card = discard[i];
                card.isFaceUp = false;
                yield return MoveToDeck(card, 0);
            }
            ShuffleDeck(deck); //Shuffle the cards that were added from the discard
            deck.AddRange(temp); //Add back the original deck cards on the top of the deck
            yield return MoveToDiscard(discard[discard.Count - 1]); //Notify players that the top of the discard still remains in the discard pile
        }
    }

    //Moves a card from one position to another position and updates the game decks accordingly
    private IEnumerator MoveCard(Card card, GameObject newPos, List<Card> newDeck, int offset) {
        List<Card> origDeck = FindPile(card); //Find the deck that the card currently resides in
        origDeck.Remove(card); //Remove the card from that deck
        newDeck.Insert(newDeck.Count - offset, card); //Add the card to the new deck

        //Set card rotation and position
        card.myObj.transform.rotation = newPos.transform.rotation;
        while (Vector3.Distance(card.myObj.transform.position, newPos.transform.position) != 0.0f) {
            card.myObj.transform.position = Vector3.MoveTowards(card.myObj.transform.position, newPos.transform.position, SPEED_CONSTANT * Time.deltaTime);
            yield return null;
        }

        AlignPile(deck, deckObj); //Make sure the deck is showing the correct top card
        AlignPile(discard, discardObj); //Make sure the discard is showing the correct top card
        AlignPile(stash, stashObj); //Make sure the stash is the showing the correct top card
        AlignPile(spill, spillObj); //Make sure the spill is showing the correct top card

        LabelController.UpdateDeckPileCounter(deck.Count);
    }

    private IEnumerator MoveCard(Card card, GameObject newPos, List<Card> newDeck) {
        yield return MoveCard(card, newPos, newDeck, 0);
    }

    //Moves a card to the deck and updates the game decks accordingly
    private IEnumerator MoveToDeck(Card card, int offset) {
        yield return MoveCard(card, deckObj, deck, offset);
    }

    //Moves a card to the discard and updates the game decks accordingly
    private IEnumerator MoveToDiscard(Card card) {
        yield return MoveCard(card, discardObj, discard);
        NotifyAllObservers(card);
    }

    //Moves a card to the stash and updates the game decks accordingly
    private IEnumerator MoveToStash(Card card) {
        yield return MoveCard(card, stashObj, stash);
    }

    //Moves a card to the spill and updates the game decks accordingly
    private IEnumerator MoveToSpill(Card card) {
        yield return MoveCard(card, spillObj, spill);
    }

    //Moves a card to the hands and updates the game decks accordingly
    private IEnumerator MoveToHands(Card card, GameObject playerHand) {
        yield return MoveCard(card, playerHand, hands);
    }

    //Moves a card to the sets and updates the game decks accordingly
    private IEnumerator MoveToSets(Card card, GameObject playerSet) {
        yield return MoveCard(card, playerSet, sets);
    }

    //Finds the pile that the card is currently residing in
    private List<Card> FindPile(Card card) {
        if (deck.Contains(card)) {
            return deck;
        }
        else if (discard.Contains(card)) {
            return discard;
        }
        else if (stash.Contains(card)) {
            return stash;
        }
        else if (spill.Contains(card)) {
            return spill;
        }
        else if (hands.Contains(card)) {
            return hands;
        }
        else if (sets.Contains(card)) {
            return sets;
        }
        else {
            Debug.Log("Could not find the card(" + card.rank + card.suit + ") in any pile");
            return null;
        }
    }

    //Ensures that the piles' cards are stacked correctly
    private void AlignPile(List<Card> pile, GameObject pos) {
        float zOffset = 0.0f;
        for (int i = 0; i < pile.Count; i++) {
            pile[i].myObj.transform.position = new Vector3(pos.transform.position.x, pos.transform.position.y,  pos.transform.position.z - zOffset);
            zOffset += 0.001f;
        }
    }

    //Move a card from hand to discard, take a new card from the top of the deck
    public IEnumerator Snatch(Card handCard, Player player) {
        //Move the hand card to the discard pile
        handCard.isFaceUp = true;
        player.RemoveFromHand(handCard);
        yield return MoveToDiscard(handCard);

        //Move the card on the top of the deck to the player's hand
        Card gain = deck[deck.Count - 1];
        GameObject obj = player.AddToHand(gain);
        yield return MoveToHands(gain, obj);
    }

    //Swap a card from hand with the third card from the top of the deck
    public IEnumerator Swap(Card handCard, Player player) {
        //Move the hand card to the third from top position in the deck
        handCard.isFaceUp = false;
        player.RemoveFromHand(handCard);
        yield return MoveToDeck(handCard, 3);

        //Move the third from top card in the deck to the hand
        Card gain = deck[deck.Count - 3];
        GameObject obj = player.AddToHand(gain);
        yield return MoveToHands(gain, obj);
    }

    //Move a card face down onto the stash pile, or steal a card if the stash is occupied
    public IEnumerator Stash(Card handCard, Player player) {
        if (stash.Count == 0) { //No card in the stash
            //Move the hand card to the stash pile
            handCard.isFaceUp = false;
            player.RemoveFromHand(handCard);
            yield return MoveToStash(handCard);

            //Set the stash value and player and notify label controller
            stashValue = discard[discard.Count - 1].value;
            stashPlayer = player;
            LabelController.ChangeStashLabel(stashValue, stashPlayer);

            //Move the card on the top of the deck to the player's hand
            Card gain = deck[deck.Count - 1];
            GameObject obj = player.AddToHand(gain);
            yield return MoveToHands(gain, obj);
        }
        else { //Steal the currently stashed card
            //Show the stashed card to players
            Card stashedCard = stash[stash.Count - 1];
            stashedCard.isFaceUp = true;
            yield return new WaitForSeconds(1.5f);

            //Determine if stashed card will be a sticky card
            stashedCard.isFaceUp = stashedCard.value > stashValue ? true : false;

            //Reset the stash value and player and notify label controller
            stashValue = 0;
            stashPlayer = null;
            LabelController.ChangeStashLabel(stashValue, stashPlayer);

            //Move the stashed card from the stash pile to the player's set
            GameObject obj = player.AddToSet(stashedCard);
            yield return MoveToSets(stashedCard, obj);

            //The player's set is full and the last added card was a sticky card
            if (stashedCard.isFaceUp == false && player.SetCount() == player.set.Length) {
                if (player.StickyCount() == player.set.Length) { //Player's set is all sticky cards
                    //Increment player penalty
                    player.penalty++;
                    for (int i = 0; i < player.set.Length; i++) {
                        //Move all cards to the discard
                        player.set[i].isFaceUp = true;
                        yield return MoveToDiscard(player.set[i]);
                        player.RemoveFromSet(player.set[i]);
                    }
                }
                else { //Player's set has some face up set cards
                    for (int i = 0; i < player.set.Length; i++) {
                        if (player.set[i].isFaceUp) { //Remove face up set cards
                            //Move non-sticky set cards to the discard
                            yield return MoveToDiscard(player.set[i]);
                            player.RemoveFromSet(player.set[i]);
                        }
                    }
                }
            }
        }
    }

    //Take three cards from the deck and place on spill, if any card in spill matches the top of the discard, spot drawing cards
    public IEnumerator Spill(Card handCard, Player player) {
        for (int i = 0; i < 3; i++) {
            Card card = deck[deck.Count - 1]; //Get card from top of deck
            card.isFaceUp = true; //Flip the card
            yield return MoveToSpill(card); //Move to spill pile
            yield return new WaitForSeconds(0.75f); //Allow players to see card before next flip

            if (card.suit == discard[discard.Count - 1].suit) { //Flipped card matches discard suit, remove non-sticky cards from player's set, clear the spill pile, and end spill
                for (int j = 0; j < player.set.Length; j++) {
                    if (!(player.set[j] is null) && player.set[j].isFaceUp) { //Remove face up set cards
                    //Move non-sticky set cards to the discard
                    yield return MoveToDiscard(player.set[j]);
                    player.RemoveFromSet(player.set[j]);
                    }
                }
                while (spill.Count > 0) { //While there are cards in the spill pile, move to discard pile
                    yield return MoveToDiscard(spill[0]);
                }
                break;
            }
        }
        if (spill.Count > 0 && discard[discard.Count - 1].suit != spill[spill.Count - 1].suit) { //If the spill was successful, player can set a card
            //Move card from player's hand to the player's set
            handCard.isFaceUp = true;
            GameObject obj1 = player.AddToSet(handCard);
            player.RemoveFromHand(handCard);
            yield return MoveToSets(handCard, obj1);

            //Move the card on the top of the deck to the player's hand
            Card gain = deck[deck.Count - 1];
            GameObject obj2 = player.AddToHand(gain);
            yield return MoveToHands(gain, obj2);
        }
    }

    //Ends the player's turn and clears the spill pile
    public IEnumerator EndTurn() {
        while (spill.Count > 0) { //While there are cards in the spill pile, move to discard pile
            yield return MoveToDiscard(spill[0]);
        }
    }

    //Notifies computer players that a card has been placed in the discard pile
    private void NotifyAllObservers(Card card) {
        for (int i = 0; i < playerObjs.Length; i++) {
            Computer computer = playerObjs[i].GetComponent<Computer>();
            if (!(computer is null)) {
                computer.Notify(card);
            }
        }
    }

    //Debugging function to test synchronization between image and data of cards
    private void PrintList(List<Card> list) {
        foreach (Card card in list) {
            Debug.Log(card.rank + card.suit);
            Debug.Log(card.myObj.name);
            Debug.Log(card.myObj.GetComponent<CardDisplay>().card.rank);
            Debug.Log(card.myObj.GetComponent<CardDisplay>().card.suit);
        }
    }
}
