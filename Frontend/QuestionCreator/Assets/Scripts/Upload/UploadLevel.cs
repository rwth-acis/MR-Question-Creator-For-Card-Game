using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

static class Upload
{
    // The flag that states if an upload is a success or not
    public static bool successful;

    // The number of files that are needed to be downloaded in the level
    public static int numberOfFilesToUpload;
}

public class UploadLevel : MonoBehaviour
{
    // Define the add level window
    [SerializeField]
    private GameObject uploadLevelWindow;

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
    [SerializeField]
    private TMP_Text errorUploadFailed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //----------------------------------------------------------------------------------------------------
    // The get and post requests
    //----------------------------------------------------------------------------------------------------

    // The get request coroutine
    IEnumerator GetRequest(string path)
    {
        Debug.Log("The request was send with the uri: " + Manager.BackendAPIBaseURL + path);
        UnityWebRequest uwr = UnityWebRequest.Get(Manager.BackendAPIBaseURL + path);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }

    // The post request coroutine
    IEnumerator PostRequest(string url, string text)
    {
        // Create a custom POST request
        var request = new UnityWebRequest(Manager.BackendAPIBaseURL + url, "POST");

        // Transform the string in a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(text);

        foreach(byte oneByte in bodyRaw)
        {
            Debug.Log("One byte: " + oneByte + "of the file " + url);
        }

        // Add an upload handler with the byte array in it
        // request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);

        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);

        // Add a download handler
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        // Set a request header that states the content type
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);

        // Check if the upload worked
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);

            // Set the successful flag to false
            Upload.successful = false;
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
        }

        Upload.numberOfFilesToUpload = Upload.numberOfFilesToUpload - 1;
    }

    // The get requestion coroutine to get levels
    IEnumerator GetLevels(string path)
    {
        Debug.Log("The request was send with the uri: " + Manager.BackendAPIBaseURL + path);

        // Send the get request with the base url plus the given path
        UnityWebRequest uwr = UnityWebRequest.Get(Manager.BackendAPIBaseURL + path);

        // Wait for the answer to come
        yield return uwr.SendWebRequest();

        // Check if there was a network error
        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);

        } else {

            Debug.Log("Received: " + uwr.downloadHandler.text);
            
            // Get the level array
            string[] levelArray = GetTheArray(uwr.downloadHandler.text);

            if(levelArray.Length != 0 && levelArray[0] != "null")
            {
                foreach(string level in levelArray)
                {
                    Debug.Log("Level found: " + level);
                }
            }

            // Check if the level name exist there
            if(isContained(levelArray, levelNameInputField.text) == true)
            {
                // Enable the name already exists error message
                errorAlreadyExists.gameObject.SetActive(true);

                // Make sure the error message is disabled
                errorUploadFailed.gameObject.SetActive(false);

                Debug.Log("Level with that name was already contained");

            } else {

                // Upload the level and get a truthvalue of if it worked
                UploadLevelMethod();
            }
        }
    }

    // The coroutine that waits for the result of the upload
    IEnumerator WaitForResult()
    {
        // Wait for all files to be uploaded
        while (Upload.numberOfFilesToUpload != 0) 
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Check if the upload did not work
        if(Upload.successful == false)
        {
            // Enable the error message
            errorUploadFailed.gameObject.SetActive(true);

        } else {

            // Make sure the error message is disabled
            errorUploadFailed.gameObject.SetActive(false);

            // Close the current window and get back to the main menu
            CloseWindow();
        }
    }

    //----------------------------------------------------------------------------------------------------
    // The methods that interact with the server
    //----------------------------------------------------------------------------------------------------

    // The method that pings the node and asks a response
    public void SendPing()
    {
        // Make a get request with the right url
        StartCoroutine(GetRequest("write"));
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
            errorUploadFailed.gameObject.SetActive(false);

        } else if(Regex.IsMatch(levelNameInputField.text, @"^[a-zA-Z0-9]+$") == false)
        {
            // Activate the special character error message and deactivate the others
            errorEmptyName.gameObject.SetActive(false);
            errorSpecialCharacters.gameObject.SetActive(true);
            errorUploadFailed.gameObject.SetActive(false);

        } else {

            // Deactivate the error messages
            errorEmptyName.gameObject.SetActive(false);
            errorSpecialCharacters.gameObject.SetActive(false);

            // Get the array of all levels
            StartCoroutine(GetLevels(""));
        }
    }

    // The method used to upload a level
    public void UploadLevelMethod()
    {
        Debug.Log("Level is being uploaded");

        // Read the level name / code in the input field
        string levelName = levelNameInputField.text;

        // Get the array of files
        string[] filePaths = Directory.GetFiles(Globals.currentPath);

        // Set the number of files that are needed to upload
        Upload.numberOfFilesToUpload = filePaths.Length;

        // Make sure the upload successful flag is true
        Upload.successful = true;

        // Upload each file in the level
        foreach(string file in filePaths)
        {
            // Get the name from the path
            string fileName = Path.GetFileName(file).Substring(0, Path.GetFileName(file).Length - 5);

            // Extract the json string from the file
            string json = File.ReadAllText(file);

            Debug.Log("Uploading file: " + levelName + "/" + fileName + " with the text: " + json);

            // Upload that file at the right place
            StartCoroutine(PostRequest(levelName + "/" + fileName, json));
        }

        StartCoroutine(WaitForResult());
    }

    // Method used to close the window after the upload was successful
    public void CloseWindow()
    {
        // Close the menu and return to the main menu
        uploadLevelWindow.gameObject.SetActive(false);

        // Reset the browse level menu by setting the flag to true
        Globals.resetBrowseLevelMenu = true;
    }

    //---------------------------------------------------------------------------------------------------------------------
    // Helper Methods
    //---------------------------------------------------------------------------------------------------------------------

    // The JSON Serialization for the input questions
    [Serializable]
    public class LevelDirectories
    {
        public string[] array;
    }

    // Method used to extract an array out of the string passed by the get request
    public string[] GetTheArray(string data)
    {
        Debug.Log("The data received is: " + data);

        if(data != "null")
        {
            // Extract the LevelDirectories object
            LevelDirectories levelDirectories = JsonUtility.FromJson<LevelDirectories>(data);

            // Initialize an array of the same length as the array
            string[] levelNames = new string[levelDirectories.array.Length];

            // Initialize the current index
            int index = 0;

            // Extract the directory names (currently complete paths)
            foreach(string level in levelDirectories.array)
            {
                // Get the name of the file and save it in the level names array
                levelNames[index] = Path.GetFileName(levelDirectories.array[index]);

                // Increase the index by one
                index = index + 1;
            }

            // Return the level names array
            return levelNames;

        } else {

            // Create an empty string array since there are no levels
            string[] levelNames = new string[1];

            // Set the string in the only slot to empty
            levelNames[0] = "";

            // Return the empty array
            return levelNames;
        }
    }

    // Method that checks if a string is contained in an array of strings
    public bool isContained(string[] array, string name)
    {
        // Go through all strings of the array
        foreach (string modelName in array)
        {
            // Check if the current name and the given name are the same
            if(modelName == name)
            {
                return true;
            }
        }

        return false;
    }
}
