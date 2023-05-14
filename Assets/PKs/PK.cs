using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New paraglide profile", menuName = "Paraglide profile")]
public class PK : ScriptableObject
{
    public string description;          // Description of the paraglide profile
    public float speed;                 // Default forward speed
    public float descend;               // Default descend speed

    // DISCRETE CONTROLS //
    public float maxTilt;               // Maximum tilting degrees
    public float maxRotateSpeed;        // Maximum rotation speed

    // CONTINUOUS CONTROLS //
    [Range(0, 1)]
    public float continuousCoefficient; // Coefficient used to rotate and tilt the paraglide in continuous controls
    public float turnSharpnes;          // Defines sharpness of turning. Higher value means pilot can perform sharper turns.
}
