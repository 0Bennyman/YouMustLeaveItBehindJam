using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestruction : MonoBehaviour
{

    public GameObject wallPrefab;


    private void Start()
    {
        //ohfuckimhit();
    }

    public void ohfuckimhit()
    {
        Instantiate(wallPrefab, transform.position,wallPrefab.transform.rotation);
        Destroy(gameObject);
    }


}
