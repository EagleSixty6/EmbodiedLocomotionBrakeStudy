using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingArea : MonoBehaviour
{
    public RectTransform firstPanel;
    public RectTransform InfoPanel;
    public RectTransform calibrationPanel;
    public RectTransform pausePanel;
    public RectTransform ingameMethodRepresentation;
    public RectTransform jumpBackPoint;
    public RectTransform firstPausePanel;

    public GameObject[] controllerRays;
    public GameObject mainLvl;
    public GameObject[] forAndAfterCalibration;
    public RectTransform[] disabledtillMainMenu;
    public Text[] listOfVisualMethodTexts;
    public RectTransform[] videoList;
    public MainController mainControllerScript;
    public Toggle[] toggleList;
    public UnityEngine.Video.VideoPlayer[] videoPlayerList;
    public StartupScreen startScript;
    public RectTransform[] videoListforPausePanel;


    private int currentMethod;
    private string[] stopTypesArray;
    private Color blue;
    private Color black;
    private float joystickTimer;
    private bool unlockButtonTrainingButton;
    private int lastVideo;
    private bool uiButton;
    private bool iamActive;
   
    void Start()
    {
        stopTypesArray = new string[] { "Controller", "Speach", "OffZoneSimple", "OffZoneBack", "Skateboard", "Raycast" };
        currentMethod = 0;
        blue = new Color(0, 0, 255);
        black = new Color(0,0,0);
        joystickTimer = -0.1f;
        unlockButtonTrainingButton = false;
        iamActive = false;

    }

   
    void Update()
    {
        if (joystickTimer > 0 && joystickTimer > -0.1)
            joystickTimer = joystickTimer - Time.deltaTime;

       
       
        if (!uiButton && unlockButtonTrainingButton)
        {

            if (Input.GetButtonUp("Oculus_CrossPlatform_SecondaryThumbstick") || Input.GetButtonUp("Oculus_CrossPlatform_PrimaryThumbstick"))
            {
                mainControllerScript.GainNoMovementTimeWithoutBrakePlusReset();
                forAndAfterCalibration[0].SetActive(false);
                ingameMethodRepresentation.gameObject.SetActive(false);
                foreach (GameObject obj in controllerRays)
                {
                    obj.SetActive(true);
                }

                mainControllerScript.OnRestartMovementStuff("training");
                //pausePanel.gameObject.SetActive(true);
                firstPausePanel.gameObject.SetActive(true);
                //videoPlayerList[7].Play();
                videoListforPausePanel[currentMethod].gameObject.SetActive(true);
                videoPlayerList[currentMethod].Play();
            }
        }
    }

    //following:ui and button inputs
    public void InitTrainingUIandArea()
    {
        foreach (GameObject obj in controllerRays)
        {
            obj.SetActive(true);
        }
       
        firstPanel.gameObject.SetActive(false);
        //InfoPanel.gameObject.SetActive(true);
      
        calibrationPanel.gameObject.SetActive(true);
        videoPlayerList[6].Play();
        foreach (RectTransform rtrans in disabledtillMainMenu)
        {
            rtrans.transform.gameObject.SetActive(false);
        }
        
        uiButton = true;

        iamActive = true;
        mainControllerScript.SetTraining(true);
        mainLvl.SetActive(false);
    }

    public void StartTaskIntro()
    {
       
        InfoPanel.gameObject.SetActive(false);
        calibrationPanel.gameObject.SetActive(true);
        videoPlayerList[currentMethod].Stop();
        videoListforPausePanel[currentMethod].gameObject.SetActive(false);
        videoPlayerList[6].Play();
       
        
    }

    public void SetCurrentMethod(int method)
    {
        currentMethod = method;
    
    }

    public void StartSzene()
    {
        if(iamActive)
        {
            videoPlayerList[currentMethod].Stop();
            videoListforPausePanel[currentMethod].gameObject.SetActive(false);
            firstPausePanel.gameObject.SetActive(false);
            videoPlayerList[7].Stop();
            //calibrationPanel.gameObject.SetActive(true);
            pausePanel.gameObject.SetActive(false);
            calibrationPanel.gameObject.SetActive(false);
            videoPlayerList[6].Stop();
            foreach (GameObject obj in forAndAfterCalibration)
            {
                obj.SetActive(true);
                Debug.Log("training environment an");
            }

            mainControllerScript.stopAudioSource();
            mainControllerScript.StartScriptAndIntroductionProcedure(currentMethod); // starte immer mit 0 denn es ist egal :)
            AdjustVisualTextRepresentation(currentMethod);

            uiButton = false;

            foreach (GameObject obj in controllerRays)
            {
                obj.SetActive(false);
            }
        }
    }

    public void CalibrationFinished()
    {     
        unlockButtonTrainingButton = true;      
    }

    public void ContinueToTaskOverview()
    {
        videoPlayerList[currentMethod].Stop();
        videoListforPausePanel[currentMethod].gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
        firstPausePanel.gameObject.SetActive(false);
        videoPlayerList[7].Play();

    }

    public void BacktoMainMenu()
    {
        //enable panel und counter panel
        //firstPanel.gameObject.SetActive(true);
        firstPanel.gameObject.SetActive(false);
        videoPlayerList[7].Stop();
        //calibrationPanel.gameObject.SetActive(true);
        foreach (RectTransform rtrans in disabledtillMainMenu)
        {
            rtrans.gameObject.SetActive(false);     // evt wieder anschalten 
        }
        foreach (GameObject obj in forAndAfterCalibration)
        {
            obj.SetActive(false);
            Debug.Log("training environment aus");
        }
        
        
        ingameMethodRepresentation.gameObject.SetActive(false);
       
        pausePanel.gameObject.SetActive(false);
        unlockButtonTrainingButton = false;
        iamActive = false;
        mainControllerScript.SetTraining(false);

        startScript.OnSecondStartButtonClicked();
    }


    public void AdjustVisualTextRepresentation(int method)
    {
        listOfVisualMethodTexts[currentMethod].GetComponent<Text>().color = black;
        
        currentMethod = method;
        listOfVisualMethodTexts[currentMethod].GetComponent<Text>().color = blue;

        if (method == 1)
        { 
            //aktiviere reminder panel auf dem steht was zu tun ist in der Methode fuer 10 sek
        }
    }




    public void OnToggleChanged(int toggleNumberInList)
    {
        if (toggleList[toggleNumberInList].isOn)
        {
            videoList[lastVideo].transform.gameObject.SetActive(false);
            videoPlayerList[lastVideo].Stop();
            videoList[toggleNumberInList].transform.gameObject.SetActive(true);
            videoPlayerList[toggleNumberInList].Play();
            lastVideo = toggleNumberInList;
        }
        
    }
}
