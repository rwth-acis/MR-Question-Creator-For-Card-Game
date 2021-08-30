using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using System.Net;
using i5.Toolkit.Core.ModelImporters;
using i5.Toolkit.Core.ProceduralGeometry;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using System.Linq;

// Last Method: ReturnPreviewModelButtonFromModelName

// TODO: Create a way of deleting questions (meaning renaming files)

static class Menus
{
    // Save current and last menu for return function
    public static GameObject lastMenu;
    public static GameObject currentMenu;

    // Save an index for the current question
    public static int currentQuestionIndex;
    public static int currentModelIndex;

    // Save the current uri and current model name that is being edited
    public static string  currentUri;
    public static string editedModelName;
    public static int editedModelIndex;

    // Save an index for the current displayed page of the displayed questions
    public static int currentPage;
    public static int numberOfPages;

    // Flag for edit mode and the old files name, so that the content gets saved back in the same file
    public static bool editModeOn;
    public static string editedFileName;
    public static int editedButtonIndex;

    // The temporary save path
    public static string tempSavePath;
    public static bool directorySelection;

    // The name of the current model preview button
    public static string modelPreviewButtonName;

    // Save the last object url that was entered
    public static string lastUrl;

    // Save the url of the models
    public static string url1 = "";
    public static string url2 = "";
    public static string url3 = "";
    public static string url4 = "";
    public static string url5 = "";
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
    public GameObject importModelWindow;
    public GameObject duplicateModelWindow;
    public GameObject replaceModelWindow;
    public GameObject modelMarchesWindow;

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

    // The input field to type in the url of the 3D model to inport
    public TMP_InputField enterUrl;

    // The input field to type in the new name of the 3D model that will be inported
    public TMP_InputField enterNewModelName;

    // The input field to type in the uri of the new 3D model to replace the old one
    public TMP_InputField enterNewModelUri;

    // The input fields to rename models that match models in the end directory
    public TMP_InputField enterNewModelNameMatch1;
    public TMP_InputField enterNewModelNameMatch2;
    public TMP_InputField enterNewModelNameMatch3;
    public TMP_InputField enterNewModelNameMatch4;
    public TMP_InputField enterNewModelNameMatch5;

    // Defining the button
    // Button to select a directory as end save directory
    public Button selectButton;

    // The five preview model button used to edit created questions
    public Button previewModel1;
    public Button previewModel2;
    public Button previewModel3;
    public Button previewModel4;
    public Button previewModel5;

    // The five preview question button used to edit created questions
    public Button previewQuestion1;
    public Button previewQuestion2;
    public Button previewQuestion3;
    public Button previewQuestion4;
    public Button previewQuestion5;

    // The four buttons with sprites to navigate back and forth all created questions (e.g from questions 1-5 to questions 6-10)
    public Button nextEnabled;
    public Button nextDisabled;
    public Button previousEnabled;
    public Button previousDisabled;

    // Change and create buttons are place one over the other and have different scripts. Activated / Deactivated in edit already end saved questions or when creating new questions
    public Button changeInput;
    public Button createInput;
    public Button changeMultipleChoice;
    public Button createMultipleChoice;

    // Buttons that gets disabled / deactivated / activated one editing already end saved questions (Brows directories menu)
    public Button addQuestion;
    public Button browsDirectoriesButton;
    public Button saveCreatedQuestions;
    public Button changeQuestion;

    // Buttons that stand before the text inputs of the multiple choice answers
    public Button enableMultipleChoiceAnswer3;
    public Button enableMultipleChoiceAnswer4;
    public Button enableMultipleChoiceAnswer5;

    // Buttons used to delete questions
    public Button deleteInputQuestion;
    public Button deleteMultipleChoiceQuestion;

    // The five preview model matches button shown when trying to save models in the end save directory when there are matches
    public Button modelNameMatch1;
    public Button modelNameMatch2;
    public Button modelNameMatch3;
    public Button modelNameMatch4;
    public Button modelNameMatch5;

    // Define the toggles used in the multiple choice mode
    public Toggle firstAnswerCorrect;
    public Toggle secondAnswerCorrect;
    public Toggle thirdAnswerCorrect;
    public Toggle fourthAnswerCorrect;
    public Toggle fifthAnswerCorrect;

    // Define the error texts that need to be displayed if an input field is empty
    // For the question creators
    public TextMeshProUGUI errorNoQuestionInput;
    public TextMeshProUGUI errorNoAnswerInput;
    public TextMeshProUGUI errorNoQuestionMultipleChoice;
    public TextMeshProUGUI errorNotEnoughAnswersMultipleChoice;

    // When clicking on the "save" button to save the questions in the back end directory
    public TextMeshProUGUI noPathSelected;
    public TextMeshProUGUI noQuestionCreated;

    // For the import model window 
    public TextMeshProUGUI noUrlTypedInErrorMessage;
    public TextMeshProUGUI urlObjectOfWrongTypeErrorMessage;

    // For the replace model window
    public TextMeshProUGUI noUrlTypedInErrorMessageReplacement;
    public TextMeshProUGUI urlObjectOfWrongTypeErrorMessageReplacement;

    // For the rename duplicate model window
    public TextMeshProUGUI noNameToRenameTypedIn;
    public TextMeshProUGUI nameAlreadyExist;

    // Define the heading text that gives the current page
    public TextMeshProUGUI headingPageNumber;

    // Define the heading text of the duplicate name window
    public TextMeshProUGUI duplicateName;

    // Define the heading text of the replace / delete model window
    public TextMeshProUGUI wishToReplaceHeading;

    // Define the model matches window
    public TextMeshProUGUI matchesHeading;

    // The JSON Serialization for the input questions
    [Serializable]
    public class InputQuestion
    {
        public string exerciseType = "input question";
        public string name;
        public string question;
        public string answer;
        public int numberOfModels;
        public string model1Name;
        public string model2Name;
        public string model3Name;
        public string model4Name;
        public string model5Name;
    }

    // The JSON Serialization for the multiple choice questions
    [Serializable]
    public class MultipleChoiceQuestion
    {
        public string exerciseType = "multiple choice question";
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
        public int numberOfModels;
        public string model1Name;
        public string model2Name;
        public string model3Name;
        public string model4Name;
        public string model5Name;
    }

    // The JSON Serialization for the Models
    [Serializable]
    public class Model
    {
        public string modelName;
        public string modelUrl;
        public int numberOfQuestionsUsedIn;

    }

    // The JSON Serialization for the log file
    [Serializable]
    public class Log
    {
        public int numberOfQuestions; // The number of already existing questions in the folder so that the new ones can be renamed
        public int numberOfModels; // The number of already existing model files in the folder so that the new ones can be renamed
        public string heading; // Heading of the description, name that users can give
        public string description; // The description text of the content / concepts that are needed for solving the exercises
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Start method, used to initialize all Menus variables and set all buttons on interactable or not
    // -------------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        // Disable all buttons that cannot be used at the begining
        if(Globals.currentlyChangingFile != true)
        {
            GameObject.Find("Add3DModel2").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel3").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel4").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel5").GetComponent<Button>().interactable = false;
        }

        // Disable the delete button and interactability of the preview question one only if the user is not changing a file currently
        if(Globals.currentlyChangingFile != true)
        {
            GameObject.Find("PreviewQuestion1").GetComponent<Button>().interactable = false;
        }

