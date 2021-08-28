using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using i5.Toolkit.Core.Utilities;

public class BackendConnector : MonoBehaviour
{

    // Sends the save data to the backend

    // parameter "levelName" is the name of the level to save
    // parameter "saveName" is the name of the file to save
    // parameter "saveData" is the data to be saved

    // public static async Task<bool> Save(string levelName, string saveName, string saveJson)
    // {
    //     Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saves/" + levelName + "/" + saveName, saveJson);
    //     ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
    //     if (resp.Successful)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // // The method used to access the list of all level folders that were uploaded
    // public static async Task<string[]> GetLevels()
    // {
    //     Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saves", null, -1, null, true);
    //     ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);

    //     // Check if the operation was a success
    //     if (!resp.Successful)
    //     {
    //         Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
    //         string[] newArray = new string[0];
    //         return newArray;

    //     } else {

    //         // Get the array of levels
    //         string[] levels = JsonArrayUtility.FromJson<string>(resp.ResponseBody);
    //         return levels;
    //     }
    // }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
