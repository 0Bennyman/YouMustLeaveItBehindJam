using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFlipping : MonoBehaviour
{

    public GameObject flipPoint,Player;

    public GameObject[] legs;

    private float rotatePoint,rotateMax;

    public float dist;

    public GameObject point1, point2;

    [HideInInspector]
    public bool lookedAt;

    private bool flip1,flipped;

    private Outline outL;

    private void Start()
    {
        outL = gameObject.GetComponent<Outline>();
        DisplayHighlight();
    }

    public void DisplayHighlight()
    {
        if (lookedAt)
        {
            foreach (GameObject l in legs)
            {
                l.GetComponent<Outline>().enabled = true;
            }
            outL.enabled = true;
            StopCoroutine("RemoveHighlight");
            StartCoroutine("RemoveHighlight");
        }
        else
        {
            foreach (GameObject l in legs)
            {
                l.GetComponent<Outline>().enabled = false;
            }
            outL.enabled = false;
        }
    }

    public void FlipTable() //Different Spirit and Player versions?
    {
        if (flipped)
        {
            return;
        }

        flipped = true;

        if (Vector3.Distance(point1.transform.position, Player.transform.position) < Vector3.Distance(point2.transform.position,Player.transform.position))
        {
            StartCoroutine("FlippingTable", point2.transform.position);
        }
        else
        {

            StartCoroutine("FlippingTable", point1.transform.position);
            flip1 = true;
        }

    }


    IEnumerator RemoveHighlight()
    {
        yield return new WaitForSeconds(.5f);
        lookedAt = false;
        DisplayHighlight();
    }


    IEnumerator FlippingTable(Vector3 point)
    {
        yield return new WaitForSeconds(.03f);

        if (Vector3.Distance(transform.position, point) > .1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, point,12*Time.deltaTime);

            if (flip1)
            {
                if (transform.localEulerAngles.x < 90)
                {
                    transform.Rotate(30, 0, 0);
                }

            }
            else
            {
                float angle = transform.localEulerAngles.x;
                angle = (angle > 180) ? angle - 360 : angle;

                if (angle > -89)
                {
                    transform.Rotate(-30, 0, 0);
                }
            }



            StartCoroutine("FlippingTable", point);
        }
        else
        {
            if (flip1)
            {
                if (transform.localEulerAngles.x < 90)
                {
                    float difference = 90 - transform.localEulerAngles.x;
                    transform.Rotate(difference, 0, 0);
                }
            }
            else
            {
                float angle = transform.localEulerAngles.x;
                angle = (angle > 180) ? angle - 360 : angle;

                if (angle > -90)
                {
                    float difference = -90 + angle;
                    transform.Rotate(difference, 0, 0);
                }
            }


            foreach (GameObject l in legs)
            {
                Destroy(l.GetComponent<Outline>());
            }

            Destroy(outL);
            Destroy(this);
        }

    }


}
