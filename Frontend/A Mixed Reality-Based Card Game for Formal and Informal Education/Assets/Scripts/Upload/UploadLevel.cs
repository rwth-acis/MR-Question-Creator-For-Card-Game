using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UploadLevel : MonoBehaviour
{
    // Define the level name input field
    [SerializeField]
    private TMP_InputField levelNameInputField;

    // Define the error messages
    [SerializeField]
    private TMP_Text errorAlreadyExists;
    [SerializeField]
    private TMP_Text errorEmptyName;
    [SerializeField]
    private TMP_Text errorSpecialCharacters;

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
        // First check that the name is non empty, and do not contain special characters
        if(levelNameInputField.text == "")
        {
            // Activate the empty name error message and deactivate the others
            errorEmptyName.gameObject.SetActive(true);
            errorSpecialCharacters.gameObject.SetActive(false);
            errorAlreadyExists.gameObject.SetActive(false);

        } else if(Regex.IsMatch(levelNameInputField.text, @"^[a-zA-Z0-9]+$") == false)
        {
            // Activate the special character error message and deactivate the others
            errorEmptyName.gameObject.SetActive(false);
            errorSpecialCharacters.gameObject.SetActive(true);
            errorAlreadyExists.gameObject.SetActive(false);

        } else {

            // Deactivate the error messages
            errorEmptyName.gameObject.SetActive(false);
            errorSpecialCharacters.gameObject.SetActive(false);

            // Upload the level and get a truthvalue of if it worked
            bool uploadWorked = UploadLevelMethod();

            // Check if the upload did not work
            if(uploadWorked == false)
            {
                // Enable the error message
                errorAlreadyExists.gameObject.SetActive(true);

            } else {
                // Make sure the error message is disabled
                errorAlreadyExists.gameObject.SetActive(false);
            }
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

            // Initialize the successful flag
            bool successful = true;

            // Upload each file in the level
            foreach(string file in filePaths)
            {
                // Get the name from the path
                string fileName = Path.GetFileName(file);

                // Upload that file at the right place
                // bool success = BackendConnector.Save(levelName, fileName, file);
                bool success = false;

                // Check if it was a success
                if(success == false)
                {
                    // If it was not a success, set the successful flag to false
                    successful = false;
                }
            }

            // Check if the process was unsuccessful
            if(successful == false)
            {
                // Delete everything TODO
            }

            // Return the sucessful flag
            return successful;
        }
    }
}
