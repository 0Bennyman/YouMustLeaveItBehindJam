using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyAI : MonoBehaviour
{
    public GameObject controlledCamera;

    public GameObject eye1, eye2;

    private PlayerMove playMov;

    public Vector3[] PatrolPointsStandard, PatrolPointsAlerted;

    public int curPoint;

    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public bool alerted,chasePlayer;

    private GameObject player;

    public bool isFiring;
    
    

    private void OnDrawGizmosSelected()
    {
        if (PatrolPointsStandard[curPoint] == new Vector3(0, 0, 0))
        {
            PatrolPointsStandard[curPoint] = transform.position;
        }

        if (PatrolPointsAlerted[curPoint] == new Vector3(0, 0, 0))
        {
            PatrolPointsAlerted[curPoint] = transform.position;
        }

        Debug.DrawLine(new Vector3(PatrolPointsStandard[curPoint].x, PatrolPointsStandard[curPoint].y + 1, PatrolPointsStandard[curPoint].z), new Vector3(PatrolPointsStandard[curPoint].x, PatrolPointsStandard[curPoint].y, PatrolPointsStandard[curPoint].z), Color.green);
        Debug.DrawLine(new Vector3(PatrolPointsAlerted[curPoint].x, PatrolPointsAlerted[curPoint].y + 1, PatrolPointsAlerted[curPoint].z), new Vector3(PatrolPointsAlerted[curPoint].x, PatrolPointsAlerted[curPoint].y, PatrolPointsAlerted[curPoint].z), Color.red);
    }

    private void Start()
    {
        playMov = gameObject.GetComponent<PlayerMove>();
        playMov.enabled = false;
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine("checkHealth");  

        transform.position = PatrolPointsStandard[0];
        curPoint = 1;
        StartPatrol();
    }


    private void Update()
    {
        if (!isFiring && chasePlayer)
        {
            isFiring = true;
            StartCoroutine("ShootPlayer");
        }

        if (!alerted && !chasePlayer)
        {
            agent.SetDestination(PatrolPointsStandard[curPoint]);
            if (Vector3.Distance(transform.position, PatrolPointsStandard[curPoint]) < .6)
            {
                NextPoint();
            }
        }
        else if (alerted && !chasePlayer)
        {
            agent.SetDestination(PatrolPointsAlerted[curPoint]);
            if (Vector3.Distance(transform.position, PatrolPointsAlerted[curPoint]) < .6)
            {
                NextPoint();
            }
        }

        if (chasePlayer)
        {
            agent.SetDestination(player.transform.position);
        }

    }


    public void StartPatrol()
    {
        if (!alerted)
        {
            agent.SetDestination(PatrolPointsStandard[curPoint]);
        }
        else
        {
            agent.SetDestination(PatrolPointsAlerted[curPoint]);
        }

    }

    private void NextPoint()
    {
        curPoint+=1;

        if (!alerted)
        {
            if (curPoint >= PatrolPointsStandard.Length)
            {
                curPoint = 0;
            }
        }
        else
        {
            if (curPoint >= PatrolPointsAlerted.Length)
            {
                curPoint = 0;
            }
        }
        StartPatrol();
    }

    IEnumerator checkHealth()
    {
        yield return new WaitForSeconds(.3f);

        if (playMov.curHealth<= 0)
        {
            Destroy(gameObject);
        }

        StartCoroutine("checkHealth");

    }

    IEnumerator ShootPlayer()
    {
        yield return new WaitForSeconds(.5f);
        RaycastHit hit;

        if (Physics.Raycast(controlledCamera.transform.position, (player.transform.position-controlledCamera.transform.position ),out hit))
        {
            if (hit.transform.tag == "Player")
            {
                int randomChance = Random.Range(0, 2);
                if (randomChance == 1)
                {
                    player.GetComponent<PlayerMove>().GetHurt();
                }
            }

        }



        StartCoroutine("ShootPlayer");
    }


    public void GetPossesed()
    {
        controlledCamera.SetActive(true);
        playMov.enabled = true;
        playMov.lockMovement = false;
        playMov.controller.enabled = true;
        eye1.SetActive(false);
        eye2.SetActive(false);
        agent.isStopped = true;
        this.enabled = false;
    }

    public void GetUnpossesed()
    {
        eye1.SetActive(true);
        eye2.SetActive(true);
    }

}
