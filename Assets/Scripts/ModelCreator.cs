using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreator : MonoBehaviour
{
    int treeIndex = 0;
    int clusterIndex = 0;
    List<GameObject> clusters;

    List<Mushroom> trees = new List<Mushroom>();

    int groundSize = 240;

    // Start is called before the first frame update
    void Start()
    {
        CreateGround();
        CreateRiver();
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
                Mushroom tree = new Mushroom(lPos);
                tree.transform.SetParent(clusterTrans);
                tree.transform.localPosition = lPos;
            }
            else
            {
                treeIndex = 0;
                clusterIndex++;
            }
        }

        // if (Mathf.Floor(Time.realtimeSinceStartup) % 3 == 0)
        //     CreateRiver();
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "ground";
        ground.transform.localPosition = Vector3.zero;
        ground.transform.localScale = new Vector3(groundSize, 0.1f, groundSize);

        clusters = new List<GameObject>();
    }

    void CreateRiver()
    {
        List<int> riverEdgeL = new List<int>();
        List<int> riverEdgeR = new List<int>();

        int w = 50;
        int x1 = -100;
        int x2 = 0;
        int zStep = 10;
        int zTop = groundSize / 2;

        for (int z = 120; z > -120; z -= 10)
        {
            w += Random.Range(-10, 10);
            w = Mathf.Max(w, 10);
            x1 += Random.Range(-10, 10) + 10;
            x1 = Mathf.Min(Mathf.Max(x1, -100), 100);
            x2 = x1 + w;
            x2 = Mathf.Min(Mathf.Max(x2, -100), 120);
            riverEdgeL.Add(x1);
            riverEdgeR.Add(x2);

            // Vector3 posL = new Vector3(x1, 1, z);
            // Vector3 posR = new Vector3(x2, 1, z);
            // Util.CreatePoint(posL);
            // Util.CreatePoint(posR);
        }

        River river = new River(riverEdgeL, riverEdgeR, zStep, zTop);
    }
}