using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPhysics : MonoBehaviour
{
    static public int windDirection = 0;               // Index for wind direction 
    static public float windIntensity = 0;              
    static public bool windActive = false;             // Bool whether the wind is active and affecting paraglide or not
    public Rigidbody rb;

    private Vector3[] winds;
    private Vector3 north, south, west, east,
                    n_east, n_west, s_east, s_west;     // Wind directions

    // Initialize wind directions
    void Start()
    {
        rb = GameObject.Find("PK").GetComponent<Rigidbody>();

        // Default wind directions          // INDICES:
        north = new Vector3(0, 0, 1);       // 0
        n_east = new Vector3(1, 0, 1);      // 1
        east = new Vector3(1, 0, 0);        // 2
        s_east = new Vector3(1, 0, -1);     // 3
        south = new Vector3(0, 0, -1);      // 4
        s_west = new Vector3(-1, 0, -1);    // 5
        west = new Vector3(-1, 0, 0);       // 6
        n_west = new Vector3(-1, 0, 1);     // 7

        winds = new[] { north, n_east, east, s_east, south, s_west, west, n_west };

        // Disable all air flows after 5 minutes
        StartCoroutine(DisableFlows(300f));
    }

    private void Update()
    {

    }

    /// <summary>
    /// Applies force of the constant wind on the paraglide
    /// </summary>
    public void ApplyWind()
    {
        rb.velocity += winds[windDirection] * windIntensity;
    }

    /// <summary>
    /// Deactivates all air flows after given time, if pilot is in one at the moment, remove the force first
    /// </summary>
    /// <param name="time"> Time in seconds </param>
    /// <returns></returns>
    public IEnumerator DisableFlows(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (Transform flow in transform)
        {
            if (flow.GetComponent<AirFlow>().GetInFlowLeft())
            {
                flow.GetComponent<AirFlow>().RemoveAirForce(flow.GetComponent<AirFlow>().force, -flow.GetComponent<AirFlow>().force);
                Debug.Log("Removed force " + flow.GetComponent<AirFlow>().force + " from left wing");
            }
            if (flow.GetComponent<AirFlow>().GetInFlowRight())
            {
                flow.GetComponent<AirFlow>().RemoveAirForce(-flow.GetComponent<AirFlow>().force, -flow.GetComponent<AirFlow>().force);
                Debug.Log("Removed force " + flow.GetComponent<AirFlow>().force + " from right wing");
            }
            flow.GetComponent<AirFlow>().SetIsActive(false);
            Debug.Log("Disabled flow " + flow.GetComponent<AirFlow>().force);
        }
    }

}
