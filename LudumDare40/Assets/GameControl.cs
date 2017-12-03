using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputType { None, Left, Right, Down, Up};
public enum GameState { Start, Run, Wait, Dead, GameOver};
public enum Direction { Left, Right, Down, Up };
public class GameControl : MonoBehaviour
{
    public GameObject[] startPoints;
    public GameObject[] desks;

    public GameObject childPref;    // Prefab for child


    int childIndex = 0; // which child is being controlled
    List<ChildControl> children;

    GameState state = GameState.Run;


    public float levelTime = 120;
    public float legTime = 20;

    float lastTimer = 0;
    float mainTimer = 0; // timer for level
    float legTimer = 0; // timer for leg

    public float waitTime = 2;
    float waitTimer;
    // Use this for initialization
    void Start()
    {
        // Set Timers
        mainTimer = levelTime;
        lastTimer = levelTime;
        legTimer = legTime;

        children = new List<ChildControl>();


        // Create Child Data - Child ref, Start Position, End Position, and a recording of input.
        for (int i = 0; i < startPoints.Length; i++)
        {
            ChildControl c = GameObject.Instantiate(childPref).GetComponent<ChildControl>();
            c.transform.position = startPoints[i].transform.position;
            bool ghost;
            if (i == 0) ghost = false;
            else ghost = true;

            c.Init(ghost, startPoints[i].transform.position, desks[i].GetComponent<DeskControl>());
            children.Add(c);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (state == GameState.Start)
        {

        }
        else if (state == GameState.Run)
        {
            HandleRun();
        }
        else if(state == GameState.Wait || state == GameState.Dead)
        {
            // Update Ghosts
            for (int i = 0; i < childIndex && i < children.Count; i++)
            {
                children[i].Run();
            }

            if (waitTimer <= 0)
            {                
                if(state == GameState.Wait) NextLeg();
                else if(state == GameState.Dead) ResetLeg();

                state = GameState.Run;
            }
            else waitTimer -= Time.deltaTime;
        }
        else if (state == GameState.GameOver)
        {
            Debug.Log("Game Over");
        }
    }

    void HandleRun()
    {
        if(childIndex >= children.Count)
        {
            state = GameState.GameOver;
            Debug.Log("Level Complete");
            return;
        }


        // Handle Controled Child
        children[childIndex].Run();
       

        // Handle Ghosts
        if (children[childIndex].HasMoved)
        {
            // Update timers, if either reach 0 - gameover
            mainTimer -= Time.deltaTime;
            legTimer -= Time.deltaTime;

            if (mainTimer <= 0 || legTimer <= 0)
            {
                state = GameState.GameOver;
                return;
            }

            // Update Ghosts
            for (int i = 0; i < childIndex && i < children.Count; i++)
            {
                children[i].Run();
            }

            // Has Reached Goal
            if (children[childIndex].HitGoal())
            {
                state = GameState.Wait;
                waitTimer = waitTime;
            }
            // Hit Ghost
            else if (children[childIndex].HitGhost())
            {
                state = GameState.Dead;
                waitTimer = waitTime;
            }
        }        
    }
    
    void NextLeg()
    {
        // Update Timers
        lastTimer = mainTimer;
        legTimer = legTime;

        // Next child
        childIndex++;

        for(int i = 0; i < children.Count; i++)
        {
            if (i == childIndex)
                children[i].Reset(false);
            else children[i].Reset(true);
        }
    }

    void ResetLeg()
    {
        // Reset Timers
        mainTimer = lastTimer;
        legTimer = legTime;

        for (int i = 0; i < children.Count; i++)
        {
            if (i == childIndex)
                children[i].Reset(false);
            else children[i].Reset(true);
        }
    }
}
