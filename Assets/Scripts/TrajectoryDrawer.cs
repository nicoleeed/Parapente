using UnityEngine;

public static class TrajectoryDrawer
{
    /// <summary>
    /// Draws trajectory based on inputed array of points on an inputed renderer component
    /// </summary>
    /// <param name="renderer"> LineRenderer to render the trajectory line </param>
    /// <param name="points"> Array of points which define the line to render </param>
    public static void DrawLineTrajectory(LineRenderer renderer, Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] += new Vector3(0f, -1.5f, 0);
        }

        renderer.positionCount = points.Length;
        renderer.SetPositions(points);
    }
    /// <summary>
    /// Draws trajectory based on inputed array of points on an inputed renderer component and sets the material color based on the color parameter
    /// </summary>
    /// <param name="renderer"> LineRenderer to render the trajectory line </param>
    /// <param name="points"> Array of points which define the line to render </param>
    /// <param name="color"> Color to set the material to </param>
    public static void DrawLineTrajectory(LineRenderer renderer, Vector3[] points, Color color)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] += new Vector3(0f, -1.5f, 0);
        }

        renderer.material.SetColor("_Color", color);
        renderer.positionCount = points.Length;
        renderer.SetPositions(points);
    }

    /// <summary>
    /// Instantiates the inputed object in positions of inputed array of Vector3
    /// </summary>
    /// <param name="prefab"> Object to be instantiated to create a trajectory </param>
    /// <param name="points"> Array of points which sets the positions of prefab instances </param>
    public static void DrawPrefabTrajectory(GameObject prefab, Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            GameObject.Instantiate(prefab, points[i], prefab.transform.rotation);
        }
    }

    /// <summary>
    /// Instantiates the inputed object in positions of inputed array of Vector3 and sets the color of the shared material
    /// </summary>
    /// <param name="prefab"> Object to be instantiated to create a trajectory </param>
    /// <param name="points"> Array of points which sets the positions of prefab instances </param>
    /// <param name="color"> Color to set the material to </param>
    public static void DrawPrefabTrajectory(GameObject prefab, Vector3[] points, Color color)
    {
        for (int i = 0; i < points.Length; i++)
        {
            var instance = GameObject.Instantiate(prefab, points[i], prefab.transform.rotation);
            instance.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);
        }
    }
}
