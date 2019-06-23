using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public ScriptableWeapons weapon;

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
            outL.enabled = true;
            StopCoroutine("RemoveHighlight");
            StartCoroutine("RemoveHighlight");
        }
        else
        {
            outL.enabled = false;
        }
    }


    IEnumerator RemoveHighlight()
    {
        yield return new WaitForSeconds(.5f);
        lookedAt = false;
        DisplayHighlight();
    }


}
