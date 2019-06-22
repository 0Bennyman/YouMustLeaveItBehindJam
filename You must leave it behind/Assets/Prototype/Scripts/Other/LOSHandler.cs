using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSHandler : MonoBehaviour
{

    public BoxCollider box;

    public bool lightSource, enemyLOS;

    private bool isSpirit, isPlayer,reCheck;

    public float alertIncrease,alertMax;

    private float alertTimer;

    public GameObject myHost;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Spirit" && lightSource && box.enabled && !reCheck)
        {
            box.enabled = false;
            isSpirit = true;
            StopCoroutine("checkLOS"); //Just incase
            StartCoroutine("checkLOS",other.gameObject);
            StartCoroutine("boxReturn",other.gameObject);
        }

        if (other.tag=="Player" && enemyLOS)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Player")
                {
                    alertTimer += alertIncrease * Time.deltaTime;
                }
                else
                {
                    if (alertTimer > 0)
                    {
                        alertTimer -= alertIncrease * Time.deltaTime;
                    }
                }
            }

            if (alertTimer >= alertMax)
            {
                myHost.GetComponent<EnemyAI>().alerted = true;
                //myHost.GetComponent<EnemyAI>().chasePlayer = true;
                myHost.GetComponent<EnemyAI>().StartPatrol();
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Spirit")
        {
            other.GetComponent<SpiritMove>().ReturnToNormal();
        }

    }

    IEnumerator checkLOS(GameObject obj)
    {
        yield return new WaitForSeconds(.01f);


        if (!box.enabled)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (obj.transform.position - transform.position), out hit, Mathf.Infinity))
            {
                
                if (isSpirit && hit.transform.tag == "Spirit")
                {
                    obj.GetComponent<SpiritMove>().GetDamaged();
                }else if (isSpirit && hit.transform.tag != "Spirit")
                {
                    obj.GetComponent<SpiritMove>().ReturnToNormal();
                }



            }

            StartCoroutine("checkLOS",obj);
        }

    }

    IEnumerator boxReturn(GameObject obj)
    {
        yield return new WaitForSeconds(.2f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, (obj.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            //If we can no longer see the Spirit
            if (hit.transform.tag != "Spirit" &&isSpirit)
            {
                //obj.GetComponent<SpiritMove>().ReturnToNormal();
            }
        }


        box.enabled = true;
        StartCoroutine("reset", obj);
    }

    IEnumerator reset(GameObject obj)
    {
        yield return new WaitForSeconds(.2f);

        if (box.enabled)
        {
            obj.GetComponent<SpiritMove>().ReturnToNormal();
        }


    }



}
