using UnityEngine;
using UnityEngine.UI;

public class TestHUD : MonoBehaviour
{
    // TEST FEATURE //
    // HUD used to display important values during the flight
    public Text windDirection;       // Direction of the current wind
    public Text windIntensity;       // Intensity of the current wind
    public Text inFlow;              // Sum of forces of all air flows the paraglide is in
    public Text currentSpeed;        // Current speed of the paraglide (basic speed, doesn't include speed of environment physical forces)
    public Text descendSpeed;        // Current paraglide's descend speed

    private readonly string[] directions = new string[] { "North", "North-East", "East", "South-East", "South", "South-West", "West", "North-West" };
    private Movement movement;
    

    // Initialization
    void Start()
    {
        movement = GameObject.Find("PK").GetComponent<Movement>();
        UpdateHUD();
    }

    // Updates values in the HUD
    public void UpdateHUD()
    {
        if (!EnvironmentPhysics.windActive)
            windDirection.text = "None";
        else
            windDirection.text = directions[EnvironmentPhysics.windDirection].ToString();
        windIntensity.text = EnvironmentPhysics.windIntensity.ToString();
        inFlow.text = movement.GetAirForce().ToString();
        currentSpeed.text = GameObject.Find("PK").GetComponent<Rigidbody>().velocity.magnitude.ToString("F2") + " m/s";
        descendSpeed.text = (movement.GetDescend() + movement.GetAirForce().y).ToString("F2") + " m/s";
    }
}
