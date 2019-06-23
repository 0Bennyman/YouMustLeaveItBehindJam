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

    public GameObject bulletHole;

    private ScriptableWeapons weaponStats;

    public Transform target;
    public Transform pivot;

    private Vector3 shootPos;

    private bool doubleJump,shouldCrouch,hasGun;

    private float crouchCur;

    public float crouchMin;

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

    private PlayerLook playLook;

    private float curFireRate,curAmmo;

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
        playLook = myCamera.GetComponent<PlayerLook>();
        //normalCameraPos = myCamera.transform.position;

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
        if (!hasGun || lockMovement)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Mouse1) && weaponStats.Scope)
        {
            sniperScoped.SetActive(!sniperScoped.activeSelf);
            sniperNonScoped.SetActive(!sniperNonScoped.activeSelf);
        }


        if (curFireRate > 0)
        {
            curFireRate -= weaponStats.fireRate*Time.deltaTime;
            return;
        }

        if (curAmmo <= 0)
        {
            //Reload
            curAmmo = weaponStats.maxAmmo;
            curFireRate = 2;
        }

        Ray ray = myCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playLook.Recoil(weaponStats.recoilAmount);

            curFireRate = 1;
            curAmmo -= 1;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.GetComponent<PlayerMove>().curHealth -= weaponStats.Damage;
                    
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

                    //Now we gott re-send a ray through this glass
                    shootPos = myCamera.transform.position;
                    StartCoroutine("SendRay",hit.point);

                }

                if (hit.transform.tag == "GlassPiece")
                {
                    expForce = 0.09f;
                    StartCoroutine("DelayExplosion", hit.point);
                }

                if (hit.transform.tag == "Wall")
                {
                    GameObject bullet = Instantiate(bulletHole, hit.point,hit.transform.rotation);
                    Destroy(bullet, 30);
                }


            }



        }




    }

    public void PickupGun(GameObject gun)
    {
        weaponStats = gun.GetComponent<WeaponStats>().weapon;
        hasGun = true;
        GameObject scope = Instantiate(weaponStats.prefabModelScoped, transform.position, transform.rotation, myCamera.transform);
        GameObject nonScope = Instantiate(weaponStats.prefabModel, transform.position, transform.rotation, myCamera.transform);
        scope.transform.position = sniperScoped.transform.position;
        scope.transform.rotation = sniperScoped.transform.rotation;
        sniperScoped = scope;
        nonScope.transform.position = sniperNonScoped.transform.position;
        nonScope.transform.rotation = sniperNonScoped.transform.rotation;
        sniperNonScoped = nonScope;
        //sniperScoped = weaponStats.prefabModelScoped;
        Destroy(gun);
    }

    void CheckCrouch()
    {
        normalCameraPos = transform.position;


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            shouldCrouch = !shouldCrouch;
            crouchCur = 0;

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
            switchSpirit.Spirit.GetComponent<BoxCollider>().enabled = false;
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
            crouchCur -= 2 * Time.deltaTime;

        }
        else
        {
            crouchCur += 1 * Time.deltaTime;
        }

        print(shouldCrouch);

        myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, new Vector3(myCamera.transform.position.x, normalCameraPos.y+crouchCur, myCamera.transform.position.z), 8 * Time.deltaTime);



        if (shouldCrouch && crouchCur <= crouchMin)
        {
            myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, new Vector3(myCamera.transform.position.x, normalCameraPos.y + crouchMin, myCamera.transform.position.z), 8 * Time.deltaTime);
        }
        else if (!shouldCrouch && crouchCur >= .1)
        {
            myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, new Vector3(myCamera.transform.position.x, normalCameraPos.y, myCamera.transform.position.z), 8 * Time.deltaTime);
        }
        else
        {
            StartCoroutine("Crouching");
        }


    }

    IEnumerator SendRay(Vector3 point)
    {
        yield return new WaitForSeconds(.1f);

         if (Physics.Raycast(shootPos,(point - shootPos), out hit, Mathf.Infinity))
         {
            print("Resending Ray");
            print(hit.transform.gameObject.name);

            if (hit.transform.tag == "GlassPiece")
            {
                print(hit.transform.gameObject.name);
                Destroy(hit.transform.gameObject);
                StartCoroutine("SendRay", point);
            }
            else
            {
                if (hit.transform.tag == "Enemy")
                {
                    print(hit.transform.gameObject.name);
                    hit.transform.GetComponent<PlayerMove>().curHealth -= weaponStats.Damage;

                    if (!isPlayer)
                    {
                        animRec.personDamage = hit.transform.gameObject;
                    }

                }
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