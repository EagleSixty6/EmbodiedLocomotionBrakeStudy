using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Threading;

public class StartupScreen : MonoBehaviour
{


#if PLATFORM_ANDROID
      
        
       
#else

   


    [DllImport("ValueBase.dll")]
    static public extern IntPtr init();

    [DllImport("ValueBase.dll")]
    static public extern void createIntValue(IntPtr pClassNameObject, string key, int value);

    [DllImport("ValueBase.dll")]
    static public extern void createRealValue(IntPtr pClassNameObject, string key, float value);

    [DllImport("ValueBase.dll")]
    static public extern void createStringValue(IntPtr pClassNameObject, string key, string value);

    [DllImport("ValueBase.dll")]
    static public extern void createBoolValue(IntPtr pClassNameObject, string key, bool value);

    [DllImport("ValueBase.dll")]
    static public extern void saveToFile(IntPtr pClassNameObject, string filename);

    [DllImport("ValueBase.dll")]
    static public extern bool loadFromFile(IntPtr pClassNameObject, string filename);

    [DllImport("ValueBase.dll")]
    static public extern void shutdown(IntPtr ptr);

    [DllImport("ValueBase.dll")]
    static public extern int toIntValue(IntPtr pClassNameObject, string filename);

    [DllImport("ValueBase.dll")]
    static public extern float toRealValue(IntPtr pClassNameObject, string filename);

    [DllImport("ValueBase.dll")]
    static public extern string toStringValue(IntPtr pClassNameObject, string filename);

    [DllImport("ValueBase.dll")]
    static public extern bool toBoolValue(IntPtr pClassNameObject, string filename);
#endif

    public RigLogging rigLogScript;
    public GameObject[] tempDisabled;
    public GameObject[] UiRayAndCanvas;
    private MainController mainControllerScript;
    public RectTransform startpanel1;
    public RectTransform startpanel2;
    public RectTransform cyclePanel;
    public RectTransform[] videoListforCyclePanel;
    public TrainingArea trainingAreaScript;
    public RectTransform warningPanel;
    public RectTransform endPanel;
    public Text endPanelTimer;
    public RectTransform debug;
    public SphereSpawner spheresScript;
    public RectTransform pausePanel;
    public RectTransform[] videoListforPausePanel;
    public GameObject endScreenDataWriting;
    public GameObject endScreenFinished;
    public RectTransform questionairePanel;

    [SerializeField]
    private Button pausePanelRecalibButton;
    [SerializeField]
    private Text pausePanelRecalibWarning;

    public RectTransform methodInfoBeforeCalPanel;
    public RectTransform methodInfoBeforeTEXT;
    public RectTransform[] videoListBeforeCalPanelInfo;
    public Button endApplicationButton;
    
    public RectTransform armAndHeadPanel;
    public AudioClip importantInfoClip;
    public Transform newRigSpherePosition;
    public GameObject cubesParentObj;
    public GameObject baelleAufgabe;
 
    public Cubes cubesScript;
    //public HeightCalibrationPanel heightCalibScript;
    public UnityEngine.Video.VideoPlayer[] videoPlayerList;
    public Transform hmd;
    public int userID;

    [SerializeField]
    public Text pathTField;

    [SerializeField]
    private Dropdown dropwownMethod;

    [SerializeField]
    private Text seedTextField;

    [SerializeField]
    private Text inforOldMethod;

    [SerializeField]
    private Text inforNewMethod;

    [SerializeField]
    private Text pauseInfoText;

    private List<Vector3> cubesTransformList = new List<Vector3>();
    private List<int> newMethodsOrder = new List<int>();
    
    private ValueBase vb = new ValueBase();
    private ValueBase singleHMDHeightVB = new ValueBase();
    public List<string> cubesLeftOrRightList;
    public List<int> percentageHeight;
    public List<int> methodsOrder;
    private float userHeight;
    private float userArmHeight;

    
    private bool constalyCheckForUserHeight = false;
    private int tryCounter = 0;
    private List<int> lvlSeedList = new List<int>();
    private string lastRestartMethodWas;
    private string actualTask;
    private int actualInput;
    private int cubeCycleCounter;
    private int sphereCycleCounter;
    string aufgabe;
    private bool iamActiveRightnow = true;
    string[] stopTypesArray;
    private IntPtr valueBase;

    //## LOGGING ##
    //Logging Timer
    private float wholeStudyTimer;
    private float timeStandingUpPRound;
    private int extraKalibrationsPRound;
    private float standUpTimer;
    private bool pauseAllowed = false;

   
    void Start()
    {
        standUpTimer = 0f;
        extraKalibrationsPRound = 0;
        mainControllerScript = this.GetComponentInParent<MainController>();
        actualTask = "all";      

        for (int i = 0; i < tempDisabled.Length; i++)
        {
            tempDisabled[i].SetActive(false);

        }

        for (int i = 1; i < 7; i++)
        {
            lvlSeedList.Add(i*1000);
        }
      
        cubeCycleCounter = 0;
        sphereCycleCounter = 0;
    }


