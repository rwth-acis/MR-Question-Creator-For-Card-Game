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
    public static bool reset = false;
    // Path of the selected directory. Used for the creator menu to set the path where to save the questions and 3D models
    public static string selectedPath;
    public static string selectedPathShorten;

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
    public GameObject selectButton;

    // Define the input field, the error text and window so that they can get disabled / enabled when needed
    public TMP_InputField mainInputField; // The input field to create directories
    public TextMeshProUGUI errorText;
    public GameObject window;
    
    // Define the windows
    public GameObject proceedSelectionWindow;
    public GameObject veilSmallWindow;

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

    [SerializeField]
    private Sprite[] switchSprites;
    private Image switchImage;

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
        Button previous = GameObject.Find("Previous").GetComponent<Button>();
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
            // Get the array of all files
            Globals.fileArray = GetFilesArray();
            int numberOfFiles = GetNumberOfFiles(Globals.fileArray);

            // Case there are files in the folder
            if(numberOfFiles != 0)
            {
                Globals.theseAreFiles = true;
                // First rename the buttons that should have button names, check that they are enabled
                // for that initialize the range of the for loop

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
        GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
    }

    // Method that is activated when pressing previous (change the other directories)
    public void PreviousPage(){
        Globals.currentPage = Globals.currentPage - 1;
        DisableOrEnableButtons();
        RenameButtons(Globals.currentPath);
        GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
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
        //Print the time of when the function is first called.
        Globals.flagVariable = false;

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.500F);

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

        if(Globals.flagVariable == true && Globals.theseAreFiles == false){
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
            // Activate the creator menu, deactivate the brows directories menu
            creatorMenu.SetActive(true);
            browsDirectoriesMenu.SetActive(false);

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

            // Here change the button from "save" to "change"
            saveButton.gameObject.SetActive(false);
            changeButton.gameObject.SetActive(true);

            // Disable the add button and brows directories button so that no additional question can be created and the path can't be changed.
            addButton.interactable = false;
            browsDirectoriesButton.interactable = false;

            // Set the flag that a file is currently beeing changed (since it changes the index of the file in the buttons of preview in the creator menu)
            Globals.currentlyChangingFile = true;

            // Set the reset flag, so that everything is reset the next time the user accesses this menu
            Globals.reset = true;
        }

        // Case theses are files, and it was the description that was clicked
        if(Globals.flagVariable == true && Globals.theseAreFiles == true && fileName == "Description")
        {
            // TODO create a "give description window"
        }
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
                GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;

            } else {
                // Case neither files not directories
                Globals.currentPage = 1;
                Globals.numberOfPages = 1;
                GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
            }

        // Case directories
        } else {
            // Actualize the page heading
            Globals.currentPage = 1;
            double value = (double)Globals.numberOfDirectories/(double)5;
            Globals.numberOfPages = System.Convert.ToInt32(System.Math.Ceiling(value));
            GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
        }
    }

    // Method for the back button 
    // It should change the menu to the previous menu (main menu or creator)
    // The distinction is done with the fact that the "select" button is enabled or not
    public void Back()
    {
        // First reset the globals so that everything is reset the next time the user enters the menu
        Globals.reset = true;

        // Then display the right menu
        if(selectButton.activeSelf == true){
            creatorMenu.SetActive(true);
        } else {
            mainMenu.SetActive(true);
        }
        browsDirectoriesMenu.SetActive(false);

        // Disable the select button after exiting
        selectButton.gameObject.SetActive(false);
    }
    
    // Method that resets the progression when going back to the main menu and back in the brows directories menu
    public void GoingBackIn()
    {
        if(Globals.reset == true){
            Globals.currentPath = Globals.rootDirectoryPath;
            Globals.currentPathShorten = "";
            Globals.depth = 1;

            // Then I actualize in a function the directories, page numbers, heading
            ActualizeGlobals();

            // Then I disable / enable the previous and next button based on the number of pages
            DisableOrEnableButtons();

            // Then I rename / delete the name of the predefined buttons and disable those that have no name
            RenameButtons(Globals.currentPath);
            
            // Reset the flag
            Globals.reset = false;
        }
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

        // Get back to the old menu (always creator)
        Back();
    }
}
