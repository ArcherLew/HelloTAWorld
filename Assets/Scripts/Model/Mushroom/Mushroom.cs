using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom
{
    GameObject _gameObject;
    public GameObject gameObject
    {
        get
        {
            if (_gameObject == null)
            {
                _gameObject = new GameObject();
                transform = _gameObject.transform;
            }
            return _gameObject;
        }
    }
    public Transform transform;

    List<Crown> crowns = new List<Crown>();
    Trunk trunk;

    public Mushroom() { }

    public Mushroom(Vector3 crownOffset)
    {
        gameObject.name = "tree";

        Vector3 centerPos = Vector3.zero;
        int crownCount = 1; // todo: count random, pos random
        for (int i = 0; i < crownCount; i++)
        {
            float x = Random.Range(-3.0f, 3.0f);
            float y = Random.Range(5.0f, 20.0f);
            float z = Random.Range(-3.0f, 3.0f);
            Vector3 crownPos = new Vector3(x, y, z) + crownOffset * Random.Range(0.0f, 1.0f);
            centerPos += crownPos;
            Crown tc = new Crown(8, 4, crownPos);
            tc.transform.SetParent(transform);
        }

        float d = Random.Range(0.5f, 1.0f);
        Trunk tt = new Trunk(centerPos / crownCount, d);
        tt.transform.SetParent(transform);
    }
}

