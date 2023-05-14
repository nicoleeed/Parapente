using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerDebug : MonoBehaviour
{
    // USED TO DISPLAY HEADSET'S AND CONTROLLERS' Y-POSITION
    public Text left;
    public Text right;
    public Text head;

    public GameObject leftC, rightC, headset;

    void Update()
    {
        left.text = leftC.transform.localPosition.y.ToString("F2");
        right.text = rightC.transform.localPosition.y.ToString("F2");
        head.text = headset.transform.localPosition.y.ToString("F2");
    }
}
