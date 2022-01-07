using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubes : MonoBehaviour
{
    public Camera cam;
    public LayerMask mask;
    public AudioClip clip;
    public Material newMaterial;
    public AudioSource aSource;
    public Material oldMaterial;
    public Material oldGreen;

   
    GameObject currentCube;
    List<GameObject> changedObjectsList;
    private bool trainingActive = false;
    private int numberOfactivatedCubes = 0;
    private List<float> distancesWhenActivated;

    //logging
    private int walkedThrewCounter;
    private List<GameObject> countedCubes;
    private string walkedthrewCubes;
    private string startedCubes;

  
    void Start()
    {
        changedObjectsList = new List<GameObject>();
        currentCube = null;
        distancesWhenActivated = new List<float>();
        walkedThrewCounter = 0;
        countedCubes = new List<GameObject>();
    }

   
    void Update()
    {
        if(!trainingActive)
        {
            if (currentCube != null)
            {
                if (!countedCubes.Contains(currentCube) && Vector2.Distance(new Vector2(cam.transform.position.x, cam.transform.position.z), new Vector2(currentCube.transform.position.x, currentCube.transform.position.z)) < 0.1f)
                {
                   
                    walkedThrewCounter += 1;
                    countedCubes.Add(currentCube);
                    walkedthrewCubes += CalcCubeWalkedThrewName(currentCube) + ", ";

                }
                //as long as the cube isnt in the list of recognized objects
                if (!changedObjectsList.Contains(currentCube))
                {
                    //Debug.Log("- CUBES.cs  "+ cam.transform.position.y + "cube pos: "+ (currentCube.transform.position.y - 0.08));
                    if (cam.transform.position.y < currentCube.transform.position.y - 0.18 && Vector3.Distance(cam.transform.position, currentCube.transform.position) < 1.5)
                    {
                        // fuege cube der average distance liste hinzu .. average wird in startscript berechnet.
                        distancesWhenActivated.Add(Vector3.Distance(cam.transform.position, currentCube.transform.position));

                        aSource.PlayOneShot(clip);
                        foreach (Transform child in currentCube.transform)
                        {
                            child.GetComponent<MeshRenderer>().material = newMaterial;
                        }
                        changedObjectsList.Add(currentCube);
                        startedCubes += CalcCubeWalkedThrewName(currentCube) + ", ";
                        

                        //logging
                        numberOfactivatedCubes += 1;

                    }
                }
                else
                    currentCube = null;

                
            }

            SpCheckForCubes();

        }
    }

    //get the cubes that were walked through
    private string CalcCubeWalkedThrewName(GameObject cube)
    {
        if (cube.transform.position.z != 0)
        {
            return "c" + (int)cube.transform.position.z;
        }
        else
            return "c0";
    }
        

    public void activateTraining(bool training)
    {
        trainingActive = training;
    }

    public void ResetAllCubes()
    {
       
        if (changedObjectsList != null)
        {
            int count = changedObjectsList.Count;
            foreach (GameObject obj in changedObjectsList)
             {
                 foreach (Transform child in obj.transform)
                 {
                     Debug.Log("loesche" + child);
                     if (child.name.Equals("FakePlane"))
                     {
                         child.GetComponent<MeshRenderer>().material = oldGreen;
                     }
                     else
                         child.GetComponent<MeshRenderer>().material = oldMaterial;
                 }
               
                 
             }
        }
        changedObjectsList.Clear();
        

    }

    //check if invisible spherecasts coming from player hits the cube
    private void SpCheckForCubes()
    {
       
        Collider[] hitCollider = Physics.OverlapSphere(cam.transform.position, 1.5f, mask);
        
        for (int i = 0; i < hitCollider.Length; i++)
        {
            if(Vector3.Distance(cam.transform.position, hitCollider[i].gameObject.transform.position)<= 1.1f) //was 0.35
            {
                currentCube = hitCollider[i].gameObject;
                break;
            }
        }
        

      

    }

    public List<float> GetDistances()
    {
        return distancesWhenActivated;
    }

    public void ClearDistances()
    {
        distancesWhenActivated.Clear();
    }

    public int GetNumberofActivatedCubesAndReset()
    {
        int tmp = numberOfactivatedCubes;
        numberOfactivatedCubes = 0;
        
        return tmp;
    }

    public int GetWalkedThrewNumberAndReset()
    {
        int tmp = walkedThrewCounter;
        walkedThrewCounter = 0;
        countedCubes.Clear();
        return tmp;

    }
    

    public string GetStartedCubesStringAndReset()
    {
        string tmp = startedCubes;
        startedCubes = "";
        Debug.Log("startedCubesString: " + tmp);
        return tmp;
    
    }

    public string GetWalkedThrewCubesStringAndReset()
    {
        string tmp = walkedthrewCubes;
        walkedthrewCubes = "";
        Debug.Log("WalkedThrewString: " + tmp);
        return tmp;

    }
}
