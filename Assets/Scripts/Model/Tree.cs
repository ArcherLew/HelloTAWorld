﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
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

    List<TreeCrown> crowns = new List<TreeCrown>();
    TreeTrunk trunk;
    
    public Tree(){}

    public Tree(Vector3 treePos)
    {
        gameObject.name = "tree";

        Vector3 centerPos = Vector3.zero;
        int crownCount = 1; // todo: count random, pos random
        for (int i = 0; i < crownCount; i++)
        {
            float x = Random.Range(-3.0f, 3.0f);
            float y = Random.Range(10.0f, 20.0f);
            float z = Random.Range(-3.0f, 3.0f);
            Vector3 crownPos = new Vector3(x, y, z);
            centerPos += crownPos;
            TreeCrown tc = new TreeCrown(8, 4, crownPos);
            tc.transform.SetParent(transform);
        }

        float d = Random.Range(1.0f, 2.0f);
        TreeTrunk tt = new TreeTrunk(centerPos / crownCount, d);
        tt.transform.SetParent(transform);


        transform.position = treePos;
    }
}

