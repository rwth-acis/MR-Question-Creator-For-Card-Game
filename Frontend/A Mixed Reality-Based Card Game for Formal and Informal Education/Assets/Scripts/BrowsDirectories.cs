using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

static class Globals
{
    public static string currentPath;
    public static string rootDirectoryPath;
    public static int currentPage;
    public static int numberOfPages;
    public static int numberOfDirectories;
    public static string[] directoriesArray;
}

public class BrowsDirectories : MonoBehaviour
{
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

    // Initialize the global variables
    public void InitializeGlobalVariables()
    {
        string scriptPath = GetCurrentFilePath();
        string rootDirectoryPath = GetPathToRootDirectory(scriptPath);
        //Debug.Log(rootDirectoryPath);
        Globals.rootDirectoryPath = rootDirectoryPath;
        Globals.currentPath = rootDirectoryPath;
        Globals.currentPage = 1;
        Globals.directoriesArray = GetDirectoriesArray();
        Globals.numberOfDirectories = GetNumberOfDirectories(Globals.directoriesArray);
    }

    // Initial disabling of the buttons (when accessing the menu from the main menu)
    public void DisableButtons()
    {
        //GameObject.Find("buttonName").GetComponentInChildren<Text>().text = "la di da";Previous.interactable = false;
        Button previous = GameObject.Find("Previous").GetComponent<Button>();
        previous.interactable = false;
        Button next = GameObject.Find("Next").GetComponent<Button>();
        next.interactable = false;
    }

    // Method that creates the buttons depending of the directory we are currently in
    public void RenameButtons(string path)
    {
        // TODO

        // Case there are no directories to be displayed
        if(Globals.numberOfDirectories == 0){
            // TODO no directories but exercise files / nothing
        }
        // Case there are less then 4 directories, and no next or previous is needed
        else if(Globals.numberOfDirectories > 0 && Globals.numberOfDirectories <= 5)
        {
            // First rename the buttons that should have button names, check that they are enabled
            int currentIndex = 1;
            foreach (string dir in Globals.directoriesArray) {
                string lastFolderName = Path.GetFileName(dir);
                switch (currentIndex)
                {
                    case 1:
                        GameObject.Find("Directory1").GetComponentInChildren<Text>().text = lastFolderName;
                        Button directory1 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory1.interactable = true;
                    break;
                    case 2:
                        GameObject.Find("Directory2").GetComponentInChildren<Text>().text = lastFolderName;
                        Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory2.interactable = true;
                    break;
                    case 3:
                        GameObject.Find("Directory3").GetComponentInChildren<Text>().text = lastFolderName;
                        Button directory3 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory3.interactable = true;
                    break;
                    case 4:
                        GameObject.Find("Directory4").GetComponentInChildren<Text>().text = lastFolderName;
                        Button directory4 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory4.interactable = true;
                    break;
                    case 5:
                        GameObject.Find("Directory5").GetComponentInChildren<Text>().text = lastFolderName;
                        Button directory5 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory5.interactable = true;
                    break;
                }
                currentIndex = currentIndex + 1;
            }
            // Then delete the text of the others and disable them.
            for(int restToBeDeleted = currentIndex; restToBeDeleted < 6; restToBeDeleted++){
                switch (restToBeDeleted)
                {
                    case 2:
                        GameObject.Find("Directory2").GetComponentInChildren<Text>().text = "";
                        Button directory2 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory2.interactable = false;
                    break;
                    case 3:
                        GameObject.Find("Directory3").GetComponentInChildren<Text>().text = "";
                        Button directory3 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory3.interactable = false;
                    break;
                    case 4:
                        GameObject.Find("Directory4").GetComponentInChildren<Text>().text = "";
                        Button directory4 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory4.interactable = false;
                    break;
                    case 5:
                        GameObject.Find("Directory5").GetComponentInChildren<Text>().text = "";
                        Button directory5 = GameObject.Find("Directory2").GetComponent<Button>();
                        directory5.interactable = false;
                    break;
                
                }
            }

        // Case Button next / previous or both must be activated
        }else{
            // TODO case scrollbar / scrolling enabled
        }
    }

    // Test method that renames a button
    public void Actualize(){
        InitializeGlobalVariables();
        string path = Globals.currentPath;
        RenameButtons(path);
        Debug.Log("Directories should have been renamed");
    }

    // Test method that renames a button
    public void RenameOne(){
        InitializeGlobalVariables();
        foreach (string dir in Globals.directoriesArray)
        {
            Debug.Log(dir);
        }
        foreach (string dir in Globals.directoriesArray) {
            Button directory1 = GameObject.Find("Directory1").GetComponent<Button>();
            directory1.interactable = true;
            string nameOfButton = directory1.GetComponentInChildren<Text>().text;
            Debug.Log(nameOfButton);
            
            // string lastFolderName = Path.GetFileName(dir);
            // Button directory1 = GameObject.Find("Directory1").GetComponent<Button>();
            // Text ButtonText = directory1.GetComponentInChildren(typeof(Text)) as Text;
            // ButtonText.text = "Test5";
            // // directory1.interactable = false;
            // // GameObject.Find("Directory1").GetComponentInChildren<Text>().text = "i did it?";
            // Debug.Log(lastFolderName);
            // // Button directory1 = GameObject.Find("Directory2").GetComponent<Button>();
            // // directory1.interactable = true;
        }
        Debug.Log(Globals.numberOfDirectories);
    }
}
