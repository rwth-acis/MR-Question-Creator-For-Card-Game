using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

static class Menus
{
    // Save current and last menu for return function
    public static GameObject lastMenu;
    public static GameObject currentMenu;
    // Save the current index for all question or moddel collection 
    public static int currentInputQuestionIndex; // Used to access the right position in the array of the input questions
    public static int currentMultipleChoiceQuestionIndex; // Used to access the right position in the array of the input questions
    public static int currentModelCollectionIndex; // Used to access the right position in the array of the 3D model collection
    // Save an index and exercise name to be able to link different questions and models together
    public static int currentExerciseIndex; // Used to generate correct exercise names
    public static string currentExerciseName;
    // public static ExerciseCollection collection;
    // Path to the save directory in the back end
    public static string savePath; // Path to the save directory in the back end
    public static string saveIndex;
}

public class Creator : MonoBehaviour
{
    // Defining the necessary menus and windows
    public GameObject mainMenu;
    public GameObject mainCreator;
    public GameObject multipleChoiceCreator;
    public GameObject inputModeCreator;
    public GameObject enterNameWindow;
    public GameObject exitWithoutSavingWindow;

    // The veils are invisible, they are used to block access to the buttons on the menu under it
    public GameObject veilLargeWindow;
    public GameObject veilSmallWindow;

    // Defining the input fields
    // Input mode fields
    public TMP_InputField enterQuestionInput;
    public TMP_InputField enterAnswerInput;
    // Multiple Choice mode fields
    public TMP_InputField enterQuestionMultiple;
    public TMP_InputField enterFirstAnswer;
    public TMP_InputField enterSecondAnswer;
    public TMP_InputField enterThirdAnswer;
    public TMP_InputField enterFourthAnswer;
    public TMP_InputField enterFifthAnswer;
    // The name input field, used for all question modes
    public TMP_InputField enterName;

    // Define the toggles used in the multiple choice mode
    public Toggle firstAnswerCorrect;
    public Toggle secondAnswerCorrect;
    public Toggle thirdAnswerCorrect;
    public Toggle fourthAnswerCorrect;
    public Toggle fifthAnswerCorrect;

    // Define the error texts that need to be displayed if an input field is empty
    public TextMeshProUGUI errorNoQuestionInput;
    public TextMeshProUGUI errorNoAnswerInput;

    // Define the lists that will contain all exercises
    public List<InputQuestion> listOfInputExercises;
    public List<MultipleChoiceQuestion> listOfMultipleChoiceExercises;

    // The JSON Serialization for the input questions
    [Serializable]
    public class InputQuestion
    {
        public string exerciseName;
        public string type = "InputQuestion";
        public string name;
        public string question;
        public string answer;
    }

    // The JSON Serialization for the multiple choice questions
    [Serializable]
    public class MultipleChoiceQuestion
    {
        public string exerciseName;
        public string type = "MultipleChoiceQuestion";
        public string name;
        public string question;
        public int numberOfAnswers;
        public string answer1;
        public string answer2;
        public string answer3;
        public string answer4;
        public string answer5;
        public bool answer1Correct;
        public bool answer2Correct;
        public bool answer3Correct;
        public bool answer4Correct;
        public bool answer5Correct;
    }

    // Still need to save the 3D models somehow
    // The exercise name is used to link the questions to the 3D models
    // The names are the names of the files that are uploaded by the user
    [Serializable]
    public class ModelsCollection
    {
        public string exerciseName;
        public int numberOfModels;
        public string nameFirstModel;
        public string nameSecondModel;
        public string nameThirdModel;
        public string nameFourthModel;
        public string nameFifthModel;
    }

    // This is the class that groups everything
    // public class ExerciseCollection
    // {
    //     public List<InputQuestion> inputQuestions;
    //     public List<MultipleChoiceQuestion> multipleChoiceQuestions;
    //     public List<ModelsCollection> modelCollection;

    // }


