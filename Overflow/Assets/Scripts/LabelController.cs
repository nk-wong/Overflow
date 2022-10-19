using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LabelController : MonoBehaviour
{

    private static TextMeshProUGUI stashInfoLabel; //The label game object that shows which player stashed and on what value

    private static GameObject scoreboard; //The game object that holds the score labels for each player
    private static List<TextMeshProUGUI> scores; //Holds the score labels that were added to the scoreboard

    private static GameObject deckPile; //UI object representing the deck
    private static GameObject discardPile; //UI object representing the discard
    private static GameObject spillPile; //UI object representing the spill
    private static GameObject stashPile; //UI object representing the stash

    private static Color defaultColor = new Color32(188, 93, 97, 255);
    private static Color highlightColor = new Color32(108, 202, 94, 255);

    //Initializes all labels in the UI
    public static void Initialize(GameObject[] players, GameObject scoreLabelPrefab) {
        //Initialize stash label
        stashInfoLabel = GameObject.Find("StashInfoLabel").GetComponent<TextMeshProUGUI>();

        //Initialize scoreboard
        scoreboard = GameObject.Find("Scoreboard");

        //Initialize scores list
        scores = new List<TextMeshProUGUI>();

        //Initialize score labels based on the number of players in the game
        for (int i = 0; i < players.Length; i++) {
            //Create label game object
            GameObject newLabel = Instantiate(scoreLabelPrefab, Vector3.zero, Quaternion.identity);
            newLabel.name = "ScoreLabel" + (i + 1);

            //Add text to label
            TextMeshProUGUI label = newLabel.GetComponent<TextMeshProUGUI>();
            label.text = "Player" + (i + 1) + ": 0";

            //Add label to scoreboard
            newLabel.transform.SetParent(scoreboard.transform, false);

            //Add label to scores list
            scores.Add(label);
        }

        //Initialize pile counter label
        deckPile = GameObject.Find("DeckPile");
        discardPile = GameObject.Find("DiscardPile");
        spillPile = GameObject.Find("SpillPile");
        stashPile = GameObject.Find("StashPile");

        //Set the deck count
        UpdateDeckPileCounter(52);
    }

    //Changes the label based on the stash value and stash player
    public static void ChangeStashLabel(int stashValue, Player stashPlayer) {
        if (stashValue != 0 && !(stashPlayer is null)) { //If a card has been stashed, change stash info label to show stash value and player
            stashInfoLabel.text = "stash: " + stashPlayer.name + " stashed on " + stashValue;
        }
        else { //Else change to default
            stashInfoLabel.text = "stash: ";
        }
    }

    //Indicates the current playing player on the scoreboard
    public static void HighlightCurrentPlayer(int index) {
        if (!(index >= scores.Count)) { //Index corresponds to label in score list
            //Highlight player
            scores[index].color = highlightColor;
        }
        else { //Error
            Debug.Log("Could not sync " + index + " to an element in the scores list");
        }
    }

    //Returns a score label to its original text color
    public static void UnhighlightCurrentPlayer(int index) {
        if (!(index >= scores.Count)) { //Index corresponds to label in score list
            //Unhighlight player
            scores[index].color = defaultColor;
        }
        else { //Error
            Debug.Log("Could not sync " + index + " to an element in the scores list");
        }
    }

    //Changes the score label at the index to the new score
    public static void ChangeScoreLabels(int index, int newScore) {
        if (!(index >= scores.Count)) { //Index corresponds to label in scores list
            //Make change to label
            scores[index].text = "Player" + (index + 1) + ": " + newScore;
        }
        else { //Error
            Debug.Log("Could not sync " + index + " to an element in the scores list");
        }
    }

    public static void UpdateDeckPileCounter(int count) {
        TextMeshProUGUI counter = deckPile.transform.Find("Counter").GetComponentInChildren<TextMeshProUGUI>();
        counter.text = count.ToString();
    }
}
