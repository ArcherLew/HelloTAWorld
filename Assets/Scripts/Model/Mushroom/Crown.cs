                                                     using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrownVertex
{
    public Vector3 pos;
    public int deg;

    public CrownVertex(CrownLayer layer, int d)
    {
        // y = layer.height;
        // r = layer.radius;
        deg = d;

        float x = layer.radius * Mathf.Cos(deg * Mathf.Deg2Rad);
        float z = layer.radius * Mathf.Sin(deg * Mathf.Deg2Rad);
        float y = layer.height;

        pos = new Vector3(x, y, z);
    }
}

public class CrownLayer
{
    /// <summary>
    /// center = 0; top > 0; bottom < 0
    /// </summary>
    public int index = 0;
    public int verticesCount;
    public float radius;
    public float height;

    public List<CrownVertex> vertices;

    public CrownLayer(int i, int vc, float r, float h)
    {
        index = i;
        verticesCount = vc;
        radius = r;
        height = h;

        vertices = new List<CrownVertex>();
    }
}

// 树冠
public class Crown : MyMesh
{

    int centerVerticesCount;
    int centerRadius;


    CrownLayer centerLayer;
    List<CrownLayer> topLayers;
    List<CrownLayer> bottomLayers;

    public Crown()
    {

        vertexList = new List<Vector3>() {
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(-0.5f, 0, 0.866f),
            new Vector3(-0.5f, 0, -0.866f),
        };

        indexList = new List<int>() {
            0, 1, 2,
            0, 2, 3,
            0, 1, 3,
            1, 2, 3
        };

        UpdateMesh();
    }

    public Crown(int cvc, int cr, Vector3 lPos)
    {
        Init();

        centerVerticesCount = cvc;
        centerRadius = cr;

        // 初始化每层的顶点数和半径
        centerLayer = new CrownLayer(0, cvc, cr, 0);
        topLayers = CreateLayers(Random.Range(2, 5), 1);
        bottomLayers = CreateLayers(Random.Range(2, 5), -1);

        // 生成腰线顶点
        CreateLayerVertices(centerLayer);

        // 生成上半部每层顶点
        foreach (CrownLayer layer in topLayers)
        {
            CreateLayerVertices(layer);
        }

        // 生成下半部每层顶点
        foreach (CrownLayer layer in bottomLayers)
        {
            CreateLayerVertices(layer);
        }


        // 构造上半部分三角形顶点序列
        for (int i = 0; i < topLayers.Count; i++)
        {
            CrownLayer layer1 = i > 0 ? topLayers[i - 1] : centerLayer;
            CrownLayer layer2 = topLayers[i];

            CreateTriangles(layer1, layer2, 1);
        }

        // 构造下半部分三角形顶点序列
        for (int i = 0; i < bottomLayers.Count; i++)
        {
            CrownLayer layer1 = i > 0 ? bottomLayers[i - 1] : centerLayer;
            CrownLayer layer2 = bottomLayers[i];

            CreateTriangles(layer1, layer2, -1);
        }

        // foreach (CrownLayer layer in topLayers)
        // {
        //     for (int i = 0; i < layer.vertices.Count; i++)
        //     {
        //         Util.CreatePoint(layer.vertices[i].pos, string.Format("{0}_{1}", layer.index, i));
        //     }
        // }

        UpdateMesh("MyWorld/Mushroom Crown");

        meshRenderer.material.SetColor("_UpperColor", crownColors[Random.Range(0, 4)]);
        meshRenderer.material.SetFloat("_GlimOffset", Random.Range(0.0f, 10.0f));
        meshRenderer.material.SetFloat("_DarkPeriod", 0.95f);


        gameObject.name = "crown";
        gameObject.layer = LayerMask.NameToLayer("Crown");
        transform.localPosition = lPos;
        transform.localRotation = Quaternion.FromToRotation(Vector3.up, lPos);
    }

    static private List<Color> crownColors = new List<Color>() {
        new Color(0.137f, 0.235f, 0.541f),
        new Color(0.208f, 0.196f, 0.482f),
        new Color(0.2f, 0.432f, 0.678f),
        new Color(0.2f, 0.27f, 0.58f),
        new Color(0.173f, 0.404f, 0.58f),
    };

    private void CreateLayerVertices(CrownLayer layer)
    {
        int remainDegree = 360;
        for (int i = layer.verticesCount; i > 0; i--)
        {
            int averageDegree = remainDegree / i;
            int deg = i == 1 ? remainDegree : Random.Range(averageDegree / 2, averageDegree * 3 / 2);
            remainDegree -= deg;
            CrownVertex cv = new CrownVertex(layer, 360 - remainDegree);
            layer.vertices.Add(cv);
        }
    }

