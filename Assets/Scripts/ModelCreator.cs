using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreator : MonoBehaviour
{
    int treeCount = 0;

    List<Tree> trees = new List<Tree>();

    // Start is called before the first frame update
    void Start()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "ground";
        ground.transform.localPosition = Vector3.zero;
        ground.transform.localScale = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (treeCount < 10)
        {
            treeCount++;

            float x = Random.Range(-10.0f, 10.0f);
            float z = Random.Range(-10.0f, 10.0f);
            Tree tree = new Tree(new Vector3(x, 0, z));
        }
    }
}
