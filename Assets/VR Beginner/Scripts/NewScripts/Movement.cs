
using UnityEngine;

public class Movement : MonoBehaviour
{
	public GameObject refPosObj;
	public GameObject _camera;
	public Camera cam;
	public GameObject Rig;
	public float movementSpeed;
	public bool maskMethodActive;
	public HeadCalibration headCalibrationScript;
	public AudioClip continSound;
	public RectTransform calibratingPanel;

	private bool clearForUpdate;
	private Vector3 _leaningRefPosition; 
	private Vector3 _leaningRefPosition2;
	private Vector3 lastContact;
	private bool _leaningInitialized = false;
	public bool pauseLeaning = true;
	private MainController mainControllerScript;
	private GameObject _headJoint;
	private float movementreduction;
	Vector3 diff;
	private Vector3 camReferencePosition;
	private float _leaningForwardMaximumCM;
	private float tmpVelo = 0;
	private bool playedOneShot;
	public bool offZoneSimple;
	private float newVeloAxis;

	//logging
	private float zeroSpeedButStillRunning;
	private float timeImErstenBereich = 0f;
	private float timeImZweitenBereich = 0f;

	// Start is called before the first frame update
	void Awake()
    {
		mainControllerScript = GameObject.Find("XR Rig").GetComponent<MainController>();
		headCalibrationScript = GetComponent<HeadCalibration>();
		clearForUpdate = false;
		offZoneSimple = false;
		
		_leaningRefPosition = Vector3.zero;
		_leaningRefPosition.y = 1.0f;
		_leaningInitialized = true;
		_leaningForwardMaximumCM = 0.40f;
		
	}

  
    void Update()
    {
		if(clearForUpdate)
			UpdateMovement();
    }

	public void onMethodChanged()
	{		
		clearForUpdate = false;
		_headJoint = null;
		headCalibrationScript.StartCenterOfRotationCalibration();
		Debug.Log("Starting head calibration - Movement");
	}

	public void onAcceptedUserInput()
    {
		headCalibrationScript.FinishCenterOfRotationCalibration();
		Debug.Log("Finishing initialization FINISHED - Movement");
		calibratingPanel.gameObject.SetActive(false); // new panel dont move!
	}

	//sets new calibrated mid point
	public void setNewRefObj()
	{
		
		_headJoint = GameObject.Find("CenterOfYawRotation(Clone)");
		_leaningRefPosition = Rig.transform.InverseTransformPoint(_headJoint.transform.position);
		camReferencePosition = cam.transform.position;
		mainControllerScript.cancelUI();
		
		clearForUpdate = true;	
		Debug.Log("Initialization finished, new Object set - Movement");
		pauseLeaning = false;
		_leaningInitialized = true;
		mainControllerScript.startNewMethod();
		movementreduction = 0f;
	}

