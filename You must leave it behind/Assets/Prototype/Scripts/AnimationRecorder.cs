using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{

    public bool ShouldRecord,shouldPlay,amSpirit,isButton;

    public Vector3[] pos;
    public Quaternion[] myRot;
    public float[] spd;

    public bool[] objActive;

    public GameObject additionalObject;

    public int maxRecords = 300; //Just realised we would have to do this manually ooof

    private int curPoint;
    private float speed,speedIncrease,speedDecrease;

    public void StartRecording()
    {
        curPoint = 0;
        pos = new Vector3[250];
        myRot = new Quaternion[pos.Length];
        spd = new float[pos.Length];
        objActive = new bool[pos.Length];

        StartCoroutine("Recording");
    }

    public void PlayRecording()
    {
        curPoint = 0;
        if (pos.Length > 0)
        {
            transform.position = pos[0];
        }
        StartCoroutine("PlayingRecording");
        StartCoroutine("ContinuePlaying");
    }


    public float GetSpeed(float temp)
    {

        if (amSpirit)
        {
            //We get SpiritMove
            temp = gameObject.GetComponent<SpiritMove>().speed;
            speedIncrease = gameObject.GetComponent<SpiritMove>().speedIncrease;
            
        }
        else
        {
            //We get PlayerMove
            if (!isButton)
            {
                temp = gameObject.GetComponent<PlayerMove>().moveSpeed;
                speedIncrease = gameObject.GetComponent<PlayerMove>().moveSpeed;
            }
            else
            {
                temp = 0;
                speedIncrease = 0;
            }

        }

        return temp;

    }




    IEnumerator Recording()
    {
        yield return new WaitForSeconds(0.1f);

        if (ShouldRecord)
        {
            //Make sure to track curSpeed,rotation etc so we can get an accurate recording
            pos[curPoint] = gameObject.transform.position;
            spd[curPoint] = GetSpeed(speed);
            myRot[curPoint] = gameObject.transform.rotation;

            if (isButton)
            {
                objActive[curPoint] = additionalObject.activeSelf;
            }


            curPoint++;
            StartCoroutine("Recording");
        }
        else
        {
            //Add final Record here
            pos[curPoint] = gameObject.transform.position;
            spd[curPoint] = GetSpeed(speed);
            myRot[curPoint] = gameObject.transform.rotation;

            if (isButton)
            {
                objActive[curPoint] = additionalObject.activeSelf;
            }

        }

    }

    IEnumerator PlayingRecording()
    {
        yield return new WaitForSeconds(.1f);

        if (pos.Length > 0)
        {
            if (pos[curPoint] != new Vector3(0, 0, 0))
            {
                curPoint += 1;
                StartCoroutine("PlayingRecording");
            }
            else
            {
                shouldPlay = false;
            }
        }



    }

    IEnumerator ContinuePlaying()
    {
        yield return new WaitForSeconds(.01f);

        if (pos.Length > 0)
        {
            if (shouldPlay && pos[curPoint] != new Vector3(0,0,0))
            {
                transform.position = Vector3.MoveTowards(transform.position, pos[curPoint], speed * Time.deltaTime);
                transform.rotation = myRot[curPoint];

                if (isButton)
                {
                    additionalObject.SetActive(objActive[curPoint]);
                }

                StartCoroutine("ContinuePlaying");
            }

            if (shouldPlay && speed < spd[curPoint] && pos[curPoint] != new Vector3(0, 0, 0))
            {
                speed += speedIncrease * Time.deltaTime;
            }

        }




    }


}
