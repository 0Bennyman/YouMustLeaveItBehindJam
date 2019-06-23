using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Weaponry")]
public class ScriptableWeapons : ScriptableObject
{

    public int Damage,maxAmmo;
    public float fireRate,recoilAmount,noiseRadius;

    public GameObject throwablePrefab,prefabModel,prefabModelScoped;

    public Mesh Model;

    public bool Scope, canBeThrown;

}
