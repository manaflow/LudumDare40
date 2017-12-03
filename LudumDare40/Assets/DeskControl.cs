using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskControl : MonoBehaviour
{
    public Direction direction;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite upSprite;


    GameObject goal;
    ParticleSystem ps;
    SpriteRenderer sr; // goal sprite

    public GameObject Goal { get { return goal; } }

    public void EnableGoal()
    {
        sr.enabled = true;
        ps.Play();
    }

    public void DisableGoal()
    {
        sr.enabled = false;
        ps.Stop();
    }
	// Use this for initialization
	void Start ()
    {
        goal = transform.GetChild(0).gameObject;
        ps = goal.transform.GetChild(0).GetComponent<ParticleSystem>();
        sr = goal.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