    void Update()
    {
        if(sphereCycleCounter != 6)
            wholeStudyTimer += Time.deltaTime;

        if (iamActiveRightnow)
        {
            if (Input.GetButtonUp("Oculus_CrossPlatform_PrimaryThumbstick")&& pauseAllowed|| Input.GetButtonUp("Oculus_CrossPlatform_SecondaryThumbstick") && pauseAllowed)
            {
                spheresScript.pauseLogging = true;
                startpanel1.gameObject.SetActive(false);
                pausePanel.gameObject.SetActive(true);
                rigLogScript.active = false;
                mainControllerScript.OnRestartMovementStuff("justPause");
                videoListforPausePanel[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(true);
                videoPlayerList[newMethodsOrder[sphereCycleCounter]].Play();
              

                for (int i = 0; i < UiRayAndCanvas.Length; i++)
                {
                    UiRayAndCanvas[i].SetActive(true);

                }


                

                UiRayAndCanvas[1].transform.position = new Vector3(this.transform.position.x, UiRayAndCanvas[1].transform.position.y,
                                this.transform.position.z + 1.5f);
               
                pauseInfoText.text = "Current method was " + mainControllerScript.GetCurrentRunningMethod(newMethodsOrder[sphereCycleCounter]) + ". This" +
                  "method will be continued and the current Game: " + aufgabe + ", will be restarted or continued";

                ReceiveSphereStatus(cubeCycleCounter, false, false);
                
                if(cubeCycleCounter >=1)
                    spheresScript.CheckStatus();

                for (int i = 0; i < tempDisabled.Length; i++)
                {
                    tempDisabled[i].SetActive(false);

                }
            }

        }

        if (constalyCheckForUserHeight)
        {
            CheckIfUserStandingUp();
        }
       


    }

    //on ui button presses
    public void ContinueWithoutChanges()
    {
        for (int i = 0; i < tempDisabled.Length; i++)
        {
            tempDisabled[i].SetActive(true);

        }
        pausePanel.gameObject.SetActive(false);
        for (int i = 0; i < UiRayAndCanvas.Length; i++)
        {
            UiRayAndCanvas[i].SetActive(false);

        }
        actualInput = newMethodsOrder[sphereCycleCounter];
        rigLogScript.active = true;
        mainControllerScript.StartMethodOnly(actualInput);
        mainControllerScript.pauseTimers(true);  
        videoListforPausePanel[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(false);
        videoPlayerList[newMethodsOrder[sphereCycleCounter]].Stop();
        spheresScript.pauseLogging = false;
    }

    //button pause panel recalibrate
    public void RecalibrationIngameProcedure()
    {
        videoPlayerList[newMethodsOrder[sphereCycleCounter]].Stop();
        rigLogScript.active = false;
        rigLogScript.ResetValues();
        
        mainControllerScript.pauseTimers(true); 

        standUpTimer = 0f;
       mainControllerScript.GetAndResetBoardHoldInHandsTimer();
       mainControllerScript.GetOffZoneTime1Reset();
       mainControllerScript.GetOffZoneTime2Reset();
       spheresScript.GetNo1TimesFallenAndReset();
       spheresScript.GetNo2TimesFallenAndReset();
       spheresScript.GetTimeSphere1AndReset();
       spheresScript.GetTimeSphere2AndReset();
       
        extraKalibrationsPRound += 1;

        videoListforPausePanel[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(true);

        if (cubeCycleCounter == 1)
        {
           
            aufgabe = "Task 1 - look underneath cubes - 1 Spheres";
            DisableAllAndShowScreen("twoPause");
            lastRestartMethodWas = "twoPause";
        }
        else if (cubeCycleCounter == 0)
        {
            aufgabe = "Task 0 - look underneath cubes - 0 Spheres";
            DisableAllAndShowScreen("onePause");
            lastRestartMethodWas = "onePause";
        }
        else
        {
            aufgabe = "Task 2 - look underneath cubes - 2 Spheres";
            DisableAllAndShowScreen("threePause");
            lastRestartMethodWas = "threePause";
        }

        OnPauseResumeButtonClicked();
        spheresScript.pauseLogging = false;
    }

    public void disableMe(bool enabled)
    {
        iamActiveRightnow = enabled;
        Debug.Log("active? : "+iamActiveRightnow);
    }

    //when study has started, disable ui panels and interaction
    public void DisableAllAndShowScreen(string type)
    {
        if (!UiRayAndCanvas[1].activeSelf)
        {

            if (!type.Equals("twoPause") && !type.Equals("onePause")&& !type.Equals("threePause"))
            { 
                mainControllerScript.OnRestartMovementStuff(type);


            }
            for (int i = 0; i < tempDisabled.Length; i++)
            {
                tempDisabled[i].SetActive(false);

            }
            for (int i = 0; i < UiRayAndCanvas.Length; i++)
            {
                UiRayAndCanvas[i].SetActive(true);

            }
            

            if (type.Equals("twoPause") || type.Equals("onePause") || type.Equals("threePause"))
            {
                for (int i = 0; i < tempDisabled.Length; i++)
                {
                    tempDisabled[i].SetActive(true);

                }

                pausePanel.gameObject.SetActive(true);

                pauseInfoText.text = "Current method was " + mainControllerScript.GetCurrentRunningMethod(newMethodsOrder[sphereCycleCounter]) + ". This" +
                    "method will be continued and the current Game: " + aufgabe + ", will be restarted";


                if (type.Equals("twoPause"))
                {
                    Debug.Log("aufgabe war collect speheres");
                    mainControllerScript.OnRestartMovementStuff("twoPause");
                    newRigSpherePosition.transform.position = new Vector3(newRigSpherePosition.transform.position.x, UiRayAndCanvas[1].transform.position.y, 5 * 10 + 4);
                    UiRayAndCanvas[1].transform.position = new Vector3(newRigSpherePosition.transform.position.x, UiRayAndCanvas[1].transform.position.y,
                        newRigSpherePosition.transform.position.z );

                    spheresScript.PartialReset(1);
                    spheresScript.activated = false;

                }
                else if (type.Equals("onePause"))
                {
                    mainControllerScript.OnRestartMovementStuff("onePause");
                }
                else
                {
                    mainControllerScript.OnRestartMovementStuff("threePause");

                    newRigSpherePosition.transform.position = new Vector3(newRigSpherePosition.transform.position.x, UiRayAndCanvas[1].transform.position.y, 5 * 10 + 15 + 5 * 10 + 4);
                    UiRayAndCanvas[1].transform.position = new Vector3(newRigSpherePosition.transform.position.x, UiRayAndCanvas[1].transform.position.y,
                        newRigSpherePosition.transform.position.z );

                    spheresScript.PartialReset(2);
                    spheresScript.activated = false;
                }


                


            }

        }

    }

    //handling ui panels
    public void OnStartButtonClicked()
    {
        if (iamActiveRightnow)
        {
            armAndHeadPanel.gameObject.SetActive(true);
            startpanel1.gameObject.SetActive(false);

        }
    }

    //more changes on button press
    public void OnPresedContinueOfArmAndHead()
    {
        armAndHeadPanel.gameObject.SetActive(false);
        methodInfoBeforeCalPanel.gameObject.SetActive(true);


        if (actualTask.Equals("sphere"))
        {
            UiRayAndCanvas[2].transform.position = new Vector3(newRigSpherePosition.transform.position.x, UiRayAndCanvas[2].transform.position.y,
                newRigSpherePosition.transform.position.z + 1.5f);

            mainControllerScript.MoveRigToSecondPosition();
        }

        GenerateCubePosList(lvlSeedList[sphereCycleCounter]);

        //RandomizeMethods(); //anstatt dessem muss hier methode hin die nicht mehr ersaetzt
       
        videoListBeforeCalPanelInfo[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(true);
        videoPlayerList[newMethodsOrder[sphereCycleCounter]].Play();
        string[] stopTypesArray = new string[] { "Controller", "Speach", "GaussDoubleZone", "BackEscape", "Hoverboard", "Raycast" };
        methodInfoBeforeTEXT.gameObject.GetComponent<Text>().text = "Your first Method will be: " +
            stopTypesArray[newMethodsOrder[sphereCycleCounter]] + " this video tutorial will explain this Method to you." +
            "You will have the chance to try out everything in the training area coming next.";

        constalyCheckForUserHeight = true;


        vb.saveToFile(Application.persistentDataPath + "\\results_"+userID+".txt");
    }

    //inform training area and switch videos
    public void OnPressedContinueOfMethodInfo()
    {
        videoListBeforeCalPanelInfo[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(false);
        videoPlayerList[newMethodsOrder[sphereCycleCounter]].Stop();
        methodInfoBeforeCalPanel.gameObject.SetActive(false);
        trainingAreaScript.SetCurrentMethod(newMethodsOrder[sphereCycleCounter]);
        iamActiveRightnow = false;
        //zusaetzlich wird hier das zweite script aufgerufen !
        
    }
    
    //handling ui
    public void BackFromTraining()
    {
        startpanel2.gameObject.SetActive(true);
        videoPlayerList[6].Play();
    }

    //start of next round
    public void OnSecondStartButtonClicked()
    {
       
        if (iamActiveRightnow)
        {
            mainControllerScript.stopAudioSource();

            actualInput = newMethodsOrder[sphereCycleCounter];
            
            //mainControllerScript.StartScriptAndIntroductionProcedure(actualInput);

            for (int i = 0; i < tempDisabled.Length; i++)
            {
                tempDisabled[i].SetActive(true);

            }

            for (int i = 0; i < UiRayAndCanvas.Length; i++)
            {
                UiRayAndCanvas[i].SetActive(false);

            }

            startpanel2.gameObject.SetActive(false);
            videoPlayerList[6].Stop();
            startpanel1.gameObject.SetActive(false);

            rigLogScript.ResetValues();
            mainControllerScript.GainNoMovementTimeWithoutBrakePlusReset();
            mainControllerScript.GetAndResetGeneralRoundTimer(true);
            standUpTimer = 0f;
            mainControllerScript.GetAndResetBoardHoldInHandsTimer();
            mainControllerScript.GetOffZoneTime1Reset();
            mainControllerScript.GetOffZoneTime2Reset();
            spheresScript.GetNo1TimesFallenAndReset();
            spheresScript.GetNo2TimesFallenAndReset();
            spheresScript.GetTimeSphere1AndReset();
            spheresScript.GetTimeSphere2AndReset();

            rigLogScript.active = true;
            mainControllerScript.StartMethodOnly(actualInput);

            //baelleAufgabe.SetActive(true);
            //baelleScript.Recalibrate();
            CreateCubes();
            spheresScript.activated = true;

            pauseAllowed = true;
        }
    }


    //create positions of cubes in a list
    private void GenerateCubePosList(int seed)
    {
        List<int> tmpPercentageHeight = new List<int>(percentageHeight);
        List<string> tmpCubesLeftOrRightList = new List<string>(cubesLeftOrRightList);

        UnityEngine.Random.InitState(seed); // seed fuer die felder ist jedesmal gleich
        for (int i = 1; i < 16; i++)
        {
            int random;
            if (i != 15)
                random = UnityEngine.Random.Range(0, 15 - i);
            else
                random = 0;
        
            Vector3 vec;
            vec = new Vector3(CheckCubePlace(tmpCubesLeftOrRightList[random], seed), CheckPercentage(tmpPercentageHeight[random]), i * 10 + (int)((i-1)/5)*15);
            cubesTransformList.Add(vec);
            //Debug.Log("vier durch 5 fuer i="+i+" ist " + (int)((i- 1) / 5) * 10);

            Debug.Log("schleife: " + i + "cube list RANDOM " + tmpCubesLeftOrRightList[random]);
            tmpPercentageHeight.RemoveAt(random);
            tmpCubesLeftOrRightList.RemoveAt(random);         
        }
    }

    /*not used
    private void RandomizeMethods()
    {
        List<int> copiedMethodsOrder = new List<int>(methodsOrder);
        // seed fuer die methoden ist jedesmal anders

        UnityEngine.Random.InitState(int.Parse(seedTextField.text)); 

        for (int i = 0; i < 6; i++)
        {
            int random;

            if (i != 5)
                random = UnityEngine.Random.Range(0, 6 - i);
            else
                random = 0;

            newMethodsOrder.Add(copiedMethodsOrder[random]);
            Debug.Log("schleife " + i + "NEW methods Order " + newMethodsOrder[i]);
            copiedMethodsOrder.RemoveAt(random);
        }

    }
    */
    
   
    //instantiate the cubes
    private void CreateCubes()
    {
        for (int i = 0; i < cubesTransformList.Count; i++)
        {
            Vector3 vec = new Vector3();
            vec = cubesTransformList[i];
            Instantiate(Resources.Load("Cube") as GameObject,  vec, Quaternion.identity, cubesParentObj.transform);
            //Debug.Log("created Cube at " + vec);        
        }  
    }

    //delete cubes
    private void DeleteCubes()
    {
        foreach (Transform child in cubesParentObj.transform)
        {
            GameObject.Destroy(child.gameObject);
            
        }
    }

    //vary the desired cube position by up to 1 (same seed for all users)
    private float CheckCubePlace(string place, int seed)
    {
        UnityEngine.Random.InitState(seed);

        if (place.Equals("mid"))
        {
            return UnityEngine.Random.Range(-0.5f, 0.5f);
        }
        else if (place.Equals("right"))
        {
            return UnityEngine.Random.Range(3.5f, 4.5f);
        }
        else
            return UnityEngine.Random.Range(-3.5f, -4.5f);    
    }

    private float CheckPercentage(int percentage)
    {
        float minHeight = 0.95f;
       
        float range = (userHeight+ 0.05f) - minHeight; 

        return (((range / 100) * (percentage * 10)) + minHeight);
    }


    public void LvlReached(int number)
    {
        if (number == 0)
        {
            cubeCycleCounter += 1;
            NewSubRoundLoggingCheck(0);
        }
        else if (number == 1)
        {
            NewSubRoundLoggingCheck(1);
            cubeCycleCounter += 1;
        }
           

    }

    
    
    public void SphereLvlReached()
    {
        //NewSubRoundLoggingCheck(2);

        pauseAllowed = false;


        //if (sphereCycleCounter < 5)
        //{
           

            cyclePanel.gameObject.SetActive(true);
            questionairePanel.gameObject.SetActive(false);
            videoListforCyclePanel[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(true);
            videoPlayerList[newMethodsOrder[sphereCycleCounter]].Play();



        //}
        //else
        //{
        //    EndOfGameReached();
        //}
    }
   

    //end ui screen
    public void QuestionaireTimeReached()
    {
        NewSubRoundLoggingCheck(2);

        pauseAllowed = false;

        if (sphereCycleCounter < 5)
        {
            startpanel1.gameObject.SetActive(false);
            startpanel2.gameObject.SetActive(false);
            videoPlayerList[6].Stop();
            cyclePanel.gameObject.SetActive(false);
            questionairePanel.gameObject.SetActive(true);
           
            cubeCycleCounter = 0;
            mainControllerScript.OnRestartMovementStuff("cycle");


            spheresScript.CycleReset();
            spheresScript.activated = false;

            for (int i = 0; i < tempDisabled.Length; i++)
            {
                tempDisabled[i].SetActive(false);
            }


            for (int i = 0; i < UiRayAndCanvas.Length; i++)
            {
                UiRayAndCanvas[i].SetActive(true);

            }
            stopTypesArray = new string[] { "Controller", "Speach", "Gauss DoubleZone", "Back Escape", "Hoverboard", "Raycast" };
            inforOldMethod.text = "You have finished the method " + stopTypesArray[newMethodsOrder[sphereCycleCounter]] + ".";//mainControllerScript.GetCurrentRunningMethod(newMethodsOrder[sphereCycleCounter]) + ".";

            sphereCycleCounter += 1;
            
            baelleAufgabe.SetActive(false);
            DisableAllAndShowScreen("cycle");
            DeleteCubes();

            UiRayAndCanvas[1].transform.position = new Vector3(this.transform.position.x, UiRayAndCanvas[1].transform.position.y,
                            this.transform.position.z + 1.5f);

            //loesche infos aus bisherigen listen
            cubesTransformList.Clear(); // die hier sollte nicht leer sein deswegen cleaeren
            //mache neue cubesTransform Listen
            for (int i = 0; i < newMethodsOrder.Count; i++)
            {
                Debug.Log("MethodsListe Pos " + i + " = " + newMethodsOrder[i]);

            }
            Debug.Log("lvl seedList" + lvlSeedList[sphereCycleCounter]);
            GenerateCubePosList(lvlSeedList[sphereCycleCounter]);
            //CreateCubes();
            //baelleScript.ResetSphereTask();

            inforNewMethod.text = "Next upcoming method will be " + stopTypesArray[newMethodsOrder[sphereCycleCounter]] + ".";//mainControllerScript.GetCurrentRunningMethod(newMethodsOrder[sphereCycleCounter]) + ".";
        }
        else
        {
            for (int i = 0; i < tempDisabled.Length; i++)
            {
                tempDisabled[i].SetActive(false);
            }


            for (int i = 0; i < UiRayAndCanvas.Length; i++)
            {
                UiRayAndCanvas[i].SetActive(true);

            }

            EndOfGameReached();
        }
    }
   
    //next cycle
    public void CycleStartButtonPressed()
    {

        for (int i = 0; i < UiRayAndCanvas.Length; i++)
        {
            UiRayAndCanvas[i].SetActive(true);
        }
        actualInput = newMethodsOrder[sphereCycleCounter];
        cyclePanel.gameObject.SetActive(false);
        videoListforCyclePanel[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(false);
        videoPlayerList[newMethodsOrder[sphereCycleCounter]].Stop();
        iamActiveRightnow = false;
        
        trainingAreaScript.SetCurrentMethod(newMethodsOrder[sphereCycleCounter]);
        trainingAreaScript.InitTrainingUIandArea();
    }

    //ui
    public void OnPauseResumeButtonClicked()
    {

        videoListforPausePanel[newMethodsOrder[sphereCycleCounter]].gameObject.SetActive(false);
        videoPlayerList[newMethodsOrder[sphereCycleCounter]].Stop();
        pausePanel.gameObject.SetActive(false);

        
        actualInput = newMethodsOrder[sphereCycleCounter];
        mainControllerScript.StartScriptAndIntroductionProcedure(actualInput);

        for (int i = 0; i < tempDisabled.Length; i++)
        {
            tempDisabled[i].SetActive(true);

        }

        for (int i = 0; i < UiRayAndCanvas.Length; i++)
        {
            UiRayAndCanvas[i].SetActive(false);

        }

        //startpanel2.gameObject.SetActive(false);
        startpanel1.gameObject.SetActive(true);


    }

    public void SetUserHeight(float height)
    {
        userHeight = height;
        //speichere hoehe
        vb.createFloatValue("userheight", height);
    }

    //out of use
    public void SetUserArmHeight(float height, float headheight)
    {
        userArmHeight = height;    
    }

    //pin code - method order receiver
    public void ReceiveMethodList(List<int> methodlist, int trycount)
    {
        tryCounter = trycount;
        for (int i = 0; i < methodlist.Count; i++)
        {
            if (i == 0)
                newMethodsOrder = new List<int> { methodlist[i] -1 };
            else
                newMethodsOrder.Add(methodlist[i] -1);
            Debug.Log("adding number: " + methodlist[i]  + "to the methodlist (-1)");
        }
        Debug.Log("count"+newMethodsOrder.Count);

        OnStartButtonClicked();
       
    }

    //ui warning
    private void CheckIfUserStandingUp()
    {
        if (hmd.position.y - 0.16f > userHeight && !warningPanel.gameObject.activeSelf)
        {
            warningPanel.gameObject.SetActive(true);
            if (!mainControllerScript.trainingActive)
            {
                standUpTimer += Time.deltaTime;
                
            }
            
        }
        else if (warningPanel.gameObject.activeSelf && hmd.position.y - 0.20f < userHeight )
            warningPanel.gameObject.SetActive(false);

    }

    //end of study
    private void EndOfGameReached()
    {
        List<float> tmp = new List<float>();
        tmp = rigLogScript.GetRawHMDHeightValuesOfTheWholeGame();

        pauseAllowed = false;
        endPanel.gameObject.SetActive(true);
        //player pos
        mainControllerScript.OnRestartMovementStuff("training");
        //canvas pos
        UiRayAndCanvas[1].transform.position = new Vector3(this.transform.position.x, UiRayAndCanvas[2].transform.position.y,
                          this.transform.position.z + 1.5f);
        //++ save
        endPanelTimer.text = "Time for study: " + wholeStudyTimer;
        pathTField.text = Application.persistentDataPath;
        for (int i = 0; i < UiRayAndCanvas.Length; i++)
        {
            UiRayAndCanvas[i].SetActive(true);

        }
        for (int i = 1; i < tmp.Count; i++)
        {
            singleHMDHeightVB.createFloatValue("" + i, tmp[i]);

        }

       singleHMDHeightVB.sortSavetoFile(Application.persistentDataPath + "\\singleHMDValues_"+userID+".txt");
       singleHMDHeightVB.saveToFileAsValues(Application.persistentDataPath + "\\hmdValuesOnly_" + userID + ".txt");
       
        


        endScreenDataWriting.SetActive(false);
        endScreenFinished.SetActive(true);
        endApplicationButton.interactable = true;
        

       
       
    }



    public void SaveAIntValue(string name, int value)
    {
        vb.createIntValue(name , value);
    
    }

    //logging, bevore changes occur, depending on stage
    private void NewSubRoundLoggingCheck(int stage)
    {
        
        if (stage == 2) // double sphere
        {
            rigLogScript.active = false;

          
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time Skateboard is Hold in Hands ", mainControllerScript.GetAndResetBoardHoldInHandsTimer());
          
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Zeit OffZoneGlocke bis Stopp 1 ", mainControllerScript.GetOffZoneTime1Reset());
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Zeit OffZoneGlocke nach Stopp 2 ", mainControllerScript.GetOffZoneTime2Reset());
          
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Brake Time! ", mainControllerScript.GetAndResetBrakeTimer());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time while no movement, no brake beeing used ", mainControllerScript.GainNoMovementTimeWithoutBrakePlusReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time user is standing up (16cm above calibrated height) ", standUpTimer);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time total ", mainControllerScript.GetAndResetGeneralRoundTimer(true)); 
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Extra Calibrations ", extraKalibrationsPRound);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Average HMD Height ", rigLogScript.GetAverageHMDHeight());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", HMD Height Variance ", rigLogScript.GetHMDVariance());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Total Distance ", rigLogScript.GetDistanceTotal());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Distance Traveled Backwards ", rigLogScript.GetBackwardsDist());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Distance Traveled Sidewards ", rigLogScript.GetSidewardsDist());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + " ,Number of walked threw Cubes ", cubesScript.GetWalkedThrewNumberAndReset());
            //sphereStagesOnly
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time No1Sphere Hold in Hands ", spheresScript.GetTimeSphere1AndReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time No2Sphere Hold in Hands ", spheresScript.GetTimeSphere2AndReset());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Times No1Sphere was Released ", spheresScript.GetNo1TimesFallenAndReset());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Times No2Sphere was Released ", spheresScript.GetNo2TimesFallenAndReset());

            

            List<float> tmpDistances = cubesScript.GetDistances();
            float averageDist = 0;
            int tmpcount = 0;
            //varianz temps
            float numberMinusOne = tmpDistances.Count - 1;
            float varianz = 0f;
            foreach (float dist in tmpDistances)
            {
                averageDist += dist;
                Debug.Log("dist: " + dist);
                tmpcount += 1;
                //varianz += (float)Math.Exp(dist) - numberMinusOne;
                //Debug.Log("varianz in schleife: " + varianz);
            }
            averageDist = averageDist / tmpcount;
            //empriische varianz
            foreach (float dist in tmpDistances)
            {
                float tmp2 = dist - averageDist;
                varianz += tmp2 * tmp2;

            }
            varianz = ((1 * varianz) / (tmpcount - 1));
            Debug.Log("varianz: " + varianz);
            float standardabweichung;
            standardabweichung = Mathf.Sqrt(varianz);
            Debug.Log("average real: " + averageDist);
            Debug.Log("standardabweichung: " + standardabweichung);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Succes Rate ", (float)cubesScript.GetNumberofActivatedCubesAndReset() / 5f);
            vb.createStringValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Sequence of activated Cubes ", cubesScript.GetStartedCubesStringAndReset());
            vb.createStringValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Sequence of walked threw Cubes ", cubesScript.GetWalkedThrewCubesStringAndReset());

            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Average distance to Cube when Activated ", averageDist);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Varianz, Dist. Cube when Activated n-1 ", varianz);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Standardabweichung , Dist. Cube when Activated n-1 ", standardabweichung);

            rigLogScript.ResetValues();
            List<float> tmpl = new List<float>();
            tmpl = rigLogScript.GetRawHMDHeightValuesOfTheWholeGame();
            
            vb.createIntValue("Round " + sphereCycleCounter + " hat: " + tmpl.Count + " viele Werte", tmpl.Count);
           

            //save
            vb.saveToFile(Application.persistentDataPath + "\\results_"+userID+".txt");
            vb.saveToFileAsValues(Application.persistentDataPath + "\\resultsValuesOnly_"+userID+".txt");


            
            //reset
            rigLogScript.ResetValues();
            cubesScript.ClearDistances();
            extraKalibrationsPRound = 0;
            standUpTimer = 0f;
            spheresScript.pauseLogging = false;

           
        }
        else if (stage == 1)// single sphere
        {
            rigLogScript.active = false;

          
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time Skateboard is Hold in Hands ", mainControllerScript.GetAndResetBoardHoldInHandsTimer());
         
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Zeit OffZoneGlocke bis Stopp 1 ", mainControllerScript.GetOffZoneTime1Reset());
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Zeit OffZoneGlocke nach Stopp 2 ", mainControllerScript.GetOffZoneTime2Reset());
   
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Brake Time! ", mainControllerScript.GetAndResetBrakeTimer());
           
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time while no movement, no brake beeing used: ", mainControllerScript.GainNoMovementTimeWithoutBrakePlusReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time user is standing up (16cm above calibrated height) ", standUpTimer);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time total ", mainControllerScript.GetAndResetGeneralRoundTimer(false)); // gebe mit ob der timer bis zum naechsten kalibrierung resettet werden soll
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Extra Calibrations ", extraKalibrationsPRound);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Average HMD Height ", rigLogScript.GetAverageHMDHeight());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", HMD Height Variance ", rigLogScript.GetHMDVariance());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Total Distance ", rigLogScript.GetDistanceTotal());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Distance Traveled Backwards ", rigLogScript.GetBackwardsDist());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Distance Traveled Sidewards " +"", rigLogScript.GetSidewardsDist());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Number of walked threw Cubes ", cubesScript.GetWalkedThrewNumberAndReset());
            //sphere stages only
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time No1Sphere Hold in Hands ", spheresScript.GetTimeSphere1AndReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time No2Sphere Hold in Hands ", spheresScript.GetTimeSphere2AndReset());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Times No1Sphere was Released ", spheresScript.GetNo1TimesFallenAndReset());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Times No2Sphere was Released ", spheresScript.GetNo2TimesFallenAndReset());
           

            List<float> tmpDistances = cubesScript.GetDistances();
            float averageDist = 0;
            int tmpcount = 0;
            //varianz temps
            float numberMinusOne = tmpDistances.Count - 1;
            float varianz = 0f;
            foreach (float dist in tmpDistances)
            {
                averageDist += dist;
                Debug.Log("dist: " + dist);
                tmpcount += 1;
               
            }
            averageDist = averageDist / tmpcount;
            //empriische varianz
            foreach (float dist in tmpDistances)
            {
                float tmp2 = dist - averageDist;
                varianz += tmp2 * tmp2;

            }
            varianz = ((1 * varianz) / (tmpcount - 1));
            Debug.Log("varianz: " + varianz);
            float standardabweichung;
            standardabweichung = Mathf.Sqrt(varianz);
            Debug.Log("average real: " + averageDist);
            Debug.Log("standardabweichung: " + standardabweichung);

            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Succes Rate ", (float)cubesScript.GetNumberofActivatedCubesAndReset() / 5f);
            vb.createStringValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Sequence of activated Cubes ", cubesScript.GetStartedCubesStringAndReset());
            vb.createStringValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Sequence of walked threw Cubes ", cubesScript.GetWalkedThrewCubesStringAndReset());

            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Average distance to Cube when Activated ", averageDist);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Varianz, Dist. Cube when Activated n-1 ", varianz);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Standardabweichung , Dist. Cube when Activated n-1 ", standardabweichung);

            //reset
            rigLogScript.ResetValues();
            cubesScript.ClearDistances();  
            extraKalibrationsPRound = 0;
            standUpTimer = 0f;
            //rig log on
            rigLogScript.active = true;
            spheresScript.pauseLogging = false;

        }
        else if (stage == 0) // cubes
        {
            rigLogScript.active = false;

           
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + " , Time Skateboard is Hold in Hands ", mainControllerScript.GetAndResetBoardHoldInHandsTimer());
         
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Zeit OffZoneGlocke bis Stopp 1  ", mainControllerScript.GetOffZoneTime1Reset());
                vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Zeit OffZoneGlocke nach Stopp 2  ", mainControllerScript.GetOffZoneTime2Reset());
           
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Number of walked threw Cubes ", cubesScript.GetWalkedThrewNumberAndReset());
            vb.createFloatValue("Round: " + sphereCycleCounter +" stage: " + stage + ", Succes Rate ", (float)cubesScript.GetNumberofActivatedCubesAndReset()/5f);
            vb.createStringValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Sequence of activated Cubes ", cubesScript.GetStartedCubesStringAndReset());
            vb.createStringValue("Round: " + sphereCycleCounter +" stage: " + stage + ", Sequence of walked threw Cubes ", cubesScript.GetWalkedThrewCubesStringAndReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Brake Time! ", mainControllerScript.GetAndResetBrakeTimer());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time while no movement, no brake beeing used ", mainControllerScript.GainNoMovementTimeWithoutBrakePlusReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time user is standing up (16cm above calibrated height) ", standUpTimer);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time: ", mainControllerScript.GetAndResetGeneralRoundTimer(false)); // gebe mit ob de
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Extra Calibrations ", extraKalibrationsPRound);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Average HMD Height ", rigLogScript.GetAverageHMDHeight());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", HMD Height Variance ", rigLogScript.GetHMDVariance());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Total Distance ", rigLogScript.GetDistanceTotal());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Distance Traveled Backwards ", rigLogScript.GetBackwardsDist());

           
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time No1Sphere Hold in Hands ", spheresScript.GetTimeSphere1AndReset());
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Time No2Sphere Hold in Hands ", spheresScript.GetTimeSphere2AndReset());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Times No1Sphere was Released ", spheresScript.GetNo1TimesFallenAndReset());
            vb.createIntValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Times No2Sphere was Released ", spheresScript.GetNo2TimesFallenAndReset());


            float tmpValueofCubes = 0f; 
            //cube dist
            foreach (Transform child in cubesParentObj.transform)
            {
                if (child.gameObject.transform.position.x < 0)
                {                 
                    tmpValueofCubes += 0 - child.gameObject.transform.position.x;         
                }
                else
                {
                    tmpValueofCubes += child.gameObject.transform.position.x;
                } 
            }
        
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Distance Traveled Sidewards Raw ", rigLogScript.GetSidewardsDist());


            List<float> tmpDistances = cubesScript.GetDistances();
            float averageDist = 0;
            int tmpcount = 0;
            //varianz temps
            float numberMinusOne = tmpDistances.Count - 1;
            float varianz = 0f;
            foreach (float dist in tmpDistances)
            {
                averageDist += dist;
                Debug.Log("dist: " + dist);
                tmpcount += 1;
              
            }
            averageDist = averageDist / tmpcount;
            //empriische varianz
            foreach (float dist in tmpDistances)
            {
                float tmp2 = dist - averageDist;
                varianz += tmp2*tmp2;
                
            }
            varianz = ((1 * varianz) / (tmpcount - 1)) ;
            Debug.Log("varianz: " + varianz);
            float standardabweichung;
            standardabweichung = Mathf.Sqrt(varianz);
            Debug.Log("average real: " + averageDist);
            Debug.Log("standardabweichung: " + standardabweichung);

            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Average distance to Cube when Activated ", averageDist);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Varianz, Dist. Cube when Activated n-1 ", varianz);
            vb.createFloatValue("Round: " + sphereCycleCounter + " stage: " + stage + ", Standardabweichung , Dist. Cube when Activated n-1 ", standardabweichung);

            //reset
            standUpTimer = 0f;
            cubesScript.ClearDistances();
            rigLogScript.ResetValues();
            extraKalibrationsPRound = 0;

            rigLogScript.active = true;


            //test
            List<float> tmp = new List<float>();
            tmp = rigLogScript.GetRawHMDHeightValuesOfTheWholeGame();

            spheresScript.pauseLogging = false;



        }

       

    }

    //check if spheres released for displaying ui
    public void ReceiveSphereStatus(int count, bool no1, bool no2)
    {
        if (count == 0)
        {
            pausePanelRecalibButton.interactable = true;
            pausePanelRecalibWarning.gameObject.SetActive(false);

        }
        else if (count == 1 && no1)
        {

            pausePanelRecalibButton.interactable = true;
            pausePanelRecalibWarning.gameObject.SetActive(false);

        }
        else if (count == 2 && no1 && no2)
        {
            pausePanelRecalibButton.interactable = true;
            pausePanelRecalibWarning.gameObject.SetActive(false);
        }
        else
        {
            pausePanelRecalibButton.interactable = false;
            pausePanelRecalibWarning.gameObject.SetActive(true);
        }
    
    }


    public void EndApplication()
    {
        Application.Quit();
    
    }

   
}
