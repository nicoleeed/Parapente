using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRRoaming : MonoBehaviour
{
    public Camera headset;
    public GameObject rightController;
    public GameObject leftController;

    public float transportSpeedModifier;
    public float rotationSpeedModifier;

    private bool freeroamMode;
    private List<InputDevice> controllers;
    private List<InputDevice> headMountedDisplays;

    /// <summary>
    /// Sets free roam mode on or off
    /// <param name="on"> Boolean value to set freeroamMode to </param>
    /// </summary>
    public void SetFreeRoam(bool on)
    {
        freeroamMode = on;
    }

    // Start is called before the first frame update
    void Start()
    {
        controllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);
        headMountedDisplays = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, headMountedDisplays);
    }

    // Update is called once per frame
    /// <summary>
    /// Update moves the object if appropriate input from keyboard or vr controllers is recieved
    /// </summary>
    void Update()
    {
        if (freeroamMode)
        {
            // VR controllers are not connected
            if (controllers.Count == 0)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        // Rotates the camera up
                        transform.Rotate(-Vector3.right * rotationSpeedModifier * Time.unscaledDeltaTime);
                    else
                        // Moves the camera forwards
                        transform.position += transform.forward * transportSpeedModifier * Time.unscaledDeltaTime;

                }

                if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        // Rotates the camera down
                        transform.Rotate(Vector3.right * rotationSpeedModifier * Time.unscaledDeltaTime);
                    else
                        // Moves the camera backwards
                        transform.position -= transform.forward * transportSpeedModifier * Time.unscaledDeltaTime;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    // Rotates the camera to the left
                    transform.Rotate(-Vector3.up * rotationSpeedModifier * Time.unscaledDeltaTime);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    // Rotates the camera to the right
                    transform.Rotate(Vector3.up * rotationSpeedModifier * Time.unscaledDeltaTime);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        // Moves the camera down
                        transform.position -= transform.up * transportSpeedModifier * Time.unscaledDeltaTime;
                    else
                        // Moves the camera up
                        transform.position += transform.up * transportSpeedModifier * Time.unscaledDeltaTime;
                }

            } else
            {
                // Checks if HMD is available
                if (headMountedDisplays.Count > 0)
                {
                    headMountedDisplays[0].TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition);

                    var positionDifference = Vector3.zero;
                    foreach (InputDevice controller in controllers)
                    {
                        controller.TryGetFeatureValue(CommonUsages.trigger, out float inputValue);
                        if (inputValue > 0)
                        {
                            controller.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 controllerPosition);
                            positionDifference += inputValue * (controllerPosition - headPosition);
                        }

                    }
                    // Translates the object based on the position difference of controllers
                    transform.position += positionDifference * transportSpeedModifier * Time.unscaledDeltaTime;
                }

            }
        }
    }
}
