using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

    private GameObject canvas;
    private DialogueTrigger dialogueTrigger;
    private DialogueManager dialogueManager;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas");
        dialogueTrigger = canvas.GetComponent<DialogueTrigger>();
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        dialogueTrigger.TriggerDialogue();
    }
	
	// Update is called once per frame
	void Update () {
		if (dialogueManager.isFinished)
        {
            SceneManager.LoadScene("Test1", LoadSceneMode.Single);
        }
	}
}
