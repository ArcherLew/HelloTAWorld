using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk
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

    public Trunk(Vector3 lPos, float d)
    {
        float length = lPos.magnitude;
        gameObject.name = "trunk";
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.sharedMaterial = new Material(Shader.Find("MyWorld/Mushroom Trunk"));

        transform.localPosition = lPos / 2;
        transform.localScale = new Vector3(d, length, d);
        transform.localRotation = Quaternion.FromToRotation(Vector3.up, lPos);
    }
}



