using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BackendConnector : MonoBehaviour
{

    // Sends the save data to the backend

    // parameter "levelName" is the name of the level to save
    // parameter "saveName" is the name of the file to save
    // parameter "saveData" is the data to be saved

    public static async Task<bool> Save(string levelName, string saveName, string saveJson)
    {
        Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saves/" + levelName + "/" + saveName, saveJson);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (resp.Successful)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
