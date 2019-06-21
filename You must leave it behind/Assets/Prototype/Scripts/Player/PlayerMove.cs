using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;

    public int maxHealth, curHealth;


    private float jumpCount=0,maxJumps=2,expForce;



    private float rot;

    public float jumpForce;
    public CharacterController controller;
    public GameObject playerModel,crouchCamera;

    public BoxCollider flashLightBox;

    public GameObject myCamera,gunModel,startingWeapon;

    public GameObject sniperNonScoped, sniperScoped;

    private ScriptableWeapons weaponStats;

    public Transform target;
    public Transform pivot;

    private bool doubleJump,shouldCrouch,hasGun;

    public bool lockMovement = true;
    public bool isPlayer,hasFlashLight;

    public Vector3 moveDirection;
    public float gravityScale, yAdjust;

    private Vector3 crouchCameraPos,normalCameraPos;

    public SpiritSwitch switchSpirit;

    private EnemyAI ai;
    private AnimationRecorder animRec;

    public GameObject eye1, eye2;

    private RaycastHit hit;

    void Start()
    {
        if (startingWeapon!=null)
        {
            PickupGun(startingWeapon);
        }

        if (isPlayer)
        {
            lockMovement = false;
        }

        ai = gameObject.GetComponent<EnemyAI>();
        animRec = gameObject.GetComponent<AnimationRecorder>();

        curHealth = maxHealth;

    }

    void Update()
    {
        FireGun();
        PressButton();
        CheckButtonPress();
        CheckCrouch();
        MovePlayer();
    }



    public void FireGun()
    {
        if (!hasGun)
        {
            return;
        }

        Ray ray = myCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.GetComponent<PlayerMove>().curHealth -= 1;
                    if (!isPlayer)
                    {
                        animRec.personDamage = hit.transform.gameObject;
                    }

                }

                if (hit.transform.tag == "Glass")
                {
                    Vector3 pos = hit.point;
                    StartCoroutine("DelayExplosion", pos);
                    expForce = 0.34f;
                    hit.transform.GetComponent<WallDestruction>().ohfuckimhit();
                }

                if (hit.transform.tag == "GlassPiece")
                {
                    expForce = 0.09f;
                    StartCoroutine("DelayExplosion", hit.point);
                }


            }



        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && weaponStats.Scope)
        {
            sniperScoped.SetActive(!sniperScoped.activeSelf);
            sniperNonScoped.SetActive(!sniperNonScoped.activeSelf);
        }



    }

    public void PickupGun(GameObject gun)
    {
        weaponStats = gun.GetComponent<WeaponStats>().weapon;
        hasGun = true;
        Destroy(gun);
    }

    void CheckCrouch()
    {

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            shouldCrouch = !shouldCrouch;

            //Temporary! Should be an animation with the player model!

            crouchCameraPos = crouchCamera.transform.position;
            normalCameraPos = transform.position;

            StopCoroutine("Crouching");
            StartCoroutine("Crouching");
        }


    }





    void PressButton()
    {
        if (lockMovement)
        {
            return;
        }

        Ray ray = myCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {

            if (hit.transform.tag == "Button")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.transform.gameObject.GetComponent<ButtonScript>().ActivateButton();
                    if (!isPlayer)
                    {
                        animRec.buttonPressed = hit.transform.gameObject;
                    }

                }

                hit.transform.gameObject.GetComponent<ButtonScript>().DisplayText();

            }

            if (hit.transform.tag == "Table")
            {
                TableFlipping table;
                table = hit.transform.GetComponent<TableFlipping>();

                if (table == null)
                {
                    return;
                }

                table.lookedAt = true;
                table.DisplayHighlight();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    table.FlipTable();
                }
            }


        }

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

            if (hasFlashLight && flashLightBox.enabled)
            {
                flashLightBox.enabled = false;
            }

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


    IEnumerator Crouching()
    {
        yield return new WaitForSeconds(.01f);
        if (shouldCrouch)
        {
            if (Vector3.Distance(myCamera.transform.position, crouchCameraPos) > .1f)
            {
                myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, new Vector3(myCamera.transform.position.x,crouchCameraPos.y, myCamera.transform.position.z), 8 * Time.deltaTime);
                StartCoroutine("Crouching");
            }
        }
        else
        {
            if (Vector3.Distance(myCamera.transform.position, normalCameraPos) > .1f)
            {
                myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, new Vector3(myCamera.transform.position.x, normalCameraPos.y+.1f, myCamera.transform.position.z), 8 * Time.deltaTime);
                StartCoroutine("Crouching");
            }
        }

    }



    IEnumerator DelayExplosion(Vector3 pos)
    {
        yield return new WaitForSeconds(.1f);

        Vector3 explosionPos = pos;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, expForce);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                //rb.AddRelativeForce(Vector3.forward * 800);
                rb.AddExplosionForce(10000, explosionPos, 500, 0f);

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