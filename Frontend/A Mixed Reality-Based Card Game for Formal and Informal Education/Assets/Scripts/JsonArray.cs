using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class JsonArray : MonoBehaviour
{
    [Serializable]
    public class InputQuestion
    {
        public string exerciseName;
        public string type = "InputQuestion";
        public string name;
        public string question;
        public string answer;
    }

    [Serializable]
    public class InputQuestions
    {
        //employees is case sensitive and must match the string "employees" in the JSON.
        public List<InputQuestion> inputQuestions;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateExamples()
    {
        // First example
        InputQuestion inputQuestion1 = new InputQuestion();
        inputQuestion1.exerciseName = "Exercise2";
        inputQuestion1.name = "An original name";
        inputQuestion1.question = "What is the answer to everything?";
        inputQuestion1.answer = "forty two";

        // Second example
        InputQuestion inputQuestion2 = new InputQuestion();
        inputQuestion2.exerciseName = "Exercise2";
        inputQuestion2.name = "Another original name";
        inputQuestion2.question = "What is the most inteligent species on Earth?";
        inputQuestion2.answer = "The white mouse";
    }

    public void SaveExamples()
    {
        // First example
        InputQuestion inputQuestion1 = new InputQuestion();
        inputQuestion1.exerciseName = "Exercise2";
        inputQuestion1.name = "An original name";
        inputQuestion1.question = "What is the answer to everything?";
        inputQuestion1.answer = "forty two";

        // Second example
        InputQuestion inputQuestion2 = new InputQuestion();
        inputQuestion2.exerciseName = "Exercise2";
        inputQuestion2.name = "Another original name";
        inputQuestion2.question = "What is the most inteligent species on Earth?";
        inputQuestion2.answer = "The white mouse";

        // Store them in the list
        // Create a list of parts.
        // List<InputQuestion> list = new List<InputQuestion>();
        InputQuestions list = new InputQuestions();
        //List.inputQuestions = new List<InputQuestion>(InputQuestion);
        list.inputQuestions.Add(inputQuestion1);
        list.inputQuestions.Add(inputQuestion2);

        // Return them
        foreach (InputQuestion question in list.inputQuestions)
        {
            Debug.Log(question);
        }
        //Debug.Log(list.inputQuestions);
    }

    public void SaveExamples2()
    {
        // First example
        InputQuestion inputQuestion1 = new InputQuestion();
        inputQuestion1.exerciseName = "Exercise2";
        inputQuestion1.name = "An original name";
        inputQuestion1.question = "What is the answer to everything?";
        inputQuestion1.answer = "forty two";

        // Second example
        InputQuestion inputQuestion2 = new InputQuestion();
        inputQuestion2.exerciseName = "Exercise2";
        inputQuestion2.name = "Another original name";
        inputQuestion2.question = "What is the most inteligent species on Earth?";
        inputQuestion2.answer = "The white mouse";

        Debug.Log(inputQuestion1);
        Debug.Log(inputQuestion2);


        // Store them in the list
        List<InputQuestion> list = new List<InputQuestion>();
        list.Add(inputQuestion1);
        list.Add(inputQuestion2);

        // Return them
        int index = 0;
        foreach (InputQuestion question in list)
        {
            string json = JsonUtility.ToJson(question);
            Debug.Log(json);
            File.WriteAllText(@"C:\Users\Anna\Desktop\RWTH Aachen\8. Semester\Bachelorarbeit\Card Game\MR-Card-Game\Backend\Save\saveJsonList" + index +".json", json);
            index = index + 1;
        }
    }

    public void LoadSaves() 
    {
        // Get the string out of the save files
        string json1 = File.ReadAllText(@"C:\Users\Anna\Desktop\RWTH Aachen\8. Semester\Bachelorarbeit\Card Game\MR-Card-Game\Backend\Save\saveJsonList0.json");
        string json2 = File.ReadAllText(@"C:\Users\Anna\Desktop\RWTH Aachen\8. Semester\Bachelorarbeit\Card Game\MR-Card-Game\Backend\Save\saveJsonList1.json");
        Debug.Log(json1);
        Debug.Log(json2);
        InputQuestion question1 = JsonUtility.FromJson<InputQuestion>(json1);
        InputQuestion question2 = JsonUtility.FromJson<InputQuestion>(json2);
        Debug.Log("The first question is: " + question1.question + ", the answer is: " + question1.answer);
        Debug.Log("The first question is: " + question2.question + ", the answer is: " + question2.answer);
    }
}
