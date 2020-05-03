using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreator : MonoBehaviour
{
    int treeIndex = 0;
    int clusterIndex = 0;
    List<GameObject> clusters;

    List<Tree> trees = new List<Tree>();

    // Start is called before the first frame update
    void Start()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "ground";
        ground.transform.localPosition = Vector3.zero;
        ground.transform.localScale = new Vector3(220.0f, 0.1f, 220.0f);

        clusters = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (clusterIndex < 10)
        {
            Vector3 clusterPos = new Vector3(Random.Range(-100.0f, 100.0f), 0, Random.Range(-100.0f, 100.0f));
            if (clusters.Count <= clusterIndex)
            {
                GameObject clusterObj = new GameObject("cluster_" + clusterIndex);
                clusterObj.transform.position = clusterPos;
                clusters.Add(clusterObj);
            }

            if (treeIndex < 10)
            {
                Transform clusterTrans = clusters[clusterIndex].transform;
                treeIndex++;
                Vector3 lPos = new Vector3(Random.Range(-3.0f, 3.0f), 0, Random.Range(-3.0f, 3.0f));
                Tree tree = new Tree(lPos);
                tree.transform.SetParent(clusterTrans);
                tree.transform.localPosition = lPos;
            }
            else
            {
                treeIndex = 0;
                clusterIndex++;
            }
        }
    }
}