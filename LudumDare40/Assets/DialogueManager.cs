﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour {

    public bool isFinished { get; set; }

    public ImageSequencer imageSequencer;
    public Text nameText;
    public Text dialogueText;
    private string currentText;
    public string fullText;
    public float delay = 0.1f;
    private Queue<string> sentences;
    private Image backgroundImage;
	// Use this for initialization
	void Start () {
        isFinished = false;
        sentences = new Queue<string>();
	}

    public void StartDialogue(Dialogue dialogue)
    {
        if(imageSequencer != null)
        {
            imageSequencer.LoadImages();
            backgroundImage = GameObject.Find("Background").GetComponent<Image>();
        }
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    IEnumerator ShowText(string sentence)
    {
        for (int i = 0; i < sentence.Length; i++)
        {

            currentText = sentence.Substring(0, i);
            dialogueText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(ShowText(sentence));
        if (imageSequencer != null)
        {
            backgroundImage.sprite = imageSequencer.DisplayNextImage();
        }
    }

    void EndDialogue()
    {
        isFinished = true;
    }
	
}
