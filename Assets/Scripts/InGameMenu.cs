using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button replayButton;
    [SerializeField]
    private Button hideMenuButton;
    [SerializeField]
    private Button disableWindsButton;
    [SerializeField]
    private GameObject warningPanel;
    [SerializeField]
    private GameObject landingPanel;
    [SerializeField]
    private Text landingEvalutionText;
    [SerializeField]
    private GameObject landingColorPanel;

    [SerializeField]
    private GameObject environment;
    [SerializeField]
    private ReplayManager replayManager;

    private List<InputDevice> left;
    private List<InputDevice> controllers;
    private bool VRControllersActive;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Canvas>().enabled = false;
        EventManager.manager.OnReplayEnd += ShowMenu;
        EventManager.manager.OnFlightEnded += SetPostFlightMenu;
        EventManager.manager.OnFlightEnded += ShowMenu;
        EventManager.manager.OnReplayFailedToSave += ShowWarning;
        EventManager.manager.OnReplayFailedToLoad += ShowWarning;
        EventManager.manager.OnReplayFailedToLoad += ShowMenu;

        left = new List<InputDevice>();
        controllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, left);
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);

        // Triggers VRNotAvailable event, if no VR controllers found
        if (controllers.Count < 1)
        {
            VRControllersActive = false;
            EventManager.manager.VRNotAvailable();
        } else
            VRControllersActive = true;
    }

    // Update is called once per frame
    /// <summary>
    /// This Update function listens to the input and show menu if ESCAPE key is pressed, or Menu Button on VR controller
    /// </summary>
    void Update()
    {
        var menuButtonPressed = false;
        if (VRControllersActive)
        {
            foreach (InputDevice controller in left)
            {
                if (controller.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonPressed))
                {
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || menuButtonPressed)
        {
            ShowMenu();
            Globals.SetPaused(true);
        }
    }

    /// <summary>
    /// Enables in game menu and pauses the flight or replay
    /// </summary>
    public void ShowMenu()
    {
        this.GetComponent<Canvas>().enabled = true;

        if (Globals.inReplay)
        {
            Time.timeScale = 0f;
            Globals.SetReplayPaused(true);
        }

        if (Globals.isLive)
        {
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// Enables the free roam mode and hides in game menu
    /// </summary>
    public void OnChangeCameraButton()
    {
        replayManager.XRRig.GetComponent<VRRoaming>().SetFreeRoam(true);
        this.gameObject.GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Resumes the simulation in both flight scene and replay scene
    /// </summary>
    public void OnResumeButton()
    {
        if (!Globals.isLive && Globals.replayPaused)
        {
            replayManager.XRRig.GetComponent<VRRoaming>().SetFreeRoam(false);
            Time.timeScale = 1f;
            Globals.SetReplayPaused(false);
        }

        if (Globals.isLive)
        {
            Time.timeScale = 1f;
        }

        this.gameObject.GetComponent<Canvas>().enabled = false;
        Globals.SetPaused(false);
    }

    /// <summary>
    /// Hides in game menu
    /// </summary>
    public void OnHideMenuButton()
    {
        this.gameObject.GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Starts replay after saving the path to new replay file to PlayerPrefs
    /// </summary>
    public void OnReplayButton()
    {
        var path = Globals.saveFolder + FindObjectOfType<Logger>().GenerateSaveFormat();
        PlayerPrefs.SetString("jsonToLoad", path);
        Globals.SetLive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Shows warning screen about some error, depending on the scene
    /// In VR Paraglide the warning is shown if replay fails to save
    /// In Replay the warning is shown if replay fails to load
    /// </summary>
    public void ShowWarning()
    {
        warningPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Hides landing panel
    /// </summary>
    public void OnOkayButton()
    {
        landingPanel.SetActive(false);
    }

    /// <summary>
    /// Evaluates landing and shows the evaluation panel
    /// Evaluates the location of landing based on input score
    /// Evaluates the speed of landing based on current speed
    /// Evaluates landng direction based on current PK direction and current wind direction
    /// </summary>
    /// <param name="score">
    /// Index of location that the player landed in. Is used in evaluation of landing location
    /// </param>
    public void ShowLandingEvalution(int score)
    {
        switch (score)
        {
            case 0:
                //GOOD LANDING LOCATION
                landingEvalutionText.text = Globals.landingLocationGreen;
                landingColorPanel.GetComponent<Image>().color = Globals.greenColor;
                break;
            case 1:
                // ACCEPTABLE LANDING LOCATION
                landingEvalutionText.text = Globals.landingLocationYellow;
                landingColorPanel.GetComponent<Image>().color = Globals.yellowColor;
                break;
            case 2:
                // DANGEROUS LANDING LOCATION
                landingEvalutionText.text = Globals.landingLocationRed;
                landingColorPanel.GetComponent<Image>().color = Globals.redColor;
                break;
            default:
                // ERROR
                landingEvalutionText.text = "Not landed at all";
                landingColorPanel.GetComponent<Image>().color = Globals.redColor;
                break;
        }

        landingEvalutionText.text += "\n";
        var landingSpeed = FindObjectOfType<Movement>().GetSpeed();
        if (landingSpeed > Globals.highLandingSpeed)
        {
            landingEvalutionText.text += Globals.landingSpeedBad;
        } else if (landingSpeed > Globals.middleLandingSpeed)
        {
            landingEvalutionText.text += Globals.landingSpeedMiddle;
        } else
        {
            landingEvalutionText.text += Globals.landingSpeedGood;
        }

        landingEvalutionText.text += "\n";
        var windDirectionInDegrees = Quaternion.Euler(new Vector3(0, EnvironmentPhysics.windDirection * 45f, 0)) * Vector3.forward;
        var PKDirectionInDegrees = GameObject.FindGameObjectWithTag("PK").transform.forward;
        var landingAngleCoefficient = -1* Vector3.Dot(windDirectionInDegrees, PKDirectionInDegrees);

        if (landingAngleCoefficient > 1 - Globals.toleranceOfLandingDirection)
            landingEvalutionText.text += Globals.landingDirectionGood;
        else
            landingEvalutionText.text += Globals.landingDirectionBad;

        landingPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Hides warning panel
    /// </summary>
    public void HideWarning()
    {
        warningPanel.SetActive(false);
    }

    /// <summary>
    /// Disables the buttons that could not be used after the flight ended
    /// Disables resume flight button
    /// Disables disable winds button
    /// Disables hide menu button
    /// Enables replay button
    /// Sets replay button interactible to false if the flight was not recorded
    /// </summary>
    public void SetPostFlightMenu()
    {
        resumeButton.gameObject.SetActive(false);
        hideMenuButton.interactable = false;
        disableWindsButton.interactable = false;
        replayButton.gameObject.SetActive(true);
        if (!Globals.saveReplay)
            replayButton.interactable = false;
    }

    /// <summary>
    /// Sets the default values for many variables
    /// Sets the application to default state
    /// Loads Main Menu scene
    /// </summary>
    private void SetDefaultAndExit()
    {
        Time.timeScale = 1f;
        Globals.SetReplayPaused(false);
        Globals.SetLive(false);
        Globals.SetSampingFrequency(7);
        Globals.SetShowGuidelines(false);
        Globals.SetSaveReplay(false);
        //Globals.SetFlying(false);
        Globals.SetFlying(true);
        Globals.SetFixedControllerPosition(false);
        Globals.SetRotation(true);
        Globals.SetPaused(false);
        EnvironmentPhysics.windDirection = 0;
        EnvironmentPhysics.windIntensity = 0;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Exits the current scene and returns to the main menu
    /// </summary>
    public void OnExitButton()
    {
        if (Globals.isLive && Globals.saveReplay)
        {
            if (!FindObjectOfType<Logger>().SaveLog())
            {
                SetPostFlightMenu();
                replayButton.interactable = false;
                EventManager.manager.ReplayFailedToSave();
            }
            else
            {
                SetDefaultAndExit();
            }


        } else
        {
            SetDefaultAndExit();
        }

    }

    // Disables environment physics - flows and wind
    public void OnDisableButton()
    {
        EnvironmentPhysics env = GameObject.Find("Environment Physics").GetComponent<EnvironmentPhysics>();
        if (env != null)
            StartCoroutine(env.DisableFlows(0));
        EnvironmentPhysics.windIntensity = 0;
        EnvironmentPhysics.windDirection = 0;
        EnvironmentPhysics.windActive = false;
        GameObject.FindObjectOfType<GuidelineManager>().EliminateVisualCurrents();
        disableWindsButton.interactable = false;
    }
}
