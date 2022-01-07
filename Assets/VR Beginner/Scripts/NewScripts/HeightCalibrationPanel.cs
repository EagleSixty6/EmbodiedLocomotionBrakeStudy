using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

//UI Panel for HMD height setup at the start of the study
public class HeightCalibrationPanel : MonoBehaviour
{

    public StartupScreen mainScreenScript;

    [SerializeField]
    public RectTransform alarmText;

    [SerializeField]
    private Text headHeightTextField;

    [SerializeField]
    private Text armHeightTextField;

    [SerializeField]
    public Transform leftController;

    [SerializeField]
    public Transform rightController;

    [SerializeField]
    public Transform HMD;

    [SerializeField]
    public Button continueButton;

    [SerializeField]
    public Button continueHeightButton;

    [SerializeField]
    public Button continueArmButton;

    [SerializeField]
    public Button redoHeightButton;

    [SerializeField]
    public Button redoArmButton;

    private bool checkForControllerHeight;
    private bool checkForHMDHeight;
    private float armHeight;
    private float hmdHeight;
    private bool releasedHandRight;
    private bool releasedHandMain;

    void Start()
    {
        checkForControllerHeight = true;
        checkForHMDHeight = true;       
    }

   
    void Update()
    {
        
        //trigger can enable the button
        if (checkForHMDHeight)
        {
            headHeightTextField.text = ""+Mathf.Round(HMD.position.y * 100);


            if ( Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.5 && releasedHandRight ||
                Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.5  && releasedHandMain )
            {
                continueHeightButton.interactable = false;
                redoHeightButton.interactable = true;

                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.5)
                    releasedHandRight = false;
                else if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.5)
                    releasedHandMain = false;

                ConfirmHMDHeight();
            }
        }

        if (!releasedHandRight)
        {
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") < 0.1)
            {
                releasedHandRight = true;
            }
        }

        if (!releasedHandMain)
        {
            if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") < 0.1)
            {
                releasedHandMain = true;
            }


        }

    }

    //was used for testing purposes
    public void ConfirmArmHeight()
    {
        checkForControllerHeight = false;
        armHeight = int.Parse(armHeightTextField.text);

        if (!checkForHMDHeight)
        {
            
                alarmText.gameObject.SetActive(false);
                continueButton.interactable = true;
                mainScreenScript.SetUserArmHeight(armHeight/100, hmdHeight / 100);
                mainScreenScript.SetUserHeight(hmdHeight/100);
          
        }       
    }

    public void ConfirmHMDHeight()
    {
        checkForHMDHeight = false;
        hmdHeight = int.Parse(headHeightTextField.text);
        continueButton.interactable = true;
        mainScreenScript.SetUserHeight(hmdHeight / 100);
        mainScreenScript.SetUserArmHeight(5000 / 100, hmdHeight / 100);
       
    }

    public void RedoHMDHeight()
    {
        checkForHMDHeight = true;
        if (continueButton.interactable)
            continueButton.interactable = false;

        armHeightTextField.text = "50";
    }

    public void RedoArmHeight()
    {
        checkForControllerHeight = true;
        if (continueButton.interactable)
            continueButton.interactable = false;

        armHeightTextField.text = "50";
    }

    


}
