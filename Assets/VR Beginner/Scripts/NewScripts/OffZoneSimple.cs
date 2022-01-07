using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//alias Gauss
public class OffZoneSimple : MonoBehaviour
{
    public MainController mainControllerScript;
    public GameObject _camera;
    public bool offZoneSimpleEnabled;
  
    public Image panel;
    public RectTransform dontMovePanel;

    public Movement movementScript;
    private Vector3 refPosition;
    private Vector3 camRefPosition;
    public bool moving = false;  
    public bool init = false;
    public Slider waitSlider;
    private float distanceTmp;
    bool waitTimerActive = false;
    float waitTimer = 0f;

    void Start()
    {
        offZoneSimpleEnabled = false;
        init = false;
        refPosition = Vector3.zero;
        moving = false;
        distanceTmp = 0f;
    }
   

    void Update()
    {
        if (offZoneSimpleEnabled)
        {
            
      

            if (!init)
            {
                
                refPosition = movementScript.getLeaningRefPosition();
                camRefPosition = movementScript.getCameraReferencePosition();
              
                waitTimer = 0f;
                init = true;
            }

            if (waitTimerActive)
            {
                waitTimer += Time.deltaTime;

                waitSlider.value = waitTimer / 2f;
                if (waitTimer > 2f)
                {
                    waitTimer = 0f;
                    dontMovePanel.gameObject.SetActive(false);
                    waitTimerActive = false;
                }
            }

            if (movementScript == null)
                Debug.Log("MScript null");

            else if(movementScript.getCurrentLeaningPosition() == null)
                Debug.Log("MPosScript null");

         
            if (Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition) > 0.3f && moving)
            {
                mainControllerScript.checkForMovementChanges("OffZoneSimple", "slowStop");
               
                moving = false;
                
            }
            else if (!moving && Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition) < 0.3f ) 
            {
                moving = true;
             
                mainControllerScript.checkForMovementChanges("OffZoneSimple", "slowStart");
                Debug.Log("Update mein movement auf true, moving is now : " + moving);
            }


           
            ChangeColourAccordingtoDistance();
            if (dontMovePanel.gameObject.activeInHierarchy); 
                waitTimerActive = true; 

        }
        else if(init)
        {
            init = false;
            moving = false;
            distanceTmp = 0f;
            refPosition = Vector3.zero;
            panel.transform.gameObject.SetActive(false);
            waitTimer = 0f;
        }

    }

    private void ChangeColourAccordingtoDistance()
    {
        if (movementScript.getLeaningRefPosition() != Vector3.zero && refPosition == Vector3.zero)
            refPosition = movementScript.getLeaningRefPosition();

        if (movementScript.getCurrentLeaningPosition() == Vector3.zero)
            return;

        if (System.Math.Round(Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition),2) != distanceTmp)
        {
            float distToMid = Mathf.Clamp(Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition) / 0.175f, 0, 2);
            if (distToMid > 1.0f)
            {
                distToMid = 2.0f - distToMid;

                
            }

            if (distToMid < 0.92)
            {
                UnityEngine.Color oldColor = panel.color;
                panel.color = new UnityEngine.Color(oldColor.r, oldColor.g, oldColor.b, distToMid);
            }
            else
            {
                UnityEngine.Color oldColor = panel.color;
                panel.color = new UnityEngine.Color(oldColor.r, oldColor.g, oldColor.b, 0.92f); ;
            }
           
            distanceTmp = Vector3.Distance(movementScript.getCurrentLeaningPosition(), refPosition);
        }
        if (!panel.transform.gameObject.activeSelf)
        {
            panel.transform.gameObject.SetActive(true);
        }

    }


}
