using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerRight : MonoBehaviour
{
    public GameObject HUD;

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
        bool grip;              // Used to toggle HUD

        controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out grip);

        // Toggle HUD
        if (gripLast != grip)
        {
            if (grip)           
                // Button was pressed this frame
                HUD.SetActive(!HUD.activeInHierarchy);           
            gripLast = grip;
        }
    }
}
