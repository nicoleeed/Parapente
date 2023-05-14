using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRObjectHider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.manager.OnVRNotAvailable += HideThisObject;
    }

    // Sets the object inactive in case of VR being not available.
    // Mainly used for hiding raycast beams of VR controllers, when no controllers active
    public void HideThisObject()
    {
        this.gameObject.SetActive(false);
    }
}
