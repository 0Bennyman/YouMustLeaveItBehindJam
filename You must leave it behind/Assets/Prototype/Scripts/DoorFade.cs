using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFade : MonoBehaviour
{
    public float dist,dist2;

    public float minAlpha,alphaSpeed;

    private float alpha =1;

    public GameObject spirit;

    private MeshRenderer mesh;

    private Color myColor;

    private void Start()
    {
        mesh = gameObject.GetComponent<MeshRenderer>();
        myColor = mesh.material.color;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, spirit.transform.position) > 100)
        {
            alpha = 1;
            if (!mesh.enabled)
            {
                mesh.enabled = true;
            }
        }


        if (Vector3.Distance(transform.position, spirit.transform.position) < dist)
        {
            if (alpha > minAlpha)
            {
                alpha -= alphaSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (alpha < 1)
            {
                alpha += alphaSpeed*Time.deltaTime;
            }
        }

        if (Vector3.Distance(transform.position, spirit.transform.position) < dist2)
        {
            if (alpha > 0)
            {
                alpha -= alphaSpeed * Time.deltaTime;
            }
            else
            {
                mesh.enabled = false;
            }
        }else if (alpha < minAlpha)
        {
            alpha += alphaSpeed * Time.deltaTime;
            mesh.enabled = true;
        }


        mesh.material.color = new Color(myColor.r, myColor.g, myColor.b, alpha);

    }


}
