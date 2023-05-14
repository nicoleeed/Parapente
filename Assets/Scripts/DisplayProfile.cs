using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayProfile : MonoBehaviour
{
    public PK profile;
    public GameObject values;           // Values canvas

    public Text description;
    public Text speed;
    public Text descend;
    public Text maxTilt;
    public Text maxRotateSpeed;
    public Text continuousCoefficient;
    public Text turnSharpness;

    /// <summary>
    /// Overwrites current description of the paraglide profile parameters and displays them
    /// </summary>
    public void DisplayValues()
    {
        description.text = profile.description;
        speed.text = profile.speed.ToString();
        descend.text = profile.descend.ToString();
        maxTilt.text = profile.maxTilt.ToString();
        maxRotateSpeed.text = profile.maxRotateSpeed.ToString();
        continuousCoefficient.text = profile.continuousCoefficient.ToString();
        turnSharpness.text = profile.turnSharpnes.ToString();
        values.SetActive(true);
    }

    // Hides values
    public void HideValues()
    {
        values.SetActive(false);
    }

    // Sets the chosen profile
    public void ChooseProfile()
    {
        Globals.SetProfile(profile);
    }
}
