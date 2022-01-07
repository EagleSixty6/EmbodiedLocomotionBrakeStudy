using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//checking for collision with hands
public class HandsCollision : MonoBehaviour
{

    public CollectiblesHandler collectiblesScript;
    private string myName;

    private void Start()
    {
        if (this.gameObject.name.Equals("LeftHand Controller"))
        {
            myName = "left";
            Debug.Log("erkenne linken Controller mit HandsCollision script vorhanden");
        }
        else if (this.gameObject.name.Equals("RightHand Controller"))
        {
            myName = "right";
            Debug.Log("erkenne rechten Controller mit HandsCollision script vorhanden");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("collectible"))
        {
            collectiblesScript.ReceivedSpehereObject(myName, other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("collectible"))
        {
            collectiblesScript.ReleasedSpehereObject(myName);
        }
    }
}
