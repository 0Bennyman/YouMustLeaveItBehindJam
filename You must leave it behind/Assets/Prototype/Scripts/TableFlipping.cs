using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFlipping : MonoBehaviour
{

    public GameObject flipPoint;

    public GameObject[] legs;

    private float rotatePoint,rotateMax;

    [HideInInspector]
    public bool lookedAt;

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

    public void FlipTable()
    {
        StartCoroutine("FlippingTable",flipPoint.transform.position);
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
            if (transform.localEulerAngles.x < 90)
            {
                transform.Rotate(30, 0, 0);
            }

            StartCoroutine("FlippingTable", point);
        }
        else
        {
            if (transform.localEulerAngles.x < 90)
            {
                float difference = 90 - transform.localEulerAngles.x;
                transform.Rotate(difference, 0, 0);
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
