using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DifficultyManager : MonoBehaviour
{
    [SerializeField] Slider horizontalGridCountSlider;
    [SerializeField] Slider verticalGridCountSlider;
    public Slider colourCountSlider;

    [SerializeField] Text horizontalGridText;
    [SerializeField] Text verticalGridText;
    [SerializeField] Text colourText;

    [SerializeField] GameObject playButton;
    [SerializeField] GameObject settingsObject;
    [SerializeField] GameObject readyButton;

    GameManager gameManager;

    byte value;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        value = (byte)colourCountSlider.value;

        horizontalGridText.text = horizontalGridCountSlider.value.ToString();
        verticalGridText.text = verticalGridCountSlider.value.ToString();
        colourText.text = colourCountSlider.value.ToString();
    }

    public void OnHorizontalGridCountSliderValueChanged()
    {
        if (horizontalGridCountSlider.value % 2 != 0)
            horizontalGridCountSlider.value++;
        gameManager.horizontalGridCount = (byte)(horizontalGridCountSlider.value / 2);
    }

    public void OnVerticalGridCountSliderValueChanged()
    {
        gameManager.verticalGridCount = (byte)(verticalGridCountSlider.value*2  -2);
    }

    public void OnColourSliderValueChanged()
    {
        gameManager.fixedColourCount = (byte)colourCountSlider.value;
        gameManager.SetColoursRandomly();
    }

    public void OnPressedPlayButton()
    {
        settingsObject.SetActive(true);
        playButton.SetActive(false);
        gameManager.OnSettingsActivated();

    }

    public void OnPressedReadyButton()
    {
        gameManager.LoadGameScene();
    }

}
