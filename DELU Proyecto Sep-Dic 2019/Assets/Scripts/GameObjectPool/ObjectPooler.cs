using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pooler
{
    public string sName = "default";
    public int iQuantity = 10;
    public bool bIsGrowable = false;
    public GameObject gObject;
    [HideInInspector]
    public List<GameObject> lInstantiated;
}

public class ObjectPooler : MonoBehaviour
{
    [SerializeField]
    private Pooler[] pools = null;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < pools.Length; i++)
        {
            pools[i].lInstantiated = new List<GameObject>();
            for (int j = 0; j < pools[i].iQuantity; j++)
            {
                GameObject obj = (GameObject)Instantiate(pools[i].gObject);
                obj.SetActive(false);
                pools[i].lInstantiated.Add(obj);
            }
        }
    }


    public GameObject GetFromPool(string poolName)
    {
        foreach(Pooler pool in pools)
        {
            if (pool.sName == poolName)
            {
                foreach(GameObject obj in pool.lInstantiated)
                {
                    if (!obj.activeInHierarchy)
                    {
                        obj.SetActive(true);
                        return obj;
                    }
                }
                if (pool.bIsGrowable)
                {
                    pool.iQuantity += 1;
                    GameObject obj = (GameObject)Instantiate(pool.gObject);
                    obj.SetActive(false);
                    pool.lInstantiated.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
}
