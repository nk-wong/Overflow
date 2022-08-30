using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LabelController : MonoBehaviour
{

    private static TextMeshProUGUI stashValueLabel; //The label game object that shows what value a card was stashed on
    private static TextMeshProUGUI stashPlayerLabel; //The label game object that shows which player stashed

    private static GameObject scoreboard; //The game object that holds the score labels for each player
    private static List<TextMeshProUGUI> scores; //Holds the score labels that were added to the scoreboard

    //Initializes all labels in the UI
    public static void Initialize(GameObject[] players, GameObject scoreLabelPrefab) {
        //Initialize stash labels
        stashValueLabel = GameObject.Find("StashValueLabel").GetComponent<TextMeshProUGUI>();
        stashPlayerLabel = GameObject.Find("StashPlayerLabel").GetComponent<TextMeshProUGUI>();

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
            newLabel.transform.parent = scoreboard.transform;

            //Add label to scores list
            scores.Add(label);
        }
    }

    //Changes the labels based on the stash value and stash player
    public static void ChangeStashLabels(int stashValue, Player stashPlayer) {
        //If a card has been stashed, change stash value label to top of discard value, else change to default
        stashValueLabel.text = (stashValue != 0) ? "Stash Value: " + stashValue : "Stash Value: ";

        //If a card has been stashed, change stash player label to player who stashed, else change to default
        stashPlayerLabel.text = (!(stashPlayer is null)) ? "Stash Player: " + stashPlayer.name : "Stash Player: ";
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
}
