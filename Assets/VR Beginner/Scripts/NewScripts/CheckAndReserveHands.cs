using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;

public class CheckAndReserveHands : MonoBehaviour
{
    //public List<InputDevice> allDevices = new List<InputDevice>();

    InputDevice leftHandDevice;
    InputDevice rightHandDevice;

    bool leftHandReserved;
    bool rightHandReserved;
    bool leftHandPrevUpdate;
    bool rightHandPrevUpdate;

    MainController mainControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        mainControllerScript = GameObject.Find("XR Rig").GetComponent<MainController>();

        leftHandReserved = false;
        rightHandReserved = false;
        leftHandPrevUpdate = false;
        rightHandPrevUpdate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftHandPrevUpdate && leftHandReserved)
        {
            NotifyMovementScripts("left", true);
            //leftHandPrevUpdate = true;
        }
        else if (leftHandPrevUpdate && !leftHandReserved)
        {
            NotifyMovementScripts("left", false);      
        }

        leftHandPrevUpdate = leftHandReserved;

        if (!rightHandPrevUpdate && rightHandReserved)
        {
            NotifyMovementScripts("right", true);
        }
        else if (rightHandPrevUpdate && !rightHandReserved)
        {
            NotifyMovementScripts("right", false);
        }

        rightHandPrevUpdate = rightHandReserved;


    }

    private void NotifyMovementScripts(string hand, bool reserved)
    {
        mainControllerScript.NotifyControllerOccupation(hand, reserved);
    }

    public void ReserveAHand(string hand)
    {
        if (hand.Equals("right"))
        {
            if (rightHandReserved)
            {
                Debug.LogWarning("hand already reserved, check Scripts !");
            }
            else
                rightHandReserved = true;
            
        }
        else if (hand.Equals("left"))
        {
            if (leftHandReserved)
            {
                Debug.LogWarning("hand already reserved, check Scripts !");
            }
            else
                leftHandReserved = true;
        }
    
    
    }

    public void ReleaseAHand(string hand)
    {

        if (hand.Equals("right"))
        {
            if (!rightHandReserved)
            {
                Debug.LogWarning("hand already release, check Scripts !");
            }
            else
                rightHandReserved = false;
        }
        else if (hand.Equals("left"))
        {
            if (!leftHandReserved)
            {
                Debug.LogWarning("hand already release, check Scripts !");
            }
            else
                leftHandReserved = false;
        }


    }

}
