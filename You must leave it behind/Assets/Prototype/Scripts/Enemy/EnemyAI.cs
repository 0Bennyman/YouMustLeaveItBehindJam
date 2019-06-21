using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    public GameObject controlledCamera;

    public GameObject eye1, eye2;

    private PlayerMove playMov;

    public Vector3[] PatrolPointsStandard, PatrolPointsAlerted;

    public int curPoint;

    private Vector3 nonPatrolPoint, nonPatrolPointAlerted; //These will get the first array point of patrol points above
                                                           //These will not patrol but instead just stand and wait

    private NavMeshAgent agent;
    
    

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


        StartCoroutine("checkHealth");  

        transform.position = PatrolPointsStandard[0];
        curPoint = 1;
        agent.SetDestination(PatrolPointsStandard[curPoint]);
    }

    IEnumerator checkHealth()
    {
        yield return new WaitForSeconds(1);

        if (playMov.curHealth<= 0)
        {
            Destroy(gameObject);
        }

        StartCoroutine("checkHealth");

    }


    public void GetPossesed()
    {
        controlledCamera.SetActive(true);
        playMov.enabled = true;
        playMov.lockMovement = false;
        playMov.controller.enabled = true;
        eye1.SetActive(false);
        eye2.SetActive(false);

    }

    public void GetUnpossesed()
    {
        eye1.SetActive(true);
        eye2.SetActive(true);
    }

}
