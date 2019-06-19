using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpiritSwitch : MonoBehaviour
{

    public GameObject Player, Spirit,sCamera,pCamera;

    public AnimationRecorder[] allRecorders;

    public SpiritMove moveSpirit;
    private AnimationRecorder animRecord;

    private GameObject curBody, curCamera;

    public Image dot,fade;

    public GameObject distSlider;

    public float speed,timerMax,timerSpeed;

    private float timer;

    private bool curSpirit;



    private void Start()
    {
        animRecord = gameObject.GetComponent<AnimationRecorder>();
        curBody = Player;
        curCamera = pCamera;
    }








    public void SwitchToPlayerBody(GameObject bodyToSwitch)
    {
        if (bodyToSwitch == null)
        {
            bodyToSwitch = Player;
        }

        StopCoroutine("SpiritGoToBody");//Spam protection
        StartCoroutine("SpiritGoToBody", bodyToSwitch);
    }

    public void SwitchToPlayerBodyFinal(GameObject bodyToSwitch)
    {

        if (bodyToSwitch == Player)
        {
            moveSpirit.speed = 0;
            //Spirit.transform.position = new Vector3(999, 999, 999);
            //Spirit.SetActive(false);
            sCamera.SetActive(false);
            moveSpirit.enabled = false;

            PlayerMove playMov = Player.GetComponent<PlayerMove>();
            playMov.enabled = true;
            playMov.lockMovement = false;
            playMov.myCamera.SetActive(true);
            dot.color = Color.white;
            dot.color = new Color(dot.color.r, dot.color.g, dot.color.b, 0.3f);

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);

            curSpirit = false;

            curCamera = pCamera;
            curBody = Player;

            foreach (AnimationRecorder anim in allRecorders)
            {
                anim.ShouldRecord = false;
                anim.shouldPlay = true;
                anim.PlayRecording();
            }

            StopCoroutine("countTimer");

            //animRecord.ShouldRecord = false;
            //animRecord.shouldPlay = true;
            //animRecord.PlayRecording();

            StartCoroutine("FadeIn");

        }
        else
        {
            moveSpirit.speed = 0;
            sCamera.SetActive(false);
            moveSpirit.enabled = false;
            //Spirit.transform.position = new Vector3(999, 999, 999);
            //Spirit.SetActive(false);

            EnemyAI ai = bodyToSwitch.GetComponent<EnemyAI>();
            ai.GetPossesed();

            curSpirit = false;

            curCamera = ai.controlledCamera;
            curBody = bodyToSwitch;

            dot.color = Color.white;
            dot.color = new Color(dot.color.r, dot.color.g, dot.color.b, 0.3f);

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);
            StartCoroutine("FadeIn");

        }

    }

    public void SwitchToSpiritForm(GameObject body,GameObject bodyCamera)
    {
        Spirit.transform.position = body.transform.position;
        sCamera.transform.rotation = bodyCamera.transform.rotation;
        bodyCamera.SetActive(false);
        Spirit.SetActive(true);
        moveSpirit.enabled = true;
        sCamera.SetActive(true);
        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);
        moveSpirit.vulnrable = true;
        curSpirit = true;


        PlayerMove aiP = body.GetComponent<PlayerMove>();
        aiP.enabled = false;


        StartCoroutine("FadeIn");
        
        if (body == Player)
        {
            foreach (AnimationRecorder anim in allRecorders)
            {
                anim.ShouldRecord = true;
                anim.shouldPlay = false;
                anim.StartRecording();
            }


            timer = 0;
            StartCoroutine("countTimer");
        }
    }

    public void PossessEnemy()
    {
        //Maybe not nessesary if we just do it in 1 void
    }



    IEnumerator SpiritGoToBody(GameObject body)
    {
        yield return new WaitForSeconds(0.01f);

        Spirit.transform.position = Vector3.MoveTowards(Spirit.transform.position, body.transform.position,speed*Time.deltaTime);
        sCamera.transform.LookAt(body.transform);

        if (Vector3.Distance(Spirit.transform.position, body.transform.position) > .5f)
        {
            moveSpirit.speed = 0;
            StartCoroutine("SpiritGoToBody", body);
        }
        else
        {
            SwitchToPlayerBodyFinal(body);
        }

    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(.01f);

        if (fade.color.a > 0)
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, fade.color.a - .8f * Time.deltaTime);
            StartCoroutine("FadeIn");
        }

    }

    IEnumerator countTimer()
    {
        yield return new WaitForSeconds(.01f);

        timer += timerSpeed * Time.deltaTime;

        if (timer > timerMax)
        {
            if (!curSpirit)
            {
                SwitchToSpiritForm(curBody, curCamera);
            }

            StartCoroutine("SpiritGoToBody", Player);
        }
        else
        {
            StartCoroutine("countTimer");
        }

    }

    IEnumerator delayedBackToPlayer()
    {
        yield return new WaitForSeconds(.2f);

        StartCoroutine("SpiritGoToBody", Player);
    }

}
