using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustParaglide : MonoBehaviour
{
 
    // Constantly adjusts paraglide model to match height of the VR headset
    void Update()
    {
        float headPosition = GameObject.Find("PK").transform.Find("XR Rig").transform.Find("Camera Offset").transform.Find("Main Camera").transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, headPosition - 1.3f, transform.localPosition.z);
    }
}
