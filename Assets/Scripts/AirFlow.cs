using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirFlow : MonoBehaviour
{
    [Tooltip("For descending flow set negative value")]
    public float force;                 // Force magnitude of the airflow. Positive value means ascending flow, negative value means descending flow
    public Rigidbody rb;

    private bool isActive = true;       // Bool whether the flow is active and affects paraglide or not     
    private bool inFlowLeft = false;    // Bool whether the left part of paraglide is in flow
    private bool inFlowRight = false;   // Bool whether the right part of paraglide is in flow

    private Movement paraglideMovement;

    // Initialization
    void Start()
    {
        paraglideMovement = GameObject.Find("PK").GetComponent<Movement>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    /// <summary>
    /// Checks which part of the paraglide entered the flow
    /// and applies the force accordingly.
    /// </summary>
    /// <param name="other"> Collider of a part of the paraglide </param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("is not Active");
        if (isActive)
        {
            if (other.CompareTag("PK Left"))
            {
                ApplyAirForce(-force, force);
                inFlowLeft = true;
                Debug.Log("Left wing entered, adding force " + force / 2f);
            }
            else if (other.CompareTag("PK Right"))
            {
                ApplyAirForce(force, -force);
                inFlowRight = true;
                Debug.Log("Right wing entered, adding force " + force / 2f);
            }
        }    
    }

    /// <summary>
    /// Checks which part of the paraglide left the flow
    /// and removes the force accordingly.
    /// </summary>
    /// <param name="other"> Collider of a part of the paraglide </param>
    private void OnTriggerExit(Collider other)
    {
        if (isActive)
        {
            if (other.CompareTag("PK Left"))
            {
                RemoveAirForce(-force, force);
                inFlowLeft = false;
                Debug.Log("Left wing exited, removing force " + force / 2f);
            }
            else if (other.CompareTag("PK Right"))
            {
                RemoveAirForce(force, -force);
                inFlowRight = false;
                Debug.Log("Right wing exited, removing force " + force / 2f);
            }
        }
    }

    /// <summary>
    /// Removes the force of air flow.
    /// </summary>
    /// <param name="tiltZ"> Force to be removed to restore the tilt of the paraglide </param>
    /// <param name="turnY"> Force to be removed to restore the rotation of the paraglide </param>
    public void RemoveAirForce(float tiltZ, float turnY)
    {
        paraglideMovement.turnY -= turnY;
        paraglideMovement.airTiltZ -= tiltZ;
        Vector3 direction = transform.up.normalized;
        paraglideMovement.RemoveAirForce(force / 2f, direction);
    }

    /// <summary>
    /// Applies the force of air flow on each part separately
    /// If there is only one part of the paraglide in the flow, the paraglide will slightly rotate and tilt.
    /// If both parts are in the flow, the same force is applied to both parts and thus the paraglide won't rotate nor tilt, only ascend.
    /// </summary>
    /// <param name="tiltZ"> Force to tilt the paraglide </param>
    /// <param name="turnY"> Force to rotate the paraglide </param>
    public void ApplyAirForce(float tiltZ, float turnY)
    {
        paraglideMovement.turnY += turnY;
        paraglideMovement.airTiltZ += tiltZ;
        Vector3 direction = transform.up.normalized;
        paraglideMovement.AddAirForce(force / 2f, direction);
    }


    // GETTERS / SETTERS //

    public bool GetInFlowRight()
    {
        return inFlowRight;
    }

    public bool GetInFlowLeft()
    {
        return inFlowLeft;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool val)
    {
        isActive = val;
    }
}
