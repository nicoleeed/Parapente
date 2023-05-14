using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindMenu : MonoBehaviour
{
    public Text directionText;
    public Text intensityText;

    private readonly string[] directions = new string[] { "North", "North-East", "East", "South-East", "South", "South-West", "West", "North-West" };

    private void Start()
    {
        directionText.text = "None";
        intensityText.text = "0 m/s";
    }

    /// <summary>
    /// Activates constant wind with direction based on given index
    /// </summary>
    /// <param name="index"> Index to directions array </param>
    public void SetWind(int index)
    {
        EnvironmentPhysics.windActive = true;
        EnvironmentPhysics.windDirection = index;
        directionText.text = directions[index];
    }

    /// <summary>
    /// Increases wind intensity by 0.5 m/s 
    /// </summary>
    public void IncreaseWindIntensity()
    {
        EnvironmentPhysics.windIntensity += 0.5f;
        intensityText.text = EnvironmentPhysics.windIntensity.ToString("F1") + " m/s";
    }

    /// <summary>
    /// Decreases wind intensity by 0.5 m/s
    /// </summary>
    public void DecreaseWindIntensity()
    {
        if (EnvironmentPhysics.windIntensity >= 0.5f)
        {
            EnvironmentPhysics.windIntensity -= 0.5f;
            intensityText.text = EnvironmentPhysics.windIntensity.ToString("F1") + " m/s";
        }     
    }

    /// <summary>
    /// Deactivates wind and sets intensity to 0 m/s
    /// </summary>
    public void DisableWind()
    {
        EnvironmentPhysics.windActive = false;
        EnvironmentPhysics.windIntensity = 0f;
        directionText.text = "None";
        intensityText.text = EnvironmentPhysics.windIntensity.ToString("F1") + " m/s";
    }
}
