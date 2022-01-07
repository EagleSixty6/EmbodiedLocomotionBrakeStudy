using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FulfillmentCounter : MonoBehaviour
{
    public GameObject cubeCounterText;
    public GameObject startCanvasTextandCo;
    public List<GameObject> lvlCheckpoints;
    public StartupScreen startScript;
    

    int cubeCounter;

  
    void Start()
    {
        cubeCounter = 0;
        cubeCounterText.GetComponent<UnityEngine.UI.Text>().text = "0 / 10";
    }

   

    public void CountUp()
    {
        cubeCounter++;

        UpdateCubeCounterCanvas();
    }

    private void UpdateCubeCounterCanvas()
    {
        if (cubeCounter < 11)
            cubeCounterText.GetComponent<UnityEngine.UI.Text>().text = "" + cubeCounter + " / 10";
       
            

    }

    public void resetCounter()
    {
        cubeCounter = 0;
    }

    public void CountUpBallsAndGiveFeedback()
    {
        Debug.Log("treffer mit ball im korb oder so");
        
    }


    //wenn erste wand betreten wird mache/ continue / aktiviere baelle aufgabe
    //wenn danach noch eine betreten wird mache von vorne und lese von der liste welche eingabe methode kommt 
    //und nehme die entsprechenden cubes in der richtige reihenfolge -> startupscreem
    // zaehle alle werte und speichere diese (ergebnisse)
    // achte darauf wenn der knopf gedrueckt wird das nur die noch nicht beendete aufagbe dieser stage neu gestartet wird sonst nichts.
    //loesche in diesem fall dann die entsprechenden ergebnisse

    
    public void LevelBoarderReached(GameObject obj)
    {
        Debug.Log("Reached wall: " + obj.name);
            startScript.LvlReached(0);
  
    }

    public void ResetLvlCheckpoints()
    {
        foreach (GameObject obj in lvlCheckpoints)
        {
            
            obj.GetComponent<LvlCheckmark>().Reset();
        }
    }

}
