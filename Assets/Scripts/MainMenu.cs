using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class MainMenu : MonoBehaviour
{
    public Canvas NewFlightCanvas;
    public Canvas ReplayCanvas;
    public Canvas MainMenuCanvas;
    public Canvas ProfileCanvas;
    public Canvas deleteFileCanvas;
    public Canvas WindCanvas;
    public Toggle SaveReplayToggle;
    public Toggle ShowGuidelinesToggle;
    public Toggle continuousRotation;
    public Toggle fixedControllerPosition;
    public Slider samplingFrequencySlider;
    private List<InputDevice> controllers;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuCanvas.enabled = true;
        NewFlightCanvas.enabled = false;
        ReplayCanvas.enabled = false;
        ProfileCanvas.enabled = false;
        deleteFileCanvas.enabled = false;
        WindCanvas.enabled = false;
        CheckForSavesFolder();
        SetSamplingFreqency();

        // If no controllers found on start of main menu, it fires VRNotAvailable()
        controllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);
        if (!(controllers.Count > 0)) {
            EventManager.manager.VRNotAvailable();
        }
    }

    /// <summary>
    /// Switches submenus in main menu, based on arguement id
    /// </summary>
    /// <param name="id"> Id of the submenu to show </param>
    public void SwitchMenu(int id)
    {
        switch (id)
        {
            case 0:
                NewFlightCanvas.enabled = false;
                ReplayCanvas.enabled = false;
                MainMenuCanvas.enabled = true;
                ProfileCanvas.enabled = false;
                WindCanvas.enabled = false;
                break;
            case 1:
                NewFlightCanvas.enabled = true;
                WindCanvas.enabled = false;
                ReplayCanvas.enabled = false;
                MainMenuCanvas.enabled = false;
                ProfileCanvas.enabled = false;
                break;
            case 2:
                NewFlightCanvas.enabled = false;
                WindCanvas.enabled = false;
                ReplayCanvas.enabled = true;
                MainMenuCanvas.enabled = false;
                ProfileCanvas.enabled = false;
                break;
            case 3:
                NewFlightCanvas.enabled = false;
                ReplayCanvas.enabled = false;
                MainMenuCanvas.enabled = false;
                WindCanvas.enabled = false;
                ProfileCanvas.enabled = true;
                break;
            case 4:
                NewFlightCanvas.enabled = false;
                ReplayCanvas.enabled = false;
                MainMenuCanvas.enabled = false;
                ProfileCanvas.enabled = false;
                WindCanvas.enabled = true;
                break;
        }
    }

    /// <summary>
    /// This method checks whether a directory for save files exist on local driveor not. If not, it creates one
    /// </summary>
    private void CheckForSavesFolder()
    {
        if (!Directory.Exists(Globals.saveFolder))
            Directory.CreateDirectory(Globals.saveFolder);
    }

    /// <summary>
    /// Loads flight scene and pauses the time
    /// </summary>
    public void StartFlight()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 0f;
    }


    /// <summary>
    /// Loads replay scene, after saving the path to replay file to memory
    /// </summary>
    public void StartReplay()
    {
        string pathToLoad = Application.persistentDataPath + this.GetComponent<ReplayMenu>().GetSelected();
        PlayerPrefs.SetString("jsonToLoad", pathToLoad);
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Sets the sampling frequency of upcoming flight
    /// </summary>
    public void SetSamplingFreqency()
    {
        var freq = samplingFrequencySlider.value;
        Globals.SetSampingFrequency((int)freq);
    }

    /// <summary>
    /// Toggles saving the replay after the flight is over 
    /// Enables/disables interactivty of samplingFrequencySlider based on boolean value
    /// </summary>
    public void SetSaveReplayToggle()
    {
        Globals.SetSaveReplay(SaveReplayToggle.isOn);
        if (SaveReplayToggle.isOn)
            samplingFrequencySlider.interactable = true;
        else
            samplingFrequencySlider.interactable = false;
    }

    /// <summary>
    /// Toggles showing guidelines to the player during the flight
    /// </summary>
    public void SetShowGuidelinesToggle()
    {
        Globals.SetShowGuidelines(ShowGuidelinesToggle.isOn);
    }

    /// <summary>
    /// Sets controls to continuous
    /// </summary>
    public void SetContinuousRotation()
    {
        Globals.SetRotation(continuousRotation.isOn);
        fixedControllerPosition.isOn = false;
        fixedControllerPosition.interactable = !continuousRotation.isOn;
        if (continuousRotation)
            Globals.SetFixedControllerPosition(false);
    }

    /// <summary>
    /// Sets the discrete controls to use fixed controller levels
    /// </summary>
    public void SetFixedControllerPositions()
    {
        Globals.SetFixedControllerPosition(fixedControllerPosition.isOn);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

}
