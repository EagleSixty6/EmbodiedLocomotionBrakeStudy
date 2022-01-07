using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;
using UnityEngine.XR;

public class BoardBehaviour : MonoBehaviour
{
    public GameObject parentTransformWhileMoving;
    public Transform[] pivotPoints;
    public bool boardScriptActivated;
    public GameObject cubeObj;
    public AudioClip hoverboardClip;
    public AudioSource audioSource;
    public GameObject newHoverboard;

    private InputDevice leftController;
    private InputDevice rightController;
    private CheckAndReserveHands checkHandsScript;
    public MainController mainControllerScript;
    private bool foundNoClosePoints;
    private bool grabbed = false;
    private int pivotIndex = 1;
    Quaternion tmp;
    GameObject _headJoint;
    public GameObject controller;
    public Transform interpolationStartPosition;
    public Transform rotationPosition;
    Vector3 startPosition;
    Vector3 SR;
    bool tempStop = false;
    bool moving = false;
    int counter = 0;

    //check fuer controller
    bool rightTriggerValue;
    bool leftTriggerValue;
    bool rightConSendTicks = false;
    bool leftConSendTicks = false;
    string currentController;
    bool leftControllerEntered = false;
    bool rightControllerEntered = false;
    float hapticTimer = 0f;

    //soundKram
    Vector3 refPosition;
    public Movement movementScript;
    float distanceTmp;
    float soundTimer;
    bool soundplaying;
    float currentVelocity;

    //logging
    float timerWhileHoldInHand = 0f;

   
    void Start()
    {
      

        soundTimer = 0f;
        distanceTmp = 0f;
        soundplaying = false;

        startPosition = Vector3.zero;
        grabbed = false;
        boardScriptActivated = false;
    
        tempStop = false;
        moving = false;

        rightTriggerValue = false;
        leftTriggerValue = false;

        Debug.Log("BoardStartAktiviert");

        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

        if (leftHandDevices.Count == 1)
        {

            leftController = leftHandDevices[0];
            
        }
        else if (leftHandDevices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        }


        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

        if (rightHandDevices.Count == 1)
        {
            rightController = rightHandDevices[0];
        }
        else if (rightHandDevices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        }


    }

    void Update()
    {
        if (boardScriptActivated)
        {
            pivotPoints[1].transform.localPosition = new Vector3(movementScript.getLeaningRefPosition().x, 0.35f, movementScript.getLeaningRefPosition().z) ;

            if (newHoverboard.activeInHierarchy == false && counter == 0)
            {
                mainControllerScript.checkForMovementChanges("Skateboard", "stop");
                newHoverboard.SetActive(true);
                this.GetComponent<BoxCollider>().enabled = true;
                refPosition = movementScript.getLeaningRefPosition();
                
            }
            counter++;

            if (grabbed)
            {
                timerWhileHoldInHand += Time.deltaTime;
                rightController.StopHaptics();
                leftController.StopHaptics();
                changeParentAndCheckforClosePoints();         
            }
            else
            { 
                          
                hapticTimer += Time.deltaTime;

                if (rightConSendTicks && hapticTimer >= 0.1)
                {
                    rightController.SendHapticImpulse(0u, 0.3f, 0.1f);
                    hapticTimer = 0f;
                }

                if (leftConSendTicks && hapticTimer >= 0.1)
                {
                    leftController.SendHapticImpulse(0u, 0.3f, 0.1f);
                    hapticTimer = 0f;
                }

                SnapToClosePoints();

                if (moving)
                    ChangeSoundAccordingToDistance();
                else if(!moving && audioSource.isPlaying)
                {
                    Debug.Log("stoppeaudioabersolltebaldaufjedenfall");
                    soundplaying = false;
                    
                        Debug.Log("stoppeaudio");
                       
                    audioSource.Stop();
                    soundTimer = 0f;
                  
                }
                    
            }
        }
        else if (!boardScriptActivated && newHoverboard.activeInHierarchy == true)
        {
            newHoverboard.SetActive(false);
            this.GetComponent<BoxCollider>().enabled = false;
            pivotIndex = 1;
            moving = false;
            tempStop = false; 
            grabbed = false;
            cubeObj.SetActive(false);
            Debug.Log("board script deaktivschleife");
            counter = 0;
            rightConSendTicks = false;
            leftConSendTicks = false;
            hapticTimer = 0f;

            leftControllerEntered = false;
            rightControllerEntered = false;

          

            if (soundTimer > 0f)
            {
                audioSource.Stop();
            }
            else
                audioSource.Stop();
                           
            distanceTmp = 0f;
            soundplaying = false;
            soundTimer = 0f;

              SnapToClosePoints();
           
        }



    }

