using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerLeft : MonoBehaviour
{ 
    private XRController controller = null;

    // Bool values to determine whether the button was pressed current frame or not
    private bool gripLast = false;

    // Initialization
    void Start()
    {
        controller = GetComponent<XRController>();
    }

    void Update()
    {
        // Used buttons
        // CHANGE: SET TO TRUE
        bool grip = true;                    // Used to start the flight

        // CHANGE:COMMENTED
        //controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out grip);

        // Start flight
        if (gripLast != grip)
        {
            if (grip)                 // Button was pressed this frame
            {
                if (!Globals.isFlying)
                {
                    Time.timeScale = 1f;
                    Globals.SetFlying(true);
                    Globals.SetLive(true);
                }
            }            
            gripLast = grip;
        }
    }
}
