using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigLogging : MonoBehaviour
{
    private float distance;
    private Vector3 startPoint;
    private float distTimer;
    private float distanceBackwards;
    private float distanceSidewards;//gemessen an orientierung des koordinatensystems
    private float headsetHeight;
    private float numberofsecondsrunning;
    public Transform hmd;

    private bool init;
    public bool active;

    private List<float> hmdHeightValues;
    private List<float> hmdHeightValuesOfTheWholeGame;
    private GameObject cubeParent;


    void Start()
    {
        hmdHeightValues = new List<float>();
        hmdHeightValuesOfTheWholeGame = new List<float>();
        distTimer = 0f;
        init = true;
        startPoint = this.gameObject.transform.position;
        numberofsecondsrunning = 0f;
        headsetHeight = 0f;
    }

    void Update()
    {
        if (active)
        {
            
            if (init)
            {
                startPoint = this.gameObject.transform.position;
                init = false;
            }

            distTimer += Time.deltaTime;

            if (distTimer >= 0.2f)
            {
                countDistances();
                distTimer = 0f;
                Debug.Log("counting");
            }

        }
        else if (!init)
        {
            init = true;
        }
 
    }

    private void countDistances()
    {
        distance += Vector3.Distance(startPoint, this.gameObject.transform.position);
        
        numberofsecondsrunning += 1f;

        //avg
        headsetHeight += hmd.transform.position.y;
        hmdHeightValues.Add(hmd.transform.position.y);
        
        if (startPoint.z > this.gameObject.transform.position.z)
        {
            distanceBackwards += startPoint.z - this.gameObject.transform.position.z;
        }

        if (startPoint.x > this.gameObject.transform.position.x)
        {
            distanceSidewards += startPoint.x - this.gameObject.transform.position.x;
           
        }
        else
        {
            distanceSidewards +=  this.gameObject.transform.position.x - startPoint.x;
           
        }

        startPoint = this.gameObject.transform.position;

    }

    public void ResetValues()
    {
        distance = 0f;
        distanceSidewards = 0f;
        distanceBackwards = 0f;
        init = true;
        numberofsecondsrunning = 0;
        headsetHeight = 0f;

        for (int i = 0; i< hmdHeightValues.Count; i++)
        {
            hmdHeightValuesOfTheWholeGame.Add(hmdHeightValues[i]);
        }

        hmdHeightValues.Clear();
    }

    //provide various values:
    public float GetDistanceTotal()
    {
        return distance;
    }

    public float GetSidewardsDist()
    {
        return distanceSidewards;
    }

    public float GetBackwardsDist()
    {
        return distanceBackwards;
    }

    public float GetAverageHMDHeight()
    {
        float tmp = headsetHeight / numberofsecondsrunning;
        return tmp;
    }

    public float GetHMDVariance()
    {
        float variance = 0f;
        float tmp = 0;
        float average = headsetHeight / numberofsecondsrunning;

        for (int i = 0; i < hmdHeightValues.Count; i++)
        {
            tmp = hmdHeightValues[i] - average;
            variance += tmp * tmp;         
        }
        variance = ((1 * variance) / (hmdHeightValues.Count - 1));
        return variance;
    }

    public List<float> GetRawHMDHeightValuesOfTheWholeGame()
    {
        return hmdHeightValuesOfTheWholeGame;
    }

    
}
