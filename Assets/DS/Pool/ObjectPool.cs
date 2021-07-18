using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A simple objectPool script
/// </summary>
public class ObjectPool : MonoBehaviour
{

    [SerializeField] GameObject prefab;
    [SerializeField] int poolSize;

    GameObject[] pool;

    private void Awake()
    {

        pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject g = Instantiate(prefab, transform);
            g.SetActive(false);
            pool[i] = g;
        }
    }


    public void DeactivateAllPoolObjects()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].gameObject.activeInHierarchy)
                pool[i].SetActive(false);
        }
    }

    public GameObject GetObject()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].gameObject.activeInHierarchy)
                continue;

            pool[i].SetActive(true);
            return pool[i];
        }

        Debug.LogError("Incrementa el tamaño de esta pool " + gameObject.name);
        return null;
    }
}
