using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This static class holds all the important information about the state of the application
/// It also holds constant values that are used inside the application
/// Static methods inside this class are used to set the state of application
/// </summary>
public static class Globals
{
    public static bool continuousRotation = true;
    public static bool fixedControllerPosition = false;
    public static bool isFlying = false;
    public static bool isPaused = false;
    public static bool isLive;
    public static bool inReplay;
    public static bool replayPaused;
    public static bool saveReplay = true;
    public static bool showGuidelines;

    public static string saveFolder = Application.persistentDataPath + "/Saves/";
    public static int samplingFrequency;
    public static string deafultTerrain = "MJavorovy_3DTerrain";

    public static string landingLocationGreen = "Good landing location";
    public static string landingLocationYellow = "Acceptable landing location";
    public static string landingLocationRed = "Dangerous landing location";

    public static string landingSpeedGood = "Speed is low enough";
    public static string landingSpeedMiddle = "Speed should be lower";
    public static string landingSpeedBad = "Speed too high";

    public static string landingDirectionGood = "Good landing angle";
    public static string landingDirectionBad = "Bad landing angle";

    public static float highLandingSpeed = 9f;
    public static float middleLandingSpeed = 6f;
    public static float toleranceOfLandingDirection = 0.3f;

    public static PK profile;

    public static Color redColor = new Color(0.815f, 0.070f, 0.172f);
    public static Color greenColor = new Color(0.212f, 0.913f, 0.259f);
    public static Color yellowColor = new Color(1f, 0.816f, 0.05f);

    // Is called when flight is Live, used for recording replay
    public static void SetLive(bool on)
    {
        isLive = on;
    }

    // Is called when replay is paused/unpaused
    public static void SetReplayPaused(bool on)
    {
        replayPaused = on;
    }

    // Is called when replay of flight is live
    public static void SetReplayLive(bool on)
    {
        inReplay = on;
    }

    // Stores sampling frequency at which next flight will be recorded
    public static void SetSampingFrequency(int freq)
    {
        samplingFrequency = freq;
    }

    // Sets the global tag to save next flight
    public static void SetSaveReplay(bool on)
    {
        saveReplay = on;
    }

    // Sets the global tag to show guidelines in the next flight
    public static void SetShowGuidelines(bool on)
    {
        showGuidelines = on;
    }

    // Sets the paraglide profile to be used in the next flight
    public static void SetProfile(PK pk)
    {
        profile = pk;
    }

    // Sets the global tag of continuous rotation in the next flight
    public static void SetRotation(bool on)
    {
        continuousRotation = on;
    }

    // Sets the global tag to use fixed controller levels in the next flight
    public static void SetFixedControllerPosition(bool on)
    {
        fixedControllerPosition = on;
    }

    // Sets the global tag if the simulation is running
    public static void SetFlying(bool on)
    {
        isFlying = on;
    }

    // Is called if the simulation is paused / unpaused
    public static void SetPaused(bool on)
    {
        isPaused = on;
    }
}
