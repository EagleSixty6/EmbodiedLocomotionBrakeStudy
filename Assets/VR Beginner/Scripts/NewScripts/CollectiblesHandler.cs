using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;



public class CollectiblesHandler : MonoBehaviour
{



    CheckAndReserveHands reserveHandsScript;
    bool rightHandOccupied;
    bool leftHandOccupied;
    int rightHandSphereNumber;
    int leftHandSphereNumber;
    bool rightGripValue;
    bool leftGripValue;
    float leftHandTimer;
    float rightHandTimer;
    bool rightHandTriggerEnteredSphere;
    bool leftHandTriggerEnteredSphere;
    public SphereSpawner sphereScript;
    private bool no1WasReleased = true;
    private bool no2WasReleased = true;
    bool reserveConLeft = false;
    bool reserveConRight = false;

    void Start()
    {
        reserveHandsScript = GameObject.Find("XR Rig").GetComponent<CheckAndReserveHands>();
        //ringScript = GameObject.Find("Ring").GetComponent<Basket>();


        rightHandOccupied = false;
        leftHandOccupied = false;
        leftHandTimer = 0f;
        rightHandTimer = 0f;
        rightHandSphereNumber = 0;
        leftHandSphereNumber = 0;
    }

   
    //called from spheres onGrabbed
    public void ReceivedGrab(int no)
    {

        bool thisOneIsalreadysend = false;
        if (rightHandOccupied && rightHandSphereNumber == no )
        {
            reserveHandsScript.ReleaseAHand("right");
            sphereScript.HandReleased("right", no, false);
            rightHandSphereNumber = 3;
            rightHandOccupied = false;
        }
        else if   (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.0 && !rightHandOccupied )//
        {
                rightHandOccupied = true;
                rightHandSphereNumber = no;
                Debug.Log("right Hand in use");
                reserveHandsScript.ReserveAHand("right");
                //ringScript.OnBallGrabbed("right");//old task
                sphereScript.OnBallGrabbed("right", no);
               

        }
        

        if (leftHandOccupied && leftHandSphereNumber == no)
        {
            reserveHandsScript.ReleaseAHand("left");
            sphereScript.HandReleased("left", no, false);
            leftHandSphereNumber = 3;
            leftHandOccupied = false;

        }
        else if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.0 && !leftHandOccupied ) // 
        {
            

            leftHandOccupied = true;
            leftHandSphereNumber = no;
            Debug.Log("left Hand in use");
            reserveHandsScript.ReserveAHand("left");
            //ringScript.OnBallGrabbed("left");//old task
            sphereScript.OnBallGrabbed("left", no);
        }

       
      
        Debug.Log("rightOccupied?: " + rightHandOccupied + "  leftoccupied?: " + leftHandOccupied);
    }

    //called from spheres onReleased
    public void ReleasedGrab(int no)
    {
        if (no == 0)
            no1WasReleased = true;
        else
            no2WasReleased = true;

       

        bool thisOneIsalreadysend = false; // important, else two calls at the same time when both are released at the same time
        if (rightHandOccupied)
        {
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") < 0.5)
            {
                rightHandOccupied = false;
                Debug.Log("right Hand not used anymore");
                reserveHandsScript.ReleaseAHand("right");
                //ringScript.HandReleased("right");//old task
                sphereScript.HandReleased("right", no, true);
                thisOneIsalreadysend = true;
            }
         

        }

        if (leftHandOccupied && !thisOneIsalreadysend)
        {
            if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") < 0.5)
            {
                leftHandOccupied = false;
                Debug.Log("left Hand not used anymore");
                reserveHandsScript.ReleaseAHand("left");
                //ringScript.HandReleased("left"); //old task
                sphereScript.HandReleased("left", no, true);

            }
          

        }
        Debug.Log("rightOccupied?: " + rightHandOccupied + "  leftoccupied?: " + leftHandOccupied);
    

    }

    //information from hands
    public void ReceivedSpehereObject(string hand, GameObject sphere)
    {
        if (hand.Equals("right"))
        {
            rightHandTriggerEnteredSphere = true;
        }
        else if (hand.Equals("left"))

        {
            leftHandTriggerEnteredSphere = true;
        }

    }
    //information from hands when no spehre is touching trigger
    public void ReleasedSpehereObject(string hand)
    {
        if (hand.Equals("right"))
        {
            rightHandTriggerEnteredSphere = false;
        }
        else if (hand.Equals("left"))
        {
            leftHandTriggerEnteredSphere = false;
        }

        Debug.Log("releasedsphere controller: " + hand);

    }

    public void gotSphere(int hand)
    {
        if (hand == 0 )
        {
            
        }
        else if (hand == 1 )
        {
          
        }

        Debug.Log("gotsphere controller: " + hand);
    

}

    public void lostSphere(int hand)
    {

        if (hand == 0)
        {
          
        }
        else
        {
           
        }
           

        Debug.Log("rightOccupied?: " + rightHandOccupied + "  leftoccupied?: " + leftHandOccupied);
    }


    public void ReserveControllerHand(int hand, bool status)
    {
        if (hand == 0)
            reserveConLeft = status;
        else if (hand == 1)
            reserveConRight = status;
    
    }

}
