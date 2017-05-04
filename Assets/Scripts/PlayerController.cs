using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private enum JumpStatus
    {
        grounded,
        jumping,
        falling,
        walled
    }

    [SerializeField]
    private JumpStatus jumpStatus;

    //Movement Variables
    #region
    [SerializeField]
    private float walkSpeed = 5f,
        runSpeed = 7.5f,
        movementSmoothing = 0.05f,
        jumpForce = 400f,
        wallJumpForce = 300f,
        dashSpeed = 50f,
        deadZone = 2;

    private Vector3 movementVector;
    private Vector3 wallNormalVector;
    #endregion


    //Component Variables
    #region
    private Rigidbody playerRB;
    #endregion

    void Start ()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        if (playerRB.velocity.y < -deadZone && jumpStatus != JumpStatus.walled)
            jumpStatus = JumpStatus.falling;
        else if (playerRB.velocity.y > deadZone)
            jumpStatus = JumpStatus.jumping;
    }

    void FixedUpdate ()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            PlayerMovement(runSpeed);
        else
            PlayerMovement(walkSpeed);

        //JumpStatus Switch
        #region
        switch (jumpStatus)
        {
            case JumpStatus.grounded:
                //play jump animation
                if (Input.GetKeyDown(KeyCode.Space))
                    playerRB.AddForce(Vector3.up * jumpForce);
                break;
            case JumpStatus.jumping:
                //play jumping animation
                break;
            case JumpStatus.falling:
                //Play fall animation
                break;
            case JumpStatus.walled:
                //Play wall jump animation
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerRB.AddForce((Vector3.up + wallNormalVector) * wallJumpForce);
                }
                break;
        }
        #endregion
    }

    void PlayerMovement (float moveSpeed)
    {
        //Old Move Style
        //movementVector = (transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime);

        //New Move Style
        Vector3 moveDir = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDir * moveSpeed * Time.fixedDeltaTime;
        Vector3 currentVelocity = playerRB.velocity;
        movementVector = Vector3.SmoothDamp(movementVector, targetMoveAmount, ref currentVelocity, movementSmoothing);
        playerRB.MovePosition(transform.position + movementVector);
    }

    void PlayerDash ()
    {
        //Make a Dash
    }

    void OnCollisionEnter (Collision col)
    {
        if (col.collider.CompareTag("Wall") && jumpStatus != JumpStatus.grounded)
        {
            jumpStatus = JumpStatus.walled;
            wallNormalVector = col.contacts[0].normal;
        }
        
        else if (col.collider.CompareTag("Platform"))
            jumpStatus = JumpStatus.grounded;
    }
}