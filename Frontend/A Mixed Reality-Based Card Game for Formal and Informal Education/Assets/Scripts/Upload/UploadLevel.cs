using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UploadLevel : MonoBehaviour
{
    // Define the level name input field
    [SerializeField]
    private TMP_InputField levelNameInputField;

    // Define the error window
    [SerializeField]
    private GameObject errorWindow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // The method started when a user clicks on the upload level button
    public void TryUploadingLevel()
    {
        // Upload the level and get a truthvalue of if it worked
        bool uploadWorked = UploadLevelMethod();

        // Check if the upload did not work
        if(uploadWorked == false)
        {
            // Enable the error window
            errorWindow.SetActive(true);
        }
    }

    // The method used to upload a level
    public bool UploadLevelMethod()
    {
        // Read the level name / code in the input field
        string levelName = levelNameInputField.text;

        // Check that no level with that name exist 
        if(false)
        {
            // Return false, which will trigger an error message visible in the application
            return false;
        } else {
            // Get the array of files
            string[] filePaths = Directory.GetFiles(Globals.currentPath);

            // Initialize the successfull flag
            bool successfull = true;

            // Upload each file in the level
            foreach(string file in filePaths)
            {
                // Get the name from the path
                string fileName = Path.GetFileName(file);

                // Upload that file at the right place
                bool success = BackendConnector.Save(levelName, fileName, file);

                // Check if it was a success
                if(success == false)
                {
                    // If it was not a success, set the successfull flag to false
                    successfull = false;
                }
            }

            // Check if the process was unsuccessfull
            if(sucessfull == false)
            {
                // Delete everything TODO
            }

            // Return the sucessfull flag
            return successfull;
        }
    }
}
