using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

//Name: Ross Hutchins
//ID: HUT18001284

public class MainMenuScript : MonoBehaviour
{

    //Reference to the mainGameScript.
    [SerializeField] private MainGameScript mainGame;

    //Reference to the two panels of main menu.
    [SerializeField] private GameObject Main;
    [SerializeField] private GameObject Help;
    [SerializeField] private GameObject Settings;

    [SerializeField] private Button PlayButton;
    [SerializeField] private Button HelpButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button BackButtonB;

    [SerializeField] private Text VolumeTxt;
    [SerializeField] private Text SensitivityTxt;

    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Slider SensitivitySlider;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;

        PlayButton.Select();
        PlayButton.OnSelect(null);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPlayClick()
    {
        //Begin a new game.
        mainGame.gameObject.SetActive(true);
        mainGame.beginGame();
        gameObject.SetActive(false);
    }

    public void onHelpClick()
    {
        Help.SetActive(true);

        BackButton.Select();
        BackButton.OnSelect(null);

        Main.SetActive(false);
    }

    public void onSettingsClick()
    {

        //Before navigating to settings, check to see if we need to update our values.
        VolumeSlider.value = AudioListener.volume;
        SensitivitySlider.value = mainGame.playerMovement.sensitivity;
        onVolumeChange();
        onSensitivityChange();

        Settings.SetActive(true);

        BackButtonB.Select();
        BackButtonB.OnSelect(null);

        Main.SetActive(false);
    }

    public void onBackClick()
    {
        Main.SetActive(true);

        HelpButton.Select();
        HelpButton.OnSelect(null);

        Help.SetActive(false);
        Settings.SetActive(false);
    }

    public void onVolumeChange()
    {
        VolumeTxt.text = "Volume - " + Mathf.Floor(VolumeSlider.value * 100.0f).ToString() + "%";
        AudioListener.volume = VolumeSlider.value;
    }

    public void onSensitivityChange()
    {
        //Always keep it a flat number, using floor function.
        SensitivitySlider.value = Mathf.Floor(SensitivitySlider.value);

        //Update the text and player values.
        SensitivityTxt.text = "Sensitivity - " + SensitivitySlider.value.ToString();
        mainGame.playerMovement.sensitivity = SensitivitySlider.value;
    }

    //Self-explanatory.
    public void onExitClick()
    {
        Application.Quit();
    }
}
