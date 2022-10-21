using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonBehavior : MonoBehaviour
{
    private Button button;
    private Color normalTextColor;
    [SerializeField] private Color highlighedTextColor;

    public void PlayGame() {
        SceneManager.LoadScene("Game");
    }

    public void LoadRules() {
        SceneManager.LoadScene("Rules");
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        Application.Quit();
    }
  
    public void OnPointerEnter(string buttonName) {
        button = GameObject.Find(buttonName).GetComponent<Button>();
        normalTextColor = button.GetComponentInChildren<TextMeshProUGUI>().color;

        button.GetComponentInChildren<TextMeshProUGUI>().color = highlighedTextColor;
    }

    public void OnPointerExit() {
        button.GetComponentInChildren<TextMeshProUGUI>().color = normalTextColor;
    }

    public void ScrollTop(ScrollRect scrollRect) {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
