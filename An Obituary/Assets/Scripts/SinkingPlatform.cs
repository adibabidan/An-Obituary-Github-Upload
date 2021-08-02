using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkingPlatform : CollisionObject
{
    public float maxSinkingDistance;
    public float sinkingRate;

    protected float startHeight;
    protected bool sinking;
    protected PlayerController playerController;
    protected float move;

    // Start is called before the first frame update
    void Start()
    {
        startHeight = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(sinking == true)
        {
            if (transform.position.y > startHeight - maxSinkingDistance)
            {
                move = -sinkingRate * Time.deltaTime;
                transform.position = new Vector2(transform.position.x, transform.position.y + move);
                playerController.Movement(Vector2.up * move, true);
                if (transform.position.y <= startHeight - maxSinkingDistance)
                {
                    transform.position = new Vector2(transform.position.x, startHeight - maxSinkingDistance);
                }
            }
        }
        else
        {
            if (transform.position.y < startHeight)
            {
                move = sinkingRate * Time.deltaTime;
                transform.position = new Vector2(transform.position.x, transform.position.y + move);
                if (transform.position.y >= startHeight)
                {
                    transform.position = new Vector2(transform.position.x, startHeight);
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            playerController = other.GetComponent<PlayerController>();
            sinking = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            sinking = false;
        }
    }
}
