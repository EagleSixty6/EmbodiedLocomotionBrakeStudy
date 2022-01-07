using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedChooserUI : MonoBehaviour
{
    
    public Text codeWarning;
    public StartupScreen startScript;

    [SerializeField]
    private Text seedTextField;

    [SerializeField]
    private Button startButton;

    private int userid;

    //permutationen nach Latin Square
    private static List<int> einseins = new List<int> { 2, 5, 1, 3, 4, 6 };
    private static List<int> einszwei = new List<int> { 1, 2, 6, 4, 3, 5 };
    private static List<int> einsdrei = new List<int> { 4, 3, 5, 6, 1, 2 };
    private static List<int> einsvier = new List<int> { 5, 1, 3, 2, 6, 4 };
    private static List<int> einsfuenf = new List<int> { 3, 6, 4, 5, 2, 1 };
    private static List<int> einssechs = new List<int> { 6, 4, 2, 1, 5, 3 };
    //2
    private static List<int> zweieins = new List<int> { 4, 2, 3, 1, 6, 5 };
    private static List<int> zweizwei = new List<int> { 3, 4, 1, 2, 5, 6 };
    private static List<int> zweidrei = new List<int> { 6, 5, 2, 4, 1, 3 };
    private static List<int> zweivier = new List<int> { 5, 1, 6, 3, 4, 2 };
    private static List<int> zweifuenf = new List<int> { 1, 3, 5, 6, 2, 4 };
    private static List<int> zweisechs = new List<int> { 2, 6, 4, 5, 3, 1 };
    //3
    private static List<int> dreieins = new List<int> { 5, 2, 4, 1, 6, 3 };
    private static List<int> dreizwei = new List<int> { 6, 5, 2, 4, 3, 1 };
    private static List<int> dreidrei = new List<int> { 3, 1, 5, 6, 2, 4 };
    private static List<int> dreivier = new List<int> { 2, 4, 6, 3, 1, 5 };
    private static List<int> dreifuenf = new List<int> { 4, 3, 1, 2, 5, 6 };
    private static List<int> dreisechs = new List<int> { 1, 6, 3, 5, 4, 2 };
    //4
    private static List<int> viereins = new List<int> { 1, 4, 5, 6, 2, 3 };
    private static List<int> vierzwei = new List<int> { 6, 3, 2, 4, 1, 5 };
    private static List<int> vierdrei = new List<int> { 5, 1, 4, 3, 6, 2 };
    private static List<int> viervier = new List<int> { 3, 2, 6, 5, 4, 1 };
    private static List<int> vierfuenf = new List<int> { 4, 5, 1, 2, 3, 6 };
    private static List<int> viersechs = new List<int> { 2, 6, 3, 1, 5, 4 };
    //liste der listen
    private static List<List<int>> listOfLists = new List<List<int>> {einseins, einszwei, einsdrei, einsvier, einsfuenf, einssechs, zweieins, zweizwei,
    zweidrei,zweivier,zweifuenf,zweisechs,dreieins,dreizwei,dreidrei,dreivier,dreifuenf,dreisechs,viereins,vierzwei,vierdrei,viervier,vierfuenf,viersechs};

    private List<int> chosenList;
    private int roundOfThatList;
    private bool codeCheck;

   

    void Update()
    {
        if (seedTextField.text.Length == 4 && codeCheck)
        {
            codeWarning.gameObject.SetActive(false);
            startButton.interactable = true;
        }
        else if (startButton.interactable && seedTextField.text.Length != 3)
            startButton.interactable = false;

        if (!codeCheck && !codeWarning.gameObject.activeSelf)
            codeWarning.gameObject.SetActive(true);
        else if (codeCheck && codeWarning.gameObject.activeSelf)
        {
            codeWarning.gameObject.SetActive(false);
        }

    
    }

    //print input of the study code
    public void keyPressed(int number)
    {


        if (number == 11)
        {
            seedTextField.text = "";
            codeCheck = true;
        }       
        else if (seedTextField.text.Length != 4)
        {
            if (seedTextField.text.Equals(""))
                codeCheck = true;
            else if (seedTextField.text.Length == 1)
                codeCheck = true;
            else if (seedTextField.text.Length == 2 && number <= 4 && number >= 1)
                codeCheck = true;
            else if (seedTextField.text.Length == 3 && number <= 6 && number >= 1 && codeCheck)
                codeCheck = true;
            else
                codeCheck = false;

            seedTextField.text = seedTextField.text + number;
        }
        else if (seedTextField.text.Length > 3)
        {
            seedTextField.text = "";
        }
    }

    //analyse input study code and send corresponding list
    public void SendList()
    {
        if (int.Parse(seedTextField.text) >= 2000)
        {
            roundOfThatList = (int)Mathf.Round(int.Parse(seedTextField.text) / 1000);
            Debug.Log("first number: " + roundOfThatList);
        }
        else
            roundOfThatList = 1;
        

        int tmpSecondNumber; // nicht mehr relevant
        // 1xxx - 1000 = xxx / 100 = x.xx = int x
        tmpSecondNumber = (int.Parse(seedTextField.text) - (roundOfThatList * 1000))/100;
        Debug.Log("second number: "+tmpSecondNumber);

        // = xxx - x00 = xx 
        int thirdNumber; // relevant
        thirdNumber = (((int.Parse(seedTextField.text) - (roundOfThatList * 1000))) - (tmpSecondNumber*100)) / 10;
        Debug.Log("third number " + thirdNumber);
        // xxxx - x000 - x00 -x0 
        int fourthNumber; // relevant
        fourthNumber = (int.Parse(seedTextField.text) - (roundOfThatList * 1000)) - (tmpSecondNumber * 100) - (thirdNumber * 10);
        Debug.Log("fourth number " + fourthNumber);

        int finalnumber; 
        finalnumber = (thirdNumber - 1) * 6 + fourthNumber - 1;
        Debug.Log("finalnumber die in liste gesucht wird: " + finalnumber);

        //
        startScript.SaveAIntValue("seed", finalnumber);
        startScript.SaveAIntValue("userid", roundOfThatList * 1000 + tmpSecondNumber*100 + thirdNumber*10 + fourthNumber);
        startScript.ReceiveMethodList(listOfLists[finalnumber], roundOfThatList);

        userid = roundOfThatList * 1000 + tmpSecondNumber * 100 + thirdNumber * 10 + fourthNumber;
        startScript.userID = userid;
    }

   
}
