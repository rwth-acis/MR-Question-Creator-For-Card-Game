using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

// TODO: create a way for users to re-access questions they created through the preview
// TODO: create the save multiple question method
// TODO: create the next and previous page methods in the creator menu for the display of exercises

static class Menus
{
    // Save current and last menu for return function
    public static GameObject lastMenu;
    public static GameObject currentMenu;

    // Save the current index for all question or moddel collection, this is for the end array
    public static int inputQuestionIndex; // Used to access the right position in the array of the input questions
    public static int multipleChoiceQuestionIndex; // Used to access the right position in the array of the input questions

    // // Save the current index for all question or moddel collection, this is for the arrays before the creation of the end array
    // public static int currentInputQuestionIndex; // Used to access the right position in the array of the input questions
    // public static int currentMultipleChoiceQuestionIndex; // Used to access the right position in the array of the input questions

    // Save an index and exercise name to be able to link different questions and models together
    public static int currentExerciseIndex; // Used to generate correct exercise names
    public static string currentExerciseName;

    // Save an index for the current question
    public static int currentQuestionIndex;

    // Save an index for the current displayed page of the displayed questions
    public static int currentPage;
    public static int numberOfPages;

    // Path to the save directory in the back end
    public static string savePath; // Path to the save directory in the back end
    public static string saveIndex;

    // Number used to create new names for files
    public static int questionNumber;
    public static bool editModeOn;

    // The temporary save path
    public static string tempSavePath;
    
    // Set the maximum of questions of each type, and model sets
    public static int maxNumber = 100;
}

public class Creator : MonoBehaviour
{
    // Defining the necessary menus and windows
    public GameObject mainMenu;
    public GameObject mainCreator;
    public GameObject browsDirectoriesMenu;
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
    // The read only input field that displays the current save path
    public TMP_InputField savePathText;

    // Defining the button
    public Button selectButton;
    public Button previewQuestion1;
    public Button previewQuestion2;
    public Button previewQuestion3;
    public Button previewQuestion4;
    public Button previewQuestion5;
    public Button nextEnabled;
    public Button nextDisabled;
    public Button previousEnabled;
    public Button previousDisabled;
    public Button changeInput;
    public Button createInput;
    public Button changeMultipleChoice;
    public Button createMultipleChoice;

    // Define the toggles used in the multiple choice mode
    public Toggle firstAnswerCorrect;
    public Toggle secondAnswerCorrect;
    public Toggle thirdAnswerCorrect;
    public Toggle fourthAnswerCorrect;
    public Toggle fifthAnswerCorrect;

    // Define the error texts that need to be displayed if an input field is empty
    public TextMeshProUGUI errorNoQuestionInput;
    public TextMeshProUGUI errorNoAnswerInput;
    public TextMeshProUGUI headingPageNumber;

    // Define the lists that will contain all exercises
    public List<InputQuestion> listOfInputExercises;
    public List<MultipleChoiceQuestion> listOfMultipleChoiceExercises;

    // The JSON Serialization for the input questions
    [Serializable]
    public class InputQuestion
    {
        public string exerciseType = "input question";
        public string exerciseName;
        public string name;
        public string question;
        public string answer;
    }

    // The JSON Serialization for the multiple choice questions
    [Serializable]
    public class MultipleChoiceQuestion
    {
        public string exerciseType = "multiple choice question";
        public string exerciseName;
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
    public class Models
    {
        public string exerciseName;
        public int numberOfModels;
        public string nameFirstModel;
        public string nameSecondModel;
        public string nameThirdModel;
        public string nameFourthModel;
        public string nameFifthModel;
    }

    // This is the class that groups everything that is later saved in the json file
    // There will be a maximum of 100 different models sets, multiple choice questions and input questions
    public class ExerciseCollection
    {
        public int indexInputQuestion = 0;
        public int indexMultipleChoiceQuestion = 0;
        public int indexModelQuestion = 0;
        public InputQuestion[] inputQuestions = new InputQuestion[Menus.maxNumber];
        public MultipleChoiceQuestion[] multipleChoiceQuestions = new MultipleChoiceQuestion[Menus.maxNumber];
        public Models[] models = new Models[Menus.maxNumber];

    }

