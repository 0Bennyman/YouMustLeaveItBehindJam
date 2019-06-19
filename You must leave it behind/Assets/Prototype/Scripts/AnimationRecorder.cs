using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{

    public bool ShouldRecord,shouldPlay;

    public Vector3[] pos;

    private int curPoint,speed;

    public void StartRecording()
    {
        curPoint = 0;
        pos = new Vector3[250];
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


    IEnumerator Recording()
    {
        yield return new WaitForSeconds(0.1f);

        if (ShouldRecord)
        {
            //Make sure to track curSpeed,rotation etc so we can get an accurate recording
            pos[curPoint] = gameObject.transform.position;
            curPoint++;
            StartCoroutine("Recording");
        }
        else
        {
            //Add final Record here
            pos[curPoint] = gameObject.transform.position;
        }

    }

    IEnumerator PlayingRecording()
    {
        yield return new WaitForSeconds(.1f);

        if (pos.Length > 0)
        {
            if (pos[curPoint] != new Vector3(0, 0, 0))
            {
                curPoint+=1;
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
                transform.position = Vector3.MoveTowards(transform.position, pos[curPoint], 5 * Time.deltaTime);
                StartCoroutine("ContinuePlaying");
            }
        }


    }


}
