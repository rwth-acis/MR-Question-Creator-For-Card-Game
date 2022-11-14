using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetupRootDirectory());
    }

    public void ExitApplication()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    // The coroutine used to setup the root directory if it does not already exist, and that initialize the globals upon start
    IEnumerator SetupRootDirectory()
    {
        Debug.Log("Creating the root directory!");

        // Get the path to the folder of the application
        string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MRQuestion Creator");

        // Check if the folder for this application exists in the Documents folder
        if(!Directory.Exists(directoryPath))
        {
            // Create the root directory for the application
            Directory.CreateDirectory(directoryPath);

            // Create the temp save folder
            Directory.CreateDirectory(Path.Combine(directoryPath, "tempSave"));

            // Create the level save folder
            Directory.CreateDirectory(Path.Combine(directoryPath, "levelSave"));
        }

        if(!Directory.Exists(Path.Combine(directoryPath, "tempSave"))){
            Directory.CreateDirectory(Path.Combine(directoryPath, "tempSave"));
        }

        if(!Directory.Exists(directoryPath))
        {
            Debug.Log("Cannot create a folder in Documents");
        }

        while(!Directory.Exists(directoryPath))
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
