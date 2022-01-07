using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeachController : MonoBehaviour
{
    public bool mainSpeachProtocolEnabled;
    public bool initializationEnabled;
    
    MainController mainControllerScript;
    public SpeachRecognition speachRecognitionScript;
   

    void Start()
    {
        initializationEnabled = false;
        mainSpeachProtocolEnabled = false;
        mainControllerScript = GameObject.Find("XR Rig").GetComponent<MainController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (speachRecognitionScript.recognizedKeyword)
        {
            checkForSpeachResults();
            speachRecognitionScript.recognizedKeyword = false;
        }
    }

    private void checkForSpeachResults()
    {

        if (speachRecognitionScript.calibration && mainSpeachProtocolEnabled)
        {
            //type = "calibrate";
            if (!mainControllerScript.checkForMovementChanges("Speach", "calibrate"))
                Debug.LogError("Couldnt apply a calibrate order to maincontroller - SpeachController");

            speachRecognitionScript.calibration = false;
        }
        else if (speachRecognitionScript.go && mainSpeachProtocolEnabled)
        {
            //type = "go";
            if (!mainControllerScript.checkForMovementChanges("Speach", "slowStart"))
                Debug.LogError("Couldnt apply a go order to maincontroller - SpeachController");

            speachRecognitionScript.go = false;
        }
        else if (speachRecognitionScript.stop && mainSpeachProtocolEnabled)
        {
            //type = "stop";
            if (!mainControllerScript.checkForMovementChanges("Speach", "slowStop"))
                Debug.LogError("Couldnt apply a stop order to maincontroller - SpeachController");
           


            speachRecognitionScript.stop = false;
        }
        else if (speachRecognitionScript.secondStepVerification && initializationEnabled)
        {
            Debug.Log("Received finish order from user");
            initializationEnabled = false;
            speachRecognitionScript.secondStepVerification = false;
            mainControllerScript.sendSecondStepVerification();        
        }
        else 
        { 
            speachRecognitionScript.stop = false;
            speachRecognitionScript.calibration = false;
            speachRecognitionScript.go = false;
            speachRecognitionScript.secondStepVerification = false;
        }

        //false when script is activated
        if (!mainSpeachProtocolEnabled && speachRecognitionScript.go)
        {
            speachRecognitionScript.go = false;
            
        }
            

    }
}
