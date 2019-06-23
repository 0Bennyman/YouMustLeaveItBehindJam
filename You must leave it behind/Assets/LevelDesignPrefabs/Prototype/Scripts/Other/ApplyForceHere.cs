using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForceHere : MonoBehaviour
{

    private void Start()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddRelativeForce(Vector3.forward * 800);

        Destroy(this);
    }

}
