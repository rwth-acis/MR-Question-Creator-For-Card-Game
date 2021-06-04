using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

static class Variables
{
    // Save current and last menu for return function
    public static string path;
}

public class Json : MonoBehaviour
{

    public static string SAVE_FOLDER = @"C:\Users\Anna\Desktop\RWTH Aachen\8. Semester\Bachelorarbeit\Card Game\MR-Card-Game\Backend\Save\";
    
    public TextAsset jsonFile;
    public TextAsset jsonFile2;

    [Serializable]
    public class MyClass
    {
        public int level;
        public float timeElapsed;
        public string playerName;
    }

    [System.Serializable]
    public class Employee
    {
        //these variables are case sensitive and must match the strings "firstName" and "lastName" in the JSON.
        public string firstName;
        public string lastName;
    }

    [System.Serializable]
    public class Employees
    {
        //employees is case sensitive and must match the string "employees" in the JSON.
        public Employee[] employees;
    }

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
        public InputQuestion[] inputQuestions = new InputQuestion[10];
    }

    // Start is called before the first frame update
    void Start()
    {
        InputQuestions inputQuestionsInJson = JsonUtility.FromJson<InputQuestions>(jsonFile.text);
 
        foreach (InputQuestion question in inputQuestionsInJson.inputQuestions)
        {
            Debug.Log("Found question: Exercise name: " + question.exerciseName + ", type: " + question.type + ", name: " + question.name + ", question: " + question.question + ", answer: " + question.answer );
        }

        string scriptPath = GetCurrentFilePath();
        string rootPath = GetPathToRootDirectory(scriptPath);
        Variables.path = rootPath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test()
    {
        MyClass myObject = new MyClass();
        myObject.level = 1;
        myObject.timeElapsed = 47.5f;
        myObject.playerName = "Dr Charles Francis";
        string json = JsonUtility.ToJson(myObject);
        MyClass myObject2 = JsonUtility.FromJson<MyClass>(json);
        Debug.Log(myObject2.level);
        Debug.Log(myObject2.timeElapsed);
        Debug.Log(myObject2.playerName);
    }
    
    public void TestWriting()
    {
        InputQuestion inputQuestion = new InputQuestion();
        inputQuestion.exerciseName = "Exercise2";
        inputQuestion.name = "An original name";
        inputQuestion.question = "What is the answer to everything?";
        inputQuestion.answer = "forty two";
        string json = JsonUtility.ToJson(inputQuestion);
        //File.WriteAllText(@"C:\Users\Anna\Desktop\RWTH Aachen\8. Semester\Bachelorarbeit\Card Game\MR-Card-Game\Backend\Save\save2.json", json);
        File.WriteAllText(Variables.path, json);
    }

    // Helper method to get the path to this script file
    string GetCurrentFileName([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
    {
        return fileName;
    }

    // Method that returns you the path to this script
    private string GetCurrentFilePath()
    {
        string scriptPath = GetCurrentFileName();
        return scriptPath;
        
    }

    // Method that returns you the path to the save directory in the back end
    private string GetPathToRootDirectory(string scriptPath)
    {
        string rootPath = Path.GetFullPath(Path.Combine(scriptPath, @"..\..\..\..\..\"));
        string rootDirectoryPath = Path.GetFullPath(Path.Combine(rootPath, @"Backend\Save\save2.json"));
        return rootDirectoryPath;
    }

    // Add questions to the array and write it in the file
    public void WriteArrayInFile()
    {
        InputQuestion inputQuestion1 = new InputQuestion();
        inputQuestion1.exerciseName = "Exercise2";
        inputQuestion1.name = "An original name";
        inputQuestion1.question = "What is the answer to everything?";
        inputQuestion1.answer = "forty two";
        InputQuestion inputQuestion2 = new InputQuestion();
        inputQuestion2.exerciseName = "Exercise2";
        inputQuestion2.name = "Another original name";
        inputQuestion2.question = "What is the most inteligent species on Earth?";
        inputQuestion2.answer = "The white mouse";
        InputQuestions questionArray = new InputQuestions();
        questionArray.inputQuestions[0] = inputQuestion1;
        questionArray.inputQuestions[1] = inputQuestion2;
        string json = JsonUtility.ToJson(questionArray);
        File.WriteAllText(@"C:\Users\Anna\Desktop\RWTH Aachen\8. Semester\Bachelorarbeit\Card Game\MR-Card-Game\Backend\Save\save3.json", json);
        //File.WriteAllText(Variables.path, json);
    }
}
