using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

    public AudioClip introductionSequence;
    public AudioClip verificationSound;
    public GameObject introductionPanel;
    public RaycastStop raycastScript;
    public OffZoneSimple offZoneSimpleScript;
    public ControllerBased controllerStopScript;
    public GameObject methodOverviewPanel;
    public Transform secondStartPoint;
    public TrainingArea trainingAreaScript;
    public StartupScreen startScreenScript;
    public RigLogging rigLogScript;
    public SphereSpawner sphereScript;

    public Text current;
    public Text previous;
    public Text next;
    public Cubes cubesScript;
    public Slider waitSlider;

    public RectTransform dontMovePanel;

    bool waitTimerActive;
    float waitTimer;
    BoardBehaviour boardScript;
    SpeachController speachConScript;
    Movement movementScript;
    string[] stopTypesArray;
    int listPointer;
    float timer;
    bool busyWithProcedureOne = false;
    public AudioSource audioSource;
    float mOverVTimer;
    bool myScriptisDisabled = true;
    bool slowStop = false;
    bool slowStart = false;
    float slowStartTimer;
    int lastMETHOD = 12;
    public bool trainingActive = false;

    //logging
    bool startRoundTimer = false;
    float roundTimer = 0f;
    float brakeTime = 0f;
    public bool countBrakeTime = false;

    //speedChanges
    bool tmp1 = false;
    bool tmp2 = false;
    bool tmp3 = false;
    bool tmp4 = false;
    bool tmp5 = false;
    bool tmp6 = false;
    bool tmp7 = false;
    bool tmp8 = false;
    bool tmp9 = false;


    //test purposed ui layout
    private void setTexts()
    {
        if (listPointer < 1)
        {
            next.text = "next: " + stopTypesArray[listPointer + 1];
            current.text = stopTypesArray[listPointer];
            previous.text = "prev: " + stopTypesArray[5];
        }
        else if (listPointer > 4)
        {
            next.text = "next: " + stopTypesArray[0];
            current.text = stopTypesArray[listPointer];
            previous.text = "prev: " + stopTypesArray[listPointer - 1];

        }
        else
        {
            next.text = "next: " + stopTypesArray[listPointer + 1];
            current.text = stopTypesArray[listPointer];
            previous.text = "prev: " + stopTypesArray[listPointer - 1];
        }
    }

    void Update()
    {

        if (startRoundTimer)
        {
            roundTimer += Time.deltaTime;        
        }


        if (startRoundTimer && countBrakeTime)
        {
            brakeTime += Time.deltaTime;
        }


        if (!myScriptisDisabled)
            checkForNewInputs();

        if (methodOverviewPanel.activeInHierarchy)
            mOverVTimer = mOverVTimer + Time.deltaTime;

        if (mOverVTimer > 4.0f)
        {
            methodOverviewPanel.SetActive(false);
            mOverVTimer = 0.0f;
        }

        if (slowStop)
        {
            doSlowStop();          
        }
        else if (slowStart)
        {
            doSlowStart();
        }



    }

    //calibration verification
    public void sendSecondStepVerification()
    {
        Debug.Log("Finishing initialization - MainController");
      
        audioSource.clip = verificationSound;
        audioSource.Play();
        movementScript.onAcceptedUserInput();

        if(listPointer == 2)
            dontMovePanel.gameObject.SetActive(true); 

        busyWithProcedureOne = false;

        speachConScript.initializationEnabled = false;

        if (trainingActive)
        {
            trainingAreaScript.CalibrationFinished();
            rigLogScript.active = false;
        }       
        else
        {
            rigLogScript.active = true; 
            startRoundTimer = true;
            sphereScript.RealReset();
            sphereScript.activated = true;
        }

    }
    public void cancelUI()
    {
    }


    public void startIntroductionProcedure()
    {
        introductionPanel.SetActive(true);

        busyWithProcedureOne = true;
        speachConScript.initializationEnabled = true;

        if (trainingActive)
            timer = 1f;

        movementScript.onMethodChanged();

    }


    private void checkForNewInputs()
    {
        if (timer > 0)
            timer = timer - Time.deltaTime;      
    }

    //stop method script handler
    private void onChangingStopMethod(string newMethod, string oldMethod)
    {
        switch (oldMethod)
        {
            case ("Controller"):
                controllerStopScript.controllerBasedScriptActivated = false;
          
                break;
            case ("Speach"):
                speachConScript.mainSpeachProtocolEnabled = false;

                break;
            case ("OffZoneSimple"):
                offZoneSimpleScript.offZoneSimpleEnabled = false;             
                movementScript.offZoneSimple = false;
                break;
            case ("OffZoneBack"):
                movementScript.maskMethodActive = false;
        
                break;
            case ("Skateboard"):
                boardScript.boardScriptActivated = false;
          
                break;
            case ("Raycast"):
                raycastScript.rayStopEnabled = false;
             
                break;
            default:
                Debug.LogError("Wrong string at onChangingStopMethod() Line 97 Script MainController, coming from checkFornewInputs()");
                break;


        }
        //STARTS NEW INTRODUCTION(BIG CALIBRATION) WHEN CHANGING A METHOD
        if (newMethod.Equals("Controller") || newMethod.Equals("Speach") || newMethod.Equals("OffZoneSimple") || newMethod.Equals("OffZoneBack")
            || newMethod.Equals("Skateboard") || newMethod.Equals("Raycast"))
        {

            if (newMethod.Equals("OffZoneBack"))
            {
                startNewMethod();
            }
            else
            {
                startNewMethod();
            }


        }
        else
            Debug.LogError("Wrong string at onChangingStopMethod() Line 117 Script MainController, coming from checkFornewInputs()");

    }

    //procedure on change of a method during tests
    public void startNewMethod()
    {
        if (!movementScript.pauseLeaning)
            applyMovemeventChanges("stop");



        switch (stopTypesArray[listPointer])
        {
            case ("Controller"):
                movementScript.offZoneSimple = false;

                controllerStopScript.controllerBasedScriptActivated = true;
                controllerStopScript.movement = false;
                dontMovePanel.gameObject.SetActive(false); 
                break;
            case ("Speach"):
                movementScript.offZoneSimple = false;
                speachConScript.mainSpeachProtocolEnabled = true;
                dontMovePanel.gameObject.SetActive(false); 
                break;
            case ("OffZoneSimple"):
                offZoneSimpleScript.offZoneSimpleEnabled = true;
                offZoneSimpleScript.moving = false;
                offZoneSimpleScript.init = false;
                movementScript.offZoneSimple = true;
                break;
            case ("OffZoneBack"):
                applyMovemeventChanges("slowStart");
                movementScript.offZoneSimple = false;
                movementScript.maskMethodActive = true;
                dontMovePanel.gameObject.SetActive(false); 
                break;
            case ("Skateboard"):
                movementScript.offZoneSimple = false;
                boardScript.boardScriptActivated = true;
                dontMovePanel.gameObject.SetActive(false); 
                break;
            case ("Raycast"):
                movementScript.offZoneSimple = false;
                raycastScript.init = false;
                raycastScript.rayStopEnabled = true;
                dontMovePanel.gameObject.SetActive(false);
                break;
            default:
                Debug.LogError("Wrong string at onChangingStopMethod() Line 94 Script MainController, coming from checkFornewInputs()");
                break;
        }

    }


    //checks for changes regarding the stop feature of the current status control method
    public bool checkForMovementChanges(string typeOfMethod, string typeOfChange)
    {


        switch (stopTypesArray[listPointer])
        {
            case ("Controller"):
                if (stopTypesArray[listPointer].Equals(typeOfMethod))
                {
                    return applyMovemeventChanges(typeOfChange);
                }
                else
                    return false;
            case ("Speach"):
                if (stopTypesArray[listPointer].Equals(typeOfMethod))
                {
                    if (applyMovemeventChanges(typeOfChange))
                    {
                        audioSource.PlayOneShot(verificationSound, 2f);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            case ("OffZoneSimple"):
                if (stopTypesArray[listPointer].Equals(typeOfMethod))
                {
                    return applyMovemeventChanges(typeOfChange);
                }
                else
                    return false;
            case ("OffZoneBack"):
                if (stopTypesArray[listPointer].Equals(typeOfMethod))
                {
                    return applyMovemeventChanges(typeOfChange);
                }
                else
                    return false;
            case ("Skateboard"):
                if (stopTypesArray[listPointer].Equals(typeOfMethod))
                {
                    return applyMovemeventChanges(typeOfChange);
                }
                else
                    return false;
            case ("Raycast"):
                if (stopTypesArray[listPointer].Equals(typeOfMethod))
                {
                    return applyMovemeventChanges(typeOfChange);
                }
                else
                    return false;
            default:
                Debug.LogError("Wrong string at onForMovementMethod() Line 94 Script MainController, coming from checkFornewInputs()");
                return false;

        }


    }

    //applies slowbraking, slowstart or calibration when asked
    private bool applyMovemeventChanges(string change)
    {
        float percentage;
        if (change.Equals("stop"))
        {
            countBrakeTime = true;
            return movementScript.openStop();
        }
        else if (change.Equals("slowStop"))
        {
            slowStart = false;
            countBrakeTime = true;
            Debug.Log("machSlowStop befehl");

            return slowStop = true;
        }
        else if (change.Equals("slowStart"))
        {
            slowStop = false;
            countBrakeTime = false;
            Debug.Log("machSlowStart befehl");
            return slowStart = true;
        }
        else if (change.Equals("start"))
        {
            countBrakeTime = false;
            return movementScript.openStart();
        }
        else if (change.Equals("calibrate"))
        {
            return movementScript.openCalibrateEasy();
        }
        else if (float.TryParse(change, out percentage))
        {
            Debug.Log("slower! new speed at: " + percentage);
            return movementScript.openSlowStop(percentage);
        }
        else
            return false;

    }

    //primitive slow stop 
    public void doSlowStop()
    {

        slowStartTimer += Time.deltaTime;

        if (slowStartTimer >= 0.0 && slowStartTimer <= 0.02 && !tmp1)
        {
            movementScript.openSlowStop(1);       
            tmp1 = true;
        }
        else if (slowStartTimer > 0.02 && slowStartTimer <= 0.04 && !tmp2)
        {
            movementScript.openSlowStop(2);
            tmp2 = true;
        }
        else if (slowStartTimer > 0.04 && slowStartTimer <= 0.06 && !tmp3)
        {
            movementScript.openSlowStop(3);
            tmp3 = true;
        }
        else if (slowStartTimer > 0.06 && slowStartTimer <= 0.08 && !tmp4)
        {
            movementScript.openSlowStop(4);
            tmp4 = true;
        }
        else if (slowStartTimer > 0.08 && slowStartTimer <= 0.1 && !tmp5)
        {
            movementScript.openSlowStop(5);
            tmp5 = true;
        }
        else if (slowStartTimer > 0.1 && slowStartTimer <= 0.12 && !tmp6)
        {
            movementScript.openSlowStop(6);
            tmp6 = true;
        }
        else if (slowStartTimer > 0.12 && slowStartTimer <= 0.14 && !tmp7)
        {
            movementScript.openSlowStop(7);
            tmp7 = true;
        }
        else if (slowStartTimer > 0.14 && slowStartTimer <= 0.16 && !tmp8)
        {
            movementScript.openSlowStop(8);
            tmp8 = true;
        }
        else if (slowStartTimer > 0.16 && slowStartTimer <= 0.18 && !tmp9)
        {
            movementScript.openSlowStop(10);
            tmp9 = true;
        }
        else if (tmp9)
        {
           
            slowStartTimer = 0f;
            slowStop = false;
            tmp1 = false;
            tmp2 = false;
            tmp3 = false;
            tmp4 = false;
            tmp5 = false;
            tmp6 = false;
            tmp7 = false;
            tmp8 = false;
            tmp9 = false;
            Debug.Log("slowStop ausgefuerht");
        }


    }

    //primitive slow start
    public void doSlowStart()
    {
        slowStartTimer += Time.deltaTime;

        if (slowStartTimer >= 0.0 && slowStartTimer <= 0.02 && !tmp1)
        {
            movementScript.openSlowStop(9);
            tmp1 = true;
        }
        else if (slowStartTimer > 0.02 && slowStartTimer <= 0.04 && !tmp2)
        {
            movementScript.openSlowStop(8);
            tmp2 = true;
        }
        else if (slowStartTimer > 0.04 && slowStartTimer <= 0.06 && !tmp3)
        {
            movementScript.openSlowStop(7);
            tmp3 = true;
        }
        else if (slowStartTimer > 0.06 && slowStartTimer <= 0.08 && !tmp4)
        {
            movementScript.openSlowStop(6);
            tmp4 = true;
        }
        else if (slowStartTimer > 0.08 && slowStartTimer <= 0.1 && !tmp5)
        {
            movementScript.openSlowStop(5);
            tmp5 = true;
        }
        else if (slowStartTimer > 0.1 && slowStartTimer <= 0.12 && !tmp6)
        {
            movementScript.openSlowStop(4);
            tmp6 = true;
        }
        else if (slowStartTimer > 0.12 && slowStartTimer <= 0.14 && !tmp7)
        {
            movementScript.openSlowStop(3);
            tmp7 = true;
        }
        else if (slowStartTimer > 0.14 && slowStartTimer <= 0.16 && !tmp8)
        {
            movementScript.openSlowStop(2);
            tmp8 = true;
        }
        else if (slowStartTimer > 0.16 && slowStartTimer <= 0.18 && !tmp9)
        {
            movementScript.openSlowStop(0);
            tmp9 = true;
        }
        else if (tmp9)
        {
            slowStartTimer = 0f;
            slowStart = false;
            tmp1 = false;
            tmp2 = false;
            tmp3 = false;
            tmp4 = false;
            tmp5 = false;
            tmp6 = false;
            tmp7 = false;
            tmp8 = false;
            tmp9 = false;
        }




    }


    //method to get the reference point
    public Vector3 TransferLeaningRef()
    {
        return movementScript.getLeaningRefPosition();

    }

    //procedure on restart of any method & leaning / the level
    public void OnRestartMovementStuff(string type)
    {
       
        controllerStopScript.controllerBasedScriptActivated = false;
        speachConScript.mainSpeachProtocolEnabled = false;
        offZoneSimpleScript.offZoneSimpleEnabled = false;
        movementScript.maskMethodActive = false;
        boardScript.boardScriptActivated = false;
        raycastScript.rayStopEnabled = false;
        movementScript.offZoneSimple = false;

        movementScript.pauseLeaning = true;

        applyMovemeventChanges("stop");
        Debug.Log("offzonesimple enabled oder nicht (darf jetzt nicht) " + offZoneSimpleScript.offZoneSimpleEnabled);

        if (type.Equals("full"))
        {
            MoveRigToStartPosition();
            cubesScript.ResetAllCubes();

        }
        else if (type.Equals("onePause"))
        {
            MoveRigToStartPosition();
            cubesScript.ResetAllCubes();         
        }
        else if (type.Equals("twoPause"))
        {
            MoveRigToSecondPosition();//

            cubesScript.ResetAllCubes();

        }
        else if (type.Equals("threePause"))
        {
            MoveRigToThirdPosition();// 
           
            cubesScript.ResetAllCubes();

        }
        else if (type.Equals("cycle"))
        {
            MoveRigToStartPosition();
        }
        else if (type.Equals("training"))
        {
            MoveRigToStartPosition();
        }
        else if (type.Equals("justPause"))
        {
            pauseTimers(false);
        }
        else
        {
            Debug.LogError("Type mismatch, Line 543 MainController, OnCallingRestartScreen, you gave the wrong string parameter !");
        }
        myScriptisDisabled = true;
    }

    //notifies other scripts when a controller is already in use
    public void NotifyControllerOccupation(string hand, bool reserved)
    {
        switch (stopTypesArray[listPointer])
        {
            case ("Controller"):
                controllerStopScript.OccupyControllers(hand, reserved);
                break;
            case ("Speach"):

                break;
            case ("OffZoneSimple"):

                break;
            case ("OffZoneBack"):

                break;
            case ("Skateboard"):

                break;
            case ("Raycast"):

                break;
            default:
                Debug.LogError("Wrong string at onChangingStopMethod() Line 592 Script MainController, coming from NotifyControllerOccupation()");
                break;
        }

    }

    //moving the cameraRig
    public void MoveRigToSecondPosition()
    {
        Vector3 position = new Vector3();
        position = new Vector3(0, this.transform.position.y, 5 * 10 );
        cubesScript.ResetAllCubes();
        this.transform.position = position;
    }

    //moving the cameraRig
    public void MoveRigToThirdPosition()
    {
        Vector3 position = new Vector3();
        position = new Vector3(0, this.transform.position.y, 5 * 10 + 15 + 5 * 10);
        cubesScript.ResetAllCubes();
        this.transform.position = position;
    }

    //moving the cameraRig
    public void MoveRigToStartPosition()
    {
        Vector3 position = new Vector3();
        position = new Vector3(0f, this.transform.position.y, 0f);
        this.transform.position = position;
    }

    //start study procedure without recalibrating
    public void StartMethodOnly(int type)
    {
        speachConScript = GameObject.Find("SFXPlayer").GetComponent<SpeachController>();
        movementScript = GameObject.Find("Locomotion System").GetComponent<Movement>();
        boardScript = GameObject.Find("MoveableBoard").GetComponent<BoardBehaviour>();
       
        stopTypesArray = new string[] { "Controller", "Speach", "OffZoneSimple", "OffZoneBack", "Skateboard", "Raycast" };
        listPointer = type;

        if (lastMETHOD != 12)
        {
            onChangingStopMethod(stopTypesArray[lastMETHOD], stopTypesArray[lastMETHOD]);
        }
        else
        {
            lastMETHOD = listPointer;

        }
        myScriptisDisabled = false;
        rigLogScript.active = true; 
        startRoundTimer = true;
        myScriptisDisabled = false;
    }

    //introduction start
    public void StartScriptAndIntroductionProcedure(int type)
    {
        //audioSource = GameObject.Find("SFXSource").GetComponent<AudioSource>(); // darf nicht die selbe wie fuer speach sein
        speachConScript = GameObject.Find("SFXPlayer").GetComponent<SpeachController>();
        movementScript = GameObject.Find("Locomotion System").GetComponent<Movement>();
        boardScript = GameObject.Find("MoveableBoard").GetComponent<BoardBehaviour>();
       
        stopTypesArray = new string[] { "Controller", "Speach", "OffZoneSimple", "OffZoneBack", "Skateboard", "Raycast" };
        listPointer = type;
        startIntroductionProcedure();

        if (lastMETHOD != 12)
        {
            onChangingStopMethod(stopTypesArray[listPointer], stopTypesArray[lastMETHOD]);
        }
        else
        {
            lastMETHOD = listPointer;

        }

        methodOverviewPanel.SetActive(false);
        mOverVTimer = 0.0f;
        setTexts();

        myScriptisDisabled = false;



    }

    public void setAudioSource(AudioClip aClip)
    {
        audioSource.Stop();
        audioSource.clip = aClip;
        audioSource.Play();
    }

    public void pitchCurrentAudio(float pitch)
    {
        audioSource.pitch = pitch;
        //float[] spectrum = new float[256];
        //Debug.Log(" "+ AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular));
    }

    public void stopAudioSource()
    {
        audioSource.Stop();
    }

    public void playOneShotAudioClip(AudioClip oneShot)
    {
        audioSource.PlayOneShot(oneShot);

    }

    public void StopSpecialSounds()
    {
        Debug.Log("name ; " + audioSource.clip.name);
        if (audioSource.clip.name == "HeavyWobbleHoverboardFast")
        {
            audioSource.Stop();
            audioSource.pitch = 1;
        }
    }

    public void SendVelociy(float velocity)
    {
        boardScript.SetVelocity(velocity);
    }

    //training area
    public void SetTraining(bool training)
    {
        trainingActive = training;
        cubesScript.activateTraining(training);
        startScreenScript.disableMe(!training);
        raycastScript.TrainingActive(training);
    }

    public string GetCurrentRunningMethod(int i)
    {
        return stopTypesArray[i];
    }

    //logging,logging,logging
    public float GetAndResetGeneralRoundTimer(bool deactivate)
    {
        startRoundTimer = false;
        float tmpTime = roundTimer;
        roundTimer = 0f;
        if (deactivate)
            startRoundTimer = false;
        else
            startRoundTimer = true;
       
        return tmpTime;
    }

    public float GainNoMovementTimeWithoutBrakePlusReset()
    {
        return movementScript.GetTimeWhileNullMovementAndReset();
    }

    public float GetAndResetBrakeTimer()
    {
        float tmp = brakeTime;
        brakeTime = 0f;
        return tmp;
    }

    public float GetAndResetBoardHoldInHandsTimer()
    {
        return boardScript.GetTimeAndResetTime();
    }

    public float GetOffZoneTime1Reset()
    {
        return movementScript.GetTimeBereich1PlusReset();
    }

    public float GetOffZoneTime2Reset()
    {
        return movementScript.GetTimeBereich2PlusReset();
    }

    public void pauseTimers(bool value)
    {
        startRoundTimer = value;
        countBrakeTime = value;
    }

   
}