    // Array that stores the input questions until they are all created and the user creates the end exercise
    public class InputQuestionCollection
    {
        public InputQuestion[] inputQuestionCollection = new InputQuestion[50];

    }

    // Array that stores the multiple choice questions until they are all created and the user creates the end exercise
    public class MultipleChoiceCollection
    {
        public MultipleChoiceQuestion[] multipleChoiceQuestionCollection = new MultipleChoiceQuestion[50];

    }

    // Array that stores the models until they are all created and the user creates the end exercise
    public class ModelsCollection
    {
        public Models[] modelsCollections = new Models[1];

    }


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

        // Set the current question and model index
        Menus.inputQuestionIndex = 0;
        Menus.multipleChoiceQuestionIndex = 0;

        // Set the current exercise index
        Menus.currentExerciseIndex = 0;

        // Initialize the save path. This will have to be saved later on
        string scriptPath = GetCurrentFilePath();
        Menus.tempSavePath = GetPathToRootDirectory(scriptPath);

        // Initialize the collection
        // Menus.collection = new ExerciseCollection();

        Menus.questionNumber = 0;
        Menus.currentPage = 1;
        Menus.numberOfPages = 1;
        Menus.editModeOn = false;
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
        string rootDirectoryPath = Path.GetFullPath(Path.Combine(rootPath, @"Backend\TempSave\"));
        return rootDirectoryPath;
    }

    // Method used to enter the creator
    public void EnterCreator() 
    {
        // Set the right exercise name
        Menus.currentExerciseName = "exerciseName" + Menus.currentExerciseIndex;
        Menus.currentExerciseIndex = Menus.currentExerciseIndex + 1;
        // Set the right page as current
        Menus.currentPage = 1;
        // Menus.currentInputQuestionIndex = 0;
        // Menus.currentMultipleChoiceQuestionIndex = 0;
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

    // Method that activates everything for the naming window
    public void ActivateNamingWindow()
    {
        enterNameWindow.SetActive(true);
        veilSmallWindow.SetActive(true);
    }

    // Method that deactivates everything for the exit without saving window and resets the name that was typed in
    public void DeactivateNamingWindow()
    {
        enterNameWindow.SetActive(false);
        veilSmallWindow.SetActive(false);
        enterName.text = "";
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

    // Method that is activated when a user clicks on the "create" button for the input question
    public void ProceedToSaveInput()
    {
        // Disable both error texts
        errorNoQuestionInput.gameObject.SetActive(true);
        errorNoAnswerInput.gameObject.SetActive(true);

        // Case both fields contain characters, disable the error messages
        if(enterQuestionInput.text != "" && enterAnswerInput.text != "")
        {
            ActivateNamingWindow();
        } else {
            // If the question is not typed in, display the error messages for the question
            if(enterQuestionInput.text == "") {
                errorNoQuestionInput.gameObject.SetActive(true);

            // If the answer is not typed in, display the error messages for the answer
            }
            if(enterAnswerInput.text == "") {
                errorNoAnswerInput.gameObject.SetActive(true);
            }
        }
    }

    // Method that displays the name of the question in the creator menu
    public void DisplayName(string name)
    {
        // Get the name that needs to be displayed in the question preview
        string displayedName;
        if(name == "")
        {
            displayedName = "empty name";
        } else {
            displayedName = name;
        }

        // Display the name of the question in the creator menu
        // If it should be displayed on the current page, then display it
        if((Menus.currentPage - 1) * 5 <= Menus.currentQuestionIndex && Menus.currentQuestionIndex < Menus.currentPage * 5)
        {
            // Get the concerned button
            Button rightButton = GetRightButton(Menus.currentQuestionIndex);

            // Change the name and enable it
            rightButton.GetComponentInChildren<TMP_Text>().text = displayedName;
            rightButton.interactable = true;
        }
    }

    // Method that returns you the right button given the index
    public Button GetRightButton(int questionIndex)
    {
        int index = questionIndex - (Menus.currentPage - 1) * 5;
        // Display it at the right place
        switch(index)
        {
            case 0:
                return previewQuestion1;
            break;
            case 1:
                return previewQuestion2;
            break;
            case 2:
                return previewQuestion3;
            break;
            case 3:
                return previewQuestion4;
            break;
            case 4:
                return previewQuestion5;
            break;
            default:
                return previewQuestion1;
            break;
        }
    }

    // Method that returns the right number of the question (used to name the .json files)
    public string ReturnQuestionIndex(int index)
    {
        string number;
        if(index < 10){
            // case two zero then the number to have 00X
            number = "00" + Convert.ToString(index);
        } else if(index < 100){
            // Case one zero then the number to have 0XY
            number = "0" + Convert.ToString(index);
        } else {
            // Case no zero, only the number to have XYZ
            number =  Convert.ToString(index);
        }
        return number;
    }

    // Method that saves the question, answer and name of an input question when the naming window is displayed and user clicks on the create button
    public void SaveInputQuestion()
    {
        // First enter the information that is already known
        InputQuestion inputQuestion = new InputQuestion();
        inputQuestion.exerciseName = Menus.currentExerciseName;
        inputQuestion.question = enterQuestionInput.text;
        inputQuestion.answer = enterAnswerInput.text;

        // Then check if the name is empty or not
        if(enterName.text != "")
        {
            // Case custom name was entered
            inputQuestion.name = enterName.text;
        } else {
            // Case custom name was not entered, create an empty name (will not be displayed in the game)
            inputQuestion.name = "";
        }
        Debug.Log(inputQuestion.name);
        Debug.Log(inputQuestion.question);
        Debug.Log(inputQuestion.answer);

        // Save it in the temp file (since the path can be changed anytime, we don't want to copy it everytime)
        string json = JsonUtility.ToJson(inputQuestion);
        string index = ReturnQuestionIndex(Menus.currentQuestionIndex);
        Debug.Log(json);
        Debug.Log(index);
        Debug.Log(Menus.tempSavePath + "Question" + index + ".json");
        File.WriteAllText(Menus.tempSavePath + "Question" + index + ".json", json);

        // Display the question name in the question preview on the creator menu
        DisplayName(inputQuestion.name);
    }

    // Method that saves the question, answer and name of an input question when the naming window is displayed and user clicks on the create button
    public void SaveMultipleChoiceQuestion()
    {
        // First enter the information that is already known
        MultipleChoiceQuestion multipleChoiceQuestion = new MultipleChoiceQuestion();
        multipleChoiceQuestion.exerciseName = Menus.currentExerciseName;
        multipleChoiceQuestion.question = enterQuestionMultiple.text;
        multipleChoiceQuestion.answer1 = enterFirstAnswer.text;
        multipleChoiceQuestion.answer2 = enterSecondAnswer.text;

        // Initialize the number of answers
        int answerCounter = 2;

        // Set the other answers if they exist. Find the number of existing answers
        if(multipleChoiceQuestion.answer3 != null)
        {
            multipleChoiceQuestion.answer3 = enterThirdAnswer.text;
            answerCounter = answerCounter + 1;
        }
        if(multipleChoiceQuestion.answer4 != null)
        {
            multipleChoiceQuestion.answer4 = enterFourthAnswer.text;
            answerCounter = answerCounter + 1;
        }
        if(multipleChoiceQuestion.answer5 != null)
        {
            multipleChoiceQuestion.answer5 = enterFifthAnswer.text;
            answerCounter = answerCounter + 1;
        }
        
        // Set the final number of answers
        multipleChoiceQuestion.numberOfAnswers = answerCounter;

        // Then check if the name is empty or not
        if(enterName.text != "")
        {
            // Case custom name was entered
            multipleChoiceQuestion.name = enterName.text;
        } else {
            // Case custom name was not entered, create an empty name (will not be displayed in the game)
            multipleChoiceQuestion.name = "";
        }

        // Set the toggles on the right values
        multipleChoiceQuestion.answer1Correct = firstAnswerCorrect.isOn;
        multipleChoiceQuestion.answer2Correct = secondAnswerCorrect.isOn;
        multipleChoiceQuestion.answer3Correct = thirdAnswerCorrect.isOn;
        multipleChoiceQuestion.answer4Correct = fourthAnswerCorrect.isOn;
        multipleChoiceQuestion.answer5Correct = fifthAnswerCorrect.isOn;

        // Save it in the temp file (since the path can be changed anytime, we don't want to copy it everytime)
        string json = JsonUtility.ToJson(multipleChoiceQuestion);
        string index = ReturnQuestionIndex(Menus.currentQuestionIndex);
        Debug.Log(json);
        Debug.Log(index);
        Debug.Log(Menus.tempSavePath + "Question" + index + ".json");
        File.WriteAllText(Menus.tempSavePath + "Question" + index + ".json", json);

        // Display the question name in the question preview on the creator menu
        DisplayName(multipleChoiceQuestion.name);
    }

    // Method that is activated when a user names a question. Since the window is unique and is used for all questions, the right save method has to be started
    public void SaveQuestion()
    {
        // Case it was the input mode creator that was active in the background
        if(Menus.currentMenu == inputModeCreator){
            Debug.Log("Input creator was in the background!");
            SaveInputQuestion();
            Debug.Log("Save input question was successfully completed!");

        // Case it was the multiple choice creator that was active in the background
        } else if(Menus.currentMenu == multipleChoiceCreator){
            SaveMultipleChoiceQuestion();
        }

        // Then desactivate the current mode and delete everything that was written. For this the method exit without saving YES can be reused.
        ExitWithoutSavingYes();

        // Deactivate the naming window
        DeactivateNamingWindow();

        // Actualize the number of pages
        ActualizePageNumber();

        // Enable the right buttons
        ActualizeButtons();

        // Increase the current question index by one
        Menus.currentQuestionIndex = Menus.currentQuestionIndex + 1;
    }

    // Method that enable or disable the next and previous buttons of the preview created questions section
    public void ActualizeButtons()
    {
        // Display the right next button
        if(Menus.currentPage < Menus.numberOfPages)
        {
            nextEnabled.gameObject.SetActive(true);
            nextDisabled.gameObject.SetActive(false);
        } else {
            nextEnabled.gameObject.SetActive(false);
            nextDisabled.gameObject.SetActive(true);
        }

        // Display the right previous button
        if(Menus.currentPage > 1){
            previousEnabled.gameObject.SetActive(true);
            previousDisabled.gameObject.SetActive(false);
        } else {
            previousEnabled.gameObject.SetActive(false);
            previousDisabled.gameObject.SetActive(true);
        }
    }

    // Method that is used to actualize the page number
    public void ActualizePageNumber()
    {
        double value = (double) (Menus.currentQuestionIndex + 1) / (double) 5;
        Debug.Log(Menus.currentQuestionIndex);
        Debug.Log(value);
        Menus.numberOfPages = System.Convert.ToInt32(System.Math.Ceiling(value));
        Debug.Log(Menus.numberOfPages);
        if(Menus.numberOfPages > 1){
            // Case there is more than one page
            headingPageNumber.text = "Page " + Menus.currentPage + "/" + Menus.numberOfPages;
        } else {
            // Case there are no directories, but a page still needs to be displayed
            headingPageNumber.text = "Page " + Menus.currentPage + "/" + 1;
        }
    }

    // Method to get to the next page of previews of questions that were created
    public void NavigateNext()
    {
        // Increase the index of the current page
        Menus.currentPage = Menus.currentPage + 1;

        // Actualize the heading
        headingPageNumber.text = "Page " + Menus.currentPage + "/" + Menus.numberOfPages;

        // Enable / Disable the right buttons
        ActualizeButtons();

        // Actualize the names displayed on the preview buttons
        PreviewCreatedQuestions();
    }

    // Method to get to the previous page of previews of questions that were created
    public void NavigatePrevious()
    {
        // Increase the index of the current page
        Menus.currentPage = Menus.currentPage - 1;

        // Actualize the heading
        headingPageNumber.text = "Page " + Menus.currentPage + "/" + Menus.numberOfPages;

        // Enable / Disable the right buttons
        ActualizeButtons();

        // Actualize the names displayed on the preview buttons
        PreviewCreatedQuestions();

    }

    // Method that displays the right names on the preview buttons
    public void PreviewCreatedQuestions()
    {
        // Get the number of questions that need to be displayed in the preview
        int numberToDisplay = 0;
        if(Menus.currentPage != Menus.numberOfPages)
        {
            numberToDisplay = 5;
        } else {
            numberToDisplay = (Menus.currentQuestionIndex) - (Menus.numberOfPages - 1) * 5;
        }
        
        // Get the first index
        int firstIndex = (Menus.currentPage - 1) * 5;

        // Display the right exercise names for all exercises that were created and that should be displayed on this page
        for(int questionNumber = 0; questionNumber < numberToDisplay; questionNumber = questionNumber + 1)
        {
            // Get the right name of file
            string index = ReturnQuestionIndex(firstIndex + questionNumber);

            // Get the right button
            Button rightButton = GetRightButton(firstIndex + questionNumber);

            string path = Menus.tempSavePath + "Question" + index + ".json";

            // Get the right file, if it was created write "deleted" and disable the button. If it was not created, disable the button and write nothing
            // Case the file exist, then get its name, and display it
            if (File.Exists(path))
            {
                // Extract the string
                string json = File.ReadAllText(path);

                // Check what type it is, if input question or multiple choice
                if(json.Contains("input question") == true)
                {
                    // Case input question
                    InputQuestion question = JsonUtility.FromJson<InputQuestion>(json);

                    // Rename the button
                    rightButton.GetComponentInChildren<TMP_Text>().text = question.name;
                    rightButton.interactable = true;

                } else {
                    // Case multiple choice question
                    MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(json);

                    // Rename the button
                    rightButton.GetComponentInChildren<TMP_Text>().text = question.name;
                    rightButton.interactable = true;
                }
            } else {

                // Rename the button in "deleted"
                rightButton.GetComponentInChildren<TMP_Text>().text = "deleted";
                rightButton.interactable = false;
            }
        }

        // For the other buttons that should be on this page, disable the buttons
        for(int buttonNumber = numberToDisplay; buttonNumber < 5; buttonNumber = buttonNumber + 1)
        {
            // First get the right button
            Button rightButton = GetRightButton(firstIndex + buttonNumber);

            // Delete the name of the button and disable it
            rightButton.GetComponentInChildren<TMP_Text>().text = "";
            rightButton.interactable = false;
        }
    }

    // Method that permitts to enter edit mode again on a certain question that is selected
    public void EnterEditMode()
    {
        // Get the button name
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        Button currentButton = GameObject.Find(buttonName).GetComponent<Button>();
        Debug.Log(buttonName);

        // Get the name of the file
        string fileName = GetFileNameFromButtonName(buttonName);

        // Extract the string
        string json = File.ReadAllText(Menus.tempSavePath + fileName + @"\");
        //string json = File.ReadAllText(Menus.tempSavePath + fileName);

        // get the Button

        // Check what type it is, if input question or multiple choice
        if(json.Contains("input question") == true)
        {
            // Case input question
            InputQuestion question = JsonUtility.FromJson<InputQuestion>(json);

            // Open the input mode window, copy the content back in
            EnableEditModeInput(question);

        } else {

            // Case multiple choice question
            MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(json);
        }

    }

    // Method that enables the edit mode for input
    public void EnableEditModeInput(InputQuestion question)
    {
        // Activate input mode
        ActivateInputMode();

        // Activate the "change" instead of "create" button
        changeInput.gameObject.SetActive(true);
        createInput.gameObject.SetActive(false);

        // Copy the answers that were already set back in
        enterQuestionInput.text = question.question;
        enterAnswerInput.text = question.answer;
        enterName.text = question.name;

    }

    // Method that returns you the name of the save file when giving the button name in
    public string GetFileNameFromButtonName(string buttonName)
    {
        // First get the index inside the page
        int indexOnPage = 0;

        switch(buttonName)
        {
            case "PreviewQuestion1":
                indexOnPage = 0;
            break;
            case "PreviewQuestion2":
                indexOnPage = 1;
            break;
            case "PreviewQuestion3":
                indexOnPage = 2;
            break;
            case "PreviewQuestion4":
                indexOnPage = 3;
            break;
            case "PreviewQuestion5":
                indexOnPage = 4;
            break;
        }

        // Then get the overall Index
        int index = (Menus.currentPage - 1) * 5 + indexOnPage;

        // Get the question index string that is the ending of the save file
        string ending = ReturnQuestionIndex(index);

        // Transform that in the file name and return it
        string name = "Question" + ending + ".json";
        return name;
    }


    // Method that creates the save file
    // public void CreateSaveFile()
    // {
    //     // First create the save file
    // }

    // Method that extracts all entries out of the json file, then adds the new entry and saves the file again
    // public void SaveInputQuestionInSaveFile(InputQuestion question)
    // {
    //     // First, check if the save file exist or not. If it does not exist, create a file with the initial array size
    //     if(!File.Exists(Menus.savePath))
    //     {
    //     ExerciseCollection oldCollection = new ExerciseCollection();
    //     oldCollection.indexInputQuestion = 0;
    //     oldCollection.indexMultipleChoiceQuestion = 0;
    //     oldCollection.indexModelQuestion = 0;
    //     oldCollection.inputQuestions[] = new InputQuestion[collection.indexInputQuestion];
    //     oldCollection.multipleChoiceQuestions[] = new MultipleChoiceQuestion[collection.indexMultipleChoiceQuestion];
    //     oldCollection.models[] = new Models[collection.indexModelQuestion];
    //     } else {
    //         // Extract the written text of the save file
    //         string oldSave = File.ReadAllText(Menus.savePath); // need to add an encoding

    //         // Convert it to an exercise collection
    //         ExerciseCollection oldCollection = JsonUtility.FromJson(oldSave);
        
    //     }

    //     // Then create the new collection
    //     ExerciseCollection newCollection = new ExerciseCollection();
    //     collection.indexInputQuestion = oldCollection.indexInputQuestion + 1;
    //     collection.indexMultipleChoiceQuestion = oldCollection.indexMultipleChoiceQuestion;
    //     collection.indexModelQuestion = oldCollection.indexModelQuestion;
    //     collection.inputQuestions[] = new InputQuestion[collection.indexInputQuestion];
    //     collection.multipleChoiceQuestions[] = oldCollection.multipleChoiceQuestions[];
    //     collection.models[] = oldCollection.modelsCollection[];

    //     // Copy the whole array that existed in the input question collection of the old collection

    //     // Copy the new question at the right place in the array
    //     collection.InputQuestions[collection.indexInputQuestion] = question;

    //     // Convert it to json again
    //     string newSave = JsonUtility.ToJson(collection);

    //     // Save it in the File again
    //     File.WriteAllText(Menus.savePath, newSave); 

    //     // Increase the index of the input questions
    //     collection.indexInputQuestion = collection.indexInputQuestion + 1;
    // }

    // public void SaveInputQuestionInSaveFile(InputQuestion question)
    // {
    //     if(!File.Exists(Menus.savePath + "save5"))
    //     {
    //         // Case there is no file, so no data to extract, so create a new exercise collection
    //         ExerciseCollection oldCollection = new ExerciseCollection();
    //     } else {
    //         // Extract the written text of the save file
    //         string oldSave = File.ReadAllText(Menus.savePath + "save5"); // need to add an encoding

    //         // Convert it to an exercise collection
    //         ExerciseCollection oldCollection = new ExerciseCollection();
    //         oldCollection = JsonUtility.FromJson(oldSave);
    //     }

    //     // Copy the new question at the right place in the array
    //     oldCollection.InputQuestions[collection.indexInputQuestion] = question;

    //     // Increase the index of the input questions
    //     oldCollection.indexInputQuestion = collection.indexInputQuestion + 1;

    //     // Convert it to json
    //     string newSave = JsonUtility.ToJson(oldCollection);

    //     // Save it in the File
    //     File.WriteAllText(Menus.savePath + "save5", newSave); 
    // }

    // Method that returns the array of files in a directory
    static string[] GetFilesArray(string path) 
    {
        string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
        return files;
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

            // Reset the selected paths for the saving
            Globals.selectedPath = "";
            Globals.selectedPathShorten = "";

            // Reset the input field that displayed the path
            savePathText.text = "";

            // Delete everything that was in the temporary save file
            string[] files = GetFilesArray(Menus.tempSavePath);
            foreach(string file in files)
            {
                File.Delete(file);
            }

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

    // Method that gives access to the brows directories menu
    public void SetDirectory()
    {
        // First set the selection button of the brows directories menu active
        selectButton.gameObject.SetActive(true);

        // Then enable / disable the menus
        mainCreator.SetActive(false);
        browsDirectoriesMenu.SetActive(true);
    }

    // Method that gets the selected path (where to save the exercises) from the brows directory script  and actualizes the text that should display it
    public void ActualizeSavePath()
    {
        // Change the text
        savePathText.text = @"...\" + Globals.selectedPathShorten;
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
