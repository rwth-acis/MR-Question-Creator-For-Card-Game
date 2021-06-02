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

    // Defining the input fields
    public TMP_InputField enterQuestionInput;
    public TMP_InputField enterAnswerInput;
    public TMP_InputField enterQuestionMultiple;
    public TMP_InputField enterAnswerMultiple;
    public TMP_InputField enterName;

    // Define the error texts that need to be displayed if an input field is empty
    public TextMeshProUGUI errorNoQuestionInput;
    public TextMeshProUGUI errorNoAnswerInput;


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
    public void ExitWithoutSaving(string nameA, string nameB, string textA, string textB)
    {
        // First check if something was added
        if(GameObject.Find(nameA).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != textA || GameObject.Find(nameB).GetComponent<Button>().GetComponentInChildren<TMP_Text>().text != textB)
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
        ExitWithoutSaving("Add3DModel1", "PreviewQuestion1", "+","");
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainMenu;
    }

    // Method to navigate from the multiple choice mode back to the main creator
    // For now it only checks if the answers are not filled, ignores the question, can be improved TODO
    public void GetBackFromMultipleChoice()
    {
        ExitWithoutSaving("AddAnswer1", "AddAnswer2","enter first answer", "enter second answer");
        Menus.lastMenu = mainMenu;
        Menus.currentMenu = mainCreator;
    }

    // Method to navigate from the input mode back to the main creator
    public void GetBackFromInputMode()
    {
        if(enterQuestionInput.text == "" && enterAnswerInput.text == "")
        {
            // Case nothing typed in
            mainCreator.SetActive(true);
            inputModeCreator.SetActive(false);
            veilLargeWindow.SetActive(false);
            Debug.Log(enterQuestionInput.text);
        } else {
            // Case there is something typed in
            // Summon the "exit without saving" window
            exitWithoutSavingWindow.SetActive(true);
            veilSmallWindow.SetActive(true);
        }
    }

    // Method that saves the question and answer of an input question, for now only creates a debug log
    // Is triggered when the user clicks on the "create" button
    public void SaveInputQuestion()
    {
        // First check if every information was typed in
        if(enterQuestionInput.text != "" && enterAnswerInput.text != "")
        {
            // Case both fields contain characters, disable the error messages
            Debug.Log(enterQuestionInput.text);
            Debug.Log(enterAnswerInput.text);
            errorNoQuestionInput.gameObject.SetActive(false);
            errorNoAnswerInput.gameObject.SetActive(false);
        } else {
            // Case question and answer were not typed in, display error messages
            if(enterQuestionInput.text == "" && enterAnswerInput.text == "") {
                errorNoQuestionInput.gameObject.SetActive(true);
                errorNoAnswerInput.gameObject.SetActive(true);

            // Case question is not typed in, display error message
            } else if(enterAnswerInput.text == "" && enterQuestionInput.text != "") {
                errorNoAnswerInput.gameObject.SetActive(true);
                errorNoQuestionInput.gameObject.SetActive(false);

            // Case answer is not typed in, display error message
            } else {
                errorNoAnswerInput.gameObject.SetActive(false);
                errorNoQuestionInput.gameObject.SetActive(true);
            }
        }
    }

    // mainCreator;
    // multipleChoiceCreator;
    // inputModeCreator;

    // Method to exit a exercise creation without saving
    // It is needed to get access to the current menu / window, and delete everything that was entered
    public void ExitWithoutSavingYes()
    {
        // Case input mode creator
        if(Menus.currentMenu == inputModeCreator) 
        {
            // First delete everything that was entered
            enterQuestionInput.text = "";
            enterAnswerInput.text = "";

            // Then it is needed to set the right windows as current menu
            exitWithoutSavingWindow.SetActive(false);
            inputModeCreator.SetActive(false);
            mainCreator.SetActive(true);
            Menus.lastMenu = mainMenu;
            Menus.currentMenu = mainCreator;

            // Disable the veil
            veilLargeWindow.SetActive(false);
            veilSmallWindow.SetActive(false);

        // Case multiple choice mode creator
        } else if(Menus.currentMenu == multipleChoiceCreator) {

            // First delete everything that was entered
            enterQuestionMultiple.text = "";

            // Reset the buttons that preview the answers
            GameObject.Find("AddAnswer1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "enter first answer";
            GameObject.Find("AddAnswer2").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "enter second answer";
            GameObject.Find("AddAnswer3").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "+";
            GameObject.Find("AddAnswer4").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("AddAnswer5").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";

            // Reset the toggles
            GameObject.Find("Answer1Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer2Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer3Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer4Correct").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Answer5Correct").GetComponent<Toggle>().isOn = false;

            // Reset the interactability of buttons
            GameObject.Find("AddAnswer4").GetComponent<Button>().interactable = false;
            GameObject.Find("AddAnswer5").GetComponent<Button>().interactable = false;

            // Then it is needed to set the right windows as current menu
            exitWithoutSavingWindow.SetActive(false);
            multipleChoiceCreator.SetActive(false);
            mainCreator.SetActive(true);
            Debug.Log(Menus.lastMenu + " was the last menu");
            Debug.Log(Menus.currentMenu + " was the current menu");
            Menus.lastMenu = mainMenu;
            Menus.currentMenu = mainCreator;

            // Disable the veil
            veilLargeWindow.SetActive(false);
            veilSmallWindow.SetActive(false);

        // Case main creator menu is the current menu
        } else if(Menus.currentMenu == mainCreator) {

            // Reset the buttons that preview names of 3D models and currently created questions
            GameObject.Find("Add3DModel1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "+";
            GameObject.Find("Add3DModel2").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("Add3DModel3").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("Add3DModel4").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("Add3DModel5").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion1").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion2").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion3").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion4").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";
            GameObject.Find("PreviewQuestion5").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "";

            // Reset the toggle, check the one that is selected as default, because it is a toggle group the other is set to false automatically
            GameObject.Find("MultipleChoice").GetComponent<Toggle>().isOn = true;

            // Reset the interactability of the buttons
            GameObject.Find("Add3DModel2").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel3").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel4").GetComponent<Button>().interactable = false;
            GameObject.Find("Add3DModel5").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion1").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion2").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion3").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion4").GetComponent<Button>().interactable = false;
            GameObject.Find("PreviewQuestion5").GetComponent<Button>().interactable = false;

            // Then it is needed to set the right windows as current menu
            mainCreator.SetActive(false);
            multipleChoiceCreator.SetActive(false);
            mainMenu.SetActive(true);
            Menus.lastMenu = mainMenu;
            Menus.currentMenu = mainCreator;

            // Disable the veil
            veilSmallWindow.SetActive(false);
        }
    }
}