	public virtual void UpdateMovement()
	{


		if (_leaningInitialized && !pauseLeaning)
		{


			diff = Rig.transform.InverseTransformPoint(_headJoint.transform.position) - _leaningRefPosition;

			float _velocityAxis = diff.magnitude;
			diff = this.transform.TransformDirection(diff); 
			diff = Vector3.ProjectOnPlane(diff, Vector3.up);
			Vector3 _movementDirection = diff.normalized;
			


			
			//logging
			if (_velocityAxis < 0.07f)
			{
				zeroSpeedButStillRunning += Time.deltaTime;
			}

			float velocity;

			//special changes for the gauss stop method
			if (offZoneSimple)
			{
				_velocityAxis = Mathf.Clamp(_velocityAxis / 0.175f, 0, 2);

				if (_velocityAxis > 1.0f)
				{
					timeImZweitenBereich += Time.deltaTime;
					_velocityAxis = 2.0f - _velocityAxis;
				}
				else
					timeImErstenBereich += Time.deltaTime;
			
			}
			else
            {
				_velocityAxis = Mathf.Clamp(_velocityAxis / _leaningForwardMaximumCM, 0, 1);				
			}
			
			velocity = Mathf.Pow(_velocityAxis * 1, 1.53f) * (movementSpeed - movementreduction);

			if (System.Math.Round(tmpVelo, 1) != System.Math.Round(velocity, 1))
			{
				mainControllerScript.SendVelociy((float)System.Math.Round(velocity, 1) / movementSpeed);
				tmpVelo = velocity;
			}

			_velocityAxis = _velocityAxis * velocity; 

			Quaternion rotation = Quaternion.Euler(0, -_camera.transform.eulerAngles.y, 0); 
			Matrix4x4 m = Matrix4x4.Rotate(rotation);
			diff = m.MultiplyPoint3x4(diff);

			if (maskMethodActive)
			{
				Vector3 backwards = Vector3.back;
				float angle = Vector3.Angle(backwards, diff);
				if (angle <= 45.0f && angle >= -45.0f)
				{
					_velocityAxis = 0.0f;
				}
				playedOneShot = false;
			}
			else if (!maskMethodActive && refPosObj.activeSelf)
			{
				refPosObj.SetActive(false);
			}

			//maskMethod equals BackEscape
			if (diff.z < -0.2f && maskMethodActive)
			{
				pauseLeaning = true; 
				lastContact = _camera.transform.position + _camera.transform.forward * 0.2f; 
				refPosObj.SetActive(true);
				refPosObj.transform.position = lastContact;
				lastContact.y = 0.0f;
				playedOneShot = false;
				mainControllerScript.countBrakeTime = true;
			}
			else
			{
				Translate(Time.deltaTime, _velocityAxis, _movementDirection, Rig.transform); 
			}


		}
		else if (pauseLeaning && maskMethodActive)
		{

			Vector3 camPos = _camera.transform.position;
			camPos.y = 0.0f;
			
			if (Vector3.Distance(camPos, lastContact) < 0.15f) //diff between last and current point
			{
				pauseLeaning = false;
				refPosObj.SetActive(false);
				mainControllerScript.countBrakeTime = false;
				if (!playedOneShot)
				{
					mainControllerScript.playOneShotAudioClip(continSound);
					playedOneShot = true;
				}

			}
		}
		else if (pauseLeaning && refPosObj.activeSelf && !maskMethodActive)
		{
			refPosObj.SetActive(false);
		}
			
	}

	

	private void Translate(float deltaTime, float velocityAxis, Vector3 movementDirection, Transform trans)
	{
		Vector3 tmpResult = trans.position + velocityAxis * deltaTime *  movementDirection;

		trans.position = tmpResult;
	}

	public bool openStop()
	{
		if (!pauseLeaning)
		{
			pauseLeaning = true;
			movementreduction = 0f;
			return true;
		}
		else
			return false;
		
	}

	public bool openStart()
	{
		if (!pauseLeaning)
			return false;
        else
		{
			movementreduction = 0f;
			pauseLeaning = false;
			return true;
		}
	}

	public bool openCalibrateEasy()
	{
		pauseLeaning = false;
		setLeaningRefPosition();

		if (maskMethodActive)
		{
			pauseLeaning = false;
			clearForUpdate = true;
		}
			

		return true;
	}

	public void AdjustMovement(float adjust)
	{
		newVeloAxis = adjust;
	}

	public bool openSlowStop(float value)
	{
		if (value > 9f)
		{
			movementreduction = 0f;
			return openStop();
		}
		else if (value < 1f)
		{
			movementreduction = 0f;
			return openStart();
		}
		else if (value >= 1f && value <= 9f)
		{		
			movementreduction = (movementSpeed/10) * value;
			openStart();
			return true;
		}
		else
			return false;
	
	}






	public void setLeaningRefPosition() 
	{
		
		_leaningRefPosition = Rig.transform.InverseTransformPoint(_headJoint.transform.position); 
		_leaningRefPosition2 = Rig.transform.InverseTransformPoint(_headJoint.transform.localPosition);
	
	}

	public Vector3 getLeaningRefPosition()
	{
		return _leaningRefPosition;
	}

	public Vector3 getCameraReferencePosition()
	{
		return camReferencePosition;
	}


	public Vector3 getDiffPos()
	{
		return diff;
	}



	public Vector3 getCurrentLeaningPosition()
    {

		if (_headJoint == null)
			return Vector3.zero;
		
		return Rig.transform.InverseTransformPoint(_headJoint.transform.position);
	}

	

	//logging...
	public float GetTimeWhileNullMovementAndReset()
	{ 
		float tmp = zeroSpeedButStillRunning;
		zeroSpeedButStillRunning = 0f;
		return tmp;
	}

	public float GetTimeBereich1PlusReset()
	{
		float tmp = timeImErstenBereich;
		timeImErstenBereich = 0f;
		return tmp;
	}

	public float GetTimeBereich2PlusReset()
	{
		float tmp = timeImZweitenBereich;
		timeImZweitenBereich = 0f;
		return tmp;
	
	}
}
