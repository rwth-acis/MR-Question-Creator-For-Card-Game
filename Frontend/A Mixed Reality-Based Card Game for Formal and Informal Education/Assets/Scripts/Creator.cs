using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

static class Menus
{
    public static GameObject lastMenu;
    public static GameObject currentMenu;
}

public class Creator : MonoBehaviour
{
    // Defining the necessary menus and windows
    public GameObject mainMenu;
    public GameObject mainCreator;
    public GameObject multipleChoiceCreator;
    public GameObject inputModeCreator;
    public GameObject enterAnswerWindow;
    public GameObject enterNameWindow;
    public GameObject exitWithoutSavingWindow;
    // The veils are invisible, they are used to block access to the buttons on the menu under it
    public GameObject veilLargeWindow;
    public GameObject veilSmallWindow;


    // Start is called before the first frame update
    void Start()
    {
        // Disable all buttons that cannot be used at the begining
        GameObject.Find("Add3DModel2").GetComponent<Button>().interactable = false;
        GameObject.Find("Add3DModel3").GetComponent<Button>().interactable = false;
        GameObject.Find("Add3DModel4").GetComponent<Button>().interactable = false;
        GameObject.Find("Add3DModel5").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion1").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion2").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion3").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion4").GetComponent<Button>().interactable = false;
        GameObject.Find("PreviewQuestion5").GetComponent<Button>().interactable = false;

        // Set last and current menu
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainCreator;
        
    }

    // Method activated when clicking on the "Add" button in the creator screen, opens the right exercise creator window
    public void AddExercise()
    {
        // Enabling the right creator window depending on which mode was chosen
        if(GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn == true)
        {
            Debug.Log("Multiple choice is on!");
            veilLargeWindow.SetActive(true);
            multipleChoiceCreator.SetActive(true);
            Menus.currentMenu = multipleChoiceCreator;
        } else {
            Debug.Log("Input mode is on!");
            veilLargeWindow.SetActive(true);
            inputModeCreator.SetActive(true);
            Menus.currentMenu = inputModeCreator;
        }
        Menus.lastMenu = mainCreator;
    }

    // Method that summons the "are you sure you do not want to save" window if something was already done
    public void ExitWithoutSaving(string nameA, string nameB)
    {
        // First check if something was added
        if(GameObject.Find(nameA).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != "+" || GameObject.Find(nameB).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != "")
        {
            Debug.Log("Not empty!");
            exitWithoutSavingWindow.SetActive(true);
            veilSmallWindow.SetActive(true);
        } else {
            Debug.Log("Nothing to save!");
            Menus.lastMenu.SetActive(true);
            Menus.currentMenu.SetActive(false);
            veilLargeWindow.SetActive(false);
        }
    }

    public void ExitCreator()
    {
        // First check if something was added
        ExitWithoutSaving("Add3DModel1", "PreviewQuestion1");
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainMenu;
    }

    // Method to navigate from the multiple choice mode back to the main creator
    public void GetBackFromMultipleChoice()
    {
        ExitWithoutSaving("AddAnswer1", "AddAnswer2");
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainCreator;
        // mainCreator.SetActive(true);
        // multipleChoiceCreator.SetActive(false);
        // veilLargeWindow.SetActive(false);
    }

    // Method to navigate from the input mode back to the main creator
    public void GetBackFromInputMode()
    {
        mainCreator.SetActive(true);
        inputModeCreator.SetActive(false);
        veilLargeWindow.SetActive(false);
    }
}
