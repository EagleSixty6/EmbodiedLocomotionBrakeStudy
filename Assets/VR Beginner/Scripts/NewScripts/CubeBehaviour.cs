using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//old script on displaying cubes after activation
public class CubeBehaviour : MonoBehaviour
{
    private bool raycastEntered;
    private float timer;
    public AudioClip myClip;
    private AudioSource myASource;

    public GameObject[] objectsArray;
    public GameObject parent;
    public Material newMaterial;
    bool upwards;
    float tmp1;
    bool tmp;


  
    void Start()
    {
        tmp = false;
        raycastEntered = false;
        timer = 0f;
        upwards = false;
        tmp1 = 0;
        myASource = GameObject.Find("XR Rig").GetComponent<AudioSource>();
    }

   
    void Update()
    {      
        if (raycastEntered)
        {            
            //timer = timer + Time.deltaTime;
            if (!tmp)
            {
                for (int i = 0; i < objectsArray.Length; i++)
                {
                    objectsArray[i].GetComponent<MeshRenderer>().material = newMaterial;
                }
                myASource.PlayOneShot(myClip);
                upwards = true;
                raycastEntered = false;
                tmp = true;
            }


        }
       

      
           


    }


    public void OnRaycastEnter()
    {
        raycastEntered = true;
    }

    public void OnRaycastExit()
    {
        raycastEntered = false;
        timer = 0f;
    }

}
