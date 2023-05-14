using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for handling events that arise during the run of the application
/// For each event there is a method that Invokes it
/// The methods are called from other classes
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager manager;

    public event Action OnReplayFailedToLoad;
    public event Action OnReplayEnd;
    public event Action OnVRNotAvailable;
    public event Action OnReplayFailedToSave;
    public event Action OnFlightEnded;

    private void Awake()
    {
        if (manager != null && manager != this)
        {
            Destroy(this);
        }
        else
        {
            manager = this;
        }
    }

    public void FlightEnded()
    {
        OnFlightEnded?.Invoke();
    }

    public void VRNotAvailable()
    {
        OnVRNotAvailable?.Invoke();
    }

    public void ReplayFailedToLoad()
    {
        OnReplayFailedToLoad?.Invoke();
    }

    public void ReplayEnded()
    {
        OnReplayEnd?.Invoke();
    }

    public void ReplayFailedToSave()
    {
        OnReplayFailedToSave?.Invoke();
    }
}
