using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

// TODO: Create a way of deleting questions (meaning renaming files)
// TODO: Create a way to add a description to the log file (need new window between selecting empty folder and selecting the path)
// TODO: 

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

    // Flag for edit mode and the old files name, so that the content gets saved back in the same file
    public static bool editModeOn;
    public static string editedFileName;
    public static int editedButtonIndex;

    // The temporary save path
    public static string tempSavePath;
    public static bool directorySelection;
    
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

    // Buttons that gets disabled through editing already existing questions (Brows directories menu)
    public Button addQuestion;
    public Button browsDirectoriesButton;
    public Button saveCreatedQuestions;
    public Button changeQuestion;

    // Buttons that stand before the text inputs of the multiple choice answers
    public Button enableMultipleChoiceAnswer3;
    public Button enableMultipleChoiceAnswer4;
    public Button enableMultipleChoiceAnswer5;

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
    public TextMeshProUGUI errorNoQuestionMultipleChoice;
    public TextMeshProUGUI errorNotEnoughAnswersMultipleChoice;
    public TextMeshProUGUI noPathSelected;
    public TextMeshProUGUI noQuestionCreated;

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
        Menus.tempSavePath = GetPathToTempSave(scriptPath);

        // Initialize the collection
        // Menus.collection = new ExerciseCollection();

        Menus.questionNumber = 0;
        Menus.currentPage = 1;
        Menus.numberOfPages = 1;
        Menus.editModeOn = false;
        Menus.directorySelection = false;
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
    private string GetPathToTempSave(string scriptPath)
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

        // Reset the question index, so that the naming begins again at Question000.
        Menus.currentQuestionIndex = 0;
        
        // Enable the buttons that could have been disabled through editing already existing questions.
        addQuestion.interactable = true;
        browsDirectoriesButton.interactable = true;
        saveCreatedQuestions.gameObject.SetActive(true);
        changeQuestion.gameObject.SetActive(false);
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Activate or deactivate menus methods
    // -------------------------------------------------------------------------------------------------------------------

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

    // -------------------------------------------------------------------------------------------------------------------
    // Navigation methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method that deactivates the naming window when clicked on cancel
    public void CancelEnterName()
    {
        if(Menus.editModeOn == false)
        {
            // Case no edit mode, the string can be deleted
            DeactivateNamingWindow();
        } else {
            // Case edit mode is on, need to let the name there
            enterNameWindow.SetActive(false);
            veilSmallWindow.SetActive(false);
        }
    }

    // Method activated when clicking on the "Add" button in the creator screen, opens the right exercise creator window
    public void AddExercise()
    {
        // Disable edit mode
        Menus.editModeOn = false;

        // Enabling the right creator window depending on which mode was chosen
        if(GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn == true)
        {
            Debug.Log("Multiple choice is on!");

            // Deactivate the "change" and activate the "create" button
            changeMultipleChoice.gameObject.SetActive(false);
            createMultipleChoice.gameObject.SetActive(true);
            ActivateMultipleChoiceMode();
        } else {
            Debug.Log("Input mode is on!");

            // Deactivate the "change" and activate the "create" button
             changeInput.gameObject.SetActive(false);
             createInput.gameObject.SetActive(true);
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

            Debug.Log("all files have been deleted");

            // Reset the page count and question index
            Menus.currentPage = 1;
            Menus.numberOfPages = 1;
            Menus.currentQuestionIndex = 0;

            // Disable the errors
            noQuestionCreated.gameObject.SetActive(false);
            noPathSelected.gameObject.SetActive(false);

        // Case input mode creator
        } else if(Menus.currentMenu == inputModeCreator)
        {
            Debug.Log("Current Menu is input mode");
            // First delete everything that was entered
            enterQuestionInput.text = "";
            enterAnswerInput.text = "";
            enterName.text = "";

            // Then it is needed to set the right windows as current menu and deactivate / activate the right menus
            DeactivateInputMode();
            DeactivateExitWithoutSaveWindow();

        // Case multiple choice mode creator
        } else if(Menus.currentMenu == multipleChoiceCreator)
        {
            Debug.Log("Current menu is multiple choice");

            // First delete everything that was entered
            enterQuestionMultiple.text = "";

            // Reset the text that was typed in
            enterFirstAnswer.text = "";
            enterSecondAnswer.text = "";
            enterThirdAnswer.text = "";
            enterFourthAnswer.text = "";
            enterFifthAnswer.text = "";
            enterName.text = "";

            // Disable the text fields
            enterThirdAnswer.gameObject.SetActive(false);
            enterFourthAnswer.gameObject.SetActive(false);
            enterFifthAnswer.gameObject.SetActive(false);

            // Enable the button
            enableMultipleChoiceAnswer3.gameObject.SetActive(true);
            enableMultipleChoiceAnswer4.gameObject.SetActive(true);
            enableMultipleChoiceAnswer5.gameObject.SetActive(true);

            // Reset the toggles
            GameObject.Find("Answer1Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer2Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer3Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer4Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer5Correct").GetComponent<Toggle>().isOn = false;

            // Then it is needed to set the right windows as current menu and deactivate / activate the right menus
            DeactivateMultipleChoiceMode();
            DeactivateExitWithoutSaveWindow();
        }
    }

    // Method used to add the third multiple choice fields
    public void AddAnswerPossibility3()
    {
        // First, check if the two previous answers have been typed in
        if(enterFirstAnswer.text != "" && enterSecondAnswer.text != "")
        {
            // Case both fields full
            // Deactivate the button
            enableMultipleChoiceAnswer3.gameObject.SetActive(false);

            // Enable the hidden text field
            enterThirdAnswer.gameObject.SetActive(true);
        }
    }

    // Method used to add the fourth multiple choice fields
    public void AddAnswerPossibility4()
    {
        // First, check if the two previous answers have been typed in
        if(enterFirstAnswer.text != "" && enterSecondAnswer.text != "" && enterThirdAnswer.text != "")
        {
            // Case all previous fields full
            // Deactivate the button
            enableMultipleChoiceAnswer4.gameObject.SetActive(false);

            // Enable the hidden text field
            enterFourthAnswer.gameObject.SetActive(true);

            // Set the next button as interactable
            enableMultipleChoiceAnswer5.interactable = true;
        }
    }
    // Method used to add the fifth multiple choice fields
    public void AddAnswerPossibility5()
    {
        // First, check if the two previous answers have been typed in
        if(enterFirstAnswer.text != "" && enterSecondAnswer.text != "" && enterThirdAnswer.text != ""&& enterFourthAnswer.text != "")
        {
            // Case all previous fields full
            // Deactivate the button
            enableMultipleChoiceAnswer5.gameObject.SetActive(false);

            // Enable the hidden text field
            enterFifthAnswer.gameObject.SetActive(true);
        }
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Save methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method that is activated when a user clicks on the "create" button for the input question
    public void ProceedToSaveInput()
    {
        // Disable both error texts
        errorNoQuestionInput.gameObject.SetActive(false);
        errorNoAnswerInput.gameObject.SetActive(false);

        // Case both fields contain characters, no error message needs to be enabled
        if(enterQuestionInput.text != "" && enterAnswerInput.text != "")
        {
            ActivateNamingWindow();
        } else {
            // If the question is not typed in, display the error messages for the question
            if(enterQuestionInput.text == "")
            {
                errorNoQuestionInput.gameObject.SetActive(true);
            }
            // If the answer is not typed in, display the error messages for the answer
            if(enterAnswerInput.text == "")
            {
                errorNoAnswerInput.gameObject.SetActive(true);
            }
        }
    }

    // Method that is activated when a user clicks on the "create" button for the multiple choice question
    public void ProceedToSaveMultipleChoice()
    {
        // Disable both error texts
        errorNoQuestionMultipleChoice.gameObject.SetActive(false);
        errorNotEnoughAnswersMultipleChoice.gameObject.SetActive(false);

        // First get the number of answers entered
        int numberOfAnswers = 0;
        if(enterFirstAnswer.text != "")
        {
            numberOfAnswers = numberOfAnswers + 1;
        }
        if(enterSecondAnswer.text != "")
        {
            numberOfAnswers = numberOfAnswers + 1;
        }
        if(enterThirdAnswer.text != "")
        {
            numberOfAnswers = numberOfAnswers + 1;
        }
        if(enterFourthAnswer.text != "")
        {
            numberOfAnswers = numberOfAnswers + 1;
        }
        if(enterFifthAnswer.text != "")
        {
            numberOfAnswers = numberOfAnswers + 1;
        }

        Debug.Log("question: " + enterQuestionMultiple.text);
        Debug.Log("number of answers: " + numberOfAnswers);


        // Check if the question was entered, and enough answer fields have been filled
        if(enterQuestionMultiple.text != "" && numberOfAnswers > 1)
        {
            // Case all necessary information have been entered, proceed with naming
            ActivateNamingWindow();
        } else {
            // If the question is not typed in, display the error messages for the question
            if(enterQuestionMultiple.text == "")
            {
                errorNoQuestionMultipleChoice.gameObject.SetActive(true);
            }
            // If not enough answers have been typed in, display the error messages for the answers
            if(numberOfAnswers < 2)
            {
                errorNotEnoughAnswersMultipleChoice.gameObject.SetActive(true);
            }
        }
    }

    // Method that displays the name of the question in the created question preview in the creator menu
    public void DisplayName(string name, int index)
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
        if((Menus.currentPage - 1) * 5 <= index && index < Menus.currentPage * 5)
        {
            // Get the concerned button
            Button rightButton = GetRightButton(index);

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
        if(index < 10)
        {
            // case two zero then the number to have 00X
            number = "00" + Convert.ToString(index);
        } else if(index < 100)
        {
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

        // Check if it is a new question or not, and save it at the appropriate place
        SaveAtTheRightPlace(json);

        // Display the question name in the question preview on the creator menu

        DisplayNameCorrectly(inputQuestion.name);
    }

    //
    public void DisplayNameCorrectly(string name)
    {
        if(Menus.editModeOn == false)
        {
            // Display the new question name at the next place
            DisplayName(name, Menus.currentQuestionIndex);
        } else {

            // Display the already existing question name at the old place
            DisplayName(name, Menus.editedButtonIndex);
        }
    }

    // Method that saves a json string at the right place, depending on if edit mode is on or not
    public void SaveAtTheRightPlace(string json)
    {
        // Check if it is a new question or not, and save it at the appropriate place
        if(Menus.editModeOn == false)
        {
            // Case it is a new question
            string index = ReturnQuestionIndex(Menus.currentQuestionIndex);
            Debug.Log(json);
            Debug.Log(index);
            Debug.Log(Menus.tempSavePath + "Question" + index + ".json");
            File.WriteAllText(Menus.tempSavePath + "Question" + index + ".json", json);
        } else {

            // Case it is an already existing question
            File.WriteAllText(Menus.tempSavePath + Menus.editedFileName, json);
        }
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
        if(enterThirdAnswer.text != "")
        {
            multipleChoiceQuestion.answer3 = enterThirdAnswer.text;
            answerCounter = answerCounter + 1;
        }
        if(enterFourthAnswer.text != "")
        {
            multipleChoiceQuestion.answer4 = enterFourthAnswer.text;
            answerCounter = answerCounter + 1;
        }
        if(enterFifthAnswer.text != "")
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

        // Check if it is a new question or not, and save it at the appropriate place
        SaveAtTheRightPlace(json);

        // Display the question name in the question preview on the creator menu
        DisplayNameCorrectly(multipleChoiceQuestion.name);
    }

    // Method that is activated when a user names a question. Since the window is unique and is used for all questions, the right save method has to be started
    public void SaveQuestion()
    {
        // Check what menu is in the background and save the right type of question
        if(Menus.currentMenu == inputModeCreator)
        {
            // Case it was the input mode creator that was active in the background and no edit mode
            SaveInputQuestion();

        } else if(Menus.currentMenu == multipleChoiceCreator)
        {
            // Case it was the multiple choice creator that was active in the background and no edit mode
            SaveMultipleChoiceQuestion();
        }

        // Then desactivate the current mode and delete everything that was written. For this the method exit without saving YES can be reused.
        ExitWithoutSavingYes();

        // Deactivate the naming window
        DeactivateNamingWindow();

        if(Menus.editModeOn == false)
        {
            // Actualize the number of pages
            ActualizePageNumber();

            // Enable the right buttons
            ActualizeButtons();

            // Increase the current question index by one
            Menus.currentQuestionIndex = Menus.currentQuestionIndex + 1;
        }

        // Deactivate the "no question" error message on the creator menu
        noQuestionCreated.gameObject.SetActive(false);
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

    // Method that permitts to enter edit mode again on a certain question that is selected
    public void EnterEditMode()
    {
        // Get the button name
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        Button currentButton = GameObject.Find(buttonName).GetComponent<Button>();
        Debug.Log(buttonName);

        // Get the name of the file
        string fileName = "";
        if(Globals.currentlyChangingFile == false)
        {
            fileName = GetFileNameFromButtonName(buttonName);
        } else {
            fileName = Globals.fileName;
        }

        // Write the file name in Menus variable, so that the content gets saved in the same file
        Menus.editedFileName = fileName;

        // Extract the string
        //string json = File.ReadAllText(Menus.tempSavePath + fileName + @"\");
        string json = File.ReadAllText(Menus.tempSavePath + fileName);

        // Enter edit mode
        Menus.editModeOn = true;

        // Check what type it is, if input question or multiple choice
        if(json.Contains("input question") == true)
        {
            // Case input question
            InputQuestion question = JsonUtility.FromJson<InputQuestion>(json);

            // Set the current mode to edit mode (so that the naming window does not create a new file)

            // Open the input mode window, copy the content back in
            EnableEditModeInput(question);

        } else {

            // Case multiple choice question
            MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(json);
            EnableEditModeMultipleChoice(question);
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

    // Method that enables the edit mode for multiple choice question
    public void EnableEditModeMultipleChoice(MultipleChoiceQuestion question)
    {
        // Activate input mode
        ActivateMultipleChoiceMode();

        // Activate the "change" instead of "create" button
        changeMultipleChoice.gameObject.SetActive(true);
        createMultipleChoice.gameObject.SetActive(false);

        // Copy the answers that were already set back in
        enterName.text = question.name;
        enterQuestionMultiple.text = question.question;
        enterFirstAnswer.text = question.answer1;
        enterSecondAnswer.text = question.answer2;
        firstAnswerCorrect.isOn = question.answer1Correct;
        secondAnswerCorrect.isOn = question.answer2Correct;

        // If more than the two first answer have been entered
        if(question.answer3 != "")
        {
            enterThirdAnswer.text = question.answer3;
            thirdAnswerCorrect.isOn = question.answer3Correct;
            // Disable the button that is in overlay
            enableMultipleChoiceAnswer3.gameObject.SetActive(false);
            // Enable the hidden text field
            enterThirdAnswer.gameObject.SetActive(true);
        }
        if(question.answer4 != "")
        {
            enterFourthAnswer.text = question.answer4;
            fourthAnswerCorrect.isOn = question.answer4Correct;
            // Disable the button that is in overlay
            enableMultipleChoiceAnswer4.gameObject.SetActive(false);
            // Enable the hidden text field
            enterFourthAnswer.gameObject.SetActive(true);
        }
        if(question.answer5 != "")
        {
            enterFifthAnswer.text = question.answer5;
            fifthAnswerCorrect.isOn = question.answer5Correct;
            // Disable the button that is in overlay
            enableMultipleChoiceAnswer5.gameObject.SetActive(false);
            // Enable the hidden text field
            enterFifthAnswer.gameObject.SetActive(true);
        }
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
        Menus.editedButtonIndex = index;

        // Get the question index string that is the ending of the save file
        string ending = ReturnQuestionIndex(index);

        // Transform that in the file name and return it
        string name = "Question" + ending + ".json";
        return name;
    }

    // Method that returns the array of files in a directory
    static string[] GetFilesArray(string path) 
    {
        string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
        return files;
    }

    // Method that gives access to the brows directories menu
    public void SetDirectory()
    {
        // First set the selection button of the brows directories menu active
        Menus.directorySelection = true;

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

    // Method that copies all files form one directory to another (used for temp save directory and end save directory)
    public void CopyFromPath1ToPath2(string path1, string path2)
    {
        // Copy all files that are in the path1 to the directory in path 2
        //Directory.CreateDirectory(path2);

        foreach(var file in Directory.GetFiles(path1))
        {
            File.Copy(file, Path.Combine(path2, Path.GetFileName(file)));
        }
    }

    // Method that saves all questions that the user created in the right folder and does the setup with the 3D models
    public void SaveQuestionsInEndDirectory()
    {
        // First disable the error messages
        noPathSelected.gameObject.SetActive(false);
        noQuestionCreated.gameObject.SetActive(false);

        // Check if a path was selected
        if(Globals.selectedPath != "" && Globals.selectedPath != null && previewQuestion1.GetComponentInChildren<TMP_Text>().text != "")
        {
            // Case at least one question was created, and the path is not null

            // First need to check if there are question in the folder. If there is a log file, then there are some.
            string pathToLogFile = Globals.selectedPath + "Description.json";

            if (!File.Exists(pathToLogFile))
            {
                Debug.Log("There was no log file!");
                // Case there is no log file, no questions in the folder
                // Create a new log file
                Log logFile = new Log();

                // Set all information that are already known
                logFile.numberOfQuestions = Menus.currentQuestionIndex;

                // Generate the json string and save it in the temp save directory
                string json = JsonUtility.ToJson(logFile);
                File.WriteAllText(Menus.tempSavePath + "Description.json", json);

            } else {

                Debug.Log("There was a log file!");

                // Case there is already a log file, and questions
                // Load the log game object
                string json = File.ReadAllText(Globals.selectedPath + "Description.json");
                Log logFile = JsonUtility.FromJson<Log>(json);

                // Get the number of questions
                int number = logFile.numberOfQuestions;
                Debug.Log("The old number of question was: " + number);

                // Rename all questionXYZ files in the temp save folder accordingly
                int newNumber = renameFilesAdding(Menus.tempSavePath, number);

                Debug.Log("The new number of question was: " + newNumber);

                // Actualize the number of questions
                logFile.numberOfQuestions = newNumber;

                // Convert it back to json
                string jsonNew = JsonUtility.ToJson(logFile);

                // Delete the old file
                File.Delete(Globals.selectedPath + "Description.json");

                // Create the new file
                File.WriteAllText(Menus.tempSavePath + "Description.json", jsonNew);
            }

            // Copy all files form the temp save directory to the end save directory
            CopyFromPath1ToPath2(Menus.tempSavePath, Globals.selectedPath);


            // Exit the creator
            ExitWithoutSavingYes();
            
        } else {
            // If no question was created, display the no question error message
            if(previewQuestion1.GetComponentInChildren<TMP_Text>().text == "")
            {
                noQuestionCreated.gameObject.SetActive(true);
            }
            // If no path selected, display the no path error message 
            if(Globals.selectedPath == "" || Globals.selectedPath == null)
            {
                noPathSelected.gameObject.SetActive(true);
            }
        }
    }

    // Method that saves the question back in the back-end save directory after editing it (when choosing the question in the brows directories menu)
    public void ChangeQuestionInEndDirectory() 
    {
        // Check if the question was changed or deleted
        if(previewQuestion1.GetComponentInChildren<TMP_Text>().text != "deleted")
        {
            Debug.Log("the path to the file is: " + Globals.filePath);
            // Case it was not deleted, then delete the old file in the back end folder
            File.Delete(Globals.filePath);
            Debug.Log("Deleted the file at: " + Globals.filePath);

            // Copy it back in
            CopyFromPath1ToPath2(Menus.tempSavePath, Globals.selectedPath);

            // Exit the creator
            ExitWithoutSavingYes();
        }
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Creation of the log file in the back end, renaming of files
    // -------------------------------------------------------------------------------------------------------------------

    // The JSON Serialization for the log file
    [Serializable]
    public class Log
    {
        public int numberOfQuestions; // The number of already existing questions in the folder so that the new ones can be renamed
        public int numberOfModels; // The number of already existing model files in the folder so that the new ones can be renamed
        public string heading; // Heading of the description, name that users can give
        public string description; // The description text of the content / concepts that are needed for solving the exercises
    }

    // Method that renames all files in the given path to QuestionX given an index X
    public int renameFilesAdding(string path, int index)
    {
        // First rename the files with names that do not exist
        int loopIndex = index;
        foreach(var file in Directory.GetFiles(path))
        {
            System.IO.File.Move(file, Menus.tempSavePath + loopIndex.ToString());
            Debug.Log(Path.GetFileName(file));
            loopIndex = loopIndex + 1;
        }

        // Then rename them with names that it should have in the new save folder
        int newIndex = index;
        foreach(var file in Directory.GetFiles(path))
        {
            // Generate the right index at the end of the name
            string ending = ReturnQuestionIndex(newIndex);
            string name = "Question" + ending;
            System.IO.File.Move(file, Menus.tempSavePath + name + ".json");
            Debug.Log(Path.GetFileName(file));
            newIndex = newIndex + 1;
        }
        return newIndex;
    }
}
