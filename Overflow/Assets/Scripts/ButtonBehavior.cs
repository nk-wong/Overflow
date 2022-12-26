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
    
    // Start is called before the first frame update
    void Start() {
        if (SceneManager.GetActiveScene().name == "Options") {
            Resolution[] resolutions = Screen.resolutions;

            TMP_Dropdown resolutionDropdown = GameObject.Find("Resolution").GetComponent<TMP_Dropdown>();
            resolutionDropdown.ClearOptions();

            int currentResolutionIndex = 0;
            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++) {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void PlayGame() {
        SceneManager.LoadScene("Game");
    }

    public void LoadRules() {
        SceneManager.LoadScene("Rules");
    }

    public void LoadCredits() {
        SceneManager.LoadScene("Credits");
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

    public void OpenURL(string link) {
        Application.OpenURL(link);
    }

    public void SetVolume(float volume) {
        //TODO: implement when game has a soundtrack (use https://www.youtube.com/watch?v=YOaYQrN1oYQ for reference)
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel((qualityIndex - 2) * -1);
    }

    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex) {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
