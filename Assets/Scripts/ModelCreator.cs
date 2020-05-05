using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreator : MonoBehaviour
{
    int clusterCount = 30;
    int mrMinCount = 1;
    int mrMaxCount = 10;
    bool showTerrian = true;


    int clusterIndex = 0;
    List<GameObject> clusters;
    int clusterRadius = 5;

    List<int> riverEdgeL;
    List<int> riverEdgeR;
    List<Mushroom> trees = new List<Mushroom>();

    int groundSize = 240;

    // Start is called before the first frame update
    void Start()
    {
        clusters = new List<GameObject>();

        CreateTerrain();
        CreateSkyBox();
    }

    // Update is called once per frame
    void Update()
    {
        CreateMushrooms();
    }

    /// <summary>
    /// 随机生成蘑菇
    /// </summary>
    void CreateMushrooms()
    {
        if (clusterIndex < clusterCount)
        {
            int x = Random.Range(-100, 100);
            int z = Random.Range(-100, 100);
            int i = 48 - (Mathf.FloorToInt(z / 5) + 20);
            if (x >= riverEdgeL[i] && x <= riverEdgeR[i])
            {
                // Util.LogR(clusterIndex, x, z, i, riverEdgeL[i], riverEdgeR[i]);
                return;
            }
            // else
            //     Util.Log(clusterIndex, x, z, i, riverEdgeL[i], riverEdgeR[i]);

            Vector3 clusterPos = new Vector3(x, 0, z);

            GameObject clusterObj = new GameObject("cluster_" + clusterIndex);
            clusterObj.transform.position = clusterPos;
            clusters.Add(clusterObj);

            int count = Random.Range(mrMinCount, mrMaxCount);
            for (int j = 0; j < count; j++)
            {
                Transform clusterTrans = clusterObj.transform;
                Vector3 lPos = new Vector3(Random.Range(-clusterRadius, clusterRadius), 0, Random.Range(-clusterRadius, clusterRadius));
                Mushroom tree = new Mushroom(lPos);
                tree.transform.SetParent(clusterTrans);
                tree.transform.localPosition = lPos;
            }

            clusterIndex++;
        }
    }

    /// <summary>
    /// 创建地形
    /// </summary>
    void CreateTerrain()
    {
        riverEdgeL = new List<int>();
        riverEdgeR = new List<int>();

        int w = 50;
        int x1 = -100;
        int x2 = 0;
        int zStep = 5;
        int zTop = groundSize / 2;

        for (int z = 120; z >= -120; z -= zStep)
        {
            w += Random.Range(-5, 6);
            w = Mathf.Max(w, 10);
            x1 += Random.Range(-5, 5) + 5;
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

        if (showTerrian)
        {
            CreateGrounds(riverEdgeL, riverEdgeR, zStep, zTop);
            CreateRiver(riverEdgeL, riverEdgeR, zStep, zTop, -2);
        }
    }

    /// <summary>
    /// 随机生成河流
    /// </summary>
    /// <param name="riverEdgeL"></param>
    /// <param name="riverEdgeR"></param>
    /// <param name="zStep"></param>
    /// <param name="zTop"></param>
    void CreateRiver(List<int> riverEdgeL, List<int> riverEdgeR, int zStep, int zTop, int yUpper)
    {
        River river = new River(riverEdgeL, riverEdgeR, zStep, zTop, yUpper);
    }

    /// <summary>
    /// 随机生成陆地
    /// </summary>
    /// <param name="riverEdgeL"></param>
    /// <param name="riverEdgeR"></param>
    /// <param name="zStep"></param>
    /// <param name="zTop"></param>
    void CreateGrounds(List<int> riverEdgeL, List<int> riverEdgeR, int zStep, int zTop)
    {
        int xMin = -groundSize / 2;
        List<int> groundEdgeL = new List<int>();
        List<int> groundEdgeR = new List<int>();
        for (int i = 0; i < riverEdgeL.Count; i++)
        {
            if (riverEdgeL[i] >= xMin)
            {
                groundEdgeL.Add(xMin);
                groundEdgeR.Add(riverEdgeL[i]);
            }
            if (riverEdgeL[i] == xMin)
                break;
        }

        Ground groundL = new Ground(groundEdgeL, groundEdgeR, zStep, zTop);


        groundEdgeL = new List<int>();
        groundEdgeR = new List<int>();

        int xMax = groundSize / 2;
        for (int i = 0; i < riverEdgeR.Count; i++)
        {
            if (riverEdgeR[i] <= xMax)
            {
                groundEdgeL.Add(riverEdgeR[i]);
                groundEdgeR.Add(xMax);
            }
            if (riverEdgeR[i] == xMax)
                break;
        }

        Ground groundR = new Ground(groundEdgeL, groundEdgeR, zStep, zTop);
    }

    void CreateSkyBox()
    {
        Material skybox = new Material(Shader.Find("Skybox/Procedural"));
        skybox.SetFloat("_SunSize", 1.0f);
        skybox.SetFloat("_SunSizeConvergence", 5.0f);
        skybox.SetFloat("_AtmosphereThickness", 0.2f);
        skybox.SetFloat("_Exposure", 0.15f);
        skybox.SetColor("_SkyTint", new Color(0.184f, 0.494f, 1.0f));
        skybox.SetColor("_GroundColor", new Color(0.1f, 0.1f, 0.1f));
        RenderSettings.skybox = skybox;
    }

}