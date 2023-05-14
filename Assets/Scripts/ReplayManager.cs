using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ReplayManager : MonoBehaviour
{
    public GameObject PK;
    public GameObject trajectoryRenderer;

    public GameObject XRRig;
    public GameObject PKCamera;

    public GameObject VRMenu;
    public GameObject VanillaMenu;
    public GameObject windCurrentPrefab;

    public GameObject[] windCurrents;
    public float[] windCurrentsSpeeds;

    public GameObject[] terrainTypes;

    [SerializeField]
    private Material cableMaterial;
    private SaveObject replay;
    private int savePointsCounter;
    private float frameTimer;

    private Vector3 currentPosition;
    private Vector3 nextPointPosition;
    private Quaternion currentRotation;
    private Quaternion nextPointRotation;


    /// <summary>
    /// Loads replay data from .json file, sets the starting position of PK and his rotation, draws trajectory and cables
    /// If replay fails to load, warning is shown
    /// </summary>
    void Start()
    {
        // If VR is not available, set in game menu to be non-vr version
        EventManager.manager.OnVRNotAvailable += SwitchInGameMenus;
        EventManager.manager.OnVRNotAvailable += TurnOffDefaultCamera;
        Globals.SetReplayLive(true);
        // Loads the replay
        replay = Logger.LoadLog(PlayerPrefs.GetString("jsonToLoad"));
        if (replay == null)
        {
            // Replay is currupt or not found, shows warning
            EventManager.manager.ReplayFailedToLoad();
            Globals.SetReplayLive(false);
        } else
        {
            // We try to match the terrain name from replay's header
            var terrain = FindTerrainType(replay.header.terrainPrefabName);
            if (terrain != null)
                GameObject.Instantiate(terrain);
            else
            {
                // If the terrain name does not match, we will load default terrain
                Debug.Log("Terrain prefab missing");
                var defaultTerrain = (GameObject)Resources.Load(Globals.deafultTerrain);
                GameObject.Instantiate(defaultTerrain);
            }
            for (int i = 0; i < replay.windCurrentPositions.Count; i++)
            {
                // We instantiate visual currents based on the currents saved to the replay
                GameObject instance = GameObject.Instantiate(windCurrentPrefab, replay.windCurrentPositions[i], replay.windCurrentRotations[i]);
                instance.transform.localScale = replay.windCurrentScales[i];
                var col = instance.AddComponent<CapsuleCollider>();
                col.isTrigger = true;
                if (replay.windCurrentSpeeds[i] > 0f)
                    instance.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("_WindDirection"), -0.5f);
                else
                    instance.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("_WindDirection"), 1f);
            }
            // Setting the PK Profile for Alternative Trajectory calculation
            PK.GetComponent<AlternativeTrajectory>().SetPKProfile(replay.header.PKProfile);

            // Preparing replay data for playing
            savePointsCounter = 0;
            SetTransformInfoPoints();

            // Show the original trajectory of the flight
            TrajectoryDrawer.DrawLineTrajectory(trajectoryRenderer.GetComponent<LineRenderer>(), replay.PKPositions.ToArray(), Globals.greenColor);
            // Draws cable objects
            DrawCables();
        }
    }

    // Update is called once per frame
    /// <summary>
    /// Moves PK around based on recorded data only if replay is not paused and has not ended
    /// </summary>
    void LateUpdate()
    {
        if (Globals.inReplay)
        {
            if (!Globals.replayPaused)
            {
                if (savePointsCounter < replay.PKRotations.Count - 2 && frameTimer >= 1f / replay.header.sampleFrequency)
                {
                    savePointsCounter++;
                    SetTransformInfoPoints();
                    frameTimer = 0f;
                }

                if (savePointsCounter == replay.PKRotations.Count - 2 && frameTimer > 1f / replay.header.sampleFrequency)
                {
                    Globals.SetReplayLive(false);
                    EventManager.manager.ReplayEnded();
                }

                PK.transform.position = Vector3.Lerp(currentPosition, nextPointPosition, frameTimer * replay.header.sampleFrequency);
                PK.transform.rotation = Quaternion.Slerp(currentRotation, nextPointRotation, frameTimer * replay.header.sampleFrequency);
                XRRig.transform.position = PKCamera.transform.position;
                frameTimer += Time.deltaTime;

            }
        }

    }

    /// <summary>
    /// Disables the PK camera
    /// </summary>
    private void TurnOffDefaultCamera()
    {
        PKCamera.GetComponent<Camera>().enabled = false;
    }

    /// <summary>
    /// This method is responsible for finding cable position object in terrain prefab and adding their position to arrays of Vector3
    /// It creates three GamebObjects that have component LineRenderer to render cables
    /// It calls GuidelineManager.RenderCables() method for each array of positions
    /// </summary>
    private void DrawCables()
    {
        var cablePositions = GameObject.FindGameObjectsWithTag("Cable");
        var sortedCables = cablePositions.OrderBy(go => go.name).ToList();

        var (middle, left, right) = (sortedCables.GetRange(0, cablePositions.Length / 3),
            sortedCables.GetRange(cablePositions.Length / 3, cablePositions.Length / 3),
            sortedCables.GetRange(cablePositions.Length / 3 * 2, cablePositions.Length / 3));

        LineRenderer middleR = (new GameObject("middleR")).AddComponent<LineRenderer>();
        LineRenderer leftR = (new GameObject("leftR")).AddComponent<LineRenderer>();
        LineRenderer rightR = (new GameObject("rightR")).AddComponent<LineRenderer>();

        middleR.material = cableMaterial;
        leftR.material = cableMaterial;
        rightR.material = cableMaterial;
        GuidelineManager.RenderCables(left.ToArray(), leftR);
        GuidelineManager.RenderCables(middle.ToArray(), middleR);
        GuidelineManager.RenderCables(right.ToArray(), rightR);
    }

    /// <summary>
    /// This method toggles between the VR version and default version of In Game Menu
    /// </summary>
    private void SwitchInGameMenus()
    {
        VRMenu.SetActive(false);
        VanillaMenu.SetActive(true);
    }

    /// <summary>
    /// This method tries to match terrainPrefabName name to terrain type in terrainTypes field and return it
    /// </summary>
    /// <param name="terrainPrefabName">
    /// String representing the name of terrain to be loaded
    /// </param>
    /// <returns>
    /// GameObject terrain prefab, is terrainPrefabName matches the name of one of the prefabs in terrainTypes
    /// null if not matching prefab is found
    /// </returns>
    private GameObject FindTerrainType(string terrainPrefabName)
    {
        if (terrainTypes.Length == 0)
            return null;

        for (int i = 0; i < terrainTypes.Length; i++)
        {
            if (terrainTypes[i].name.Equals(terrainPrefabName))
            {
                return terrainTypes[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Returns current speed of PK based on replay data
    /// </summary>
    /// <returns>
    /// Float value representing current speed of PK
    /// </returns>
    public float GetSpeedAtCurrentMoment()
    {
        return replay.PKCurrentSpeeds[savePointsCounter];
    }

    /// <summary>
    /// Returns current wind intensity and direction based on replay data
    /// </summary>
    /// <returns>
    /// Tuple of float and int
    /// Float value is wind intensity in m/s
    /// Int value is the id of wind direction
    /// </returns>
    public (float, int) GetWindAtCurrentMoment()
    {
        return (replay.windIntensities[savePointsCounter], replay.windDirections[savePointsCounter]);
    }

    /// <summary>
    /// Returns a List of positions of wind currents based on replay data
    /// </summary>
    /// <returns>
    /// List of Vector3 values of wind current positions
    /// </returns>
    public List<Vector3> GetReplayWindCurrentPositions()
    {
        if (replay != null)
            return replay.windCurrentPositions;
        else
            return null;
    }

    /// <summary>
    /// Returns a List of speeds of wind currents based on replay data
    /// </summary>
    /// <returns>
    /// List of float values of wind current speeds
    /// </returns>
    public List<float> GetReplayWindCurrentSpeeds()
    {
        if (replay != null)
            return replay.windCurrentSpeeds;
        else
            return null;
    }

    /// <summary>
    /// Sets the variables of PK positions and rotation for interpolation
    /// Sets the currentPosition and currentRotation to values from replay with index savePointsCounter
    /// Sets the nextPointPosition and nextPointRotation to values from replay with index savePointsCounter
    /// </summary>
    private void SetTransformInfoPoints()
    {
        currentPosition = replay.PKPositions[savePointsCounter];
        nextPointPosition = replay.PKPositions[savePointsCounter + 1];
        currentRotation = replay.PKRotations[savePointsCounter];
        nextPointRotation = replay.PKRotations[savePointsCounter + 1];
    }

    /// <summary>
    /// Pauses or unpauses the replay
    /// Pausing the game is handled by the InGameMenu class currently
    /// </summary>
    /// <param name="pause">
    /// Boolean to decide if game should be paused, or unpaused
    /// </param>
    public void PauseReplay(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        Globals.SetReplayPaused(pause);
    }
}
