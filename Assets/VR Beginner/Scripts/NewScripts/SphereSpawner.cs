using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SphereSpawner : MonoBehaviour
{

    public Transform camera;
    public StartupScreen startScript;
    public GameObject SphereNo1;
    public GameObject SphereNo2;
    public GameObject Arrow;
    public RectTransform panel;
    public Text respawnInfo;
    public Slider slider;
    public Slider slider2;
    public GameObject goalObj;
    public GameObject particle1;
    public GameObject particle2;
    public bool activated;
    public GameObject parent;
    public GameObject visualSignalN1;
    public GameObject visualSignalN2;
    public GameObject visualSignalN3;
    public Material redMaterial;
    public Material greenMaterial;
    public Material finishMaterial;
    public Material bevoreStepOn;
    public bool pauseLogging;
    private XRGrabInteractable _grabInteractible;
    
    

    private float distanceTillSpherePart1;
    private float distanceTillSpherePart2;
    private float distanceToGoal;
    private Vector3 goal;

    bool leftHandWasGrabbed;
    bool rightHandWasGrabbed;
    float no1Timer;
    float no2Timer;
    bool startNo1Timer;
    bool startNo2Timer;
    int mySeed;
    int myCycleCounter;
    float specialBetterFeelingTimer;
    bool startFeelTimer = false;
    static int numberOfCycles = 2;
    int tmpCounterForTwoBalls = 0;
    bool startSphereCheck = false;
    bool sphere1Spawned = false;
    bool sphere2Spawned = false;
    float panelTimer = 0f;
    bool startPanelTimer = false;
    bool goalActive;
    bool notifyViewScript;
    bool enableLVL2 = false;
    bool enableLVl3 = false;
    //logging
    float sphere1PickedUp;
    int timesSphere1Lost;
    bool enableNo1PickUpCounter;

    float sphere2PickedUp;
    int timesSphere2Lost;
    bool enableNo2PickUpCounter;

    private static int timeForRespawn = 2;

    void Start()
    {
        pauseLogging = false;
        distanceTillSpherePart1 = 5*10 +5; // 5m hinter der 5ten kugel
        SphereNo1.transform.position = new Vector3(-0.5f, 1, distanceTillSpherePart1 + 2.5f);
        particle1.transform.position = new Vector3(-0.5f, 0, distanceTillSpherePart1 + 2.5f);
        distanceTillSpherePart2 = 5 * 10 + 15 + 5 * 10 + 5; // 5m hinter der 10ten kugel
        SphereNo2.transform.position = new Vector3(0.5f, 1, distanceTillSpherePart2 + 2.5f);
        particle2.transform.position = new Vector3(0.5f, 0, distanceTillSpherePart2 + 2.5f);
        SphereNo1.SetActive(false);
        SphereNo2.SetActive(false);
        notifyViewScript = false;

        distanceToGoal = 185; // = 50+15+50+15+50+5 = 150+30+5 = 185
        goal = new Vector3(0f, 0f, distanceToGoal);
        goalObj.transform.position = goal;
        goalObj.SetActive(false);
        goalActive = false;
        no1Timer = 0f;
        no2Timer = 0f;
        myCycleCounter = 0;

        activated = false;

        sphere1PickedUp = 0f;
        timesSphere1Lost = 0;
        enableNo1PickUpCounter = false;

        sphere2PickedUp = 0f;
        timesSphere2Lost = 0;
        enableNo2PickUpCounter = false;

    }

 
    void Update()
    {
        if (activated)
        {
            CountUpTimers();

            CheckPosition();
        }
    }

    //timers when sphere was released
    private void CountUpTimers()
    {
        if (startNo1Timer )
        {
            no1Timer += Time.deltaTime;
        }

        if (startNo2Timer)
        {     
            no2Timer += Time.deltaTime;
        }

        if (enableNo1PickUpCounter && !pauseLogging)
        {
            sphere1PickedUp += Time.deltaTime;
        }


        if (enableNo2PickUpCounter && !pauseLogging)
        {
            sphere2PickedUp += Time.deltaTime;         
        }
           

        if (SphereNo1.transform.position.y < 0 && !startNo1Timer)
            startNo1Timer = true;

        if (SphereNo2.transform.position.y < 0 && !startNo2Timer)
            startNo2Timer = true;

        if (startPanelTimer)
        {
            panelTimer += Time.deltaTime;

            if (myCycleCounter == 1)
            {
                slider.value = no1Timer / timeForRespawn;
            }
            else if (myCycleCounter == 2)
            {
                if (startNo1Timer && startNo2Timer)
                {
                    slider.value = no1Timer / timeForRespawn;
                    slider2.gameObject.SetActive(true);
                    slider2.value = no2Timer / timeForRespawn;
                }
                else if (startNo1Timer && !startNo2Timer)
                {
                    slider2.gameObject.SetActive(false);
                    slider.value = no1Timer / timeForRespawn;
                }
                else if (startNo2Timer && !startNo1Timer)
                {
                    slider2.gameObject.SetActive(false);
                    slider.value = no2Timer / timeForRespawn;
                }
                   

            }
         

            if (slider.value < 1 && panelTimer <= timeForRespawn && !goalActive)
                respawnInfo.text = "Respawning Sphere...";

        }
            

    
    }

    //check player position
    private void CheckPosition()
    {

        if (Vector2.Distance(new Vector2(camera.position.x, camera.position.z), new Vector2(SphereNo1.transform.position.x, SphereNo1.transform.position.z)) < 20 && myCycleCounter == 0 && !particle1.activeInHierarchy)         
        {
            particle1.SetActive(true);
            SphereNo1.SetActive(true);
        }

        //temporaere activierung Paricle1
        if (Vector2.Distance(new Vector2(camera.position.x, camera.position.z), new Vector2(SphereNo1.transform.position.x, SphereNo1.transform.position.z)) < 12 &&
            myCycleCounter >= 1 && !particle1.activeInHierarchy && sphere1Spawned)
        {
            particle1.transform.position = new Vector3(SphereNo1.transform.position.x, 0, SphereNo1.transform.position.z);
            particle1.SetActive(true);
            
        }


        if ((myCycleCounter == 1 && Vector2.Distance(new Vector2(camera.position.x, camera.position.z), new Vector2(SphereNo2.transform.position.x, SphereNo2.transform.position.z)) < 20 && !particle2.activeInHierarchy))
        {
            particle2.SetActive(true);
            SphereNo2.SetActive(true);
        }    

        //temporaere activierung Paricle2
        if (Vector2.Distance(new Vector2(camera.position.x, camera.position.z), new Vector2(SphereNo2.transform.position.x, SphereNo2.transform.position.z)) < 12 &&
           myCycleCounter == 2 && !particle2.activeInHierarchy && sphere2Spawned)
        {
            particle2.transform.position = new Vector3(SphereNo2.transform.position.x, 0, SphereNo2.transform.position.z);
            particle2.SetActive(true);
            
        }
     



        if (Vector2.Distance(new Vector2(camera.position.x, camera.position.z), new Vector2(SphereNo1.transform.position.x, SphereNo1.transform.position.z)) < 9 && myCycleCounter == 0)
        {
            if (enableLVL2)
            {
                enableLVL2 = false;
                //particle1.SetActive(false);
                myCycleCounter = 1;
                visualSignalN1.GetComponent<MeshRenderer>().material = greenMaterial;
                SphereNo1.GetComponent<Rigidbody>().useGravity = false;
                startScript.LvlReached(0);// aktiviere naechsten eintrag in die Loggin datei
                
            }
        }
        
        if (myCycleCounter == 1 && Vector2.Distance(new Vector2(camera.position.x, camera.position.z), new Vector2(SphereNo2.transform.position.x, SphereNo2.transform.position.z)) < 9)
        {
            if (enableLVl3)
            {
                enableLVl3 = false;
                //particle2.SetActive(false);
                myCycleCounter = 2;
                visualSignalN2.GetComponent<MeshRenderer>().material = greenMaterial;
                SphereNo2.GetComponent<Rigidbody>().useGravity = false;
                startScript.LvlReached(1);
            }
           
        }
        else if((myCycleCounter == 2 && camera.position.z > goalObj.transform.position.z))
        {
            goalObj.SetActive(true);
            goalActive = true;
            panel.gameObject.SetActive(true);

            if (rightHandWasGrabbed || leftHandWasGrabbed)
            {
                if (panel.gameObject.activeInHierarchy)
                {
                    startPanelTimer = false;
                    panelTimer = 0f;
                    respawnInfo.text = "Drop Spheres to End the Study Phase !";
                    visualSignalN3.GetComponent<MeshRenderer>().material = finishMaterial;
                    slider.value = 0f;
                }
                else
                {
                    panel.gameObject.SetActive(true);
                    slider.gameObject.SetActive(false);
                    startPanelTimer = false;
                    panelTimer = 0f;
                    slider.value = 0f;
                }

            }
            else if (!sphere1Spawned || !sphere2Spawned)
            {
                if (!startPanelTimer)
                    startPanelTimer = true;
                respawnInfo.text = "Counting Spheres .... ";
                
               visualSignalN3.GetComponent<MeshRenderer>().material = greenMaterial;
            }
            else if ((myCycleCounter == 2 && camera.position.z > goalObj.transform.position.z && sphere1Spawned && sphere2Spawned))
            {
               
                panelTimer = 0f;
                startPanelTimer = false;
                panel.gameObject.SetActive(false);
                activated = false;
                goalActive = false;
                //startScript.SphereLvlReached();
                startScript.QuestionaireTimeReached();
            }
        }

        if (myCycleCounter >= 1  && no1Timer > timeForRespawn)
        {
            RespawnSphereNextToPlayerPos(1);
        }
        else if (myCycleCounter == 2  && no2Timer > timeForRespawn)
        {
            RespawnSphereNextToPlayerPos(2);
        }

        if (panelTimer > 6)
        {
            startPanelTimer = false;
            panel.gameObject.SetActive(false);
            panelTimer = 0f;
        }

        

    }

 
    //respawn sphere next to player
    private void RespawnSphereNextToPlayerPos(int number)
    {
        if (number >=1)
        {
            if (camera.position.x < 0)
            {
                //Arrow.SetActive(true);
                if (number == 1)
                {
                    sphere1Spawned = true;
                    no1Timer = 0f;
                    startNo1Timer = false;

                    SphereNo1.GetComponent<Rigidbody>().useGravity = false;
                    SphereNo1.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    SphereNo1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    SphereNo1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    Collider m_collider = SphereNo1.GetComponent<SphereCollider>();
                    m_collider.enabled = !m_collider.enabled;

                    SphereNo1.transform.position = new Vector3(camera.position.x + 5, 1, camera.position.z);
                    m_collider.enabled = !m_collider.enabled;
                    SphereNo1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                }

                if(myCycleCounter == 2 && sphere2Spawned && sphere1Spawned|| myCycleCounter == 1 && sphere1Spawned)
                    startScript.ReceiveSphereStatus(myCycleCounter, sphere1Spawned, sphere2Spawned);
            
                if (!goalActive)
                    respawnInfo.text = "Respawned Right";
                else
                    respawnInfo.text = "Finished !";
            }
            else
            {
                //Arrow.SetActive(true);
                if (number == 1)
                {
                    sphere1Spawned = true;
                    no1Timer = 0f;
                    startNo1Timer = false;

                    SphereNo1.GetComponent<Rigidbody>().useGravity = false;
                    SphereNo1.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    SphereNo1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    SphereNo1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    Collider m_collider = SphereNo1.GetComponent<SphereCollider>();
                    m_collider.enabled = !m_collider.enabled;

                    SphereNo1.transform.position = new Vector3(camera.position.x - 5, 1, camera.position.z);
                    m_collider.enabled = !m_collider.enabled;
                    SphereNo1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;


                    if (myCycleCounter == 2 && sphere2Spawned && sphere1Spawned || myCycleCounter == 1 && sphere1Spawned)
                        startScript.ReceiveSphereStatus(myCycleCounter, sphere1Spawned, sphere2Spawned);
                    


                }

                if (!goalActive)
                    respawnInfo.text = "Respawned Left";
                else
                    respawnInfo.text = "Finished !";


            }

            if (number == 2)
            {
                sphere2Spawned = true;
                no2Timer = 0f;
                startNo2Timer = false;

                SphereNo2.GetComponent<Rigidbody>().useGravity = false;
                SphereNo2.GetComponent<Rigidbody>().velocity = Vector3.zero;
                SphereNo2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                SphereNo2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Collider m_collider = SphereNo2.GetComponent<SphereCollider>();
                m_collider.enabled = !m_collider.enabled;

                if (camera.position.x < 0)
                {
                    SphereNo2.transform.position = new Vector3(camera.position.x + 4, 1, camera.position.z);
                    m_collider.enabled = !m_collider.enabled;
                    SphereNo2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
                else
                {
                    SphereNo2.transform.position = new Vector3(camera.position.x - 6, 1, camera.position.z);
                    m_collider.enabled = !m_collider.enabled;
                    SphereNo2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }

                if (myCycleCounter == 2 && sphere2Spawned && sphere1Spawned || myCycleCounter == 1 && sphere1Spawned)
                    startScript.ReceiveSphereStatus(myCycleCounter, sphere1Spawned, sphere2Spawned);
            }
                
        }
        
        
    }

    //procedure on a grabbed ball, i.e, block hand for certain stop methods
    public void OnBallGrabbed(string hand, int no)
    {
        Arrow.SetActive(false);


        if (hand.Equals("right"))
        {
            Debug.Log("right hand grabbed");
            rightHandWasGrabbed = true;
        }

        if (hand.Equals("left")) // changed from else if
        {
            Debug.Log("left hand grabbed");
            leftHandWasGrabbed = true;
        }
    
        if (no == 1)// &&  sphere1Spawned
        {
            if (myCycleCounter == 0)
                enableLVL2 = true;//tempausloeser fuer level2
           
            sphere1Spawned = false;
            startNo1Timer = false;
            no1Timer = 0f;
            Debug.Log("no 1 grabbed");
            enableNo1PickUpCounter = true;

            if (myCycleCounter == 1)
            {
                if (panel.gameObject.activeInHierarchy)
                {
                    panelTimer = 0f;
                    panel.gameObject.SetActive(false);
                    startPanelTimer = false;
                }
            }
            else if (myCycleCounter == 2 && slider.value >= 0 )
            {
                if (panel.gameObject.activeInHierarchy && slider2.value == 0)
                {
                    panelTimer = 0f;
                    panel.gameObject.SetActive(false);
                    startPanelTimer = false;
                }

            }
        }
        else if (no == 2 )// &&  sphere2Spawned
        {
            if (myCycleCounter == 1)
                enableLVl3 = true;

            sphere2Spawned = false;
            startNo2Timer = false;
            no2Timer = 0f;
            Debug.Log("no 2 grabbed");
            enableNo2PickUpCounter = true;

            if (myCycleCounter == 2 && slider.value == 0)
            {
                if (panel.gameObject.activeInHierarchy)
                {
                    panelTimer = 0f;
                    panel.gameObject.SetActive(false);
                    startPanelTimer = false;
                }

            }
        }



    }

    public void HandReleased(string hand, int no, bool realRelease)
    {
        if (hand.Equals("right"))
        {
            rightHandWasGrabbed = false;

            if (myCycleCounter >= 1 && realRelease)
            {
                if (no == 1)
                {
                    startNo1Timer = true;
                    Debug.Log("reset Sph 1");
                    enableNo1PickUpCounter = false;
                    timesSphere1Lost += 1;
                }
                else
                { 
                    startNo2Timer = true;
                    Debug.Log("reset Sph 2");
                    enableNo2PickUpCounter = false;
                    timesSphere2Lost += 1;
                }
                    

                if (panel.gameObject.activeInHierarchy)
                {
                    panelTimer = 0f;
                }
                else
                {
                    panel.gameObject.SetActive(true);
                    startPanelTimer = true;
                }
            }
            else
                Debug.LogWarning("Received Release from right Hand but Phase 1 or 2 not activated yet");
            Debug.Log("right hand released");
        }
        else if (hand.Equals("left"))
        {

            leftHandWasGrabbed = false;

            if (myCycleCounter >= 1 && realRelease)
            {
                if (no == 1)
                {
                    startNo1Timer = true;
                    Debug.Log("reset Sph 1");
                    enableNo1PickUpCounter = false;
                    timesSphere1Lost += 1;
                }
                else
                {
                    enableNo2PickUpCounter = false;
                    startNo2Timer = true;
                    Debug.Log("reset Sph 2");
                    timesSphere2Lost += 1;
                }

                if (panel.gameObject.activeInHierarchy)
                {
                    panelTimer = 0f;
                }
                else
                {
                    panel.gameObject.SetActive(true);
                    startPanelTimer = true;
                }
              
            }
            else
                Debug.LogWarning("Received Release from left Hand but Phase 1 or 2 not activated yet");

            Debug.Log("left hand released");
        }


    }

  

    public void PartialReset(int num) 
    {
        myCycleCounter = num;
    }

    public void RealReset()
    {
        if (myCycleCounter == 1)
        {
            startNo1Timer = false;
            startNo2Timer = false;

           

            no1Timer = 0f;
            sphere1Spawned = false;
            no2Timer = 0f;
            sphere2Spawned = false;
            //RespawnSphereNextToPlayerPos(1);

            particle1.SetActive(false);
            particle2.SetActive(false);

          
        }
        else if (myCycleCounter == 2)
        {
           
        }

       
    
    }

    //replace spheres
    public void CycleReset()
    {
        

        distanceTillSpherePart1 = 5 * 10 + 5; // 5m hinter der 5ten kugel
        
        particle1.transform.position = new Vector3(-0.5f, 0, distanceTillSpherePart1 + 2.5f);
        
        distanceTillSpherePart2 = 5 * 10 + 15 + 5 * 10 + 5; // 5m hinter der 10ten kugel  
        particle2.transform.position = new Vector3(0.5f, 0, distanceTillSpherePart2 + 2.5f);

     
        
        SphereNo2.GetComponent<Rigidbody>().useGravity = false;
        SphereNo2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        SphereNo2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        SphereNo2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //Collider m_collider = SphereNo2.GetComponent<SphereCollider>();
        //m_collider.enabled = !m_collider.enabled;

        SphereNo2.transform.position = new Vector3(0.5f, 1, distanceTillSpherePart2 + 2.5f);
        //m_collider.enabled = !m_collider.enabled;
        SphereNo2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        SphereNo1.GetComponent<Rigidbody>().useGravity = false;
        SphereNo1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        SphereNo1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        SphereNo1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //Collider m_collider2 = SphereNo1.GetComponent<SphereCollider>();
        //m_collider2.enabled = !m_collider.enabled;

        SphereNo1.transform.position = new Vector3(-0.5f, 1, distanceTillSpherePart1 + 2.5f);
        //m_collider2.enabled = !m_collider.enabled;
        SphereNo1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        SphereNo1.SetActive(false);
        SphereNo2.SetActive(false);
        particle1.SetActive(false);
        particle2.SetActive(false);

        distanceToGoal = 5 * 10 + 15 + 5 * 10 + 15 + 5 * 10 + 5;
        goal = new Vector3(0f, 0f, distanceToGoal);
        goalObj.transform.position = goal;
        goalObj.SetActive(false);

        no1Timer = 0f;
        no2Timer = 0f;
        myCycleCounter = 0;

        //activated = false;

        visualSignalN1.GetComponent<MeshRenderer>().material = redMaterial;
        visualSignalN2.GetComponent<MeshRenderer>().material = redMaterial;
        visualSignalN3.GetComponent<MeshRenderer>().material = bevoreStepOn;

        sphere1Spawned = false;
        sphere2Spawned = false;

    }


    public void NotifySphereStatus(bool status)
    {
        notifyViewScript = status;
    }

    //logging
    public float GetTimeSphere1AndReset()
    {
        float tmp = sphere1PickedUp;
        Debug.Log("Time gesamt fuer sp 1 : " + sphere1PickedUp);
        sphere1PickedUp = 0f;
        return tmp; 
    }
    public float GetTimeSphere2AndReset()
    {
        float tmp = sphere2PickedUp;
        sphere2PickedUp = 0f;
        Debug.Log("Time gesamt fuer sp 2 : " + tmp);
        return tmp;
    }
    public int GetNo1TimesFallenAndReset()
    {
        int tmp = timesSphere1Lost;
        timesSphere1Lost = 0;
        return tmp;
    }
    public int GetNo2TimesFallenAndReset()
    {
        int tmp = timesSphere2Lost;
        timesSphere2Lost = 0;
        return tmp;
    }

    public void CheckStatus()
    {

        if (myCycleCounter == 2 && sphere2Spawned && sphere1Spawned || myCycleCounter == 1 && sphere1Spawned)
            startScript.ReceiveSphereStatus(myCycleCounter, sphere1Spawned, sphere2Spawned);

    }

}