    /// <summary>
    /// 生成每层的顶点数量、半径、高度
    /// </summary>
    /// <param name="layerCount">半个树冠的总层数</param>
    /// <param name="topOrBottom">上半部(1)或下半部(-1)</param>
    private List<CrownLayer> CreateLayers(int layerCount, int topOrBottom)
    {
        float totalHeight = Random.Range(0.2f, 1f) * centerRadius;
        float hStep = totalHeight / layerCount;
        float rStep = centerRadius / layerCount;
        int vcStep = (centerVerticesCount - 2) / (layerCount - 1);

        List<CrownLayer> layers = new List<CrownLayer>();

        float h = 0;
        float r = centerRadius;
        int vc = centerVerticesCount;

        for (int i = 1; i <= layerCount; i++)
        {
            h += (Random.Range(-0.1f, 0.1f) + 1) * hStep * topOrBottom;
            r -= (Random.Range(-0.1f, 0.1f) + 1) * rStep;

            if (i == layerCount)
                vc = 1; // todo : more style
            else
            {
                vc -= Random.Range(0, 2);
                vc = Mathf.Max(vc, 1);
            }


            CrownLayer layer = new CrownLayer(i * topOrBottom, vc, r, h);
            layers.Add(layer);


            if (vc == 1)
                break;
        }

        return layers;
    }

    /// <summary>
    /// 在 lowerLayer, upperLayer 之间构建三角形
    /// todo: 后续算法优化
    /// </summary>
    /// <param name="layer1"></param>
    /// <param name="layer2"></param>
    private void CreateTriangles(CrownLayer layer1, CrownLayer layer2, int revertNormal = 1)
    {
        int i = 0;  // current index of lowerLayer.vertices
        int j = 0;  // current index of upperLayer.vertices

        int index1 = -1;
        int index2 = -1;

        int first = -1;//index1; // 头尾相连
        int second = -1;// index2;

        while (i < layer1.vertices.Count - 1 || j < layer2.vertices.Count - 1)
        {
            int curLayer = 0; // 新加入顶点所在的layer

            if (i < layer1.vertices.Count - 1 && j < layer2.vertices.Count - 1)
            {
                if (layer1.vertices[i + 1].deg <= layer2.vertices[j + 1].deg)
                {
                    curLayer = 1;
                }
                else
                {
                    curLayer = 2;
                }
            }
            else if (j == layer2.vertices.Count - 1)
            {
                curLayer = 1;
            }
            else // i == lowerLayer.vertices.Count - 1
            {
                curLayer = 2;
            }

            // 新增的位置(vertexList里的)一定是和上下两层最后一个位置(vertexList里的)组成三角形，否则就会出现交叉和破面

            index1 = AddVertex(layer1.vertices[i].pos);
            index2 = AddVertex(layer2.vertices[j].pos);

            if (first == -1)
            {
                first = index1;
                second = index2;
            }

            indexList.Add(index1);
            indexList.Add(index2);

            if (curLayer == 1)
            {
                index1 = AddVertex(layer1.vertices[++i].pos);
                indexList.Add(index1);
            }
            else if (curLayer == 2)
            {
                index2 = AddVertex(layer2.vertices[++j].pos);
                indexList.Add(index2);
            }

            Vector3 normal = RecaculateFaceNormal(revertNormal);
            // AddNormal(normal, 3);
        }

        int indexA, indexB, indexC;

        if (index1 == first)
        {
            indexA = AddVertex(vertexList[index1]);
            indexB = AddVertex(vertexList[index2]);
            indexC = AddVertex(vertexList[second]);

            indexList.Add(indexA);
            indexList.Add(indexB);
            indexList.Add(indexC);

            Vector3 normal = RecaculateFaceNormal(revertNormal);
            // AddNormal(normal, 3);
        }
        else if (index2 == second)
        {
            indexA = AddVertex(vertexList[index1]);
            indexB = AddVertex(vertexList[index2]);
            indexC = AddVertex(vertexList[first]);

            indexList.Add(indexA);
            indexList.Add(indexB);
            indexList.Add(indexC);

            Vector3 normal = RecaculateFaceNormal(revertNormal);
            // AddNormal(normal, 3);
        }
        else
        {
            if (vertexList[index1] != vertexList[index2] && vertexList[index1] != vertexList[first] && vertexList[first] != vertexList[index2])
            {
                indexA = AddVertex(vertexList[index1]);
                indexB = AddVertex(vertexList[index2]);
                indexC = AddVertex(vertexList[first]);

                indexList.Add(indexA);
                indexList.Add(indexB);
                indexList.Add(indexC);

                Vector3 normal = RecaculateFaceNormal(revertNormal);
                // AddNormal(normal, 3);
            }
            if (vertexList[first] != vertexList[index2] && vertexList[second] != vertexList[first] && vertexList[second] != vertexList[index2])
            {
                indexA = AddVertex(vertexList[index2]);
                indexB = AddVertex(vertexList[first]);
                indexC = AddVertex(vertexList[second]);

                indexList.Add(indexA);
                indexList.Add(indexB);
                indexList.Add(indexC);

                Vector3 normal = RecaculateFaceNormal(-revertNormal);
                // AddNormal(normal, 3);
            }
        }
    }
}













