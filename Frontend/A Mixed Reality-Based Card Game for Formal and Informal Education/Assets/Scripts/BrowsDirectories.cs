using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

static class Globals
{
    // The path of the directory that is currently shown
    public static string currentPath;
    public static string currentPathShorten;

    // The path of the directory that is the root of the directory structure
    public static string rootDirectoryPath;

    // Current page and number of pages
    public static int currentPage;
    public static int numberOfPages;

    // Number of directories in the current directory
    public static int numberOfDirectories;

    // Array of all directories
    public static string[] directoriesArray;

    // Number of files in the current directory
    public static int numberOfFiles;

    // Array of all files
    public static string[] filesArray;

    // Current depth
    public static int depth;

    // Flags used for reset or freezing of the menu (to not go two directories deep in one click)
    public static bool flagVariable = true;

    // Path of the selected directory. Used for the creator menu to set the path where to save the questions and 3D models
    public static string selectedPath;
    public static string selectedPathShorten;
    public static string lastMenu;

    // Flag that tells if you are currently in a directory with directories or files. Clicking on a directory should make you go on directory deeper, while clicking on
    // a question should open the edit mode
    public static bool theseAreFiles;
    public static string filePath;
    public static string fileName;
    public static string tempSavePath;
    public static string[] fileArray;
    public static bool currentlyChangingFile;
}

public class BrowsDirectories : MonoBehaviour
{
    // Here menus and buttons are defined
    public GameObject mainMenu;
    public GameObject creatorMenu;
    public GameObject browsDirectoriesMenu;
    public Button selectButton;

    // Define the page x / y text of the brows directories menu
    public TextMeshProUGUI currentPageText;

    // Define the input field, the error text and window so that they can get disabled / enabled when needed
    public TMP_InputField mainInputField; // The input field to create directories
    public TextMeshProUGUI errorText;
    public GameObject window;
    
    // Define the windows
    public GameObject proceedSelectionWindow;
    public GameObject AskDescriptionWindow;
    public GameObject EnterDescriptionWindow;
    public GameObject veilSmallWindow;
    public GameObject veilLargeWindow;

    // Define an error message that needs to be disabled in the creator menu when a path was selected
    public TextMeshProUGUI noPathSelected;

    // Define the input field of the creator menu, so that the current path can be loaded in it when editing an existing question
    // Define the buttons of the main creator that need to be disabled
    public TMP_InputField savePathField;
    public Button previewQuestion1;
    public Button addButton;
    public Button browsDirectoriesButton;
    public Button saveButton;
    public Button changeButton;
    public Button createDirectory;

    // Define the model preview buttons
    public Button previewModel1;
    public Button previewModel2;
    public Button previewModel3;
    public Button previewModel4;
    public Button previewModel5;

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

    // The log, which will become the Description.json file
    [Serializable]
    public class Log
    {
        public int numberOfQuestions; // The number of already existing questions in the folder so that the new ones can be renamed
        public int numberOfModels; // The number of already existing model files in the folder so that the new ones can be renamed
        public string heading; // Heading of the description, name that users can give
        public string description; // The description text of the content / concepts that are needed for solving the exercises
    }

    // Defining the important input fields for the log, as well as the buttons that need to be activated / deactivated.
    public TMP_InputField descriptionHeading;
    public TMP_InputField descriptionText;
    public Button changeDescription;
    public Button okDescription;
    public Button cancelDescription;
    public Button backDescription;

    // The JSON Serialization for the Models
    [Serializable]
    public class Model
    {
        public string modelName;
        public string modelUrl;
        public int numberOfQuestionsUsedIn;

    }

