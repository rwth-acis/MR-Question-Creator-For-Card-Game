using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

static class Globals
{
    public static string currentPath;
    public static string rootDirectoryPath;
    public static int currentPage;
    public static int numberOfPages;
    public static int numberOfDirectories;
    public static string[] directoriesArray;
    public static int depth;
    public static bool flagVariable = true;
    public static bool reset = false;
    public static stack<string> parentDirectory = new Stack<string>();
}

public class BrowsDirectories : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // First I initialize the Global paths
        string scriptPath = GetCurrentFilePath();
        string rootPath = GetPathToRootDirectory(scriptPath);
        Globals.rootDirectoryPath = rootPath;
        Globals.currentPath = Globals.rootDirectoryPath;
        Globals.depth = 1;

        // Then I actualize in a function the directories, page numbers, heading
        ActualizeGlobals();

        // Then I disable / enable the previous and next button based on the number of pages
        DisableOrEnableButtons();

        // Then I rename / delete the name of the predefined buttons and disable those that have no name
        RenameButtons(Globals.currentPath);
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

    // Debug Method that I can link to a button to check if the path to the directory structure is right
    // public void DebugPath()
    // {
    //     string scriptPath = GetCurrentFilePath();
    //     string rootDirectoryPath = GetPathToRootDirectory(scriptPath);
    //     Debug.Log(rootDirectoryPath);
    // }

    // Method that returns the array of directories in the current directory
    static string[] GetDirectoriesArray() 
    {
        string[] dirs = Directory.GetDirectories(Globals.currentPath, "*", SearchOption.TopDirectoryOnly);
        return dirs;
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
        if(Globals.currentPage == 1) {
            previous.interactable = false;
            textPrevious.GetComponent<TMP_Text>().colorGradient = disabledTextGradient;
            Debug.Log("disabled previous");
        } else {
            previous.interactable = true;
            textPrevious.GetComponent<TMP_Text>().colorGradient = enabledTextGradient;
            Debug.Log("enabled previous");
        }

        // Enable / Disable next button and change color
        Button next = GameObject.Find("Next").GetComponent<Button>();
        TMP_Text textNext = next.GetComponentInChildren<TMP_Text>();
        if(Globals.currentPage != Globals.numberOfPages){
            next.interactable = true;
            textNext.GetComponent<TMP_Text>().colorGradient = enabledTextGradient;
            Debug.Log("enabled next");
        } else {
            next.interactable = false;
            textNext.GetComponent<TMP_Text>().colorGradient = disabledTextGradient;
            Debug.Log("disabled next");
        }
    }

    // Method that creates the buttons depending of the directory we are currently in
    public void RenameButtons(string path)
    {
        // TODO

        // Case there are no directories to be displayed
        if(Globals.numberOfDirectories == 0){
            // For now make all disappear and disable the buttons
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

        // Case there is at least one directory, then display the numbers 5*x + 1 to 5*x + 5 (x is number of the page)
        } else {
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
                        directory1.GetComponentInChildren<TMP_Text>().text = " " + lastFolderName;
                        directory1.interactable = true;
                    break;
                    case 2:
                        Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory2.GetComponentInChildren<TMP_Text>().text = " " + lastFolderName;
                        directory2.interactable = true;
                    break;
                    case 3:
                        Button directory3 = GameObject.Find("Directory3").GetComponent<Button>();
                        directory3.GetComponentInChildren<TMP_Text>().text = " " + lastFolderName;
                        directory3.interactable = true;
                    break;
                    case 4:
                        Button directory4 = GameObject.Find("Directory4").GetComponent<Button>();
                        directory4.GetComponentInChildren<TMP_Text>().text = " " + lastFolderName;
                        directory4.interactable = true;
                    break;
                    case 5:
                        Button directory5 = GameObject.Find("Directory5").GetComponent<Button>();
                        directory5.GetComponentInChildren<TMP_Text>().text = " " + lastFolderName;
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
    }

    // Method that is activated when pressing previous (change the other directories)
    public void PreviousPage(){
        Globals.currentPage = Globals.currentPage - 1;
        DisableOrEnableButtons();
        RenameButtons(Globals.currentPath);
    }

    // Method that is activated when pressing the return arrow (get to the parent directory)
    public void ReturnOneUp(){
        // First we need the name of the current directory
        directoryName = Globals.parentDirectory.Pop();

        // Trim the path from the directory name#
        String whatToTrim = directoryName + @"\";
        Globals.currentPath = Globals.currentPath.TrimEnd(whatToTrim);
        Debug.Log(Globals.currentPath);
        // Here TODO

        // Actualize the path
        Globals.currentPath = Globals.currentPath + directoryName + @"\";

        Globals.currentPage = 1;
        String directoryName = directory.TrimStart(' ');
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
        if(Globals.flagVariable == true){
            // Increase the browsing depth
            Globals.depth = Globals.depth + 1;

            // Get the name of the directory selected
            string name = EventSystem.current.currentSelectedGameObject.name;
            Button button = GameObject.Find(name).GetComponent<Button>();
            String directory = GameObject.Find(name).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text;

            // Trim the first character away
            String directoryName = directory.TrimStart(' ');
            Globals.parentDirectory.Push(directoryName);

            // Actualize the path
            Globals.currentPath = Globals.currentPath + directoryName + @"\";

            // Actualize the other globals (directories array, number, page number, etc)
            ActualizeGlobals();
            DisableOrEnableButtons();
            RenameButtons(Globals.currentPath);
            StartCoroutine(FreezeCoroutine());
        }
        // // Increase the browsing depth
        // Globals.depth = Globals.depth + 1;
        // // Get the name of the directory selected
        // string name = EventSystem.current.currentSelectedGameObject.name;
        // Button button = GameObject.Find(name).GetComponent<Button>();
        // String directory = GameObject.Find(name).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text;
        // // Trim the first character away
        // String directoryName = directory.TrimStart(' ');
        // // Actualize the path
        // Globals.currentPath = Globals.currentPath + directoryName + @"\";
        // // Actualize the other globals (directories array, number, page number, etc)
        // ActualizeGlobals();
        // DisableOrEnableButtons();
        // RenameButtons(Globals.currentPath);
        // Debug.Log(Globals.currentPath);
        // DisableGUI(1);
        // //YourButton_Click(button, null);
        // //System.thread.sleep(500);
    }

    // Method that actualizes the global variables (when going deeper or shallower in directory structures)
    public void ActualizeGlobals()
    {
        // First I actualize the directories array and number
        Globals.directoriesArray = GetDirectoriesArray();
        Globals.numberOfDirectories = GetNumberOfDirectories(Globals.directoriesArray);

        // Then I actualize the number of pages and rename the heading accordingly
        Globals.currentPage = 1;
        double value = (double)Globals.numberOfDirectories/(double)5;
        Globals.numberOfPages = System.Convert.ToInt32(System.Math.Ceiling(value));
        if(Globals.numberOfPages > 0){
            GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + Globals.numberOfPages;
        } else {
            GameObject.Find("HeadingText").GetComponent<TMP_Text>().text = "Page " + Globals.currentPage + "/" + 1;
        }
        
    }

    // Method for the back button 
    public void Back()
    {
        Globals.reset = true;
    }

    public void GoingBackIn()
    {
        if(Globals.reset == true){
            Globals.currentPath = Globals.rootDirectoryPath;
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

    // Test method that renames a button
    // public void RenameOne(){
    //     foreach (string dir in Globals.directoriesArray)
    //     {
    //         Debug.Log(dir);
    //     }
    //     foreach (string dir in Globals.directoriesArray) {
    //         Button directory1 = GameObject.Find("Directory2").GetComponent<Button>();
    //         directory1.interactable = true;
    //         TMP_Text buttonText = directory1.GetComponentInChildren<TMP_Text>();
    //         string nameOfButton = buttonText.text;
    //         Debug.Log(nameOfButton);
    //         buttonText.text = "test";
    //         // GameObject.Find("Directory1Text").SetComponent<Text>().text = "test";
            
            
    //         // string lastFolderName = Path.GetFileName(dir);
    //         // Button directory1 = GameObject.Find("Directory1").GetComponent<Button>();
    //         // Text ButtonText = directory1.GetComponentInChildren(typeof(Text)) as Text;
    //         // ButtonText.text = "Test5";
    //         // // directory1.interactable = false;
    //         // // GameObject.Find("Directory1").GetComponentInChildren<Text>().text = "i did it?";
    //         // Debug.Log(lastFolderName);
    //         // // Button directory1 = GameObject.Find("Directory2").GetComponent<Button>();
    //         // // directory1.interactable = true;
    //     }
    //     Debug.Log(Globals.numberOfDirectories);
    // }
}
