using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Ship", menuName = "Naves/NaveOrdinariaBase", order = 0)]
public class NaveBaseSO : ScriptableObject
{
    public Animator anim;
    public int damage = 1;
    public int bulletNum = 1;
    public float reloadime = 0.5f;
    public float bulletSpeed = 6f;
    public int points = 100;
    public float movementSpeed = 5f;
}