    // Start is called before the first frame update
    void Start()
    {
        // Disable all buttons that cannot be used at the begining
        GameObject.Find("Add3DModel2").GetComponent<Button>().interactable = false;
        GameObject.Find("Add3DModel3").GetComponent<Button>().interactable = false;
        GameObject.Find("Add3DModel4").GetComponent<Button>().interactable = false;
        GameObject.Find("Add3DModel5").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion1").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion2").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion3").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion4").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion5").GetComponent<Button>().interactable = false;

        // Set last and current menu
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainCreator;

        // Set the current question index
        Menus.currentInputQuestionIndex = 0;
        Menus.currentMultipleChoiceQuestionIndex = 0;

        // Set the current exercise index
        Menus.currentExerciseIndex = 0;

        // Initialize the save path. This will have to be saved later on
        string scriptPath = GetCurrentFilePath();
        Menus.savePath = GetPathToRootDirectory(scriptPath);

        // Initialize the collection
        // Menus.collection = new ExerciseCollection();
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
        string rootDirectoryPath = Path.GetFullPath(Path.Combine(rootPath, @"Backend\Save\"));
        return rootDirectoryPath;
    }

    // Method used to enter the creator
    public void EnterCreator() 
    {
        // Set the right exercise name
        Menus.currentExerciseName = "exerciseName" + Menus.currentExerciseIndex;
    }

    // Method that activates everything for the creator menu
    public void ActivateCreatorMenu()
    {
        mainCreator.SetActive(true);
        mainMenu.SetActive(false);
        Menus.currentMenu = mainCreator;
        Menus.lastMenu = mainMenu;
        Debug.Log(Menus.currentMenu);
    }

    // Method that deactivates everything for the creator menu
    public void DeactivateCreatorMenu()
    {
        mainCreator.SetActive(false);
        mainMenu.SetActive(true);
        Menus.currentMenu = mainCreator;
        Menus.lastMenu = mainMenu;
        Debug.Log(Menus.currentMenu);
    }

    // Method that activates everything for the multiple choice mode
    public void ActivateMultipleChoiceMode()
    {
        veilLargeWindow.SetActive(true);
        multipleChoiceCreator.SetActive(true);
        Menus.currentMenu = multipleChoiceCreator;
        Menus.lastMenu = mainCreator;
        Debug.Log(Menus.currentMenu);
    }

    // Method that deactivates everything for the multiple choice mode
    public void DeactivateMultipleChoiceMode()
    {
        veilLargeWindow.SetActive(false);
        multipleChoiceCreator.SetActive(false);
        Menus.currentMenu = mainCreator;
        Menus.lastMenu = mainMenu;
    }

    // Method that activates everything for the input mode
    public void ActivateInputMode()
    {
        veilLargeWindow.SetActive(true);
        inputModeCreator.SetActive(true);
        Menus.currentMenu = inputModeCreator;
        Menus.lastMenu = mainCreator;
    }

    // Method that deactivates everything for the input mode
    public void DeactivateInputMode()
    {
        veilLargeWindow.SetActive(false);
        inputModeCreator.SetActive(false);
        Menus.currentMenu = mainCreator;
        Menus.lastMenu = mainMenu;
    }

    // Method that activates everything for the exit without saving window
    public void ActivateExitWithoutSaveWindow()
    {
        exitWithoutSavingWindow.SetActive(true);
        veilSmallWindow.SetActive(true);
    }

    // Method that deactivates everything for the exit without saving window
    public void DeactivateExitWithoutSaveWindow()
    {
        exitWithoutSavingWindow.SetActive(false);
        veilSmallWindow.SetActive(false);
    }

    // Method activated when clicking on the "Add" button in the creator screen, opens the right exercise creator window
    public void AddExercise()
    {
        // Enabling the right creator window depending on which mode was chosen
        if(GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn == true)
        {
            Debug.Log("Multiple choice is on!");
            ActivateMultipleChoiceMode();
        } else {
            Debug.Log("Input mode is on!");
            ActivateInputMode();
        }
    }

    // Method that summons the "are you sure you do not want to save" window if something was already done
    public void ExitWithoutSaving(bool isEmpty)
    {
        // First check if something was added
        if(isEmpty == true)
        {
            // Activate the exit without save window
            Debug.Log("Not empty!");
            ActivateExitWithoutSaveWindow();
        } else {
            // Everything is empty, so reset the menus
            Debug.Log("Nothing to save!");
            Menus.lastMenu.SetActive(true);
            Menus.currentMenu.SetActive(false);
            veilLargeWindow.SetActive(false);

            // Since this is only used to exit specialized exercises creator, the last menu is always the main creator
            // The currentMenu does not go to the mainMenu, so that it does not need to be set when returning in the creator menu
            Menus.currentMenu = mainCreator;
            Menus.lastMenu = mainMenu;
        }
    }

    public void ExitCreator()
    {
        // First check if something was added and save this information in a boolean variable
        bool isEmpty = (GameObject.Find("Add3DModel1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != "+" || GameObject.Find("PreviewQuestion1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != "");
        Debug.Log(isEmpty);

        // Call the exit without saving function
        ExitWithoutSaving(isEmpty);
    }

    // Method to navigate from the multiple choice mode back to the main creator
    // For now it only checks if the answers are not filled, ignores the question, can be improved TODO
    public void GetBackFromMultipleChoice()
    {
        // First check if something was added and save this information in a boolean variable
        bool isEmpty = (enterFirstAnswer.text != "" || enterSecondAnswer.text != "" || enterQuestionMultiple.text != "");
        Debug.Log(isEmpty);

        // Call the exit without saving function
        ExitWithoutSaving(isEmpty);
    }

    // Method to navigate from the input mode back to the main creator
    public void GetBackFromInputMode()
    {
        // First check if something was added and save this information in a boolean variable
        bool isEmpty = (enterQuestionInput.text != "" || enterAnswerInput.text != "");
        Debug.Log(isEmpty);

        // Call the exit without saving function
        ExitWithoutSaving(isEmpty);
    }

    // Method that saves the question and answer of an input question, for now only creates a debug log
    // Is triggered when the user clicks on the "create" button
    public void SaveInputQuestion()
    {
        // First check if every information was typed in
        if(enterQuestionInput.text != "" && enterAnswerInput.text != "")
        {
            // Case both fields contain characters, disable the error messages
            // The saving needs to be done here TODO
            Debug.Log(enterQuestionInput.text);
            Debug.Log(enterAnswerInput.text);
            errorNoQuestionInput.gameObject.SetActive(false);
            errorNoAnswerInput.gameObject.SetActive(false);
        } else {
            // Case question and answer were not typed in, display error messages
            if(enterQuestionInput.text == "" && enterAnswerInput.text == "") {
                errorNoQuestionInput.gameObject.SetActive(true);
                errorNoAnswerInput.gameObject.SetActive(true);

            // Case question is not typed in, display error message
            } else if(enterAnswerInput.text == "" && enterQuestionInput.text != "") {
                errorNoAnswerInput.gameObject.SetActive(true);
                errorNoQuestionInput.gameObject.SetActive(false);

            // Case answer is not typed in, display error message
            } else {
                errorNoAnswerInput.gameObject.SetActive(false);
                errorNoQuestionInput.gameObject.SetActive(true);
            }
        }
    }

    // Method to exit a exercise creation without saving
    // It is needed to get access to the current menu / window, and delete everything that was entered
    public void ExitWithoutSavingYes()
    {
        Debug.Log("Entered ExitWithoutSavingYes method");

        // Case main creator menu is the current menu
        if(Menus.currentMenu == mainCreator) {

            Debug.Log("the current menu is the main creator");
            Debug.Log(Menus.currentMenu);
            Debug.Log(Menus.lastMenu);

            // Reset the buttons that preview names of 3D models and currently created questions
            GameObject.Find("Add3DModel1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "+";
            GameObject.Find("Add3DModel2").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("Add3DModel3").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("Add3DModel4").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("Add3DModel5").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion2").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion3").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion4").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion5").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";

            // Reset the toggle, check the one that is selected as default, because it is a toggle group the other is set to false automatically
            GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn = true;

            // Reset the interactability of the buttons
            GameObject.Find("Add3DModel2").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel3").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel4").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel5").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion1").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion2").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion3").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion4").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion5").GetComponent<Button>().interactable = false;

            // Then it is needed to set the right windows as current menu and deactivate / activate the right menus
            DeactivateCreatorMenu();
            DeactivateExitWithoutSaveWindow();

        // Case input mode creator
        } else if(Menus.currentMenu == inputModeCreator) {

            Debug.Log("Current Menu is input mode");
            // First delete everything that was entered
            enterQuestionInput.text = "";
            enterAnswerInput.text = "";

            // Then it is needed to set the right windows as current menu and deactivate / activate the right menus
            DeactivateInputMode();
            DeactivateExitWithoutSaveWindow();

        // Case multiple choice mode creator
        } else if(Menus.currentMenu == multipleChoiceCreator) {

            Debug.Log("Current menu is multiple choice");

            // First delete everything that was entered
            enterQuestionMultiple.text = "";

            // Reset the text that was typed in
            enterFirstAnswer.text = "";
            enterSecondAnswer.text = "";
            enterThirdAnswer.text = "";
            enterFourthAnswer.text = "";
            enterFifthAnswer.text = "";

            // Reset the toggles
            GameObject.Find("Answer1Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer2Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer3Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer4Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer5Correct").GetComponent<Toggle>().isOn = false;

            // Reset the interactability of buttons
            GameObject.Find("AddAnswer4").GetComponent<Button>().interactable = false;
            GameObject.Find("AddAnswer5").GetComponent<Button>().interactable = false;

            // Then it is needed to set the right windows as current menu and deactivate / activate the right menus
            DeactivateMultipleChoiceMode();
            DeactivateExitWithoutSaveWindow();
        }
    }

    // Method that is executed when a user clicks on the "OK" button that names an exercise. The window is used for all question types, so an if(currentMenu?) is needed.
    // public void CreateQuestion() 
    // {

    //     if(Menus.currentMenu == inputModeCreator) 
    //     {
    //         CreateInputQuestion();
    //     }
    // }

    // // Method that creates an input question
    // public void CreateInputQuestion()
    // {
    //     // Save everything in an empty spot of the array in the exercise collection
    //     //InputQuestion questionName = new InputQuestion();
    //     collection.inputQuestions[Menus.currentInputQuestionIndex].exerciseName = Menus.currentExerciseName;
    //     collection.inputQuestions[Menus.currentInputQuestionIndex].question = enterQuestionInput.text;
    //     collection.inputQuestions[Menus.currentInputQuestionIndex].answer = enterAnswerInput.text;
    //     collection.inputQuestions[Menus.currentInputQuestionIndex].name = enterName.text;

    //     // Increment the index so that no two objects have the same name
    //     Menus.currentInputQuestionIndex = Menus.currentInputQuestionIndex + 1;
    // }

    // public void CreateMultipleChoiceQuestion() 
    // {
    //     // First generate a unique question object name
    //     //string name = "question" + Menus.currentMultipleChoiceQuestionIndex;

    //     // Create the question object and set its values
    //     //MultipleChoiceQuestion questionName = new MultipleChoiceQuestion();

    //     // Set the name and the question
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].name = enterName.text;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].question = enterQuestionMultiple.text;

    //     // Set the number of answers integer, will be helpfull when displaying the question later
    //     if(GameObject.Find("AddAnswer3").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text == "+") {
    //         collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].numberOfAnswers = 2;
    //     } else if(GameObject.Find("AddAnswer4").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text == "") {
    //         collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].numberOfAnswers = 3;
    //     } else if(GameObject.Find("AddAnswer5").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text == "") {
    //         collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].numberOfAnswers = 4;
    //     } else {
    //         collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].numberOfAnswers = 5;
    //     }
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer1 = enterFirstAnswer.text;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer2 = enterSecondAnswer.text;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer3 = enterThirdAnswer.text;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer4 = enterFourthAnswer.text;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer5 = enterFifthAnswer.text;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer1Correct = firstAnswerCorrect.isOn;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer2Correct = secondAnswerCorrect.isOn;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer3Correct = thirdAnswerCorrect.isOn;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer4Correct = fourthAnswerCorrect.isOn;
    //     collection.multipleChoiceQuestions[Menus.currentMultipleChoiceQuestionIndex].answer5Correct = fifthAnswerCorrect.isOn;
    // }

    // public void CreateQuestionCollection()
    // {
    //     // First create the models collection

    //     // First assign the name that will be the link between the questions and the 3D models

    //     // Add all exercises to the collection
    //     for(int counter = 0; counter < Menus.currentQuestionIndex - 1; counter = counter + 1)
    //     {
    //         // Get the name of the object
    //         //string questionName = "question" + counter;

    //         // Discover if the current question is an input or a multiple choice question
    //         if(collection.multipleChoiceQuestions[counter].type == "inputQuestion")
    //         {
    //             // Save it in the input question list of the collection
    //             collection.inputQuestions.append(collection.multipleChoiceQuestions[counter]);
    //         } else {
    //             // Save it in the multiple choice question list
    //             collection.multipleChoiceQuestions.append(collection.multipleChoiceQuestions[counter]);
    //         }
    //     }

    //     // Convert the everything to a JSON file
    //     string json = JsonUtility.ToJson(collection);
    // }

    // // Create the model collection
    // public void ModelCollection()
    // {
    //     //
    // }

    // Method that saves a question
    // public void SaveAsJson(InputQuestion obj)
    // {
    //     // Convert to json
    //     string json = JsonUtility.ToJson(obj);

    //     //write string to file
    //     System.IO.File.WriteAllText(Menus.savePath+"SaveQuestion"+Menus.saveIndex, json);

    // }

    // // Method that loads the question and transform it from a json string to a object of the right type
    // public T LoadObject<T>(string path)
    // {
    //     string json =  File.ReadAllText(path);
    //     string myObject = JsonUtility.FromJson<T>(json);
    //     return JsonUtility.FromJson<T>(myObject);
    // }

    // // Test method
    // public void TestSavingAndLoading()
    // {
    //     InputQuestion questionName = new InputQuestion();
    //     questionName.exerciseName = Menus.currentExerciseName;
    //     questionName.name = "The first question";
    //     questionName.question = "What is the first question?";
    //     questionName.answer = "This one!";
    //     SaveAsJson(questionName);
    //     InputQuestion inputquestion = LoadObject<InputQuestion>(Menus.savePath+SaveQuestion+Menus.saveIndex);
    //     Debug.Log(inputquestion.exerciseName);
    //     Debug.Log(inputquestion.type);
    //     Debug.Log(inputquestion.name);
    //     Debug.Log(inputquestion.question);
    //     Debug.Log(inputquestion.answer);
    // }
}
