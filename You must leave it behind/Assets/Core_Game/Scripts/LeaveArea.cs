using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LeaveArea : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject boss = GameObject.FindGameObjectWithTag("EnemyBoss");
            if (boss == null)
            {
                SceneManager.LoadScene(0);
            }
        }
    }


}
