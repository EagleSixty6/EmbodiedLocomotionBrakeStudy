using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCalibration : MonoBehaviour
{
    public float _samplingFrequency;// 0.1 sek - zust fuer sampling intervalle beim abtasten
    //public GameObject _camera;
    public Movement movementScript;
    public Camera _camera;
    public GameObject XR;
    // float deltaT = 0.0f;
    bool _calibrationRecordingEnabled;
    float _samplingTimer;
    List<Vector3> _hmdPositions;
    List<Vector3> _hmdForwards;
    GameObject _headJoint = null;
    Vector3 _playerOffsetAboveGround;
    Vector3 _leaningRefPosition;


    private void Start()
    {
        //StartCenterOfRotationCalibration();
    }
    void Update()
    {
        if (_calibrationRecordingEnabled)
        {
            _samplingTimer -= Time.deltaTime;
            if (_samplingTimer < 0)
            {
                _hmdPositions.Add(_camera.transform.position);
                _hmdForwards.Add(_camera.transform.forward);
                _samplingTimer = _samplingFrequency;
            }

        }
        else if (_leaningRefPosition != Vector3.zero)
        {
            Vector3 diff = _camera.transform.InverseTransformPoint(GetHeadJoint().transform.position) - _leaningRefPosition;
        }
    }

    // start of calibration
    public void StartCenterOfRotationCalibration()
    {
        _calibrationRecordingEnabled = true; //TODO wenn knopf gedrueckt von controller also calibrierung muss das hier auf true gesetzt werden
        _samplingTimer = _samplingFrequency;
        _hmdPositions = new List<Vector3>();
        _hmdForwards = new List<Vector3>();
        Destroy(GameObject.Find("CenterOfYawRotation(Clone)"));
    }

    private GameObject GetHeadJoint()
    {
        GameObject obj = GameObject.Find("CenterOfYawRotation(Clone)");
        return obj;
    }

    // finish calibration procedure
    public void FinishCenterOfRotationCalibration()
    {
       
        _calibrationRecordingEnabled = false;
        int firstSample = _hmdPositions.Count / 4; // welche sample points -> vierteln
        int secondSample = firstSample * 3;

     
        // construct a saggital plane at the head set's current/final position
        Plane saggitalPlane = new Plane();
        saggitalPlane.SetNormalAndPosition(_camera.transform.right, _camera.transform.position);
      
        // shoot a ray from two positions on the calibration arc to the plane the results is the center of (yaw) rotation
        // Note: Only one sample would be nessesary.
        float distanceToPlane;
        Ray ray = new Ray(_hmdPositions[firstSample], -_hmdForwards[firstSample]);
        saggitalPlane.Raycast(ray, out distanceToPlane);
        Vector3 firstTarget = ray.GetPoint(distanceToPlane);
   
        ray = new Ray(_hmdPositions[secondSample], -_hmdForwards[secondSample]);
        saggitalPlane.Raycast(ray, out distanceToPlane);
        Vector3 secondTarget = ray.GetPoint(distanceToPlane);

        Vector3 centerOfYawRotationGlobal = (firstTarget + secondTarget) / 2f;
        GameObject centerOfYawRotation = new GameObject("CenterOfYawRotation");

        _headJoint = Instantiate(centerOfYawRotation, centerOfYawRotationGlobal, Quaternion.identity, _camera.transform); // der gesuchte punkt als GameObject
       
        //Debug.Log("Head's center of yaw rotation distance to headset: " + (_camera.transform.position - centerOfYawRotationGlobal).magnitude);
        
         //_leaningRefPosition = _camera.transform.InverseTransformPoint(GetHeadJoint().transform.position);
         //Vector3 diff = _camera.transform.InverseTransformPoint(GetHeadJoint().transform.position) - _leaningRefPosition;
        movementScript.setNewRefObj();

        // define player offset
        RaycastHit hit;
        int layerMask = 1 << 8; // terrain
        Physics.Raycast(transform.position + new Vector3(0, 10, 0), -Vector3.up, out hit, Mathf.Infinity,
            layerMask);
        _playerOffsetAboveGround = transform.position - hit.point;

        //return _leaningRefPosition;
    }


}
