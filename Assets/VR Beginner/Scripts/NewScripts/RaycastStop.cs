using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastStop : MonoBehaviour
{
    
    public LayerMask recognisingLayer;
    
    
    public MainController mainControllerScript;
    public bool rayStopEnabled = false;
    public GameObject camera;

    Vector3 leaningReference;
    float collisionTimer = 0.25f;
    float deltaTime = 0.0f;
    Vector3 diff;
    GameObject rig;
    GameObject _headJoint;
    bool iamMoving = false;
    bool trainingactive = false;
    public bool init;

   
    void Start()
    {
        init = false;
        iamMoving = false;
        //rayList = new List<RaycastHit>();
        rig = GameObject.Find("XR Rig");
    }
    private void Update()
    {


        if (rayStopEnabled)
        {
            if (!init)
            {
                _headJoint = GameObject.Find("CenterOfYawRotation(Clone)");
                setLeaningRefPos();
                init = true;
                iamMoving = false;
                //mainControllerScript.checkForMovementChanges("Raycast", "slowStart");
            }


            deltaTime += Time.deltaTime;
            if (deltaTime > collisionTimer)
            {
                CalculateMovementDirection();
                deltaTime = 0.0f;
            }
        }
        else if(init)
        {
            deltaTime = 0f;
            iamMoving = false;
            init = false;
        }
            
    }

   

    public void TrainingActive(bool training)
    {
        trainingactive = training;
    
    }

    private void sphereCast(Vector3 movementDirection)
    {
        bool vectorInit = false;
        float resultDistance = 0.0f;
        float sphereRadius = 0.9f;
        if (!trainingactive)
        {        
            resultDistance = 0.0f;
            sphereRadius = 0.95f; // war 0.35 ist 0.65 -> bis zu 0.85 testeen
        }
        else
        {
            resultDistance = 0.0f;
            sphereRadius = 2f; // war 0.35 ist 0.65 -> bis zu 0.85 testeen
        }
       

        
      
        Collider[] hitColliders = Physics.OverlapSphere(_headJoint.transform.position + movementDirection , sphereRadius, recognisingLayer);

      
        
        foreach (var hitCollider in hitColliders)
        {
            
            if (!vectorInit)
            {
                resultDistance = Vector3.Distance(_headJoint.transform.position, hitCollider.transform.position);
                vectorInit = true;
            }
            else if (Vector3.Distance(_headJoint.transform.position, hitCollider.transform.position) < resultDistance)
                resultDistance = Vector3.Distance(_headJoint.transform.position, hitCollider.transform.position);
        }

        if (hitColliders.Length > 0 && iamMoving)
        {

            if (!trainingactive)
            {
                if (resultDistance < 0.92f)
                {
                    mainControllerScript.checkForMovementChanges("Raycast", "slowStop");
                    iamMoving = false;
                }
            }
            else
            {
                if (resultDistance < 1.9f)
                {
                    mainControllerScript.checkForMovementChanges("Raycast", "slowStop");
                    iamMoving = false;
                }
            }
        }
        else if (!iamMoving && hitColliders.Length == 0)
        {
            mainControllerScript.checkForMovementChanges("Raycast", "slowStart");
            iamMoving = true;
            
        }
        else if (!iamMoving && hitColliders.Length > 0)
        {

            if (!trainingactive)
            {
                if (resultDistance > 0.93f)
                {
                    mainControllerScript.checkForMovementChanges("Raycast", "slowStart");
                    iamMoving = true;
                }
            }
            else
            {
                if (resultDistance > 1.91f)
                {
                    mainControllerScript.checkForMovementChanges("Raycast", "slowStart");
                    iamMoving = true;
                }
            }
            
                
        }
            
    }

    private void CalculateMovementDirection()
    {

        if (_headJoint == null)
        { 
          Debug.LogWarning("cant find head joint (this can be normal during switching between training and study), waiting...");
            return;
        }    
        else
        {
            diff = rig.transform.InverseTransformPoint(_headJoint.transform.position) - leaningReference;             
        }

        //_camera.transform.localPosition - _leaningRefPosition;

        float _velocityAxis = diff.magnitude;
        diff = Vector3.ProjectOnPlane(diff, Vector3.up);
        Vector3 _movementDirection = diff.normalized;

        if (_headJoint == null)
        {

        }
        else
            sphereCast(_movementDirection);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(camera.transform.position + camera.transform.forward * 0.35f, 0.35f);

    }

    public void setLeaningRefPos()
    {
        leaningReference = mainControllerScript.TransferLeaningRef();
    
    }
}
