using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : CollisionObject
{
    public float gravityModifier = 1f;
    protected bool gravityOn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetVelocity = Vector2.zero;
        ComputeMovement();
        if(gravityOn)
        {
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        }
        velocity.x = targetVelocity.x;

        grounded = false;
        hit = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 move = Vector2.right * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement (move, true);
    }
    
    protected virtual void ComputeMovement()
    {
        
    }
}
