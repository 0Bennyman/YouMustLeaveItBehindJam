using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GameObject obj;

    public void ActivateButton()
    {
        obj.SetActive(!obj.activeSelf);
    }


}