    // Start is called before the first frame update
    void Start()
    {
        // First I initialize the Global paths
        string scriptPath = GetCurrentFilePath();
        string rootPath = GetPathToRootDirectory(scriptPath);
        Globals.rootDirectoryPath = rootPath;
        Globals.currentPath = Globals.rootDirectoryPath;
        Globals.currentPathShorten = "";
        Globals.depth = 1;
        Globals.theseAreFiles = false;

        // Then I actualize in a function the directories, page numbers, heading
        ActualizeGlobals();

        // Then I disable / enable the previous and next button based on the number of pages
        DisableOrEnableButtons();

        // Then I rename / delete the name of the predefined buttons and disable those that have no name
        RenameButtons(Globals.currentPath);

        // Add a listener to the input field
        mainInputField.onEndEdit.AddListener(delegate{AddDirectory(mainInputField);});

        // Initialize the temp save path. This will have to be saved later on
        Globals.tempSavePath = GetPathToTempSave(scriptPath);

        // Initialize the flag of changing file
        Globals.currentlyChangingFile = false;
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

    // Method that returns you the path to the root directory of the directory structure saved in the back end
    private string GetPathToRootDirectory(string scriptPath)
    {
        string rootPath = Path.GetFullPath(Path.Combine(scriptPath, @"..\..\..\..\..\"));
        string rootDirectoryPath = Path.GetFullPath(Path.Combine(rootPath, @"Backend\Directories\"));
        return rootDirectoryPath;
    }

    // Method that returns you the path to the temp save folder in the back end
    private string GetPathToTempSave(string scriptPath)
    {
        string rootPath = Path.GetFullPath(Path.Combine(scriptPath, @"..\..\..\..\..\"));
        string rootDirectoryPath = Path.GetFullPath(Path.Combine(rootPath, @"Backend\TempSave\"));
        return rootDirectoryPath;
    }

    // Method that returns the array of directories in the current directory
    static string[] GetDirectoriesArray() 
    {
        string[] dirs = Directory.GetDirectories(Globals.currentPath, "*", SearchOption.TopDirectoryOnly);
        return dirs;
    }

    // Method that returns the array of files in the given path to a directory
    static string[] GetFilesArray()
    {
        string[] files = Directory.GetFiles(Globals.currentPath, "Question*");

        // Check if the description file exists
        if (File.Exists(Globals.currentPath + "Description.json")) 
        {

            // Case it exists
            string[] description = Directory.GetFiles(Globals.currentPath, "Description.json");

            // Get the length of the files array
            int length = 0;
            foreach(string file in files)
            {
                length = length + 1;
            }
            length = length + 1;

            // Create a new array that can contain all files
            string[] array = new string[length];

            // Copy the description in the first slot
            array[0] = description[0];
            int index = 1;

            // Append all elements in the files array to the description array
            foreach(string file in files)
            {
                array[index] = file;
                index = index + 1;
            }

            // Return the array that contains the description and the questions
            return array;

        } else {

            // Case the description file does not exist
            return files;
        }
    }

    // Method that returns the number of directories in the current directory
    static int GetNumberOfDirectories(string[] dirs) 
    {
       int number = 0;
       foreach (string dir in dirs) {
           number = number + 1;
       }
       return number;
    }

    // Method that returns the number of directories in the current directory
    static int GetNumberOfFiles(string[] files) 
    {
       int number = 0;
       foreach (string file in files) {
           number = number + 1;
       }
       return number;
    }

    // Define the two previous sprites as well as the image of the button
    [SerializeField]
    private Sprite[] switchSprites;
    private Image switchImage;

    // Define the upload button
    [SerializeField]
    private Button uploadButton;

    // Disabling or enabling of the buttons
    public void DisableOrEnableButtons()
    {
        // Define the disabled color gradient
        VertexGradient disabledTextGradient;
        disabledTextGradient.bottomLeft = new Color32(99, 101, 102, 150);
        disabledTextGradient.bottomRight = new Color32(99, 101, 102, 150);
        disabledTextGradient.topLeft = new Color32(99, 101, 102, 255);
        disabledTextGradient.topRight = new Color32 (99, 101, 102, 255);

        // Define the enabled color gradient
        VertexGradient enabledTextGradient;
        enabledTextGradient.bottomLeft = new Color32(0, 84, 159, 255);
        enabledTextGradient.bottomRight = new Color32(0, 84, 159, 255);
        enabledTextGradient.topLeft = new Color32(64, 127, 183, 255);
        enabledTextGradient.topRight = new Color32 (64, 127, 183, 255);

        // Enable / Disable previous button and change color
        Button previous = GameObject.Find("PreviousBrowsDirectories").GetComponent<Button>();
        TMP_Text textPrevious = previous.GetComponentInChildren<TMP_Text>();
        if(Globals.currentPage == 1)
        {
            previous.interactable = false;
            textPrevious.GetComponent<TMP_Text>().colorGradient = disabledTextGradient;
        } else {
            previous.interactable = true;
            textPrevious.GetComponent<TMP_Text>().colorGradient = enabledTextGradient;
        }

        // Enable / Disable next button and change color
        Button next = GameObject.Find("Next").GetComponent<Button>();
        TMP_Text textNext = next.GetComponentInChildren<TMP_Text>();
        if(Globals.currentPage != Globals.numberOfPages)
        {
            next.interactable = true;
            textNext.GetComponent<TMP_Text>().colorGradient = enabledTextGradient;
        } else {
            next.interactable = false;
            textNext.GetComponent<TMP_Text>().colorGradient = disabledTextGradient;
        }

        // Enable / disable the return button
        Button returnButton = GameObject.Find("Return").GetComponent<Button>();
        switchImage = returnButton.image;
        if(Globals.currentPath != Globals.rootDirectoryPath)
        {
            //returnButtonOn.interactable = true;
            returnButton.interactable = true;
            switchImage.sprite = switchSprites[1];
        } else {
            //returnButtonOff.interactable = false;
            returnButton.interactable = false;
            switchImage.sprite = switchSprites[0];
        }
    }

    // Method that extracts the name of the question from the file
    public string ExtractQuestionName(string path)
    {
        // Initialize the question name
        string questionName = "";

        // Check if this is the description file or a question
        if(Path.GetFileName(path) != "Description.json")
        {
            // Case this is not the description file but a question file
            // Get the string form the file
            string json = File.ReadAllText(path);

            // Check what type of question it is
            if(json.Contains("input question") == true)
            {
                // Case input question
                InputQuestion question = JsonUtility.FromJson<InputQuestion>(json);
                questionName = question.name;
            } else{
                // Case multiple choice question
                MultipleChoiceQuestion question = JsonUtility.FromJson<MultipleChoiceQuestion>(json);
                questionName = question.name;
            }
        } else {
            // Case this is the description file
            questionName = "Description";
        }

        // Return the question name
        return questionName;
    }

    // Method that creates the buttons depending of the directory we are currently in
    public void RenameButtons(string path)
    {

        // Case there are no directories to be displayed
        if(Globals.numberOfDirectories == 0)
        {
            // If select directory mode is on, then enable the select button
            if(Menus.directorySelection == true)
            {
                selectButton.interactable = true;
            }

            // Get the array of all files
            Globals.fileArray = GetFilesArray();
            int numberOfFiles = GetNumberOfFiles(Globals.fileArray);

            // Case there are files in the folder
            if(numberOfFiles != 0)
            {
                // Disable the create directory button
                createDirectory.interactable = false;

                // Set the flag
                Globals.theseAreFiles = true;
                // First rename the buttons that should have button names, check that they are enabled
                // for that initialize the range of the for loop

                // Check if a file is currently being edited
                if(Globals.currentlyChangingFile == false)
                {
                    // Enable the upload button
                    uploadButton.gameObject.SetActive(true);

                } else {

                    // Disable the upload button
                    uploadButton.gameObject.SetActive(false);
                }

                // Value at the begining of the for loop
                int initialIndex = (Globals.currentPage - 1) * 5;
                // counter for the assigning of a button
                int currentFileNumber = 1;
                // Value for the end of the for loop (for the renaming loop)
                int lastIndex = 0;

                if(numberOfFiles <= (Globals.currentPage) * 5)
                {
                    lastIndex = numberOfFiles - 1;
                } else {
                    lastIndex = Globals.currentPage * 5 - 1;
                }
                // Last index that would correspond to the fifth directory if the array was full enough (for the deleting names loop)
                int lastEmptyIndex = (Globals.currentPage) * 5 - 1;

                for(int currentIndex = initialIndex; currentIndex <= lastIndex; currentIndex = currentIndex + 1)
                {
                    string file = Globals.fileArray[currentIndex];
                    string lastFileName = Path.GetFileName(file);

                    // Get the question name form the file
                    string questionName = ExtractQuestionName(file);

                    switch (currentFileNumber)
                    {
                        case 1:
                            Button directory1 = GameObject.Find("Directory1").GetComponent<Button>();
                            directory1.GetComponentInChildren<TMP_Text>().text = questionName;
                            directory1.interactable = true;
                        break;
                        case 2:
                            Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                            directory2.GetComponentInChildren<TMP_Text>().text = questionName;
                            directory2.interactable = true;
                        break;
                        case 3:
                            Button directory3 = GameObject.Find("Directory3").GetComponent<Button>();
                            directory3.GetComponentInChildren<TMP_Text>().text = questionName;
                            directory3.interactable = true;
                        break;
                        case 4:
                            Button directory4 = GameObject.Find("Directory4").GetComponent<Button>();
                            directory4.GetComponentInChildren<TMP_Text>().text = questionName;
                            directory4.interactable = true;
                        break;
                        case 5:
                            Button directory5 = GameObject.Find("Directory5").GetComponent<Button>();
                            directory5.GetComponentInChildren<TMP_Text>().text = questionName;
                            directory5.interactable = true;
                        break;
                    }
                    currentFileNumber = currentFileNumber + 1;
                }
                if(currentFileNumber != 5)
                {
                    for(int counter = numberOfFiles; counter <= lastEmptyIndex; counter = counter + 1)
                    {
                        switch (currentFileNumber)
                        {
                            case 2:
                                Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                                directory2.GetComponentInChildren<TMP_Text>().text = "";
                                directory2.interactable = false;
                            break;
                            case 3:
                                Button directory3 = GameObject.Find("Directory3").GetComponent<Button>();
                                directory3.GetComponentInChildren<TMP_Text>().text = "";
                                directory3.interactable = false;
                            break;
                            case 4:
                                Button directory4 = GameObject.Find("Directory4").GetComponent<Button>();
                                directory4.GetComponentInChildren<TMP_Text>().text = "";
                                directory4.interactable = false;
                            break;
                            case 5:
                                Button directory5 = GameObject.Find("Directory5").GetComponent<Button>();
                                directory5.GetComponentInChildren<TMP_Text>().text = "";
                                directory5.interactable = false;
                            break;
                        }
                        currentFileNumber = currentFileNumber + 1;
                    }
                }

            // Case there are no files in the folder
            } else {

            Button directory1 = GameObject.Find("Directory1").GetComponent<Button>();
            directory1.GetComponentInChildren<TMP_Text>().text = "";
            directory1.interactable = false;
            Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
            directory2.GetComponentInChildren<TMP_Text>().text = "";
            directory2.interactable = false;
            Button directory3 = GameObject.Find("Directory3").GetComponent<Button>();
            directory3.GetComponentInChildren<TMP_Text>().text = "";
            directory3.interactable = false;
            Button directory4 = GameObject.Find("Directory4").GetComponent<Button>();
            directory4.GetComponentInChildren<TMP_Text>().text = "";
            directory4.interactable = false;
            Button directory5 = GameObject.Find("Directory5").GetComponent<Button>();
            directory5.GetComponentInChildren<TMP_Text>().text = "";
            directory5.interactable = false;
            }

        // Case there is at least one directory, then display the numbers 5*x + 1 to 5*x + 5 (x is number of the page)
        } else {

            // Disable the select directory button
            selectButton.interactable = false;

            // Enable the create directory button
            createDirectory.interactable = true;

            // Directories in here
            Globals.theseAreFiles = false;

            // First rename the buttons that should have button names, check that they are enabled
            // for that initialize the range of the for loop

            // Value at the begining of the for loop
            int initialIndex = (Globals.currentPage - 1) * 5;
            // counter for the assigning of a button
            int currentDirectoryNumber = 1;
            // Value for the end of the for loop (for the renaming loop)
            int lastIndex = 0;
            if(Globals.numberOfDirectories <= (Globals.currentPage) * 5)
            {
                lastIndex = Globals.numberOfDirectories - 1;
            } else {
                lastIndex = Globals.currentPage * 5 - 1;
            }
            // Last index that would correspond to the fifth directory if the array was full enough (for the deleting names loop)
            int lastEmptyIndex = (Globals.currentPage) * 5 - 1;

            for(int currentIndex = initialIndex; currentIndex <= lastIndex; currentIndex = currentIndex + 1)
            {
            //each (string dir in Globals.directoriesArray) {
                string dir = Globals.directoriesArray[currentIndex];
                string lastFolderName = Path.GetFileName(dir);
                switch (currentDirectoryNumber)
                {
                    case 1:
                        Button directory1 = GameObject.Find("Directory1").GetComponent<Button>();
                        directory1.GetComponentInChildren<TMP_Text>().text = lastFolderName;
                        directory1.interactable = true;
                    break;
                    case 2:
                        Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory2.GetComponentInChildren<TMP_Text>().text = lastFolderName;
                        directory2.interactable = true;
                    break;
                    case 3:
                        Button directory3 = GameObject.Find("Directory3").GetComponent<Button>();
                        directory3.GetComponentInChildren<TMP_Text>().text = lastFolderName;
                        directory3.interactable = true;
                    break;
                    case 4:
                        Button directory4 = GameObject.Find("Directory4").GetComponent<Button>();
                        directory4.GetComponentInChildren<TMP_Text>().text = lastFolderName;
                        directory4.interactable = true;
                    break;
                    case 5:
                        Button directory5 = GameObject.Find("Directory5").GetComponent<Button>();
                        directory5.GetComponentInChildren<TMP_Text>().text = lastFolderName;
                        directory5.interactable = true;
                    break;
                }
                currentDirectoryNumber = currentDirectoryNumber + 1;
            }
            if(currentDirectoryNumber != 5)
            {
                for(int counter = Globals.numberOfDirectories; counter <= lastEmptyIndex; counter = counter + 1)
                {
                    switch (currentDirectoryNumber)
                    {
                        case 2:
                            Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                            directory2.GetComponentInChildren<TMP_Text>().text = "";
                            directory2.interactable = false;
                        break;
                        case 3:
                            Button directory3 = GameObject.Find("Directory3").GetComponent<Button>();
                            directory3.GetComponentInChildren<TMP_Text>().text = "";
                            directory3.interactable = false;
                        break;
                        case 4:
                            Button directory4 = GameObject.Find("Directory4").GetComponent<Button>();
                            directory4.GetComponentInChildren<TMP_Text>().text = "";
                            directory4.interactable = false;
                        break;
                        case 5:
                            Button directory5 = GameObject.Find("Directory5").GetComponent<Button>();
                            directory5.GetComponentInChildren<TMP_Text>().text = "";
                            directory5.interactable = false;
                        break;
                    }
                    currentDirectoryNumber = currentDirectoryNumber + 1;
                }
            }
        }
    }

    // Method that is activated when pressing next (change the other directories)
    public void NextPage(){
        Globals.currentPage = Globals.currentPage + 1;
        DisableOrEnableButtons();
        RenameButtons(Globals.currentPath);
        GameObject.Find("HeadingTextBrowsDirectories").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
    }

    // Method that is activated when pressing previous (change the other directories)
    public void PreviousPage(){
        Globals.currentPage = Globals.currentPage - 1;
        DisableOrEnableButtons();
        RenameButtons(Globals.currentPath);
        GameObject.Find("HeadingTextBrowsDirectories").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
    }

    // Method that is activated when pressing the return arrow (get to the parent directory)
    public void ReturnOneUp()
    {
        // First we need to actualize the current path
        Globals.currentPath = Path.GetFullPath(Path.Combine(Globals.currentPath, @"..\"));
        Globals.currentPathShorten = Path.GetFullPath(Path.Combine(Globals.currentPathShorten, @"..\"));
        
        // Then we can actualize everything
        ActualizeGlobals();
        DisableOrEnableButtons();
        RenameButtons(Globals.currentPath);
    }

    IEnumerator FreezeCoroutine()
    {
        // Print the time of when the function is first called.
        Globals.flagVariable = false;

        // Yield on a new YieldInstruction that waits for 0.25 seconds.
        yield return new WaitForSeconds(0.250F);

        //After we have waited 5 seconds print the time again.
        Globals.flagVariable = true;
    }

    // Method used to navigate in directories, when clicking on one visible directory
    public void NavigateDirectories()
    {
        // get the name of the button that was pressed and the button
        string name = EventSystem.current.currentSelectedGameObject.name;
        Button button = GameObject.Find(name).GetComponent<Button>();
        string fileName = button.GetComponentInChildren<TMP_Text>().text;

        if(Globals.flagVariable == true && Globals.theseAreFiles == false)
        {
            // Increase the browsing depth
            Globals.depth = Globals.depth + 1;

            // Get the name of the directory selected
            string directory = button.GetComponentInChildren<TMP_Text>().text;

            // Actualize the path
            Globals.currentPath = Globals.currentPath + directory + @"\";
            Globals.currentPathShorten = Globals.currentPathShorten + directory + @"\";

            // Actualize the other globals (directories array, number, page number, etc)
            ActualizeGlobals();
            DisableOrEnableButtons();
            RenameButtons(Globals.currentPath);
            StartCoroutine(FreezeCoroutine());
        }

        // Case theses are files, and it was not the description that was clicked
        if(Globals.flagVariable == true && Globals.theseAreFiles == true && fileName != "Description")
        {

            // Activate the creator menu
            creatorMenu.SetActive(true);

            // Set the path to save to that directory
            savePathField.text = Globals.currentPath;
            Globals.selectedPath = Globals.currentPath;

            // Get the button and the index
            string questionName = button.GetComponentInChildren<TMP_Text>().text;
            int buttonIndex = GetIndexFromButtonName(name);

            Debug.Log("Current button index : " + buttonIndex);

            // Get the number of the index of the file
            int fileIndex = (Globals.currentPage - 1) * 5 + buttonIndex;

            // Get the right path to the file
            string filePath = Globals.fileArray[fileIndex];
            Debug.Log("path to file: " + filePath);
            Debug.Log("name of the file: " + questionName);

            // Set the file name, so that it can be loaded back with the same name
            Globals.filePath = filePath;
            Globals.fileName = Path.GetFileName(filePath);

            // Load the selected question in the temp save file
            File.Copy(filePath, Path.Combine(Globals.tempSavePath, Path.GetFileName(filePath)));

            // Add the name of the exercise to the preview and enable it
            previewQuestion1.GetComponentInChildren<TMP_Text>().text = questionName;
            previewQuestion1.interactable = true;
            Debug.Log("PreviewQuestion1 button is now interactable.");

            // Get the json string
            string json = File.ReadAllText(filePath);

            // Find our what kind of question it is
            if(json.Contains("input question") == true)
            {
                // Extract the object
                InputQuestion question = JsonUtility.FromJson<InputQuestion>(json);

                // Load all model files
                if(question.numberOfModels >= 1)
                {
                    // Load the first model file
                    File.Copy(Path.Combine(Globals.selectedPath, question.model1Name), Path.Combine(Globals.tempSavePath, question.model1Name));
                }
                if(question.numberOfModels >= 2)
                {
                    // Load the second model file
                    File.Copy(Path.Combine(Globals.selectedPath, question.model2Name), Path.Combine(Globals.tempSavePath, question.model2Name));
                }
                if(question.numberOfModels >= 3)
                {
                    // Load the third model file
                    File.Copy(Path.Combine(Globals.selectedPath, question.model3Name), Path.Combine(Globals.tempSavePath, question.model3Name));
                }
                if(question.numberOfModels >= 4)
                {
                    // Load the fourth model file
                    File.Copy(Path.Combine(Globals.selectedPath, question.model4Name), Path.Combine(Globals.tempSavePath, question.model4Name));
                }
                if(question.numberOfModels == 5)
                {
                    // Load the fifth model file
                    File.Copy(Path.Combine(Globals.selectedPath, question.model5Name), Path.Combine(Globals.tempSavePath, question.model5Name));
                }

                // Get the models array
                string[] modelArray = GetModelsArray(Globals.tempSavePath);

                int index = 0;

                // Set the models
                foreach(string model in modelArray)
                {
                    // Get the json string
                    string jsonModel = File.ReadAllText(model);

                    // Extract the object
                    Model modelObject = JsonUtility.FromJson<Model>(jsonModel);

                    // Get the right button
                    Button previewButton = GetRightModelPreviewButton(index);

                    // Set the name of the button correctly
                    previewButton.GetComponentInChildren<TMP_Text>().text = modelObject.modelName;

                    // Make it interactable
                    previewButton.interactable  = true;

                    // Increase index by one
                    index = index + 1;
                }

                // Set the rest of the buttons correctly
                for(int rest = index; rest < 5; rest = rest + 1)
                {
                    // Get the right button
                    Button previewButton = GetRightModelPreviewButton(rest);
                    if(rest == index)
                    {
                        // Set the name of the button correctly
                        previewButton.GetComponentInChildren<TMP_Text>().text = "+";
                        previewButton.interactable  = true;
                    } else {
                        // Set the name of the button correctly
                        previewButton.GetComponentInChildren<TMP_Text>().text = "";
                        previewButton.interactable  = false;
                    }
                }
            }

            // Here change the button from "save" to "change"
            saveButton.gameObject.SetActive(false);
            changeButton.gameObject.SetActive(true);

            // Disable the add button and brows directories button so that no additional question can be created and the path can't be changed.
            addButton.interactable = false;
            browsDirectoriesButton.interactable = false;

            // Set the flag that a file is currently beeing changed (since it changes the index of the file in the buttons of preview in the creator menu)
            Globals.currentlyChangingFile = true;

            // Reset the menu so that next time the user accesses the selection is back at the root directory
            resetBrowsDirectories();

            // Deactivate the brows directories menu
            browsDirectoriesMenu.SetActive(false);
        }

        // Case theses are files, and it was the description that was clicked
        if(Globals.flagVariable == true && Globals.theseAreFiles == true && fileName == "Description")
        {
            // Activate the window to enter a description
            ActivateEnterDescriptionWindow();

            // Activate the change button and deactivate the ok button
            changeDescription.gameObject.SetActive(true);
            okDescription.gameObject.SetActive(false);
            backDescription.gameObject.SetActive(true);
            cancelDescription.gameObject.SetActive(false);

            // Load the log file
            string json = File.ReadAllText(Globals.currentPath + "Description.json");
            Log descriptionLog = JsonUtility.FromJson<Log>(json);

            // Paste the existing information in the text fields
            descriptionHeading.text = descriptionLog.heading;
            descriptionText.text = descriptionLog.description;
        }
        Debug.Log("Current path: " + Globals.currentPath);
    }

    // Method that returns the array of models (json files) in the given path
    static string[] GetModelsArray(string path) 
    {
        Debug.Log("The model array was created");
        string[] questions = Directory.GetFiles(path, "Model*", SearchOption.TopDirectoryOnly);
        return questions;
    }

    // Method that returns you the right model preview button given the index
    public Button GetRightModelPreviewButton(int index)
    {
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

    // Method that saves the changed description back in the old file
    public void ChangeDescription()
    {
        // First access the old description
        string jsonOld = File.ReadAllText(Globals.currentPath + "Description.json");
        Log descriptionLog = JsonUtility.FromJson<Log>(jsonOld);

        // Change the description and heading according to the text fields
        descriptionLog.heading = descriptionHeading.text;
        descriptionLog.description = descriptionText.text;

        // Convert this to a string
        string jsonNew = JsonUtility.ToJson(descriptionLog);

        // Delete the old file
        File.Delete(Globals.currentPath + "Description.json");

        // Create the new file
        File.WriteAllText(Globals.currentPath + "Description.json", jsonNew);

        // Deactivate the window
        DeactivateEnterDescriptionWindow();

        // Empty the text fields
        descriptionHeading.text = "";
        descriptionText.text = "";
    }

    // Method that activates all windows and veils for the ask description window
    public void ActivateAskDescriptionWindow()
    {
        // Enable the window and the veil
        veilSmallWindow.SetActive(true);
        AskDescriptionWindow.SetActive(true);
    }

    // Method that deactivates all windows and veils for the ask description window
    public void DeactivateAskDescriptionWindow()
    {
        // Disable the window and the veil
        veilSmallWindow.SetActive(false);
        AskDescriptionWindow.SetActive(false);
    }

    // Method that activates all windows and veils for the enter description window
    public void ActivateEnterDescriptionWindow()
    {
        // Enable the window and the veil
        veilLargeWindow.SetActive(true);
        EnterDescriptionWindow.SetActive(true);
    }

    // Method that deactivates all windows and veils for the enter description window
    public void DeactivateEnterDescriptionWindow()
    {
        // Disable the window and the veil
        veilLargeWindow.SetActive(false);
        EnterDescriptionWindow.SetActive(false);

        // Activate the right buttons
        changeDescription.gameObject.SetActive(false);
        okDescription.gameObject.SetActive(true);
        backDescription.gameObject.SetActive(false);
        cancelDescription.gameObject.SetActive(true);
    }

    // Get the index that the button gives
    public int GetIndexFromButtonName(string buttonName)
    {
        // First get the index inside the page
        int indexOnPage = 0;

        switch(buttonName)
        {
            case "Directory1":
                indexOnPage = 0;
            break;
            case "Directory2":
                indexOnPage = 1;
            break;
            case "Directory3":
                indexOnPage = 2;
            break;
            case "Directory4":
                indexOnPage = 3;
            break;
            case "Directory5":
                indexOnPage = 4;
            break;
        }
        return indexOnPage;
    }

    // Method that actualizes the global variables (when going deeper or shallower in directory structures)
    public void ActualizeGlobals()
    {
        // First I actualize the directories array and number
        Globals.directoriesArray = GetDirectoriesArray();
        Globals.numberOfDirectories = GetNumberOfDirectories(Globals.directoriesArray);

        // Case there are no directories
        if(Globals.numberOfDirectories == 0)
        {
            Globals.filesArray = GetFilesArray();
            Globals.numberOfFiles = GetNumberOfFiles(Globals.filesArray);
            if(Globals.numberOfFiles != 0)
            {
                // Then I actualize the number of pages and rename the heading accordingly
                Globals.currentPage = 1;
                double value = (double)Globals.numberOfFiles/(double)5;
                Globals.numberOfPages = System.Convert.ToInt32(System.Math.Ceiling(value));
                GameObject.Find("HeadingTextBrowsDirectories").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;

            } else {
                // Case neither files not directories
                Globals.currentPage = 1;
                Globals.numberOfPages = 1;
                GameObject.Find("HeadingTextBrowsDirectories").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
            }

        // Case directories
        } else {
            // Actualize the page heading
            Globals.currentPage = 1;
            double value = (double)Globals.numberOfDirectories/(double)5;
            Globals.numberOfPages = System.Convert.ToInt32(System.Math.Ceiling(value));
            currentPageText.text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
        }
    }

    // Method for the back button 
    // It should change the menu to the previous menu (main menu or creator)
    // The distinction is done with the fact that the "select" button is enabled or not
    public void Back()
    {
        // Then display the right menu
        if(selectButton.gameObject.activeSelf == true){
            creatorMenu.SetActive(true);
        } else {
            mainMenu.SetActive(true);
        }

        // First reset the globals so that everything is reset the next time the user enters the menu
        resetBrowsDirectories();

        // Disable the menu
        browsDirectoriesMenu.SetActive(false);

        // Disable the select button after exiting
        selectButton.gameObject.SetActive(false);
    }

    // Method that resets the brows directories menu
    public void resetBrowsDirectories()
    {
        Globals.currentPath = Globals.rootDirectoryPath;
        Globals.currentPathShorten = "";
        Globals.depth = 1;

        // Then I actualize in a function the directories, page numbers, heading
        ActualizeGlobals();

        // Then I disable / enable the previous and next button based on the number of pages
        DisableOrEnableButtons();

        // Then I rename / delete the name of the predefined buttons and disable those that have no name
        RenameButtons(Globals.currentPath);
    }

    public void AddDirectory(TMP_InputField input)
    {
        if (input.text.Length > 0) 
		{
            // Access to the text that was entered
            string directoryName = mainInputField.text;

            // Create new path that will exist after the directory has been created
            string newPath = Globals.currentPath + directoryName + @"\";

            // Create the new directory if it does not already exist
            if (!Directory.Exists(newPath))
            {
                // Create directory
                Directory.CreateDirectory(newPath);

                // Disable the window and enable the menu
                window.SetActive(false);

                // Save the page you were on
                int oldPageNumber = Globals.currentPage;

                // Since a new directory was created, it is needed to actualize it
                ActualizeGlobals();
                Globals.currentPage = oldPageNumber;
                DisableOrEnableButtons();
                RenameButtons(Globals.currentPath);
            } else {
                // Display error
                errorText.gameObject.SetActive(true);

            }
            // Reset the text after you used it
            mainInputField.text = "";
		}
    }

    // Method that enables the proceed selection window
    public void EnableProceedSelectionWindow()
    {
        proceedSelectionWindow.SetActive(true);
        veilSmallWindow.SetActive(true);
    }

    // Method that disable the proceed selection window
    public void DisableProceedSelectionWindow()
    {
        proceedSelectionWindow.SetActive(false);
        veilSmallWindow.SetActive(false);
    }

    // Method that disable the "are you sure you want to select that directory?" menu
    public void SelectDirectory()
    {
        // Set the selected path as currentPath
        Globals.selectedPathShorten = Globals.currentPathShorten;
        Globals.selectedPath = Globals.currentPath;

        // Disable the error message
        noPathSelected.gameObject.SetActive(false);

        // Disable the window
        DisableProceedSelectionWindow();

        // Open the ask to enter Description window if the Description.json file does not exist in the folder
        AskToEnterDescription();
    }

    // Method that checks if there was a Description.json file in the selected folder
    public void AskToEnterDescription()
    {
        // After  the "are you sure you want to select that directory?" window was disabled, if the folder was empty, ask if the user wants to enter a description
         if (!File.Exists(Globals.currentPath + "Description.json")) 
        {
            // If there wasn't a description file, ask the user to enter one
            ActivateAskDescriptionWindow();
        } else {

            // If there was already a description file, return to the old menu (always creator)
            Back();
        }
    }

    // Method that saves the entered description
    public void SaveDescription()
    {
        // Create the log object and save the given information in it
        Log description = new Log();
        description.heading = descriptionHeading.text;
        description.description = descriptionText.text;

        // Save the Description in a Description.json file
        string json = JsonUtility.ToJson(description);
        File.WriteAllText(Globals.selectedPath + "Description.json", json);

        // Disable the window
        DeactivateEnterDescriptionWindow();

        // Get back to the old menu
        Back();
    }

    public void SearchForModel()
    {
        // Save the old current and root directory paths
    }

    // Method that resets the path to the root directory
    
}