        GameObject.Find("PreviewQuestion2").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion3").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion4").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion5").GetComponent<Button>().interactable = false;

        // Set last and current menu
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainCreator;

        // Set the current exercise index
        Menus.currentQuestionIndex = 0;

        // Initialize the save path. This will have to be saved later on
        string scriptPath = GetCurrentFilePath();
        Menus.tempSavePath = GetPathToTempSave(scriptPath);

        // Initialize the collection
        // Menus.collection = new ExerciseCollection();

        Menus.currentPage = 1;
        Menus.numberOfPages = 1;
        Menus.editModeOn = false;
        Menus.directorySelection = false;

        // Initialize the edited file name
        Menus.editedFileName = "";
        Menus.editedModelName = "";

        // Initialize the current model index
        Menus.currentModelIndex = 0;
        Menus.editedModelIndex = 5;

        // Deactivate the matches buttons
        modelNameMatch1.interactable = false;
        modelNameMatch2.interactable = false;
        modelNameMatch3.interactable = false;
        modelNameMatch4.interactable = false;
        modelNameMatch5.interactable = false;
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Methods to set the paths correctly (to tempSave directory)
    // -------------------------------------------------------------------------------------------------------------------

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

    // -------------------------------------------------------------------------------------------------------------------
    // Entering / exiting the creator menu
    // -------------------------------------------------------------------------------------------------------------------

    // Method used to enter the creator
    public void EnterCreator() 
    {

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

    // Method used to exit the creator
    public void ExitCreator()
    {
        // First check if something was added and save this information in a boolean variable
        bool isEmpty = (GameObject.Find("Add3DModel1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != "+" || GameObject.Find("PreviewQuestion1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != "");

        // Call the exit without saving function
        ExitWithoutSaving(isEmpty);
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
    }

    // Method that deactivates everything for the creator menu
    public void DeactivateCreatorMenu()
    {
        mainCreator.SetActive(false);
        mainMenu.SetActive(true);
        Menus.currentMenu = mainCreator;
        Menus.lastMenu = mainMenu;
    }

    // Method that activates everything for the multiple choice mode
    public void ActivateMultipleChoiceMode()
    {
        veilLargeWindow.SetActive(true);
        multipleChoiceCreator.SetActive(true);
        Menus.currentMenu = multipleChoiceCreator;
        Menus.lastMenu = mainCreator;
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

    // Method that activates everything for the enter url window
    public void ActivateImportModelWindow()
    {
        noUrlTypedInErrorMessage.gameObject.SetActive(false);
        urlObjectOfWrongTypeErrorMessage.gameObject.SetActive(false);
        importModelWindow.SetActive(true);
        veilSmallWindow.SetActive(true);
    }

    // Method that deactivates everything for the enter url window and resets the name that was typed in
    public void DeactivateImportModelWindow()
    {
        importModelWindow.SetActive(false);
        veilSmallWindow.SetActive(false);
        enterUrl.text = "";
    }

    // Method that activates everything for the enter url window
    public void ActivateEnterNewModelNameWindow()
    {
        duplicateModelWindow.SetActive(true);
        veilSmallWindow.SetActive(true);
    }

    // Method that deactivates everything for the enter url window and resets the name that was typed in
    public void DeactivateEnterNewModelNameWindow()
    {
        duplicateModelWindow.SetActive(false);
        veilSmallWindow.SetActive(false);
        enterNewModelName.text = "";
    }

    // Method that activates everything for the enter url window
    public void ActivateReplaceModelWindow()
    {
        noUrlTypedInErrorMessageReplacement.gameObject.SetActive(false);
        urlObjectOfWrongTypeErrorMessageReplacement.gameObject.SetActive(false);
        replaceModelWindow.SetActive(true);
        veilSmallWindow.SetActive(true);
    }

    // Method that deactivates everything for the enter url window and resets the name that was typed in
    public void DeactivateReplaceModelWindow()
    {
        replaceModelWindow.SetActive(false);
        veilSmallWindow.SetActive(false);
        enterNewModelUri.text = "";
    }

    // Method that activates the matches window and pastes the right names in the buttons
    public void ActivateMatchesWindow(string[] matchesArray, int matches)
    {
        modelMarchesWindow.SetActive(true);
        veilLargeWindow.SetActive(true);

        // Initialize the index
        int index = 0;

        // If matches is 0, this method is not started
        if(matches >= 1)
        {
            // Display the match name on the first button
            modelNameMatch1.GetComponentInChildren<TMP_Text>().text = matchesArray[index];
            enterNewModelNameMatch1.gameObject.SetActive(true);
            index = index + 1;
        }

        // Display the right text on the second button
        if(matches >= 2)
        {
            // Display the match name on the second button
            modelNameMatch2.GetComponentInChildren<TMP_Text>().text = matchesArray[index];
            enterNewModelNameMatch2.gameObject.SetActive(true);
            index = index + 1;
        } else {
            modelNameMatch2.GetComponentInChildren<TMP_Text>().text = "";
            enterNewModelNameMatch2.gameObject.SetActive(false);
        }

        // Display the right text on the third button
        if(matches >= 3)
        {
            // Display the match name on the third button
            modelNameMatch3.GetComponentInChildren<TMP_Text>().text = matchesArray[index];
            enterNewModelNameMatch3.gameObject.SetActive(true);
            index = index + 1;
        } else {
            modelNameMatch3.GetComponentInChildren<TMP_Text>().text = "";
            enterNewModelNameMatch3.gameObject.SetActive(false);
        }

        // Display the right text on the fourth button
        if(matches >= 4)
        {
            // Display the match name on the fourth button
            modelNameMatch4.GetComponentInChildren<TMP_Text>().text = matchesArray[index];
            enterNewModelNameMatch4.gameObject.SetActive(true);
            index = index + 1;
        } else {
            modelNameMatch4.GetComponentInChildren<TMP_Text>().text = "";
            enterNewModelNameMatch4.gameObject.SetActive(false);
        }

        // Display the right text on the fifth button
        if(matches >= 5)
        {
            // Display the match name on the fifth button
            modelNameMatch5.GetComponentInChildren<TMP_Text>().text = matchesArray[index];
            enterNewModelNameMatch5.gameObject.SetActive(true);
            index = index + 1;
        } else {
            modelNameMatch5.GetComponentInChildren<TMP_Text>().text = "";
            enterNewModelNameMatch5.gameObject.SetActive(false);
        }
    }

    // Method that deactivates the matches window and pastes the right names in the buttons
    public void DeactivateMatchesWindow()
    {
        modelMarchesWindow.SetActive(false);
        veilLargeWindow.SetActive(false);

        // Reset the name fields
        enterNewModelNameMatch1.text = "";
        enterNewModelNameMatch2.text = "";
        enterNewModelNameMatch3.text = "";
        enterNewModelNameMatch4.text = "";
        enterNewModelNameMatch5.text = "";
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

    // Method to navigate from the multiple choice mode back to the main creator
    // For now it only checks if the answers are not filled, ignores the question, can be improved TODO
    public void GetBackFromMultipleChoice()
    {
        // First check if something was added and save this information in a boolean variable
        bool isEmpty = (enterFirstAnswer.text != "" || enterSecondAnswer.text != "" || enterQuestionMultiple.text != "");

        // Call the exit without saving function
        ExitWithoutSaving(isEmpty);
    }

    // Method to navigate from the input mode back to the main creator
    public void GetBackFromInputMode()
    {
        // First check if something was added and save this information in a boolean variable
        bool isEmpty = (enterQuestionInput.text != "" || enterAnswerInput.text != "");

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

    // -------------------------------------------------------------------------------------------------------------------
    // Exit without saving methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method that summons the "are you sure you do not want to save" window if something was already done
    public void ExitWithoutSaving(bool isEmpty)
    {
        // First check if something was added
        if(isEmpty == true)
        {
            // Activate the exit without save window
            ActivateExitWithoutSaveWindow();
        } else {
            // Everything is empty, so reset the menus
            Menus.lastMenu.SetActive(true);
            Menus.currentMenu.SetActive(false);
            veilLargeWindow.SetActive(false);

            // Since this is only used to exit specialized exercises creator, the last menu is always the main creator
            // The currentMenu does not go to the mainMenu, so that it does not need to be set when returning in the creator menu
            Menus.currentMenu = mainCreator;
            Menus.lastMenu = mainMenu;
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

            // Reset the buttons that preview names of 3D models and currently created questions
            ResetPreviewModelButtons();
            previewQuestion1.GetComponentInChildren<TMP_Text>().text = "";
            previewQuestion2.GetComponentInChildren<TMP_Text>().text = "";
            previewQuestion3.GetComponentInChildren<TMP_Text>().text = "";
            previewQuestion4.GetComponentInChildren<TMP_Text>().text = "";
            previewQuestion5.GetComponentInChildren<TMP_Text>().text = "";

            // Reset the toggle, check the one that is selected as default, because it is a toggle group the other is set to false automatically
            GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn = true;

            // Reset the interactability of the buttons
            previewQuestion1.interactable = false;
            previewQuestion2.interactable = false;
            previewQuestion3.interactable = false;
            previewQuestion4.interactable = false;
            previewQuestion5.interactable = false;

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


            // Reset the page count, model and question index
            Menus.currentPage = 1;
            Menus.numberOfPages = 1;
            Menus.currentQuestionIndex = 0;
            Menus.currentModelIndex = 0;

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

    // -------------------------------------------------------------------------------------------------------------------
    // Question creation methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method activated when clicking on the "Add" button in the creator screen, opens the right exercise creator window
    public void AddExercise()
    {
        // Disable edit mode
        Menus.editModeOn = false;

        // Enabling the right creator window depending on which mode was chosen
        if(GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn == true)
        {
            // Deactivate the "change" and activate the "create" button
            changeMultipleChoice.gameObject.SetActive(false);
            createMultipleChoice.gameObject.SetActive(true);
            ActivateMultipleChoiceMode();

            // Since we want to create a question, disable the delete button
            deleteMultipleChoiceQuestion.gameObject.SetActive(false);
        } else {

            // Deactivate the "change" and activate the "create" button
             changeInput.gameObject.SetActive(false);
             createInput.gameObject.SetActive(true);
            ActivateInputMode();

            // Since we want to create a question, disable the delete button
            deleteInputQuestion.gameObject.SetActive(false);
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

    // Method that returns you the right button given the index
    public Button GetRightModelButton(int modelIndex)
    {
        int index = modelIndex;
        // Display it at the right place
        switch(index)
        {
            case 0:
                return previewModel1;
            break;
            case 1:
                return previewModel2;
            break;
            case 2:
                return previewModel3;
            break;
            case 3:
                return previewModel4;
            break;
            case 4:
                return previewModel5;
            break;
            default:
                return previewModel5;
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

        // Save it in the temp file (since the path can be changed anytime, we don't want to copy it everytime)
        string json = JsonUtility.ToJson(inputQuestion);

        // Check if it is a new question or not, and save it at the appropriate place
        SaveAtTheRightPlace(json);

        // Display the question name in the question preview on the creator menu

        DisplayNameCorrectly(inputQuestion.name);
    }

    // Method that displays the name on the right button
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
            File.WriteAllText(Menus.tempSavePath + "Question" + index + ".json", json);
        } else {

            // Case it is an already existing question
            File.WriteAllText(Menus.tempSavePath + Menus.editedFileName, json);

            // Reset the name
            Menus.editedFileName = "";
        }
    }

    // Method that saves the question, answer and name of an input question when the naming window is displayed and user clicks on the create button
    public void SaveMultipleChoiceQuestion()
    {
        // First enter the information that is already known
        MultipleChoiceQuestion multipleChoiceQuestion = new MultipleChoiceQuestion();
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
            // Increase the current question index by one
            Menus.currentQuestionIndex = Menus.currentQuestionIndex + 1;

            // Actualize the number of pages
            ActualizePageNumber();

            // Enable the right buttons
            ActualizeButtons();
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
        double value = (double) Menus.currentQuestionIndex / (double) 5;
        Menus.numberOfPages = System.Convert.ToInt32(System.Math.Ceiling(value));
        if(Menus.numberOfPages > 1){
            // Case there is more than one page
            headingPageNumber.text = "Page " + Menus.currentPage + "/" + Menus.numberOfPages;
        } else {
            // Case there are no directories, but a page still needs to be displayed
            headingPageNumber.text = "Page " + Menus.currentPage + "/" + 1;
        }
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Editing questions again methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method that permitts to enter edit mode again on a certain question that is selected
    public void EnterEditMode()
    {
        // Get the button name
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        Button currentButton = GameObject.Find(buttonName).GetComponent<Button>();

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

            // Open the input mode window, copy the content back in
            EnableEditModeInput(question);
            
            // Enable the delete question button since the edit could have the goal of deleting the question
            deleteInputQuestion.gameObject.SetActive(true);

        } else {

            // Case multiple choice question
            MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(json);

            // Open the multiple choice mode window, copy the content back in
            EnableEditModeMultipleChoice(question);

            // Enable the delete question button since the edit could have the goal of deleting the question
            deleteMultipleChoiceQuestion.gameObject.SetActive(true);
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
    public int GetButtonIndexFromButtonName(string buttonName)
    {
        // First get the index inside the page
        int indexOnPage = 0;

        switch(buttonName)
        {
            case "Add3DModel1":
                indexOnPage = 0;
            break;
            case "Add3DModel2":
                indexOnPage = 1;
            break;
            case "Add3DModel3":
                indexOnPage = 2;
            break;
            case "Add3DModel4":
                indexOnPage = 3;
            break;
            case "Add3DModel5":
                indexOnPage = 4;
            break;
        }
        return indexOnPage;
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

    // -------------------------------------------------------------------------------------------------------------------
    // Set end save directory methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method that gives access to the brows directories menu
    public void SetDirectory()
    {
        // First set the selection button of the brows directories menu active
        Menus.directorySelection = true;

        // Then enable / disable the menus
        mainCreator.SetActive(false);
        browsDirectoriesMenu.SetActive(true);

        // Enable the select button
        selectButton.gameObject.SetActive(true);
    }

    // Method that gets the selected path (where to save the exercises) from the brows directory script  and actualizes the text that should display it
    public void ActualizeSavePath()
    {
        // Change the text
        savePathText.text = @"...\" + Globals.selectedPathShorten;
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Save to end directory methods
    // -------------------------------------------------------------------------------------------------------------------

    // Method that copies all files form one directory to another (used for temp save directory and end save directory)
    public void CopyFromPath1ToPath2(string path1, string path2)
    {
        // Copy all files that are in the path1 to the directory in path 2
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

        // Check if a path was selected, a question created
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

                // Rename all modelXYZ in the temp save folder accordingly TODO later
                int newModelNumber = RenameModels(Globals.selectedPath, 0);

                // Set all information that are already known
                logFile.numberOfQuestions = Menus.currentQuestionIndex;

                // Add the information how many models there are
                logFile.numberOfModels = newModelNumber;

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
                int newNumber = renameQuestions(Menus.tempSavePath, number);

                // Rename all modelXYZ in the temp save folder accordingly TODO later
                int newModelNumber = RenameModels(Globals.selectedPath, logFile.numberOfModels);

                Debug.Log("The new number of question was: " + newNumber);

                // Actualize the number of questions
                logFile.numberOfQuestions = newNumber;

                // Actualize the number of models
                logFile.numberOfModels = newModelNumber;

                // Convert it back to json
                string jsonNew = JsonUtility.ToJson(logFile);

                // Delete the old file
                File.Delete(Globals.selectedPath + "Description.json");

                // Create the new file
                File.WriteAllText(Menus.tempSavePath + "Description.json", jsonNew);
            }

            // Add all 3D model information to the questions
            AddModelInformation();

            // // Delete models that have no model file in the folder
            // DeleteDuplicateModels();

            // Delete all old save files that were copied to the temp save directory
            DeleteAllDuplicateFiles(Menus.tempSavePath, Globals.selectedPath);

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

    // // Method that deletes the duplicate models in the temp save folder before all files are transfered
    // public void DeleteDuplicateModels()
    // {
    //     // Get the string of paths to .obj files in the temp save filder
    //     string[] modelsTemSave = GetModelsObjArray(Menus.tempSavePath);

    //     // Get the models array of the end save folder
    //     string[] modelsEndSave = GetModelsObjArray(Globals.selectedPath);

    //     foreach(string tempModel in modelsTemSave)
    //     {
    //         foreach(string endModel in modelsEndSave)
    //         {
    //             if(Path.GetFileName(tempModel) == Path.GetFileName(endModel))
    //             {
    //                 // Delete the model that is a duplicate
    //                 File.Delete(tempModel);
    //             }
    //         }
    //     }
    // }

    // Method that deletes all files that exist in the first path in the second path
    public void DeleteAllDuplicateFiles(string path1, string path2)
    {
        // Get the array of all files in the temp save folder
        string[] tempArray = GetJsonFileArray(path1);

        // Iterate over all files in the temp save folder
        foreach(string file in tempArray)
        {
            Debug.Log("File: " + Path.GetFileName(file) + " is being tested on duplicate.");
            // Check if the file name exist in the other directory
            if(File.Exists(path2 + Path.GetFileName(file)))
            {
                Debug.Log("The file: " + Path.GetFileName(file) + " was deleted.");
                // If it exist, then delete the one in the second path
                File.Delete(path2 + Path.GetFileName(file));
            }
        }
    }

    // Method that returns the array of all .json files in a directory
    static string[] GetJsonFileArray(string path) 
    {
        string[] files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
        return files;
    }

    // Method that checks if there are 3D models with the same name in the end save directory
    public void MergeModels()
    {
        // // Get the models array of the temp save folder
        // string[] modelsTemSave = ReturnModelNames(Menus.tempSavePath);

        // Initialize the obj name array
        string[] modelsTemSave = ReturnPreviewModelNames();

        // Get the models array of the end save folder
        string[] modelsEndSave = ReturnModelNames(Globals.selectedPath);

        // Initialize a counter variable that counts the number of matches
        int matches = 0;

        // Initialize an array that contain the name of the files that coincide
        string[] modelMatches = new string[4];

        string tempModelName = "";

        // Check if no name coincide
        foreach(string modelTemp in modelsTemSave)
        {
            foreach(string modelEnd in modelsEndSave)
            {
                // string temp;
                if(modelEnd == modelTemp)
                {
                    // Add the name of the file that already exist in the end save directory to the array
                    modelMatches[matches] = modelTemp;

                    // Increase the match variable by one
                    matches = matches + 1;
                }
            }
        }

        // If no match, then can start the next method that will save evertything
        if(matches == 0)
        {
            // Launch the save method
            SaveQuestionsInEndDirectory();

        } else {

            // Activate the matches window
            ActivateMatchesWindow(modelMatches, matches);

            // Change the matches heading
            matchesHeading.text = "There are currently " + matches + " matches. Please rename the models that are not suposed to be the same. Models that are not renamed will be replaced by the already saved one.";
        }
    }

    // Method that saves the new names for the matches and proceeds to the saving
    public void SaveMatchNamesAndProceedWithSaving()
    {
        // First rename the matches
        RenameMatches();

        // Close the matches window
        DeactivateMatchesWindow();

        // Proceed with the saving
        SaveQuestionsInEndDirectory();
    }

    // Method that merges the models that were not renamed, and rename the models that were given a new name
    public void RenameMatches()
    {
        // Define the .obj ending
        string ending = ".obj";

        // Check if the first button has a name, and check if the input field was filled
        if(modelNameMatch1.GetComponentInChildren<TMP_Text>().text != "")
        {
            if(enterNewModelNameMatch1.text != "")
            {
                // Find the model Name
                string modelPath = GetFileFromModelName(Menus.tempSavePath, modelNameMatch1.GetComponentInChildren<TMP_Text>().text);

                // Extract the json
                string jsonModel = File.ReadAllText(modelPath);

                // Create the model json 
                Model modelJson = JsonUtility.FromJson<Model>(jsonModel);

                // Fill the number of questions used in
                modelJson.modelName = enterNewModelNameMatch1.text;

                // Create the new json string
                string jsonModelNew = JsonUtility.ToJson(modelJson);

                // Delete the old file
                File.Delete(modelPath);

                // Save the new model file in the end save folder
                File.WriteAllText(modelPath, jsonModelNew);
            }
        }

        // Check if the second button has a name, and check if the input field was filled
        if(modelNameMatch2.GetComponentInChildren<TMP_Text>().text != "")
        {
            if(enterNewModelNameMatch2.text != "")
            {
                // Find the model Name
                string modelPath = GetFileFromModelName(Menus.tempSavePath, modelNameMatch2.GetComponentInChildren<TMP_Text>().text);

                // Extract the json
                string jsonModel = File.ReadAllText(modelPath);

                // Create the model json 
                Model modelJson = JsonUtility.FromJson<Model>(jsonModel);

                // Fill the number of questions used in
                modelJson.modelName = enterNewModelNameMatch1.text;

                // Create the new json string
                string jsonModelNew = JsonUtility.ToJson(modelJson);

                // Delete the old file
                File.Delete(modelPath);

                // Save the new model file in the end save folder
                File.WriteAllText(modelPath, jsonModelNew);
            }
        }

        // Check if the third button has a name, and check if the input field was filled
        if(modelNameMatch3.GetComponentInChildren<TMP_Text>().text != "")
        {
            if(enterNewModelNameMatch3.text != "")
            {
                // Find the model Name
                string modelPath = GetFileFromModelName(Menus.tempSavePath, modelNameMatch3.GetComponentInChildren<TMP_Text>().text);

                // Extract the json
                string jsonModel = File.ReadAllText(modelPath);

                // Create the model json 
                Model modelJson = JsonUtility.FromJson<Model>(jsonModel);

                // Fill the number of questions used in
                modelJson.modelName = enterNewModelNameMatch1.text;

                // Create the new json string
                string jsonModelNew = JsonUtility.ToJson(modelJson);

                // Delete the old file
                File.Delete(modelPath);

                // Save the new model file in the end save folder
                File.WriteAllText(modelPath, jsonModelNew);
            }
        }

        // Check if the fourth button has a name, and check if the input field was filled
        if(modelNameMatch4.GetComponentInChildren<TMP_Text>().text != "")
        {
            if(enterNewModelNameMatch4.text != "")
            {
                // Find the model Name
                string modelPath = GetFileFromModelName(Menus.tempSavePath, modelNameMatch4.GetComponentInChildren<TMP_Text>().text);

                // Extract the json
                string jsonModel = File.ReadAllText(modelPath);

                // Create the model json 
                Model modelJson = JsonUtility.FromJson<Model>(jsonModel);

                // Fill the number of questions used in
                modelJson.modelName = enterNewModelNameMatch1.text;

                // Create the new json string
                string jsonModelNew = JsonUtility.ToJson(modelJson);

                // Delete the old file
                File.Delete(modelPath);

                // Save the new model file in the end save folder
                File.WriteAllText(modelPath, jsonModelNew);
            }
        }

        // Check if the fifth button has a name, and check if the input field was filled
        if(modelNameMatch5.GetComponentInChildren<TMP_Text>().text != "")
        {
            if(enterNewModelNameMatch5.text != "")
            {
                // Find the model Name
                string modelPath = GetFileFromModelName(Menus.tempSavePath, modelNameMatch5.GetComponentInChildren<TMP_Text>().text);

                // Extract the json
                string jsonModel = File.ReadAllText(modelPath);

                // Create the model json 
                Model modelJson = JsonUtility.FromJson<Model>(jsonModel);

                // Fill the number of questions used in
                modelJson.modelName = enterNewModelNameMatch1.text;

                // Create the new json string
                string jsonModelNew = JsonUtility.ToJson(modelJson);

                // Delete the old file
                File.Delete(modelPath);

                // Save the new model file in the end save folder
                File.WriteAllText(modelPath, jsonModelNew);
            }
        }
    }

    // Method that adds the 3D model information to all questions that will get saved in the end save directory
    public void AddModelInformation()
    {
        // Get the questions array
        string[] questions = GetQuestionsArray(Menus.tempSavePath);

        // Get the model names array in the temp save folder
        string[] tempModelArray = ReturnModelNames(Menus.tempSavePath);

        // Get the model array in the selected end save folder
        string[] endModelArray = GetModelsArray(Globals.selectedPath);

        // Extract the json string of the description
        string jsonDescription = File.ReadAllText(Menus.tempSavePath + "Description.json");
        Log description = JsonUtility.FromJson<Log>(jsonDescription);

        // Initialize the number of models
        int numberOfModels = 0;

        // Get the right number of models
        foreach(string tempModelName in tempModelArray)
        {
            numberOfModels = numberOfModels + 1;
        }

        // Initialize the number of questions
        int numberOfQuestions = 0;

        // Get the right number of questions
        foreach(string question in questions)
        {
            numberOfQuestions = numberOfQuestions + 1;
        }

        int currentModelIndex = 0;

        if(Globals.currentlyChangingFile == false)
        {
            // For each model in the temp save folder, check if the name exist in a ModelXYZ file in the end save folder
            foreach(string tempModelName in tempModelArray)
            {
                string match = FindMatchingFile(endModelArray, tempModelName);

                string modelFileName = "";

                // If there is no match, create a new ModelXYZ file and write it in each question
                if(match == "")
                {
                    // Debug.Log("There was no match, the number of models in the description file should have increased by one.");
                    // description.numberOfModels = description.numberOfModels + 1;

                } else {
                    Debug.Log("Match found for the model: " + tempModelName);
                    // Case match, extract the model object (without moving the file)
                    modelFileName = AddUseToModelFile(match, numberOfQuestions);
                }

                // Check that the model file name is not empty
                if(modelFileName == "")
                {
                    // Find the json model file name from the model name
                    modelFileName = GetFileFromModelName(Menus.tempSavePath, tempModelName);
                }

                // Save the model file name in the questions
                foreach(string question in questions)
                {
                    Debug.Log("Currently editing question: " + Path.GetFileName(question));
                    string json = File.ReadAllText(question);

                    // Find out what type of question it is
                    if(json.Contains("input question") == true)
                    {
                        Debug.Log("Entered input question edit");
                        // Case input question
                        InputQuestion inputQuestion = JsonUtility.FromJson<InputQuestion>(json);

                        // Set the models name correctly
                        // Set the first model if there is one
                        if(currentModelIndex == 0)
                        {
                            inputQuestion.model1Name = Path.GetFileName(modelFileName);
                        }

                        // Set the second model if there is one
                        if(currentModelIndex == 1)
                        {
                            inputQuestion.model2Name = Path.GetFileName(modelFileName);
                        }

                        // Set the third model if there is one
                        if(currentModelIndex == 2)
                        {
                            inputQuestion.model3Name = Path.GetFileName(modelFileName);
                        }

                        // Set the fourth model if there is one
                        if(currentModelIndex == 3)
                        {
                            inputQuestion.model4Name = Path.GetFileName(modelFileName);
                        }

                        // Set the fifth model if there is one
                        if(currentModelIndex == 4)
                        {
                            inputQuestion.model5Name = Path.GetFileName(modelFileName);
                        }

                        // IncreaseNumberOfQuestionsUsedIn(modelFileName);

                        // Set the number of models
                        inputQuestion.numberOfModels = numberOfModels;

                        // Delete old save file
                        File.Delete(question);

                        // Convert to json
                        string newJson = JsonUtility.ToJson(inputQuestion);

                        // Create new save file
                        File.WriteAllText(question, newJson);

                    } else {

                        Debug.Log("Entered multiple choice question edit");
                        // Case multiple choice question
                        MultipleChoiceQuestion multipleChoiceQuestion = JsonUtility.FromJson<MultipleChoiceQuestion>(json);

                        // Set the number of models and models
                        // Set the first model if there is one
                        if(currentModelIndex == 0)
                        {
                            multipleChoiceQuestion.model1Name = Path.GetFileName(modelFileName);
                        }

                        // Set the second model if there is one
                        if(currentModelIndex == 1)
                        {
                            multipleChoiceQuestion.model2Name = Path.GetFileName(modelFileName);
                        }

                        // Set the third model if there is one
                        if(currentModelIndex == 2)
                        {
                            multipleChoiceQuestion.model3Name = Path.GetFileName(modelFileName);
                        }

                        // Set the fourth model if there is one
                        if(currentModelIndex == 3)
                        {
                            multipleChoiceQuestion.model4Name = Path.GetFileName(modelFileName);
                        }

                        // Set the fifth model if there is one
                        if(currentModelIndex == 4)
                        {
                            multipleChoiceQuestion.model5Name = Path.GetFileName(modelFileName);
                        }

                        // IncreaseNumberOfQuestionsUsedIn(modelFileName);

                        // Set the number of models
                        multipleChoiceQuestion.numberOfModels = numberOfModels;

                        // Delete old save file
                        File.Delete(question);

                        // Convert to json
                        string newJson = JsonUtility.ToJson(multipleChoiceQuestion);

                        // Create new save file
                        File.WriteAllText(question, newJson);
                    }
                }
                // Increase the current model index by one
                currentModelIndex = currentModelIndex + 1;
            }
        } else {
            // Case we are currently changing a question file means the single question need to be actualized

            // Get the question to edit
            string questionToChange = questions[0];

            // Initialize the number of buttons that have names
            int numberOfButtonsWithModelName = 0;

            // Get the text of all model preview button
            string previewModel1Text = previewModel1.GetComponentInChildren<TMP_Text>().text;
            string previewModel2Text = previewModel2.GetComponentInChildren<TMP_Text>().text;
            string previewModel3Text = previewModel3.GetComponentInChildren<TMP_Text>().text;
            string previewModel4Text = previewModel4.GetComponentInChildren<TMP_Text>().text;
            string previewModel5Text = previewModel5.GetComponentInChildren<TMP_Text>().text;

            // Create all ModelXYZ files

            // If the first button has a heading, check if there is a .obj object of that name
            string firstModelExist = CreateOrLoadModelFileIfNeeded(previewModel1Text, endModelArray, description.numberOfModels, 1);

            if(firstModelExist != "no")
            {
                Debug.Log("Button preview 1 is not empty");
                Debug.Log("The outcome of the create or load model file was: " + firstModelExist);

                // Increase the number of buttons that have a model name by one
                numberOfButtonsWithModelName = numberOfButtonsWithModelName + 1;

                // Increase the number of models in the description by one if a new model was added
                if(firstModelExist == "increase")
                {
                    description.numberOfModels = description.numberOfModels + 1;
                    Debug.Log("The number of models in the description is: " + description.numberOfModels);
                }
            }

            // If the second button has a heading, check if there is a .obj object of that name
            string secondModelExist = CreateOrLoadModelFileIfNeeded(previewModel2Text, endModelArray, description.numberOfModels, 1);
            if(secondModelExist != "no") 
            {
                Debug.Log("Button preview 2 is not empty");
                Debug.Log("The outcome of the create or load model file was: " + secondModelExist);

                // Increase the number of buttons that have a model name by one
                numberOfButtonsWithModelName = numberOfButtonsWithModelName + 1;

                // Increase the number of models in the description by one if a new model was added
                if(secondModelExist == "increase")
                {
                    // description.numberOfModels = description.numberOfModels + 1;
                    // Debug.Log("The number of models in the description is: " + description.numberOfModels);
                }
            }

            // If the third button has a heading, check if there is a .obj object of that name
            string thirdModelExist = CreateOrLoadModelFileIfNeeded(previewModel3Text, endModelArray, description.numberOfModels, 1);
            if(thirdModelExist != "no") 
            {
                Debug.Log("Button preview 3 is not empty");
                Debug.Log("The outcome of the create or load model file was: " + thirdModelExist);

                // Increase the number of buttons that have a model name by one
                numberOfButtonsWithModelName = numberOfButtonsWithModelName + 1;

                // Increase the number of models in the description by one if a new model was added
                if(thirdModelExist == "increase")
                {
                    // description.numberOfModels = description.numberOfModels + 1;
                    // Debug.Log("The number of models in the description is: " + description.numberOfModels);
                }
            }

            // If the fourth button has a heading, check if there is a .obj object of that name
            string fourthModelExist = CreateOrLoadModelFileIfNeeded(previewModel4Text, endModelArray, description.numberOfModels, 1);
            if(fourthModelExist != "no") 
            {
                Debug.Log("Button preview 4 is not empty");
                Debug.Log("The outcome of the create or load model file was: " + fourthModelExist);

                // Increase the number of buttons that have a model name by one
                numberOfButtonsWithModelName = numberOfButtonsWithModelName + 1;

                // Increase the number of models in the description by one if a new model was added
                if(fourthModelExist == "increase")
                {
                    // description.numberOfModels = description.numberOfModels + 1;
                    // Debug.Log("The number of models in the description is: " + description.numberOfModels);
                }
            }
            
            // If the fifth button has a heading, check if there is a .obj object of that name
            string fifthModelExist = CreateOrLoadModelFileIfNeeded(previewModel5Text, endModelArray, description.numberOfModels, 1);
            if(fifthModelExist != "no") 
            {
                Debug.Log("Button preview 5 is not empty");

                // Increase the number of buttons that have a model name by one
                numberOfButtonsWithModelName = numberOfButtonsWithModelName + 1;

                // Increase the number of models in the description by one if a new model was added
                if(fifthModelExist == "increase")
                {
                    // description.numberOfModels = description.numberOfModels + 1;
                    // Debug.Log("The number of models in the description is: " + description.numberOfModels);
                }
            }

            // Copy the model files that are not needed back to the end save folder
            CopyNotUsedModelFilesBack(Menus.tempSavePath, Globals.selectedPath);

            // Now that all model files are created and loaded in the temp save file, add all model files to the question

            // Get the json string
            string jsonChangeQuestion = File.ReadAllText(questionToChange);

            // Get the modelXYZ array in the temp save folder
            string[] tempModelArray2 = GetModelsArray(Menus.tempSavePath);

            // Extract the object
            if(jsonChangeQuestion.Contains("input question") == true)
            {
                // Case it is an input question
                InputQuestion inputQuestion = JsonUtility.FromJson<InputQuestion>(jsonChangeQuestion);

                int fillLoopIndex = 0;

                foreach(string TempModelFile in tempModelArray2)
                {
                    string modelFileName = Path.GetFileName(TempModelFile);
                    switch(fillLoopIndex)
                    {
                        case 0:
                            inputQuestion.model1Name = modelFileName;
                        break;
                        case 1:
                            inputQuestion.model2Name = modelFileName;
                        break;
                        case 2:
                            inputQuestion.model3Name = modelFileName;
                        break;
                        case 3:
                            inputQuestion.model4Name = modelFileName;
                        break;
                        case 4:
                            inputQuestion.model5Name = modelFileName;
                        break;
                    }
                    fillLoopIndex = fillLoopIndex + 1;
                }

                // Set the right number of models
                inputQuestion.numberOfModels = numberOfButtonsWithModelName;

                // Delete old save file
                File.Delete(questionToChange);

                // Convert to json
                string newJson = JsonUtility.ToJson(inputQuestion);

                // Create new save file
                File.WriteAllText(questionToChange, newJson);

            } else {
                // Case it is a multiple choice question
                MultipleChoiceQuestion multipleChoiceQuestion = JsonUtility.FromJson<MultipleChoiceQuestion>(jsonChangeQuestion);

                int fillLoopIndex = 0;

                foreach(string TempModelFile in tempModelArray)
                {
                    string modelFileName = Path.GetFileName(TempModelFile);
                    switch(fillLoopIndex)
                    {
                        case 0:
                            multipleChoiceQuestion.model1Name = modelFileName;
                        break;
                        case 1:
                            multipleChoiceQuestion.model2Name = modelFileName;
                        break;
                        case 2:
                            multipleChoiceQuestion.model3Name = modelFileName;
                        break;
                        case 3:
                            multipleChoiceQuestion.model4Name = modelFileName;
                        break;
                        case 4:
                            multipleChoiceQuestion.model5Name = modelFileName;
                        break;
                    }
                }

                // Set the right number of models
                multipleChoiceQuestion.numberOfModels = numberOfButtonsWithModelName;

                // Delete old save file
                File.Delete(questionToChange);

                // Convert to json
                string newJson = JsonUtility.ToJson(multipleChoiceQuestion);

                // Create new save file
                File.WriteAllText(questionToChange, newJson);
            }
        }

        // Delete old save file
        File.Delete(Menus.tempSavePath + "Description.json");

        // Set the right number of models
        if(Globals.currentlyChangingFile == true)
        {
            // If a file is getting changed, then the number of models can just be added TODO could be wrong
            description.numberOfModels = description.numberOfModels + numberOfModels;
        }
        // Convert to json
        string newDescriptionJson = JsonUtility.ToJson(description);

        // Create new save file
        File.WriteAllText(Menus.tempSavePath + "Description.json", newDescriptionJson);
    }

    // Method that search for model match in a given array of model files given a name
    public string FindMatchingFile(string[] array, string name)
    {
        string match = "";
        foreach(string endModel in array)
        {
            // Extract the json string and get the object
            string jsonModel = File.ReadAllText(endModel);
            Model model = JsonUtility.FromJson<Model>(jsonModel);

            if(model.modelName == name)
            {
                match = endModel;
            }
        }
        return match;
    }

    // Method used to add the number of questions the model is used in
    public void IncreaseNumberOfQuestionsUsedIn(string modelLink)
    {
        string jsonModel = File.ReadAllText(modelLink);

        // Create the model json 
        Model modelJson = JsonUtility.FromJson<Model>(jsonModel);

        // Fill the number of questions used in
        modelJson.numberOfQuestionsUsedIn = modelJson.numberOfQuestionsUsedIn + 1;

        // Create the new json string
        string jsonModelNew = JsonUtility.ToJson(modelJson);

        string fileName = Path.GetFileName(modelLink);

        // Get the model file name
        // string modelFileName = fileName + ".json";
        string modelFileName = fileName;

        // Delete the old file
        File.Delete(modelLink);

        // Save the new model file in the end save folder
        File.WriteAllText(Menus.tempSavePath + modelFileName, jsonModelNew);
    }

    // // Method that creates a modelXYZ file at the given path with number of questions used in, name of model and description object given and returns name of file
    // public string CreateModelFile(string name, int numberOfQuestionsUsedIn, int nextIndex)
    // {
    //     // Case no match, create a new model and set the information
    //     Model model = new Model();
    //     model.numberOfQuestionsUsedIn = numberOfQuestionsUsedIn;
    //     model.modelName = name;

    //     // Get the index
    //     string index = ReturnQuestionIndex(nextIndex);
    //     string newJson = JsonUtility.ToJson(model);

    //     // Create the model file
    //     File.WriteAllText(Menus.tempSavePath + "Model" + index + ".json", newJson);

    //     // Return the model file name correctly
    //     return "Model" + index + ".json";
    // }

    // Method that creates or loads a model file if it is needed
    public string CreateOrLoadModelFileIfNeeded(string name, string[] array, int numberOfModels, int numberOfQuestions)
    {
        if(name != "+" && name != "")
        {
            if(File.Exists(Menus.tempSavePath + name))
            {
                // Case the object was added, check if that model exist in the end save folder
                // Check that the model was not used in the end save folder and already has a file
                string matchChange = FindMatchingFile(array, name);

                Debug.Log("Current match is: " + matchChange);

                string modelFileNameChange = "";

                // If there is no match, create a new ModelXYZ file and write it in each question
                if(matchChange == "")
                {
                    Debug.Log("A new model file was created.");
                    // Case no match, create a new model and set the information
                    // modelFileNameChange = CreateModelFile(name, 1, numberOfModels);

                    // AddNumberOfQuestionsUsedIn(Path.Combine(Menus.tempSavePath, name), Menus.currentQuestionIndex);

                    // Increase description.numberOfModels by one since it was created
                    return "increase";

                } else {
                    Debug.Log("An additional use was added to an already existing model file");
                    // Case match, extract the model object
                    modelFileNameChange = AddUseToModelFile(matchChange, numberOfQuestions);

                    // Load this file in the temp save directory
                    File.Copy(matchChange, Path.Combine(Globals.tempSavePath, modelFileNameChange));

                    return "same";
                }
            }
            return "same";

        } else {
            return "no";
        }
    }

    // Method that copies the model files that are not used (not in preview) back to the end save directory, and deletes them form the temp save folder
    public void CopyNotUsedModelFilesBack(string path1, string path2)
    {
        // Get the array of the models in preview
        string[] previewModelArray = ReturnPreviewModelNames();

        // Get the array of model save files
        string[] modelFileArray = GetModelsArray(path1);

        int[] modelUsedArray = new int[modelFileArray.Length];

        foreach(string model in previewModelArray)
        {
            // If the model name is not empty, check if it is used in a file
            if(model != "")
            {
                int counter = 0;
                foreach(string modelFile in modelFileArray)
                {
                    // Extract the object
                    string json = File.ReadAllText(modelFile);
                    Model modelObject = JsonUtility.FromJson<Model>(json);

                    // Check if the file contains the model
                    if(modelObject.modelName == model)
                    {
                        // Write a one in the model used array
                        modelUsedArray[counter] = 1;
                    }

                    // Increase the loop index by one
                    counter = counter + 1;
                }
            }
        }

        // Now we have an array with ones at the index of a model file that is used
        int index = 0;
        foreach(int used in modelUsedArray)
        {
            // Case not used, copy it back to the end save directory and delete it in the temp save directory
            if(used == 0)
            {
                // Delete the old file
                File.Delete(path2 + Path.GetFileName(modelFileArray[index]));

                // Extract the object
                string jsonModel = File.ReadAllText(modelFileArray[index]);
                Model modelObjectNotUsed = JsonUtility.FromJson<Model>(jsonModel);

                // Check the number of questions it is used in
                if(modelObjectNotUsed.numberOfQuestionsUsedIn > 0)
                {
                    // If the number is at least one, copy the new file to the end save folder
                    System.IO.File.Move(modelFileArray[index], path2 + Path.GetFileName(modelFileArray[index]));
                } else {

                    // If the number is 0, delete the .obj model
                    File.Delete(path2 + modelObjectNotUsed.modelName);
                }

                // Delete the new file in the temp save folder
                File.Delete(modelFileArray[index]);
            }

            // Increase the loop index by one
            index = index + 1;
        }
    }

    // Method that changes the number of questions that use a model with a given additional number and model file path
    public string AddUseToModelFile(string match, int numberOfQuestions)
    {
        // Case match, extract the model object
        string jsonModel = File.ReadAllText(match);
        Model model = JsonUtility.FromJson<Model>(jsonModel);

        // Increase the number of questions used in
        model.numberOfQuestionsUsedIn = model.numberOfQuestionsUsedIn + numberOfQuestions;

        // Delete the old save file
        File.Delete(match);

        // Convert to json
        string newJson = JsonUtility.ToJson(model);

        // Create new save file
        File.WriteAllText(match, newJson);

        // Set the model file name correctly
        return Path.GetFileName(match);
    }

    // Method that saves the question back in the back-end save directory after editing it (when choosing the question in the brows directories menu)
    public void ChangeQuestionInEndDirectory() 
    {
        // Check if the question was changed or deleted
        if(previewQuestion1.GetComponentInChildren<TMP_Text>().text != "deleted")
        {
            // // Case it was not deleted, then delete the old file in the back end folder
            // File.Delete(Globals.filePath);

            // Load the description in the temp save folder
            File.Copy(Path.Combine(Globals.selectedPath, "description.json"), Path.Combine(Globals.tempSavePath, "description.json"));

            // Add all 3D model information to the questions
            AddModelInformation();

            // // Delete models that have no model file in the folder
            // DeleteDuplicateModels();

            // Delete all old save files that were copied to the temp save directory
            DeleteAllDuplicateFiles(Menus.tempSavePath, Globals.selectedPath);

            // Copy it back in
            CopyFromPath1ToPath2(Menus.tempSavePath, Globals.selectedPath);

            // Actualize the number of models
            ActualizeDescriptionFile(Globals.selectedPath);

            // Exit the creator
            ExitWithoutSavingYes();
        }
    }

    // Method that actualizes the number of questions and models in the description file
    public void ActualizeDescriptionFile(string path)
    {
        // // Get the number of ModelXYZ files
        // int numberOfModels = numberOfModelFiles(path);

        // Get the number of QUestionXYZ files
        int numberOfQuestions = numberOfQuestionFiles(path);

        // Extract the description files information and object
        string json = File.ReadAllText(path + "Description.json");
        Log descriptionLog = JsonUtility.FromJson<Log>(json);

        // Reduce the number of questions by one
        descriptionLog.numberOfQuestions = numberOfQuestions;
        // descriptionLog.numberOfModels = numberOfModels;

        // Delete the old description file
        File.Delete(path + "Description.json");

        // Save the description log file back in
        string jsonActualized =  JsonUtility.ToJson(descriptionLog);
        File.WriteAllText(path + "Description.json", jsonActualized);
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Creation of the log file in the back end, renaming of files
    // -------------------------------------------------------------------------------------------------------------------

    // Method that renames all question in the given path to QuestionXYZ given an index V
    public int renameQuestions(string path, int index)
    {
        // Get the question array
        string[] questions = GetQuestionsArray(path);

        // Give them a dumy name
        int loopIndex = index;
        int lastIndex = questions.Length;
        foreach(string question in questions)
        {
            System.IO.File.Move(question, path + loopIndex.ToString());
            loopIndex = loopIndex + 1;
        }

        // Then rename them with names that it should have in the new save folder
        int newIndex = index;
        foreach(string question in questions)
        {
            // Generate the right index at the end of the name
            string ending = ReturnQuestionIndex(newIndex);
            string name = "Question" + ending;
            System.IO.File.Move(path +newIndex.ToString(), path + name + ".json");
            newIndex = newIndex + 1;
        }
        return newIndex;
    }

    // Method used to rename and create the model files when saving them in the end save directory
    public int RenameModels(string path, int index)
    {
        // Initialize the model name array
        string[] newModelNames = ReturnPreviewModelNames();

        // Get the array of models in the end save directory
        string[] oldModelNames = GetModelsArray(path);

        int numberOfModelsThatNeedNames = Menus.currentModelIndex + 1;

        // int newIndex = index;

        // // Check if there are model names not used in the 
        // for(int i = 0; i < newIndex; i = i + 1)
        // {
        //     if(numberOfModelsThatNeedNames >= 0)
        //     {
        //         // Generate the right index at the end of the name
        //         string ending = ReturnQuestionIndex(i);
        //         string name = "Model" + ending + ".json";

        //         if(oldModelNames.Contains(name) != true)
        //         {
        //             // Initialize the url
        //             string url = "";

        //             // Initialize the button name
        //             string modelName = "";

        //             // Get the right uri
        //             switch(numberOfModelsThatNeedNames)
        //             {
        //                 case 1:
        //                     url = Menus.url1;
        //                     modelName = previewModel1.GetComponentInChildren<TMP_Text>().text;
        //                 break;

        //                 case 2:
        //                     url = Menus.url2;
        //                     modelName = previewModel2.GetComponentInChildren<TMP_Text>().text;
        //                 break;

        //                 case 3:
        //                     url = Menus.url3;
        //                     modelName = previewModel3.GetComponentInChildren<TMP_Text>().text;
        //                 break;

        //                 case 4:
        //                     url = Menus.url4;
        //                     modelName = previewModel4.GetComponentInChildren<TMP_Text>().text;
        //                 break;

        //                 case 5:
        //                     url = Menus.url5;
        //                     modelName = previewModel5.GetComponentInChildren<TMP_Text>().text;
        //                 break;
        //             }

        //             // if(url == "")
        //             // {
        //             //     url = Menus.url1;
        //             //     modelName = previewModel1.GetComponentInChildren<TMP_Text>().text;
        //             // }

        //             // // Extract the file name
        //             // System.Uri uri = new Uri(url);
        //             // string modelName = System.IO.Path.GetFileName(uri.LocalPath);

        //             // Create the model file with that name
        //             Model modelJson = new Model();

        //             // Fill the model name and the url
        //             modelJson.modelName = modelName;
        //             modelJson.modelUrl = url;
        //             modelJson.numberOfQuestionsUsedIn = Menus.currentQuestionIndex;

        //             // Create the new json string
        //             string jsonModelNew = JsonUtility.ToJson(modelJson);

        //             // Save the new model file in the end save folder
        //             File.WriteAllText(Menus.tempSavePath + name, jsonModelNew);

        //             Debug.Log("The model file with name: " + name + " should have been saved in: " + Menus.tempSavePath);

        //             // // Increase the new index by one since the maximum model name was changed
        //             // newIndex = newIndex + 1;
        //         }
        //     }
        // }

        // Initialize the number of models that still need a name and file
        int numberThatStillNeed = numberOfModelsThatNeedNames;
        int indexRest = 0;

        while(indexRest <= numberOfModelsThatNeedNames)
        {
            // Generate the right index at the end of the name
            string ending = ReturnQuestionIndex(i);
            string name = "Model" + ending + ".json";

            if(isContained(oldModelNames, name) != true)
            {
                // Initialize the url
                string url = "";

                // Initialize the button name
                string modelName = "";

                // Get the right uri
                switch(numberOfModelsThatNeedNames)
                {
                    case 1:
                        url = Menus.url1;
                        modelName = previewModel1.GetComponentInChildren<TMP_Text>().text;
                    break;

                    case 2:
                        url = Menus.url2;
                        modelName = previewModel2.GetComponentInChildren<TMP_Text>().text;
                    break;

                    case 3:
                        url = Menus.url3;
                        modelName = previewModel3.GetComponentInChildren<TMP_Text>().text;
                    break;

                    case 4:
                        url = Menus.url4;
                        modelName = previewModel4.GetComponentInChildren<TMP_Text>().text;
                    break;

                    case 5:
                        url = Menus.url5;
                        modelName = previewModel5.GetComponentInChildren<TMP_Text>().text;
                    break;
                }

                // if(url == "")
                // {
                //     url = Menus.url1;
                //     modelName = previewModel1.GetComponentInChildren<TMP_Text>().text;
                // }

                // // Extract the file name
                // System.Uri uri = new Uri(url);
                // string modelName = System.IO.Path.GetFileName(uri.LocalPath);

                // Create the model file with that name
                Model modelJson = new Model();

                // Fill the model name and the url
                modelJson.modelName = modelName;
                modelJson.modelUrl = url;
                modelJson.numberOfQuestionsUsedIn = Menus.currentQuestionIndex;

                // Create the new json string
                string jsonModelNew = JsonUtility.ToJson(modelJson);

                // Save the new model file in the end save folder
                File.WriteAllText(Menus.tempSavePath + name, jsonModelNew);

                Debug.Log("The model file with name: " + name + " should have been saved in: " + Menus.tempSavePath);

                // // Increase the new index by one since the maximum model name was changed
                // newIndex = newIndex + 1;
                indexRest = indexRest + 1;
            }
        }

        // // Create as much model files as is needed
        // for(int indexRest = 0; indexRest <= numberThatStillNeed; indexRest = indexRest + 1)
        // {
        //     // Generate the right index at the end of the name
        //     string ending = ReturnQuestionIndex(newIndex + indexRest);
        //     string name = "Model" + ending + ".json";

        //     // Initialize the url
        //     string url = "";

        //     // Initialize the button name
        //     string modelName = "";

        //     // Get the right uri
        //     switch(numberThatStillNeed)
        //     {
        //         case 1:
        //             url = Menus.url1;
        //             modelName = previewModel1.GetComponentInChildren<TMP_Text>().text;
        //         break;

        //         case 2:
        //             url = Menus.url2;
        //             modelName = previewModel2.GetComponentInChildren<TMP_Text>().text;
        //         break;

        //         case 3:
        //             url = Menus.url3;
        //             modelName = previewModel3.GetComponentInChildren<TMP_Text>().text;
        //         break;

        //         case 4:
        //             url = Menus.url4;
        //             modelName = previewModel4.GetComponentInChildren<TMP_Text>().text;
        //         break;

        //         case 5:
        //             url = Menus.url5;
        //             modelName = previewModel5.GetComponentInChildren<TMP_Text>().text;
        //         break;
        //     }

        //     // if(url == "")
        //     // {
        //     //     url = Menus.url1;
        //     //     modelName = previewModel1.GetComponentInChildren<TMP_Text>().text;
        //     // }

        //     // // Extract the file name
        //     // System.Uri uri = new Uri(url);
        //     // string modelName = System.IO.Path.GetFileName(uri.LocalPath);

        //     // Create the model file with that name
        //     Model modelJson = new Model();

        //     // Fill the model name and the url
        //     modelJson.modelName = modelName;
        //     modelJson.modelUrl = url;
        //     modelJson.numberOfQuestionsUsedIn = Menus.currentQuestionIndex;

        //     // Create the new json string
        //     string jsonModelNew = JsonUtility.ToJson(modelJson);

        //     // Save the new model file in the end save folder
        //     File.WriteAllText(Menus.tempSavePath + name, jsonModelNew);

        //     Debug.Log("The model file with name: " + name + " should have been saved in: " + Menus.tempSavePath);

        //     numberThatStillNeed = numberThatStillNeed - 1;
        // }

        // // Then rename them with names that it should have in the new save folder
        // int newIndex = index;
        // foreach(string question in questions)
        // {
        //     // Generate the right index at the end of the name
        //     string ending = ReturnQuestionIndex(newIndex);
        //     string name = "Question" + ending;
        //     System.IO.File.Move(path +newIndex.ToString(), path + name + ".json");
        //     newIndex = newIndex + 1;
        // }
        // return newIndex;

        return numberOfModelsThatNeedNames + index;
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Deletion of files, be it in the current temp save directory or end save directory
    // -------------------------------------------------------------------------------------------------------------------

    // Method activated when the used clicks on the "delete" buttons of the input or multiple choice question creators
    public void DeleteQuestion()
    {
        Debug.Log("Initialy, the current question index was: " + Menus.currentQuestionIndex);
        Debug.Log("Currently changing file: " + Globals.currentlyChangingFile);

        // Case the question was only a temporary save
        if(Globals.currentlyChangingFile == false)
        {
            // Delete the file
            File.Delete(Menus.tempSavePath + Menus.editedFileName);

            // Second reduce the current question index by one
            Menus.currentQuestionIndex = Menus.currentQuestionIndex - 1;

            Debug.Log("The current question index is: " + Menus.currentQuestionIndex);

            // If there still are questions, rename them
            if(Menus.currentQuestionIndex != 0)
            {
                Debug.Log("Files are geting renamed");
                // Rename all files
                RenameFilesPostDeletion(Menus.tempSavePath, Menus.currentQuestionIndex);
            }

            // Then actualize the button preview, the integer number is the first button that should be empty
            ActualizeQuestionPreview(Menus.currentQuestionIndex + 1);

            // Actualize the page number
            ActualizePageNumber();

            // Disable buttons if needed
            ActualizeButtons();

            // Close the question window, reset the inputs
            ExitWithoutSavingYes();

        // Case the question was not a temporary save and is saved as a file in the back end folder
        } else {

            Debug.Log("Entered currently changing file loop");

            // Delete the right file in the end save folder
            File.Delete(Globals.selectedPath + Menus.editedFileName);

            // Extract the description files information and object
            string json = File.ReadAllText(Globals.selectedPath + "Description.json");
            Log descriptionLog = JsonUtility.FromJson<Log>(json);

            // Reduce the number of questions by one
            descriptionLog.numberOfQuestions = descriptionLog.numberOfQuestions - 1;

            // Extract the json string of the question
            string jsonQuestion = File.ReadAllText(Menus.tempSavePath + Menus.editedFileName);

            // Check what type of question it is
            if(jsonQuestion.Contains("input question") == true)
            {
                // Case input question
                // Extract the input question object
                InputQuestion question = JsonUtility.FromJson<InputQuestion>(jsonQuestion);

                // Get the number of models that are used for the question
                int number = question.numberOfModels;
                
                // Go over all models
                for(int counter = 0; counter < number; counter = counter + 1)
                {
                    // Initialize the model file name
                    string modelFileName = "";

                    // Find the current model file name
                    switch(counter)
                    {
                        case 0:
                            modelFileName = question.model1Name;
                        break;
                        case 1:
                            modelFileName = question.model2Name;
                        break;
                        case 2:
                            modelFileName = question.model3Name;
                        break;
                        case 3:
                            modelFileName = question.model4Name;
                        break;
                        case 4:
                            modelFileName = question.model5Name;
                        break;
                    }

                    Debug.Log("The model file name that was found is: " + modelFileName);

                    // Reducte the number of questions that use it by one
                    string jsonModel = File.ReadAllText(Menus.tempSavePath + modelFileName);
                    Model model = JsonUtility.FromJson<Model>(jsonModel);
                    model.numberOfQuestionsUsedIn = model.numberOfQuestionsUsedIn - 1;

                    // If the number is 0, delete the model file, and the .obj model in the selected path file, else actualize the file
                    if(model.numberOfQuestionsUsedIn <= 0)
                    {
                        // Delete the .obj model in the end save folder
                        File.Delete(Globals.selectedPath + model.modelName);

                        // Delete the .json file in the end save folder
                        File.Delete(Globals.selectedPath + modelFileName);

                        // Reduce the number of models of the description file
                        descriptionLog.numberOfModels = descriptionLog.numberOfModels - 1;

                    } else {

                        // Delete the .json file in the end save folder
                        File.Delete(Globals.selectedPath + modelFileName);

                        // Create the new json string
                        string jsonModelNew = JsonUtility.ToJson(model);

                        // Save the new model file in the end save folder
                        File.WriteAllText(Globals.selectedPath + modelFileName, jsonModelNew);
                    }
                }

            } else {
                // Case multiple choice question
                // Extract the multiple choice question object
                MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(jsonQuestion);

                // Get the number of models that are used for the question
                int number = question.numberOfModels;
                
                // Go over all models
                for(int counter = 0; counter < number; counter = counter + 1)
                {
                    // Initialize the model file name
                    string modelFileName = "";

                    // Find the current model file name
                    switch(counter)
                    {
                        case 0:
                            modelFileName = question.model1Name;
                        break;
                        case 1:
                            modelFileName = question.model2Name;
                        break;
                        case 2:
                            modelFileName = question.model3Name;
                        break;
                        case 3:
                            modelFileName = question.model4Name;
                        break;
                        case 4:
                            modelFileName = question.model5Name;
                        break;
                    }

                    Debug.Log("The model file name that was found is: " + modelFileName);

                    // Reducte the number of questions that use it by one
                    string jsonModel = File.ReadAllText(Menus.tempSavePath + modelFileName);
                    Model model = JsonUtility.FromJson<Model>(jsonModel);
                    model.numberOfQuestionsUsedIn = model.numberOfQuestionsUsedIn - 1;

                    // If the number is 0, delete the model file, and the .obj model in the selected path file, else actualize the file
                    if(model.numberOfQuestionsUsedIn <= 0)
                    {
                        // Delete the .obj model in the end save folder
                        File.Delete(Globals.selectedPath + model.modelName);

                        // Delete the .json file in the end save folder
                        File.Delete(Globals.selectedPath + modelFileName);

                        // Reduce the number of models of the description file
                        descriptionLog.numberOfModels = descriptionLog.numberOfModels - 1;

                    } else {

                        // Delete the .json file in the end save folder
                        File.Delete(Globals.selectedPath + modelFileName);

                        // Create the new json string
                        string jsonModelNew = JsonUtility.ToJson(model);

                        // Save the new model file in the end save folder
                        File.WriteAllText(Globals.selectedPath + modelFileName, jsonModelNew);
                    }
                }
            }

            // Rename all questions correctly
            RenameFilesPostDeletion(Globals.selectedPath, descriptionLog.numberOfQuestions);

            // Delete the old description file
            File.Delete(Globals.selectedPath + "Description.json");

            // Save the description log file back in
            string jsonActualized =  JsonUtility.ToJson(descriptionLog);
            File.WriteAllText(Globals.selectedPath + "Description.json", jsonActualized);

            // Delete the preview question name
            previewQuestion1.GetComponentInChildren<TMP_Text>().text = "";

            // Reset the model preview
            ResetPreviewModelButtons();

            // At last, close the creator and delete the file in the temp save folder
            ExitWithoutSavingYes();
            ExitWithoutSavingYes();
        }
    }

    // Method that returns the array of questions in a directory
    static string[] GetQuestionsArray(string path) 
    {
        string[] questions = Directory.GetFiles(path, "Question*", SearchOption.TopDirectoryOnly);
        return questions;
    }

    // Method that renames all questionXYZ files correctly with a given path to a folder and number of files, TODO add models
    public void RenameFilesPostDeletion(string path, int questionNumber)
    {
        int number = renameQuestions(path, 0);
    }

    // Method that actualizes the question preview after a question was deleted
    public void ActualizeQuestionPreview(int number)
    {
        // Get the question array
        string[] questions = GetQuestionsArray(Menus.tempSavePath);

        // Get the current page
        int page = Menus.currentPage;

        int maxNumber = 0;
        if(number > 6)
        {
            maxNumber = 6;
        } else {
            maxNumber = number;
        }

        // Rename all buttons that preview questions accordingly
        for(int index = 1; index < maxNumber; index = index + 1)
        {
            // Get the name of the file
            string questionName = GetFileNameFromButtonName("PreviewQuestion" + index);

            // Extract the string of the file
            string json = File.ReadAllText(Menus.tempSavePath + questionName);

            // Find out what type of question it is
            if(json.Contains("input question") == true)
            {
                // Extract the input question object
                InputQuestion question = JsonUtility.FromJson<InputQuestion>(json);

                // Get the name
                string name = question.name;

                // Rename the correct button
                GameObject.Find("PreviewQuestion" + index).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = name;

            } else {
                // Extract the multiple choice question object
                MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(json);

                // Get the name
                string name = question.name;

                // Rename the correct button
                GameObject.Find("PreviewQuestion" + index).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = name;
                
            }
        }
        for(int emptyIndex = maxNumber; emptyIndex < 6; emptyIndex = emptyIndex + 1)
        {
            // Delete the name of the button
            GameObject.Find("PreviewQuestion" + emptyIndex).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    // -------------------------------------------------------------------------------------------------------------------
    // Import of 3D models
    // -------------------------------------------------------------------------------------------------------------------

    public bool extendedLogging = true;

    // Method activated when clicking on the add 3D model button
    public void Add3DModel()
    {
        // Get the button name
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        Button currentButton = GameObject.Find(buttonName).GetComponent<Button>();
        string modelName = currentButton.GetComponentInChildren<TMP_Text>().text;

        // Set the inport button name
        Menus.modelPreviewButtonName = buttonName;

        // Check if a 3D model was already imported on that button
        if(currentButton.GetComponentInChildren<TMP_Text>().text == "+")
        {
            // Case no model added, open the import model window
            ActivateImportModelWindow();

        } else {
            // Case 3D model already imported on that button, open a window where the user can delete this model
            // Open the replace model window
            ActivateReplaceModelWindow();

            // Set the right edited model index
            SetEditedModelIndex(buttonName);

            // Change the heading
            wishToReplaceHeading.text = "You already imported a model here. Do you wish to delete or replace the model " + modelName + "?";
        }
        // Set the current model name as the one being edited
        Menus.editedModelName = modelName;
        Menus.editedModelIndex = GetButtonIndexFromButtonName(buttonName);
    }

    // Method that is triggered when clicking on the import button of the import 3D model window
    public void ImportModel()
    {
        // Disable the error messages
        noUrlTypedInErrorMessage.gameObject.SetActive(false);
        urlObjectOfWrongTypeErrorMessage.gameObject.SetActive(false);

        // Get the url form the input field
        string url = enterUrl.text;

        Menus.lastUrl = url;

        // Get the string that is displayed in the input field and deffine the .obj ending as a string
        // string url = enterUrl.text;
        string ending = ".obj";

        // Check if the impult field is non empty
        if(url == "")
        {
            Debug.Log("Url is empty");
            // If it is empty, then display the "no url typed in" error message
            noUrlTypedInErrorMessage.gameObject.SetActive(true);

        } else {

            //If it is non empty, check if the url goes to a .obj object
            if(url.EndsWith(ending) != true)
            {
                Debug.Log("Url is not ending on .obj");
                // Case the ending is not .obj, display the url does not point on .obj model error message
                urlObjectOfWrongTypeErrorMessage.gameObject.SetActive(true);

            } else {

                Debug.Log("Url correct");
                // Case the url points to a .obj object, import it

                // Extract the file name
                System.Uri uri = new Uri(url);
                string fileName = System.IO.Path.GetFileName(uri.LocalPath);
                Debug.Log("File name: " + fileName);

                // Initialize the obj name array
                string[] array = ReturnPreviewModelNames();

                if(isContained(array, fileName) == true)
                // Check if a file with that name already exist in the temp save folder
                // if(File.Exists(Menus.tempSavePath + fileName))
                {
                    Debug.Log("Went in the open window loop");
                    // Open a window that tells you that a file with that name already was imported. Ask if the user wants to replace it or to cancel.
                    ActivateEnterNewModelNameWindow();
                    duplicateName.text = "There are currently more than one file with the name: " + fileName + ". If they are not the same model, enter a new name:";

                    // Deactivate the enter url window
                    DeactivateImportModelWindow();

                    // Save the current uri
                    Menus.currentUri = url;
                    
                } else {

                    Debug.Log("The current model index is: " + Menus.currentModelIndex);

                    // Save the url in the right variable
                    switch(Menus.currentModelIndex)
                    {
                        case 0:
                            Menus.url1 = url;
                        break;

                        case 1:
                            Menus.url2 = url;
                        break;

                        case 2:
                            Menus.url3 = url;
                        break;

                        case 3:
                            Menus.url4 = url;
                        break;

                        case 4:
                            Menus.url5 = url;
                        break;
                    }

                    // // Create the model json 
                    // Model modelJson = new Model();

                    // // Fill the model name and the url
                    // modelJson.modelName = fileName;
                    // modelJson.modelUrl = url;

                    // // Create the new json string
                    // string jsonModelNew = JsonUtility.ToJson(modelJson);

                    // // Get the model file name
                    // string modelFileName = "Model" + GetModelFileNameEnding(fileName) + ".json";

                    // // Save the new model file in the end save folder
                    // File.WriteAllText(Menus.tempSavePath + modelFileName, jsonModelNew);

                    // Debug.Log("The model file with name: " + modelFileName + " should have been saved in: " + Menus.tempSavePath);

                    // Save it in the temps save folder

                    if(Globals.currentlyChangingFile == false)
                    {
                        // Increase the current model index by one
                        Menus.currentModelIndex = Menus.currentModelIndex  + 1;
                    }

                    // Preview the name of the 3D model on the right button
                    PreviewModelName(fileName, Menus.editedModelIndex);

                    // Find out how many models have to be previewed
                    int numberOfModels = numberOfModelsInPreview();

                    // Activate the right button next
                    ActivateNextPreviewModelButton(numberOfModels);

                    // Reset edited variables
                    Menus.editedModelIndex = 5;
                    Menus.editedModelName = "";

                    // Close the window
                    DeactivateImportModelWindow();

                    Debug.Log("The save url 1 is: " + Menus.url1);
                    Debug.Log("The save url 2 is: " + Menus.url2);
                    Debug.Log("The save url 3 is: " + Menus.url3);
                    Debug.Log("The save url 4 is: " + Menus.url4);
                    Debug.Log("The save url 5 is: " + Menus.url5);
                }
            }
        }
    }

    // Method that replace a model with the new given through an uri
    public void ReplaceModel()
    {
        // Disable the error messages
        noUrlTypedInErrorMessageReplacement.gameObject.SetActive(false);
        urlObjectOfWrongTypeErrorMessageReplacement.gameObject.SetActive(false);

        // Get the typed in uri
        string url = enterNewModelUri.text;

        // Get the string that is displayed in the input field and define the .obj ending as a string
        string ending = ".obj";

        // Check if the impult field is non empty
        if(url == "")
        {
            Debug.Log("Url is empty");
            // If it is empty, then display the "no url typed in" error message
            noUrlTypedInErrorMessageReplacement.gameObject.SetActive(true);

        } else {
            //If it is non empty, check if the url goes to a .obj object
            if(url.EndsWith(ending) != true)
            {
                Debug.Log("Url is not ending on .obj");
                // Case the ending is not .obj, display the url does not point on .obj model error message
                urlObjectOfWrongTypeErrorMessageReplacement.gameObject.SetActive(true);

            } else {
                Debug.Log("Url correct");
                // Case the url points to a .obj object, import it

                // Extract the file name
                System.Uri uri = new Uri(url);
                string fileName = System.IO.Path.GetFileName(uri.LocalPath);
                Debug.Log("File name: " + fileName);

                // Initialize the obj name array
                string[] array = ReturnModelNames(Menus.tempSavePath);

                if(isContained(array, fileName) == true && Menus.editedModelName != fileName)
                // Check if a file with that name already exist in the temp save folder
                //if(File.Exists(Menus.tempSavePath + fileName) && Menus.editedModelName != fileName)
                {
                    Debug.Log("Went in the open window loop");
                    // Open a window that tells you that a file with that name already was imported. Ask if the user wants to replace it or to cancel.
                    ActivateEnterNewModelNameWindow();
                    duplicateName.text = "There are currently more than one file with the name: " + fileName + ". If they are not the same model, enter a new name:";

                    // Deactivate the enter url window
                    DeactivateReplaceModelWindow();

                    // Save the current uri
                    Menus.currentUri = url;
                    
                } else {
                    // Name does not already exist, download the file and save it in the temp save folder
                    using (var client = new WebClient())
                    {
                        // client.DownloadFile(url, Menus.tempSavePath + fileName);
                    }

                    // Reduce the number the old model was used by one if the edit model mode is on
                    if(Globals.currentlyChangingFile == true) 
                    {
                        // Find the right model file
                        string modelFilePath = GetFileFromModelName(Menus.tempSavePath, Menus.editedModelName);

                        // If the file exist
                        if(modelFilePath != ""){
                            // Get the json string
                            string jsonModelFile = File.ReadAllText(modelFilePath);

                            // Extract the object
                            Model modelObject = JsonUtility.FromJson<Model>(jsonModelFile);

                            // Decrease the number of questions it is used in
                            modelObject.numberOfQuestionsUsedIn = modelObject.numberOfQuestionsUsedIn - 1;

                            // Delete the old file
                            File.Delete(modelFilePath);

                            // Get new json string
                            string jsonModelFileNew = JsonUtility.ToJson(modelObject);

                            // Save it again
                            File.WriteAllText(modelFilePath, jsonModelFileNew);
                        } else {

                            // Case the json file does not exist, means it is a new model
                            File.Delete(Menus.tempSavePath + Menus.editedModelName);
                        }
                    }

                    // Preview the name of the 3D model on the right button
                    PreviewModelName(fileName, Menus.editedModelIndex);

                    // Close the window
                    DeactivateReplaceModelWindow();

                    // Set the edited model index on a number that cannot be reached to reset it
                    Menus.editedModelIndex = 5;
                    Menus.editedModelName = "";
                }
            }
        }
    }

    // Method that returns the ModelXYZ file from the model name
    public string GetFileFromModelName(string path, string name)
    {
        // Get the model file array
        string[] modelArray = GetModelsArray(path);

        string filePath = "";

        foreach(string model in modelArray)
        {
            // Get the json string
            string json = File.ReadAllText(model);

            // Extract the object
            Model modelObject = JsonUtility.FromJson<Model>(json);

            // Check if it is the file that contains the name of the model
            if(modelObject.modelName == name)
            {
                filePath = model;
            }
        }
        return filePath;
    }

    // Method that deletes the currently edited model
    public void DeleteModel()
    {
        // Delete the model
        File.Delete(Menus.tempSavePath + Menus.editedModelName);

        // Empty the text of the button
        Button rightButton = GetRightModelButton(Menus.editedModelIndex);
        rightButton.GetComponentInChildren<TMP_Text>().text = "";

        // Reduce the current model index by one
        Menus.currentModelIndex = Menus.currentModelIndex - 1;

        // Change the preview buttons
        ActualizePreviewModelButtons();

        // Find out how many models have to be previewed
        int numberOfModels = numberOfModelsInPreview();

        // Activate the right button next
        ActivateNextPreviewModelButton(numberOfModels);

        // Deactivate the window
        DeactivateReplaceModelWindow();

        // Reduce the number the old model was used by one if the edit model mode is on
        if(Globals.currentlyChangingFile == true)
        {
            // Find the right model file
            string modelFilePath2 = GetFileFromModelName(Menus.tempSavePath, Menus.editedModelName);

            if(modelFilePath2 != "")
            {
                // Get the json string
                string jsonModelFile = File.ReadAllText(modelFilePath2);

                // Extract the object
                Model modelObject = JsonUtility.FromJson<Model>(jsonModelFile);

                // Decrease the number of questions it is used in
                modelObject.numberOfQuestionsUsedIn = modelObject.numberOfQuestionsUsedIn - 1;

                // Delete the old file
                File.Delete(modelFilePath2);

                // Get new json string
                string jsonModelFileNew = JsonUtility.ToJson(modelObject);

                // Save it again
                File.WriteAllText(modelFilePath2, jsonModelFileNew);
            }
        }

        // Reset the edited model index
        Menus.editedModelIndex = 5;

        // Reset the edited model name
        Menus.editedModelName = "";
    }

    // Method that activates the next model preview button
    public void ActivateNextPreviewModelButton(int index)
    {
        switch(index)
        {
            case 0:
                previewModel1.interactable = true;
                previewModel1.GetComponentInChildren<TMP_Text>().text = "+";
            break;
            case 1:
                previewModel1.interactable = true;
                previewModel2.interactable = true;
                previewModel2.GetComponentInChildren<TMP_Text>().text = "+";
            break;
            case 2:
                previewModel1.interactable = true;
                previewModel2.interactable = true;
                previewModel3.interactable = true;
                previewModel3.GetComponentInChildren<TMP_Text>().text = "+";
            break;
            case 3:
                previewModel1.interactable = true;
                previewModel2.interactable = true;
                previewModel3.interactable = true;
                previewModel4.interactable = true;
                previewModel4.GetComponentInChildren<TMP_Text>().text = "+";
            break;
            case 4:
                previewModel1.interactable = true;
                previewModel2.interactable = true;
                previewModel3.interactable = true;
                previewModel4.interactable = true;
                previewModel5.interactable = true;
                previewModel5.GetComponentInChildren<TMP_Text>().text = "+";
            break;
        }
    }

    // Method that displays the current model name in the preview
    public void PreviewModelName(string name, int index)
    {
        // Rename the name on the right button
        switch(index)
        {
            case 0:
                previewModel1.GetComponentInChildren<TMP_Text>().text = name;
            break;
            case 1:
                previewModel2.GetComponentInChildren<TMP_Text>().text = name;
            break;
            case 2:
                previewModel3.GetComponentInChildren<TMP_Text>().text = name;
            break;
            case 3:
                previewModel4.GetComponentInChildren<TMP_Text>().text = name;
            break;
            case 4:
                previewModel5.GetComponentInChildren<TMP_Text>().text = name;
            break;
        }
    }

    // Method that displays the current model name in the preview
    public string GetModelFileNameEnding(string name)
    {
        string ending = "";

        switch(Menus.currentModelIndex)
        {
            case 0:
                return "000";
            break;

            case 1:
                return "001";
            break;

            case 2:
                return "002";
            break;

            case 3:
                return "003";
            break;

            case 4:
                return "004";
            break;
        }

        return "005";
    }

    // Method that resets the model preview buttons correctly after a button deletion
    public void ActualizePreviewModelButtons()
    {
        // Get the array of models
        string[] models = ReturnPreviewModelNames();


        // Reset the buttons
        ResetPreviewModelButtons();

        // Create the current model index
        int modelIndex = 0;

        // Set the right names on the buttons
        foreach(string model in models)
        {
            if(model != "")
            {
                // Preview the model name
                PreviewModelName(model, modelIndex);

                // Increase the current model index by 1
                modelIndex = modelIndex + 1;
            }
        }
    }

    // Method that sets the right edited model index given a button name
    public void SetEditedModelIndex(string buttonName)
    {
        // Set the edited model index correctly
         switch(buttonName)
        {
            case "Add3DModel1":
                Menus.editedModelIndex = 0;
            break;
            case "Add3DModel2":
                Menus.editedModelIndex = 1;
            break;
            case "Add3DModel3":
                Menus.editedModelIndex = 2;
            break;
            case "Add3DModel4":
                Menus.editedModelIndex = 3;
            break;
            case "Add3DModel5":
                Menus.editedModelIndex = 4;
            break;
        }
    }

    // Method that resets the state of the preview model buttons
    public void ResetPreviewModelButtons()
    {
        // Reset the text
        previewModel1.GetComponentInChildren<TMP_Text>().text = "+";
        previewModel2.GetComponentInChildren<TMP_Text>().text = "";
        previewModel3.GetComponentInChildren<TMP_Text>().text = "";
        previewModel4.GetComponentInChildren<TMP_Text>().text = "";
        previewModel5.GetComponentInChildren<TMP_Text>().text = "";

        // Reset the interactability
        previewModel2.interactable = false;
        previewModel3.interactable = false;
        previewModel4.interactable = false;
        previewModel5.interactable = false;
    }

    // // Method that returns the array of models (.obj files) in the given path
    // static string[] GetModelsObjArray(string path) 
    // {
    //     string[] questions = Directory.GetFiles(path, "*.obj", SearchOption.TopDirectoryOnly);
    //     return questions;
    // }

    // Method that returns the array of models (json files) in the given path
    static string[] GetModelsArray(string path) 
    {
        string[] models = Directory.GetFiles(path, "Model*", SearchOption.TopDirectoryOnly);
        return models;
    }


    // // Method that changes the name of the improrted model (case two different models with same name)
    // public void RenameDuplicate()
    // {
    //     // Disable error Message
    //     noNameToRenameTypedIn.gameObject.SetActive(false);
    //     nameAlreadyExist.gameObject.SetActive(false);

    //     // Get the typed in name
    //     string newName = enterNewModelName.text;

    //     // Check that the name is non empty
    //     if(newName == "")
    //     {
    //         // Display error message
    //         noNameToRenameTypedIn.gameObject.SetActive(true);

    //     } else {

    //         // Check if the name ends with ".obj", if not add the ending
    //         string ending = ".obj";
    //         if(newName.EndsWith(ending) != true)
    //         {
    //             newName = newName + ending;
    //         }

    //         // Initialize the obj name array
    //         string[] array = ReturnModelNames(Menus.tempSavePath);

    //         // Check if a file with that name already exist in the temp save folder
    //         // if(File.Exists(Menus.tempSavePath + newName) && Menus.editedModelName != newName)
    //         if(isContained(array, newName) == true && Menus.editedModelName != newName)
    //         {
    //             // Display error message
    //             nameAlreadyExist.gameObject.SetActive(true);

    //         } else {
    //             // Get the right button to rename index
    //             int index = 0;
    //             if(Menus.editedModelIndex != 5 && Globals.currentlyChangingFile == false)
    //             {
    //                 // Case old model need to be replaced by a new one. Set the index on the edited model index
    //                 index = Menus.editedModelIndex;
    
    //                 // Delete the old model
    //                 File.Delete(Menus.tempSavePath + Menus.editedModelName);

    //             } else if(Menus.editedModelIndex == 5 && Globals.currentlyChangingFile == false) {
                
    //                 // Case a newly added model needs to be renamed. Set the index on the current model index
    //                 index = Menus.currentModelIndex;
    //             } else {

    //                 // Case we are currently changing a question
    //                 // Check if the name does not already exist in a model file
    //                 index = GetButtonIndexFromButtonName(Menus.modelPreviewButtonName);

    //                 int numberOfModels2 = 0;
    //                 foreach(string model in array)
    //                 {
    //                     if(model != "")
    //                     {
    //                         numberOfModels2 = numberOfModels2 + 1;
    //                     }
    //                 }

    //                 // Set correctly the current model index
    //                 Menus.currentModelIndex = numberOfModels2;

    //                 // Find the right model file
    //                 string modelFilePath = GetFileFromModelName(Menus.tempSavePath, Menus.editedModelName);

    //                 // If the file exist, reduce the number of uses
    //                 if(modelFilePath != "")
    //                 {
    //                     // Get the json string
    //                     string jsonModelFile = File.ReadAllText(modelFilePath);

    //                     // Extract the object
    //                     Model modelObject = JsonUtility.FromJson<Model>(jsonModelFile);

    //                     // Decrease the number of questions it is used in
    //                     modelObject.numberOfQuestionsUsedIn = modelObject.numberOfQuestionsUsedIn - 1;

    //                     // Delete the old file
    //                     File.Delete(modelFilePath);

    //                     // Get new json string
    //                     string jsonModelFileNew = JsonUtility.ToJson(modelObject);

    //                     // Save it again
    //                     File.WriteAllText(modelFilePath, jsonModelFileNew);

    //                 } else {

    //                     if(Menus.editedModelName != "")
    //                     {
    //                         // Case the json file does not exist, means it is a new model
    //                         File.Delete(Menus.tempSavePath + Menus.editedModelName);
    //                     }
    //                 }
    //             }

    //             // Search for a modelxyz file that contains that model name
    //             string modelFilePathDeletion = GetFileFromModelName(Menus.tempSavePath, newName);

    //             if(modelFilePathDeletion != "")
    //             {
    //                 // Case there is a file that has that model name in it
    //                 // Get the json string
    //                 string jsonModelFile = File.ReadAllText(modelFilePathDeletion);

    //                 // Extract the object
    //                 Model modelObject = JsonUtility.FromJson<Model>(jsonModelFile);

    //                 // Increase the number of questions it is used in
    //                 modelObject.numberOfQuestionsUsedIn = modelObject.numberOfQuestionsUsedIn + 1;

    //                 // Delete the old file
    //                 File.Delete(modelFilePathDeletion);

    //                 // Get new json string
    //                 string jsonModelFileNew = JsonUtility.ToJson(modelObject);

    //                 // Save it again
    //                 File.WriteAllText(modelFilePathDeletion, jsonModelFileNew);

    //             } else {
    //                 // Case there is no ModelXYZ file that contains that model name in the temp save folder. Download the model object.
    //                 using (var client = new WebClient())
    //                 {

    //                     // Save the url in the right variable
    //                     switch(Menus.editedModelIndex)
    //                     {
    //                         case 0:
    //                             Menus.url1 = url;
    //                         break;

    //                         case 1:
    //                             Menus.url2 = url;
    //                         break;

    //                         case 2:
    //                             Menus.url3 = url;
    //                         break;

    //                         case 3:
    //                             Menus.url4 = url;
    //                         break;

    //                         case 4:
    //                             Menus.url5 = url;
    //                         break;
    //                     }

    //                     // // Get the url form the input field
    //                     // string url = Menus.lastUrl;

    //                     // // Create a new model file
    //                     // // Create the model json 
    //                     // Model modelJson = new Model();

    //                     // // Fill the model name and the url
    //                     // modelJson.modelName = newName;
    //                     // modelJson.modelUrl = url;

    //                     // // Create the new json string
    //                     // string jsonModelNew = JsonUtility.ToJson(modelJson);

    //                     // // Get the model file name
    //                     // string modelFileName = "Model" + GetModelFileNameEnding(newName) + ".json";

    //                     // // Save the new model file in the end save folder
    //                     // File.WriteAllText(Menus.tempSavePath + modelFileName, jsonModelNew);

    //                     // Debug.Log("The model file with name: " + modelFileName + " should have been saved in: " + Menus.tempSavePath);
    //                 }
    //             }


    //             // Preview the name of the 3D model on the right button
    //             PreviewModelName(newName, Menus.editedModelIndex);

    //             // Close the window
    //             DeactivateEnterNewModelNameWindow();
    
    //             if(Menus.editedModelIndex == 5)
    //             {
    //                 // Increase the current model index by one
    //                 Menus.currentModelIndex = Menus.currentModelIndex  + 1;
    //             } else {
    //                 Menus.editedModelIndex = 5;
    //                 Menus.editedModelName = "";
    //             }

    //             // Find out how many models have to be previewed
    //             int numberOfModels = numberOfModelsInPreview();

    //             // Activate the right button next
    //             ActivateNextPreviewModelButton(numberOfModels);
    //             Debug.Log(Menus.currentModelIndex);
    //         }
    //     }
    // }

    // Method that changes the name of the improrted model (case two different models with same name)
    public void RenameDuplicate()
    {
        // Disable error Message
        noNameToRenameTypedIn.gameObject.SetActive(false);
        nameAlreadyExist.gameObject.SetActive(false);

        // Get the typed in name
        string newName = enterNewModelName.text;

        // Check that the name is non empty
        if(newName == "")
        {
            // Display error message
            noNameToRenameTypedIn.gameObject.SetActive(true);

        } else {

            // Check if the name ends with ".obj", if not add the ending
            string ending = ".obj";
            if(newName.EndsWith(ending) != true)
            {
                newName = newName + ending;
            }

            // Initialize the obj name array
            string[] array = ReturnPreviewModelNames();

            // Check if a file with that name already exist in the temp save folder
            // if(File.Exists(Menus.tempSavePath + newName) && Menus.editedModelName != newName)
            if(isContained(array, newName) == true && Menus.editedModelName != newName)
            {
                // Display error message
                nameAlreadyExist.gameObject.SetActive(true);

            } else {
                // Get the right button to rename index
                int index = 0;
                if(Menus.editedModelIndex != 5 && Globals.currentlyChangingFile == false)
                {
                    // Case old model need to be replaced by a new one. Set the index on the edited model index
                    index = Menus.editedModelIndex;
    
                    // // Delete the old model
                    // File.Delete(Menus.tempSavePath + Menus.editedModelName);

                } else if(Menus.editedModelIndex == 5 && Globals.currentlyChangingFile == false) {
                
                    // Case a newly added model needs to be renamed. Set the index on the current model index
                    index = Menus.currentModelIndex;

                } else {

                    // Case we are currently changing a question
                    // Check if the name does not already exist in a model file
                    index = GetButtonIndexFromButtonName(Menus.modelPreviewButtonName);

                    int numberOfModels2 = 0;
                    foreach(string model in array)
                    {
                        if(model != "")
                        {
                            numberOfModels2 = numberOfModels2 + 1;
                        }
                    }

                    // Set correctly the current model index
                    Menus.currentModelIndex = numberOfModels2;

                    // Find the right model file
                    string modelFilePath = GetFileFromModelName(Menus.tempSavePath, Menus.editedModelName);

                    // If the file exist, reduce the number of uses
                    if(modelFilePath != "")
                    {
                        // Get the json string
                        string jsonModelFile = File.ReadAllText(modelFilePath);

                        // Extract the object
                        Model modelObject = JsonUtility.FromJson<Model>(jsonModelFile);

                        // Decrease the number of questions it is used in
                        modelObject.numberOfQuestionsUsedIn = modelObject.numberOfQuestionsUsedIn - 1;

                        // Delete the old file
                        File.Delete(modelFilePath);

                        // Get new json string
                        string jsonModelFileNew = JsonUtility.ToJson(modelObject);

                        // Save it again
                        File.WriteAllText(modelFilePath, jsonModelFileNew);

                    }
                }

                // Search for a modelxyz file that contains that model name
                string modelFilePathDeletion = GetFileFromModelName(Menus.tempSavePath, newName);

                if(modelFilePathDeletion != "")
                {
                    // Case there is a file that has that model name in it
                    // Get the json string
                    string jsonModelFile = File.ReadAllText(modelFilePathDeletion);

                    // Extract the object
                    Model modelObject = JsonUtility.FromJson<Model>(jsonModelFile);

                    // Increase the number of questions it is used in
                    modelObject.numberOfQuestionsUsedIn = modelObject.numberOfQuestionsUsedIn + 1;

                    // Delete the old file
                    File.Delete(modelFilePathDeletion);

                    // Get new json string
                    string jsonModelFileNew = JsonUtility.ToJson(modelObject);

                    // Save it again
                    File.WriteAllText(modelFilePathDeletion, jsonModelFileNew);

                } else {

                    // Case there is no ModelXYZ file that contains that model name in the temp save folder. Set the url to the right field
                    using (var client = new WebClient())
                    {

                        // Save the url in the right variable
                        switch(Menus.editedModelIndex)
                        {
                            case 0:
                                Menus.url1 = Menus.lastUrl;
                            break;

                            case 1:
                                Menus.url2 = Menus.lastUrl;
                            break;

                            case 2:
                                Menus.url3 = Menus.lastUrl;
                            break;

                            case 3:
                                Menus.url4 = Menus.lastUrl;
                            break;

                            case 4:
                                Menus.url5 = Menus.lastUrl;
                            break;
                        }
                    }
                }


                // Preview the name of the 3D model on the right button
                PreviewModelName(newName, Menus.editedModelIndex);

                // Close the window
                DeactivateEnterNewModelNameWindow();
    
                if(Menus.editedModelIndex == 5)
                {
                    // Increase the current model index by one
                    Menus.currentModelIndex = Menus.currentModelIndex  + 1;
                } else {
                    Menus.editedModelIndex = 5;
                    Menus.editedModelName = "";
                }

                // Find out how many models have to be previewed
                int numberOfModels = numberOfModelsInPreview();

                // Activate the right button next
                ActivateNextPreviewModelButton(numberOfModels);
                Debug.Log(Menus.currentModelIndex);

                Debug.Log("The save url 1 is: " + Menus.url1);
                Debug.Log("The save url 2 is: " + Menus.url2);
                Debug.Log("The save url 3 is: " + Menus.url3);
                Debug.Log("The save url 4 is: " + Menus.url4);
                Debug.Log("The save url 5 is: " + Menus.url5);
            }
        }
    }

    // Method that checks if a string is contained in an array of strings
    public bool isContained(string[] array, string name)
    {
        foreach (string modelName in array)
        {
            if(modelName == name)
            {
                return true;
            }
        }
        return false;
    }

    // // Method used to find the model.json file in which the model with modelName is saved
    // public string FindJsonModelFileName(string modelName)
    // {
    //     // Get the .json array
    //     string[] jsonArray = GetModelsArray(Menus.tempSavePath);

    //     // Go through all elements in the json array
    //     foreach(string jsonFile in jsonArray)
    //     {
    //         // Extract the json string of the description
    //         string json = File.ReadAllText(jsonFile);
    //         Model model = JsonUtility.FromJson<Model>(json);

    //         // Check if the model name in the json file is the same as the given name
    //         if(model.modelName == modelName)
    //         {
    //             // Return the name of the json file
    //             return Path.GetFileName(jsonFile);
    //         }
    //     }

    //     // Return an empty string if no match (impossible)
    //     return "";
    // }

    // Method that checks on what preview button the model name is written
    public Button ReturnPreviewModelButtonFromModelName(string name)
    {
        if(previewModel1.GetComponentInChildren<TMP_Text>().text == name)
        {
            return previewModel1;
        }

        if(previewModel2.GetComponentInChildren<TMP_Text>().text == name)
        {
            return previewModel2;
        }

        if(previewModel3.GetComponentInChildren<TMP_Text>().text == name)
        {
            return previewModel3;
        }

        if(previewModel4.GetComponentInChildren<TMP_Text>().text == name)
        {
            return previewModel4;
        }

        if(previewModel5.GetComponentInChildren<TMP_Text>().text == name)
        {
            return previewModel5;
        }
    }

    // Method that checks if a name is contained in the model preview butons
    public string[] ReturnModelNames(string path)
    {
        // Get the .json arryy
        string[] jsonArray = GetModelsArray(path);

        int lenght = jsonArray.Length;

        // Initialize the array
        string[] array = new string[lenght];

        int index = 0;

        foreach(string jsonFile in jsonArray)
        {
            // Extract the json string of the description
            string json = File.ReadAllText(jsonFile);
            Model model = JsonUtility.FromJson<Model>(json);

            // Add the model name to the array
            array[index] = model.modelName;
            index = index + 1;
        }

        return array;
    }

    // Method that returns the array of model names of the preview models buttons
    public string[] ReturnPreviewModelNames()
    {
        // Initialize the array
        string[] array = new string[5];

        // Fill the name of the first model
        if(previewModel1.GetComponentInChildren<TMP_Text>().text != "+")
        {
            array[0] = previewModel1.GetComponentInChildren<TMP_Text>().text;
        } else {
            array[0] = "";
        }

        // Fill the name of the second model
        if(previewModel2.GetComponentInChildren<TMP_Text>().text != "+")
        {
            array[1] = previewModel2.GetComponentInChildren<TMP_Text>().text;
        } else {
            array[1] = "";
        }

        // Fill the name of the third model
        if(previewModel3.GetComponentInChildren<TMP_Text>().text != "+")
        {
            array[2] = previewModel3.GetComponentInChildren<TMP_Text>().text;
        } else {
            array[2] = "";
        }

        // Fill the name of the fourth model
        if(previewModel4.GetComponentInChildren<TMP_Text>().text != "+")
        {
            array[3] = previewModel4.GetComponentInChildren<TMP_Text>().text;
        } else {
            array[3] = "";
        }

        // Fill the name of the fifth model
        if(previewModel5.GetComponentInChildren<TMP_Text>().text != "+")
        {
            array[4] = previewModel5.GetComponentInChildren<TMP_Text>().text;
        } else {
            array[4] = "";
        }

        return array;
    }

    // Method that finds the number of models in the preview
    public int numberOfModelFiles(string path)
    {
        // Get the array
        string[] array = GetModelsArray(path);

        int number = 0;

        foreach(string file in array)
        {
            number = number + 1;
        }
        return number;
    }

    // Method that finds the number of models in the preview
    public int numberOfQuestionFiles(string path)
    {
        // Get the array
        string[] array = GetQuestionsArray(path);

        int number = 0;

        foreach(string file in array)
        {
            number = number + 1;
        }
        return number;
    }

    // Method that finds the number of models in the preview
    public int numberOfModelsInPreview()
    {
        // Get the array of names on the buttons
        string[] array = ReturnPreviewModelNames();

        // Initialize the counter
        int number = 0;
        foreach(string model in array) 
        {
            // If the string is not empty, count one more
            if(model != "")
            {
                number = number + 1;
            }
        }
        return number;
    }
}