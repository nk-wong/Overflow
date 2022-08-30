using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LabelController : MonoBehaviour
{

    private static TextMeshProUGUI stashValueLabel; //The label game object that shows what value a card was stashed on
    private static TextMeshProUGUI stashPlayerLabel; //The label game object that shows which player stashed

    //Initializes all labels in the UI
    public static void Initialize() {
        //Initialize stash labels
        stashValueLabel = GameObject.Find("StashValueLabel").GetComponent<TextMeshProUGUI>();
        stashPlayerLabel = GameObject.Find("StashPlayerLabel").GetComponent<TextMeshProUGUI>();
    }

    //Changes the labels based on the stash value and stash player
    public static void ChangeLabels(int stashValue, Player stashPlayer) {
        //If a card has been stashed, change stash value label to top of discard value, else change to default
        stashValueLabel.text = (stashValue != 0) ? "Stash Value: " + stashValue : "Stash Value: ";

        //If a card has been stashed, change stash player label to player who stashed, else change to default
        stashPlayerLabel.text = (!(stashPlayer is null)) ? "Stash Player: " + stashPlayer.name : "Stash Player: ";
    }
}