    private void changeParentAndCheckforClosePoints()
    {
        if (pivotIndex == 1 && Vector3.Distance(pivotPoints[pivotIndex].position, this.transform.position) < 0.8f)//geaendert von 0.8f
        {
            pivotIndex = 0;
            foundNoClosePoints = false;
        }
        else if (pivotIndex == 0 && Vector3.Distance(pivotPoints[pivotIndex].position, this.transform.position) < 0.5f)
        {
            pivotIndex = 1;
            foundNoClosePoints = false;
        }
        else if (pivotIndex == 0 && tempStop == false)
        {
            mainControllerScript.checkForMovementChanges("Skateboard", "slowStop");
            tempStop = true;
            moving = false;
       
            soundplaying = false;
            if (soundTimer > 0f)
            {
                audioSource.Stop();
            }
                
            soundTimer = 0f;
        }       
    }

    private void SnapToClosePoints()
    {
        if (pivotIndex == 0 )
        {
            if (moving == false)
            { 
                mainControllerScript.checkForMovementChanges("Skateboard", "slowStart");
                moving = true;
            }
            
            tempStop = false;          

            if (_headJoint == null)
                _headJoint = GameObject.Find("CenterOfYawRotation(Clone)");
    
            float yAngle = parentTransformWhileMoving.transform.rotation.eulerAngles.y;
            transform.position = new Vector3(_headJoint.transform.position.x, pivotPoints[pivotIndex + 1].position.y, _headJoint.transform.position.z);
            transform.rotation = Quaternion.Euler(0.0f, yAngle, 0.0f);     
        }
        else if (pivotIndex == 1)
        {       
            tempStop = false;
            transform.position = pivotPoints[pivotIndex - 1].position;
            if (moving == true)
            { 
                mainControllerScript.checkForMovementChanges("Skateboard", "slowStop");
                moving = false;
            }      
            transform.rotation = pivotPoints[pivotIndex - 1].rotation;
        }
    }

   

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("das ist trigger obj: " + other);          
        if (other.CompareTag("RightController"))
        {
            rightControllerEntered = true;
            leftControllerEntered = false;
        }
        
        if (other.CompareTag("LeftController"))
        {
            leftControllerEntered = true;
            rightControllerEntered = false;
        }
    }

    
    //vibration of the controllers
    public void OnHoverEnter()
    {
        if (rightControllerEntered)
        {
            HapticCapabilities capabilities;
            if (rightController.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                   rightConSendTicks = true; 
                }
            }
        }
        
        if (leftControllerEntered)
        {
            HapticCapabilities capabilities;
            if (leftController.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    
                    leftConSendTicks = true;
                }
            }
        }
      

        /*
        HapticCapabilities capabilities;
        if (rightController.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                Debug.Log("supporting Buffers");
                //rightController.SendHapticBuffer(0, );

            }
        }
        */

        //DUMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM wird hier nieeeeeeeee eintreten
        /*
        if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out rightGripValue) && rightGripValue)
        {
            Debug.Log("Grip button right is pressed while hovering Skateboard");

           
                
        }
        else if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out leftGripValue) && leftGripValue)
        {
            Debug.Log("Grip button left is pressed while hovering Skateboard");

        }
        */

    }

    public void OnHoverExit()
    {

        rightConSendTicks = false;
        leftConSendTicks = false;
        leftControllerEntered = false;
        rightControllerEntered = false;


    }

    //appearance change
    public void onGrabbed()
    {
        grabbed = true;
        newHoverboard.SetActive(false);
        cubeObj.SetActive(true);
    }

    public void onReleased()
    {
        newHoverboard.SetActive(true);
        cubeObj.SetActive(false);
        grabbed = false;
    }

    public void SetVelocity(float velo)
    {
        currentVelocity = velo;
    }

    //sound feedback of the hoverboard
    private void ChangeSoundAccordingToDistance()
    {
        if (!soundplaying)
        {
            audioSource.pitch = 1f;
            audioSource.clip = hoverboardClip;
            audioSource.Play();
            soundplaying = true;
        }
        else
        {
            soundTimer += Time.deltaTime;    
        }    

        if (System.Math.Round(Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition), 2) != distanceTmp)
        {
           
            if (currentVelocity > 0.2f)
            {
                audioSource.pitch = currentVelocity + 0.2f;
            }
            else
            {
                audioSource.pitch = 0.2f + 0.2f;
            }
                

            distanceTmp = Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition);
        }

    }

    //logging
    public float GetTimeAndResetTime()
    {
        float tmp = timerWhileHoldInHand;
        timerWhileHoldInHand = 0f;
        return tmp;
    }
}
