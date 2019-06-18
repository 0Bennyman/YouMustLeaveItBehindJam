using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject controlledCamera;

    public GameObject eye1, eye2;

    private PlayerMove playMov;

    private void Start()
    {
        playMov = gameObject.GetComponent<PlayerMove>();
        playMov.enabled = false;
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
