using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBased : MonoBehaviour
{
    public bool controllerBasedScriptActivated;
    public MainController mainControllerScript;
    public CollectiblesHandler handler;

    public AudioSource audioSource;
    public AudioClip calibrationStartSound;
    public AudioClip calibrating;
    public AudioClip one;
    public AudioClip cancelled;

    public bool movement;
    float timer = 0;
    float slowStartTimer = 0f;
    bool done = false;
    bool done1 = false;
    bool startTimer;
    bool calibStarted = false;
    bool startSlowStartProcedure;
    bool cleanup = false;

    bool rightHandOccupied;
    bool leftHandOccupied;

   

    bool allowOccupationLeft = true;
    bool allowOccupationRight = true;

    private void Start()
    {
        controllerBasedScriptActivated = false;
        movement = false;
        startTimer = false;
        done = false;
        done1 = false;
        slowStartTimer = 0f;
    }


    void Update()
    {
        if (controllerBasedScriptActivated)
        {
            CheckForControllerInputs();
            cleanup = false;
        }
        else if (!cleanup)
        {
            movement = false;
            cleanup = true;
            slowStartTimer = 0f;
            allowOccupationRight = true;
            allowOccupationLeft = true;
        }

       
    }

    public void OccupyControllers(string hand, bool occupied)
    {
        if (hand.Equals("right") )
        {
            
                rightHandOccupied = occupied;
           
        }
        else if (hand.Equals("left") )
        {
         
                leftHandOccupied = occupied;
          
        }

        Debug.Log("Occupied Controller : " + hand + " " + occupied); 
    }


    //check if any trigger button is pressed by more then half
    private void CheckForControllerInputs()
    {
        
        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.5 && movement && !leftHandOccupied)
        {
            mainControllerScript.checkForMovementChanges("Controller", "slowStop");
            fullfillCancellation(true);
            movement = false;
            //startSlowStartProcedure = false;
            allowOccupationLeft = false;
            //handler.ReserveControllerHand(0, true);
            Debug.Log("betrete sop von controller secondary left   " + leftHandOccupied);
        }
        else if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.5 && movement && !rightHandOccupied)
        {
            mainControllerScript.checkForMovementChanges("Controller", "slowStop");
            fullfillCancellation(true);
            movement = false;
            //startSlowStartProcedure = false;
            allowOccupationRight = false;
            //handler.ReserveControllerHand(1, true);
            Debug.Log("betrete sop von controller secondary right   " + rightHandOccupied);
        }
        else if (!movement && (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") < 0.5 || rightHandOccupied ) && (!movement && Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") < 0.5 || leftHandOccupied))
        {
            if (!allowOccupationRight)
            {
                allowOccupationRight = true;
                //handler.ReserveControllerHand(1, false);
            }


            if (!allowOccupationLeft)
            {
                allowOccupationLeft = true;

                //handler.ReserveControllerHand(0, false);
            }


            mainControllerScript.checkForMovementChanges("Controller", "slowStart");
            //startSlowStartProcedure = true;
            movement = true;
            fullfillCancellation(true);

            Debug.Log("betrete starteMovement ding vom Controller");

        }
        else if ((Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") < 0.5 && !allowOccupationRight))
        {

            if (!allowOccupationRight)
            {
                allowOccupationRight = true;
                //handler.ReserveControllerHand(1, false);
            }
        }
        else if ( (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") < 0.5 && !allowOccupationLeft))
        {
            if (!allowOccupationLeft)
            {
                allowOccupationLeft = true;
                //handler.ReserveControllerHand(0, false);
            }
              
        }
    }


    //following 2: old recalibration functionality
    private void fullfillCancellation(bool cancel)
    {
        if (timer > 0f && cancel)
        {
            audioSource.Stop();
            audioSource.clip = cancelled;
            audioSource.Play();
            timer = 0f;
            startTimer = false;
            done = false;
            done1 = false;
            calibStarted = false;
        }
        else if (!cancel)
        {
            audioSource.Stop();
            timer = 0f;
            startTimer = false;
            done = false;
            done1 = false;
            calibStarted = false;
        }
    }

    private void startCalibration()
    {
        startTimer = true;
        

        if (!calibStarted)
        { 
             audioSource.Stop();
             audioSource.clip = calibrationStartSound;
             audioSource.Play();
             calibStarted = true;
        }
        

        if (timer > 3.0 && timer < 6.0)
        {
            if (!done1)
            {
                audioSource.Stop();
                audioSource.clip = one;
                audioSource.Play();
                done1 = true;
            }


            if (timer >= 4.0 && timer < 5.0 && !done)
            {
                audioSource.Stop();
                audioSource.clip = calibrating;
                audioSource.Play();
                mainControllerScript.checkForMovementChanges("Controller", "calibrate");
                done = true;
            }
        }
        else if(timer >= 6.0)
            fullfillCancellation(false);

    }
    
}





