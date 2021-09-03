using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Manager : MonoBehaviour
{
    private static Manager mangerInsance;

    // Define the base path
    [SerializeField]
    private string basePath = "template";

    public static string getBasePath
    {
        get { return mangerInsance.basePath; }
    }

    // Define the backendAddress
    [SerializeField]
    private string backendAddress = "http://127.0.0.1";

    public static string getBackendAddress
    {
        get { return mangerInsance.backendAddress; }
    }

    // Define the freed port
    [SerializeField]
    private int port = 8080;

    public static int getPort
    {
        get { return mangerInsance.port; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mangerInsance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method used to access the full backend address
    public static string FullBackendAddress
    {
        get { return getBackendAddress + ":" + getPort; }
    }

    // Method used to access the full backend address followed by the base path
    public static string BackendAPIBaseURL
    {
        get
        {
            return FullBackendAddress + "/" + getBasePath + "/";
        }
    }
}

