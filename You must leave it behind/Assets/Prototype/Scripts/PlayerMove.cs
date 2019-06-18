using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;

    private float jumpCount=0,maxJumps=2;

    private float rot;

    public float jumpForce;
    public CharacterController controller;
    public GameObject playerModel;

    public GameObject myCamera;

    public Transform target;
    public Transform pivot;

    private bool doubleJump;

    public bool lockMovement = true;
    public bool isPlayer;

    public Vector3 moveDirection;
    public float gravityScale, yAdjust;

    public SpiritSwitch switchSpirit;

    private EnemyAI ai;

    void Start()
    {
        if (isPlayer)
        {
            lockMovement = false;
        }

        ai = gameObject.GetComponent<EnemyAI>();

    }

    void Update()
    {

        CheckButtonPress();
        MovePlayer();
    }


    void CheckButtonPress()
    {
        if (lockMovement)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            lockMovement = true;
            myCamera.SetActive(false);
            switchSpirit.SwitchToSpiritForm(gameObject,myCamera);
            switchSpirit.distSlider.SetActive(true);

            if (ai != null)
            {
                ai.GetUnpossesed();
            }

        }
    }

    void MovePlayer()
    {
        if (!lockMovement)
        {
            float yStore = moveDirection.y;
            moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            moveDirection = moveDirection.normalized * moveSpeed;
            moveDirection.y = yStore;
        }


        if (controller.isGrounded)
        {
            doubleJump = false;
            moveDirection.y = 0f;
            rot = 0;
            jumpCount = 0;
        }

        if (!controller.isGrounded && !lockMovement)
        {
            rot += 10;
        }

        if (!lockMovement)
        {
            if (controller.isGrounded && Input.GetButtonDown("Jump") || !doubleJump && Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
                jumpCount += 1;
                if (!controller.isGrounded && jumpCount >= 2)
                {
                    doubleJump = true;
                }

            }
        }




        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);

        if (!lockMovement)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            controller.Move(moveDirection * Time.deltaTime);
        }



    }
}


public class TimeScale
{
    //default Timescales
    public static float player = 1;
    public static float enemies = 1;
    public static float global = 1;

}