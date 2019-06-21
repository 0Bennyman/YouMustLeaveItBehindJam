using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Weaponry")]
public class ScriptableWeapons : ScriptableObject
{

    public int Damage,maxAmmo;
    public float fireRate;

    public GameObject throwablePrefab;

    public Mesh Model;

    public bool Scope, canBeThrown;

}
