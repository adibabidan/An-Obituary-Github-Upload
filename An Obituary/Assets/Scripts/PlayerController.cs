using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GravityObject
{
    public float walkSpeed;
    public float jumpLaunchSpeed;
    public float jumpReleaseMultiplier; // Should never be larger than 1
    public float wallCheckDistance;
    public float leapLaunchSpeed;
    public float leapSpeed;
    public float sprintSpeed;
    public float leapDecel;
    public float maxLeapAdjustSpeed;
    public float timeToDieSun;
    public float timeToDieSludge;
    public float distanceToDieFalling;
    
    public Transform wallCheck;
    public Transform ledgeCheck;

    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;

    public LayerMask whatIsGround;

    public SceneTransitionData spawnPositionData;

    public bool ignoreSceneTransition { get; protected set;}

    public GameObject smokeParticles;

    protected Vector2 move;
    protected bool isTouchingWall;
    protected bool isTouchingLedge;
    protected bool isClimbing;
    protected bool ledgeDetected;
    protected bool startJump;
    protected bool stopJump;
    protected bool isFacingRight;
    protected bool isWalking;
    protected bool isSprinting;
    protected bool isLeaping;
    protected float tempVelocity;
    protected bool isInSmoke;
    protected bool isInSun;
    protected bool coveredInSludge;
    protected bool forceMove;
    protected float forceMoveInput;
    protected float timeInSun;
    protected int sunCount;
    protected int smokeCount;
    protected bool falling;
    protected bool respawnBuffered;
    protected float fallStart;
    protected bool isDying;

    protected Vector2 respawnPoint;
    protected Vector2 bufferedRespawnPoint;

    protected Animator anim;
    protected Vector2 ledgePosBot;
    protected Vector2 ledgePos1;
    protected Vector2 ledgePos2;

    protected GameObject SpawnPositionObject;


    // Start is called before the first frame update
    void Start()
    {
        isClimbing = false;
        startJump = false;
        stopJump = false;
        isFacingRight = true;
        isWalking = false;
        isSprinting = false;
        isLeaping = false;
        forceMove = false;
        timeInSun = 0.0f;
        sunCount = 0;
        smokeCount = 0;
        falling = false;
        respawnBuffered = false;
        isDying = false;
        anim = GetComponent<Animator>();
        SpawnPositionObject = GameObject.Find("Spawn Locations");
        transform.position = SpawnPositionObject.transform.GetChild(spawnPositionData.spawnIndex).gameObject.transform.position;
        if (spawnPositionData.direction != 0)
        {
            StartForceMovement(spawnPositionData.direction);
            TimerManager.Instance.SetTimer(0.4f, StopForceMovement);
        }
        smokeParticles.SetActive(false);
    }

    public void EnterSludge()
    {
        coveredInSludge = true;
        TimerManager.Instance.SetTimer(timeToDieSludge, CheckIfClean); //TODO make this a custom timer so that it doesn't kill you upon reentering sludge before timeToDieSludge has passed
    }

    public void CheckIfClean()
    {
        if (coveredInSludge)
        {
            coveredInSludge = false;
            Die();
        }
    }

    public void EnterWater()
    {
        coveredInSludge = false;
    }

    public void EnterSun()
    {
        Debug.Log(sunCount);
        isInSun = true;
        sunCount += 1;
        smokeParticles.SetActive(true);
        Debug.Log(sunCount);
    }

    public void ExitSun()
    {
        Debug.Log(sunCount);
        sunCount -= 1;
        if (sunCount <= 0)
        {
            isInSun = false;
            smokeParticles.SetActive(false);
            sunCount = 0;
        }
        Debug.Log(sunCount);
    }

    public void EnterSmoke()
    {
        isInSmoke = true;
        smokeCount += 1;
    }

    public void ExitSmoke()
    {
        smokeCount -= 1;
        if (smokeCount <= 0)
        {
            isInSmoke = false;
            sunCount = 0;
        }
    }

    public void FinishLedgeClimb()
    {
        isClimbing = false;
        transform.position = ledgePos2;
        gravityOn = true;
        ledgeDetected = false;
        anim.SetBool("isClimbing", isClimbing);
    }

    public void SetRespawnPoint(Vector2 point)
    {
        bufferedRespawnPoint = point;
        respawnBuffered = true;
    }

    public void Die()
    {
        if(isClimbing)
        {
            FinishLedgeClimb();
        }
        isLeaping = false;
        falling = false;
        isInSun = false;
        isInSmoke = false;
        coveredInSludge = false;
        sunCount = 0;
        smokeCount = 0;
        isDying = true;
        bufferedRespawnPoint = respawnPoint;
    }

    public void FinishDying()
    { 
        transform.position = respawnPoint;
        smokeParticles.SetActive(false);
        isDying = false;
    }

    public void StartForceMovement(float direction)
    {
        forceMove = true;
        ignoreSceneTransition = true;
        forceMoveInput = direction;
    }

    public void StopForceMovement()
    {
        forceMove = false;
        ignoreSceneTransition = false;
    }

    protected override void HandleInputs()
    {
        CheckInputs();
        CheckMovementDirection();
        CheckLedgeClimb();
        UpdateAnimations();
    }

    protected void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking && velocity.x != 0);
        anim.SetBool("isSprinting", isSprinting && velocity.x != 0);
        anim.SetFloat("yVelocity", velocity.y);
        anim.SetBool("isJumping", !grounded && !isLeaping && !isDying);
        anim.SetBool("isLeaping", isLeaping);
        anim.SetBool("isDying", isDying);
    }

    protected void CheckInputs()
    {
        move = Vector2.zero;
        if (isDying)
        {
            isSprinting = false;
            move.x = 0.0f;
        }
        else if (forceMove)
        {
            isSprinting = false;
            move.x = forceMoveInput;
        }
        else
        {
            move.x = InputManager.GetHorizontal();

            if (InputManager.GetSprint() && !isInSmoke)
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }

            if (!isClimbing)
            {
                if (InputManager.GetJumpDown() && grounded)
                {
                    startJump = true;
                }
                if (InputManager.GetJumpUp() && velocity.y > 0 && !isLeaping)
                {
                    stopJump = true;
                }
            }
        }
        
    }

    protected void CheckMovementDirection() {
        if (!isLeaping && !isClimbing)
        {
            if (isFacingRight && move.x < 0)
            {
                FlipCharacter();
            }
            else if (!isFacingRight && move.x > 0)
            {
                FlipCharacter();
            }
        }

        if(move.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    protected void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    protected void CheckLedgeClimb()
    {
        if(ledgeDetected && !isClimbing && !isDying)
        {
            isClimbing = true;
            
            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            transform.position = ledgePos1;

            velocity = Vector2.zero;

            gravityOn = false;

            grounded = true;

            anim.SetBool("isClimbing", isClimbing);
        }
    }

    protected override void ComputeMovement()
    {
        CheckLedge();
        ResolveJump();
        ResolveMovement();
        ResolveStatuses();
        CheckRespawnBuffer();
    }

    protected void CheckRespawnBuffer()
    {
        if (respawnBuffered)
        {
            if (grounded)
            {
                if (!coveredInSludge)
                {
                    respawnPoint = bufferedRespawnPoint;
                }
            }
        }
    }

    protected void ResolveStatuses()
    {
        if (isInSun)
        {
            if (timeInSun < timeToDieSun)
            {
                timeInSun += Time.deltaTime;
            }
            else
            {
                Die();
                timeInSun = 0;
            }
        }
        else
        {
            timeInSun = 0;
        }
        if (falling)
        {
            if(grounded)
            {
                if (fallStart - transform.position.y >= distanceToDieFalling)
                {
                    Die();
                }
                falling = false;
            }
        }
        else if (!grounded)
        {
            falling = true;
            fallStart = Mathf.Round(transform.position.y);
        }
    }

    protected void ResolveJump()
    {
        if (isLeaping && grounded)
        {
            isLeaping = false;
        }
        else if (startJump)
        {
            if(isSprinting && move.x != 0)
            {
                velocity.y = leapLaunchSpeed;
                isLeaping = true;
                startJump = false;
                if(isFacingRight)
                {
                    targetVelocity.x = leapSpeed;
                    tempVelocity = leapSpeed;
                }
                else
                {
                    targetVelocity.x = -leapSpeed;
                    tempVelocity = -leapSpeed;
                }
            }
            else
            {
                velocity.y = jumpLaunchSpeed;
                startJump = false;
            }
        }
        else if (stopJump)
        {
            velocity.y *= jumpReleaseMultiplier;
            stopJump = false;
        }
    }

    protected void ResolveMovement()
    {
        if(!isClimbing)
        {
            if(isLeaping)
            {
                if(tempVelocity > maxLeapAdjustSpeed)
                {
                    if(move.x < 0)
                    {
                        tempVelocity -= leapDecel * Time.deltaTime;
                    }
                }
                else if (tempVelocity < -maxLeapAdjustSpeed)
                {
                    if (move.x > 0)
                    {
                        tempVelocity += leapDecel * Time.deltaTime;
                    }
                }
                else
                {
                    tempVelocity += move.x * leapDecel * Time.deltaTime;
                }
                targetVelocity.x = tempVelocity;
            }
            else if (grounded && isSprinting)
            {
                targetVelocity.x = move.x * sprintSpeed;
            }
            else
            {
                targetVelocity.x = move.x * walkSpeed;
            }

            if (move.x != 0)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }
        }
        else
        {
            isWalking = false;
        }
    }

    protected void CheckLedge()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }
}
