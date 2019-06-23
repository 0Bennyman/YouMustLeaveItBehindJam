using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GameObject obj,myText;

    private bool lookedAt;

    private void Start()
    {
        myText.SetActive(false);
    }

    public void ActivateButton()
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void DisplayText()
    {
        if (!lookedAt)
        {
            lookedAt = true;
            myText.SetActive(true);
            StartCoroutine("RemoveText");
        }
    }

    IEnumerator RemoveText()
    {
        yield return new WaitForSeconds(1f);

        lookedAt = false;
        myText.SetActive(false);
    }


}
