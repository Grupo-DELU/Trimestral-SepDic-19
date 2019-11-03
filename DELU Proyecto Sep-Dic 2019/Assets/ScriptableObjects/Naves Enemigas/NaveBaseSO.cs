using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Ship", menuName = "Naves/Nave Enemiga", order = 0)]
public class NaveBaseSO : ScriptableObject
{
    public Animator anim;
    public CurveScriptObject bezier;
    public bool kamikaze = false;
    public int damage = 1;
    public int bulletNum = 1;
    public float reloadime = 0.5f;
    public int points = 100;
}
