using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildControl : MonoBehaviour {

    public LayerMask collisionMask;
    public LayerMask ghostMask;
    public LayerMask childMask;
    public LayerMask goalMask;
    
    Vector3 start;
    DeskControl desk;
    

    List<InputType> inputList =  new List<InputType>();
    int inputIndex = 0; // for ghosts
    bool isGhost;

    BoxCollider2D box;
    SpriteRenderer sr;
    Animator ani;
    Vector2 dir;
    Vector3 myTransform;

    bool dead = false;

    public float speed = 0.8f; // how fast child moves

    public bool HasMoved { get { if (inputList.Count > 0) return true; return false; } }
    

	// Use this for initialization
	public void Init (bool IsGhost, Vector3 Start, DeskControl Desk)
    {
        box = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

        start = Start;
        desk = Desk;
        Reset(IsGhost);
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Run(MyInput.GetInput());
	}

    public void Run()
    {
        if (isGhost) RunGhost();
        else RunChild();
    }

    void RunChild()
    {
        InputType input;
        if(HasMoved)
        {
            input = MyInput.GetInput();
            inputList.Add(input);
            HandleMove(input);
        }
        else
        {
            // Only record after pressing a directional key
            input = MyInput.GetInputDown();
            if (input != InputType.None)
            {
                inputList.Add(input);
                HandleMove(input);
            }
        }      
    }

    void RunGhost()
    {
        if(inputIndex < inputList.Count)
        {
            HandleMove(inputList[inputIndex]);
            inputIndex++;
        }
        else
        {
            SitAtDesk();
        }
    }

    public void HandleMove(InputType input)
    {
        myTransform = transform.position;

        Vector3 delta = Vector3.zero;

        if (input == InputType.Left) dir = new Vector2(-1, 0);
        else if (input == InputType.Right) dir = new Vector2(1, 0);

        else if (input == InputType.Down) dir = new Vector2(0, -1);
        else if (input == InputType.Up) dir = new Vector2(0, 1);
        else
        { ani.SetBool("moving", false); return; }


        delta.x = dir.x * Time.deltaTime * speed;
        delta.y = dir.y * Time.deltaTime * speed;

        ani.SetBool("moving", true);

        if (delta.x != 0) HorizontalCollision(ref delta);
        if (delta.y != 0) VerticalCollsion(ref delta);

        myTransform.x += delta.x;
        myTransform.y += delta.y;

        transform.position = myTransform;

        ani.SetFloat("moveX", dir.x);
        ani.SetFloat("moveY", dir.y);
        

    }


    bool VerticalCollsion(ref Vector3 delta)
    {
        
        

        // Cast 4 rays, 2 inner, 2 outer. Use 2 outer to allow going around corners

        float x = myTransform.x;
        float y = myTransform.y + delta.y;

        int dir = 0;
        if (delta.y > 0) dir = 1;
        else if (delta.y < 0) dir = -1;

        RaycastHit2D hitL;
        RaycastHit2D hitR;
        RaycastHit2D hitM;

        hitM = Physics2D.Raycast(new Vector2(x, y), Vector2.up * dir, 0.07f, collisionMask);

        if (hitM.collider != null)
        {
            delta.y = hitM.point.y - myTransform.y - dir * 0.07f;
            return true;
        }


        // Inner - if both casts hit, snap to wall
        hitL = Physics2D.Raycast(new Vector2(x - 0.065f, y), Vector2.up * dir, 0.07f, collisionMask);
        hitR = Physics2D.Raycast(new Vector2(x + 0.065f, y), Vector2.up * dir, 0.07f, collisionMask);

        if (hitL.collider != null && hitR.collider != null)
        {
            delta.y = hitL.point.y - myTransform.y - dir * 0.07f;
            return true;
        }

        // Only slide if the gap between is large enough
        RaycastHit2D left = Physics2D.Raycast(new Vector2(x, y + (dir * 0.07f)), Vector2.left, 0.07f, collisionMask);
        RaycastHit2D right = Physics2D.Raycast(new Vector2(x, y + (dir * 0.07f)), Vector2.right, 0.07f, collisionMask);

        // If both hit then there is 2 colliders, check gap size, otherwise allow sliding
        bool canSlide = true;
        if (left.collider != null && right.collider != null)
        {
            float d = left.point.x - right.point.x;
            if (d <= 0.14f) canSlide = false;
            else canSlide = true;
        }

        // If only 1 outer rays hit, slide 
        if (hitL.collider != null)
        {
            delta.y = hitL.point.y - myTransform.y - dir * 0.07f;



            if (canSlide)
            {
                // slide down
                RaycastHit2D slide1 = Physics2D.Raycast(new Vector2(x, y + 0.02f), Vector2.right, 0.08f, collisionMask);
                RaycastHit2D slide2 = Physics2D.Raycast(new Vector2(x, y - 0.02f), Vector2.right, 0.08f, collisionMask);
                if (slide1.collider == null && slide2.collider == null)
                    myTransform.x += 0.01f;
            }
            return false;
        }
        if (hitR.collider != null)
        {
            delta.y = hitR.point.y - myTransform.y - dir * 0.07f;
            if (canSlide)
            {
                // slide down
                RaycastHit2D slide1 = Physics2D.Raycast(new Vector2(x, y + 0.02f), Vector2.left, 0.08f, collisionMask);
                RaycastHit2D slide2 = Physics2D.Raycast(new Vector2(x, y - 0.02f), Vector2.left, 0.08f, collisionMask);
                if (slide1.collider == null && slide2.collider == null)
                    myTransform.x -= 0.01f;
            }
            return false;
        }

        // else no hits, allow movement
        return false;
    }
    

    bool HorizontalCollision(ref Vector3 delta)
    {
        
        // Cast 4 rays, 2 inner, 2 outer. Use 2 outer to allow going around corners

        float x = myTransform.x + delta.x;
        float y = myTransform.y;

        int dir = 0;
        if (delta.x > 0) dir = 1;
        else if (delta.x < 0) dir = -1;
        
        RaycastHit2D hitU;
        RaycastHit2D hitD;
        RaycastHit2D hitM;

        hitM = Physics2D.Raycast(new Vector2(x, y), Vector2.right * dir, 0.07f, collisionMask);

        if (hitM.collider != null)
        {
            //if(useCape && hitM[c].transform.par) 
            delta.x = hitM.point.x - myTransform.x - dir * 0.07f;
            return true;
        }

        // Outer - if both casts hit, snap to wall
        hitU = Physics2D.Raycast(new Vector2(x, y + 0.065f), Vector2.right * dir, 0.07f, collisionMask);
        hitD = Physics2D.Raycast(new Vector2(x, y - 0.065f), Vector2.right * dir, 0.07f, collisionMask);

        if (hitU.collider != null && hitD.collider != null)
        {
            //delta.x = hitU[u].point.x - myTransform.x - dir * 0.07f;
            myTransform.x = hitU.point.x - dir * 0.07f;
            delta.x = 0;
            return true;
        }

        // Only slide if the gap between is large enough
        RaycastHit2D up = Physics2D.Raycast(new Vector2(x + (dir * 0.07f), y), Vector2.up, 0.07f, collisionMask);
        RaycastHit2D down = Physics2D.Raycast(new Vector2(x + (dir * 0.07f), y), Vector2.down, 0.07f, collisionMask);

        // If both hit then there is 2 colliders, check gap size, otherwise allow sliding
        bool canSlide = true;
        if (up.collider != null && down.collider != null)
        {
            float d = down.point.y - up.point.y;
            if (d <= 0.14f) canSlide = false;
            else canSlide = true;
        }

        // If only 1 outer rays hit, slide - check directly adj to player too.

        if (hitU.collider != null)
        {
            delta.x = hitU.point.x - myTransform.x - dir * 0.07f;
            if (canSlide)
            {
                // slide down
                RaycastHit2D slide1 = Physics2D.Raycast(new Vector2(x + 0.02f, y), Vector2.down, 0.08f, collisionMask);
                RaycastHit2D slide2 = Physics2D.Raycast(new Vector2(x - 0.02f, y), Vector2.down, 0.08f, collisionMask);
                if (slide1.collider == null && slide2.collider == null)
                    myTransform.y -= 0.01f;

            }
            return false;
        }

        if (hitD.collider != null)
        {
            delta.x = hitD.point.x - myTransform.x - dir * 0.07f;
            // RaycastHit2D slide = Ray
            if (canSlide)
            {
                if (canSlide)
                {
                    // slide down
                    RaycastHit2D slide1 = Physics2D.Raycast(new Vector2(x + 0.02f, y), Vector2.up, 0.08f, collisionMask);
                    RaycastHit2D slide2 = Physics2D.Raycast(new Vector2(x - 0.02f, y), Vector2.up, 0.08f, collisionMask);
                    if (slide1.collider == null && slide2.collider == null)
                        myTransform.y += 0.01f;

                }
            }
            return false;
        }

        // else no hits, allow movement
        return false;
    }

    public bool HitGoal()
    {
        float x = myTransform.x - 0.07f;
        float y;

        RaycastHit2D hit;

        for(int i = -1; i < 2; i++)
        {
            y = myTransform.y + (0.07f * i);

            hit = Physics2D.Raycast(new Vector2(x, y), Vector2.right, 0.14f, goalMask);
            if (hit.collider != null && hit.collider.gameObject == desk.Goal)
            {
                SitAtDesk();
                desk.DisableGoal();
                return true;
            }
        }
        return false;
    }

    public bool HitGhost()
    {
        float x = myTransform.x - 0.07f;
        float y;

        RaycastHit2D hit;

        for (int i = -1; i < 2; i++)
        {
            y = myTransform.y + (0.07f * i);

            hit = Physics2D.Raycast(new Vector2(x, y), Vector2.right, 0.14f, ghostMask);
            if (hit.collider != null)
            {
                dead = true;
                ani.SetBool("dead", true);
                return true;
            }
        }
        return false;
    }

    public void Reset(bool IsGhost)
    {
        dead = false;
        ani.SetBool("desk", false);
        ani.SetBool("dead", false);

        inputIndex = 0;
        if (IsGhost)
        {
            gameObject.layer = LayerMask.NameToLayer("Ghost") ;
            isGhost = true;
            sr.color = new Color(1, 1, 1, 0.5f);
            desk.DisableGoal();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Child");
            isGhost = false;
            sr.color = new Color(1, 1, 1, 1);
            inputList.Clear();
            desk.EnableGoal();
        }

        myTransform = start;
        myTransform.z = -2;
        transform.position = myTransform;

        dir = new Vector2(0, -1);
        ani.SetBool("moving", false);
        ani.SetFloat("moveX", dir.x);
        ani.SetFloat("moveY", dir.y);

    }

    void SitAtDesk()
    {
        ani.SetBool("desk", true);
        myTransform = desk.transform.position;
        myTransform.z = -2;
        transform.position = myTransform;

        if (desk.direction == Direction.Left) dir = new Vector2(-1, 0);
        else if (desk.direction == Direction.Right) dir = new Vector2(1, 0);
        else if (desk.direction == Direction.Up) dir = new Vector2(0, 1);
        else if (desk.direction == Direction.Down) dir = new Vector2(0, -1);

        ani.SetBool("moving", false);
        ani.SetFloat("moveX", dir.x);
        ani.SetFloat("moveY", dir.y);
    }
}
