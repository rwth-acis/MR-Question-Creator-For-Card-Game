using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;

public class Rename : MonoBehaviour
{
    Text HintBody;

    // Start is called before the first frame update
    void Start()
    {
        // HintBody = this.gameObject.GetComponent<Text>();
        // HintBody.text = "It works!";
        GameObject.Find("Directory1Text").GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
