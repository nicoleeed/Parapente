using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AlternativeTrajectory : MonoBehaviour
{
    public int speedOfRotation;

    public LineRenderer alternativeTrajectoryRenderer;
    public ReplayManager replayManager;

    private List<InputDevice> controllers;

    private PK PKProfile;
    private Quaternion lastRotation;
    private float currentSpeed;

    private List<Vector3> windCurrentPositions;
    private List<float> windCurrentSpeeds;

    /// <summary>
    /// If object is being dragged by mouse, rotate the object based on mouse Input
    /// Mouse scrolling changes the rotation around Z axis
    /// </summary>
    private void OnMouseDrag()
    {
        if (Globals.replayPaused)
        {
            float rotationX = Input.GetAxis("Mouse X") * speedOfRotation * Mathf.Deg2Rad;
            float rotationY = Input.GetAxis("Mouse Y") * speedOfRotation * Mathf.Deg2Rad;
            float rotationZ = Input.mouseScrollDelta.y * speedOfRotation * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up, -rotationX);
            transform.Rotate(Vector3.forward, rotationY);
            transform.Rotate(Vector3.right, rotationZ);
        }
    }

    /// <summary>
    /// Fills windCurrentPositions and windCurrentSpeeds lists with the windCurrent data from ReplayManager
    /// Sets lastRotation to current rotation
    /// </summary>
    private void Start()
    {
        controllers = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, controllers);
        lastRotation = transform.rotation;
        windCurrentPositions = replayManager.GetReplayWindCurrentPositions();
        windCurrentSpeeds = replayManager.GetReplayWindCurrentSpeeds();
    }

    /// <summary>
    /// Enables collider to allow interaction with mouse or controller if replay is paused
    /// Sets currentSpeed to speed of the PK at current control point
    /// If rotation of this object is not equal to lastRotation, calculates alternative trajectory and draws it
    /// If Replay is unpaused, turns off the collider, set lastRotation to current rotation and clears alternative trajectory
    /// </summary>
    private void Update()
    {
        if (Globals.replayPaused && Globals.inReplay)
        {
            this.GetComponent<BoxCollider>().enabled = true;
            currentSpeed = replayManager.GetSpeedAtCurrentMoment();
            if (transform.rotation != lastRotation)
            {
                var alternativeTrajectory = CalculateAlternativeTrajectory();
                TrajectoryDrawer.DrawLineTrajectory(alternativeTrajectoryRenderer, alternativeTrajectory.ToArray(), Color.white);
                lastRotation = transform.rotation;
            }
        } else
        {
            this.GetComponent<BoxCollider>().enabled = false;
            ClearTrajectory();
            lastRotation = transform.rotation;
        }

    }

    /// <summary>
    /// Clears the linerenderer positions
    /// </summary>
    public void ClearTrajectory()
    {
        alternativeTrajectoryRenderer.positionCount = 0;
    }


    /// <summary>
    /// Sets the variable PKProfile to PK object based on profile name
    /// </summary>
    /// <param name="profile">
    /// Is a string with the name of the profile saved in Replay
    /// </param>
    public void SetPKProfile(string profile)
    {
        PK pkvar = ScriptableObject.CreateInstance<PK>();
        JsonUtility.FromJsonOverwrite(profile, pkvar);
        PKProfile = ScriptableObject.Instantiate(pkvar);
    }

    /// <summary>
    /// Calculates the alternative trajectory based on current rotation and position
    /// Trajectory consists of 15 points
    /// Offset of each of the points is affected by current rotation, variable currentSpeed and wind currents
    /// For each point it detects if point is inside of a trigger of wind current
    /// If point is inside a wind current, it's position is changed based on current direction and speed
    /// </summary>
    /// <returns>
    /// The list of Vector3 representing points of alternative trajectory
    /// </returns>
    private List<Vector3> CalculateAlternativeTrajectory()
    {
        List<Vector3> trajectoryPoints = new List<Vector3>
        {
            transform.position
        };

        var rotationZ = transform.eulerAngles.z;
        if (rotationZ > 180)
            rotationZ -= 360;

        for (int i = 1; i < 15; i++)
        {

            var currentZ = rotationZ - i*rotationZ/15;
            var newPosition = trajectoryPoints[i-1] + new Vector3((transform.forward.x + Mathf.Abs(transform.right.x)*currentZ / -15) * currentSpeed,
                transform.forward.y + PKProfile.descend, (transform.forward.z + Mathf.Abs(transform.right.z) * currentZ / -15) * currentSpeed);

            RaycastHit hit;
            Physics.Linecast(transform.position, newPosition, out hit);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name.Contains("WindCurrent"))
                {
                    var offset = new Vector2(hit.collider.gameObject.transform.position.x - newPosition.x, hit.collider.gameObject.transform.position.z - newPosition.z);
                    if (offset.magnitude < hit.collider.gameObject.transform.localScale.y/2)
                    {
                        var index = windCurrentPositions.IndexOf(hit.collider.gameObject.transform.position);
                        newPosition.y += windCurrentSpeeds[index];
                    }

                }
            }
            trajectoryPoints.Add(newPosition);
        }

        return trajectoryPoints;
    }
}
