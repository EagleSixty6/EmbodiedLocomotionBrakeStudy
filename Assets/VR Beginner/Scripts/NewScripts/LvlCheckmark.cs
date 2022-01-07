using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlCheckmark : MonoBehaviour
{
    private bool alreadyReached;
   
    void Start()
    {
        alreadyReached = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger enter");
        if (other.gameObject.CompareTag("RightController") && !alreadyReached)
        {
            //counterScript.LevelBoarderReached(this.gameObject);
            alreadyReached = true;
        }
        else if (other.gameObject.CompareTag("RightController") && !alreadyReached)
        {
            //counterScript.LevelBoarderReached(this.gameObject);
            alreadyReached = true;
        }


    }

    public void Reset()
    {
        alreadyReached = false;
    }
}
