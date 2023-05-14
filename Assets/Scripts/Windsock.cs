using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windsock : MonoBehaviour
{
    public float maxIntensity = 5f;
    [Range(0, 80)]
    public float maxAngle = 50f;

    private ReplayManager replayManager;

    private float currentIntensity;
    private int currentDirection;
    private float frameTimer;
    private Vector3 newRotation;

    // Start is called before the first frame update
    void Start()
    {
        if (Globals.isLive)
        {
            currentDirection = EnvironmentPhysics.windDirection;
            currentIntensity = EnvironmentPhysics.windIntensity;
        }
        if (Globals.inReplay)
        {
            replayManager = FindObjectOfType<ReplayManager>();
            var wind = replayManager.GetWindAtCurrentMoment();
            currentDirection = wind.Item2;
            currentIntensity = wind.Item1;
        }

        frameTimer = 0;
        newRotation = transform.localEulerAngles;
    }

    // Update is called once per frame
    /// <summary>
    /// Checks every frame if wind properties changed
    /// If yes, then sets the new vector3 to rotate into
    /// Every frame Slerps towards the newRotation based on frametimer
    /// </summary>
    void Update()
    {
        if (Globals.inReplay)
        {
            var wind = replayManager.GetWindAtCurrentMoment();
            if (currentIntensity != wind.Item1 || currentDirection != wind.Item2)
                SetRotation(wind.Item1, wind.Item2);
        }

        if (Globals.isLive)
        {
            if (currentIntensity != EnvironmentPhysics.windIntensity || currentDirection != EnvironmentPhysics.windDirection)
                SetRotation(EnvironmentPhysics.windIntensity, EnvironmentPhysics.windDirection);
        }
        transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, newRotation, frameTimer);
        frameTimer += Time.deltaTime;

    }

    /// <summary>
    /// Sets the Vecto3 newRotation based on current wind intensity and direction
    /// </summary>
    /// <param name="intensity"> Wind speed in m/s </param>
    /// <param name="direction"> Index of wind direction </param>
    /// <returns></returns>
    void SetRotation(float intensity, int direction)
    {
        currentDirection = direction;
        currentIntensity = intensity;

        frameTimer = 0;
        var xRot = maxAngle * (maxIntensity - currentIntensity) / maxIntensity;
        var yRot = currentDirection * 45f;
        newRotation.x = xRot;
        newRotation.y = yRot;
    }
}
