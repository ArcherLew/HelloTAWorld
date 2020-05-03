using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrunk
{
    GameObject _gameObject;
    public GameObject gameObject
    {
        get
        {
            if (_gameObject == null)
            {
                _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                transform = _gameObject.transform;
            }
            return _gameObject;
        }
    }
    public Transform transform;

    public TreeTrunk(Vector3 lPos, float d)
    {
        float length = lPos.magnitude;
        gameObject.name = "trunk";
        gameObject.GetComponent<Renderer>().material.color = Color.red;//颜色        
        transform.localPosition = lPos / 2;
        transform.localScale = new Vector3(d, length, d);
        transform.localRotation = Quaternion.FromToRotation(Vector3.up, lPos);
    }
}



