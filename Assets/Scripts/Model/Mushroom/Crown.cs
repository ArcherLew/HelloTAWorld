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
public class Crown  : MyMesh
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
        vertexList = new List<Vector3>();
        indexList = new List<int>();

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

            CreateTriangles(layer1, layer2);
        }

        // 构造下半部分三角形顶点序列
        for (int i = 0; i < bottomLayers.Count; i++)
        {
            CrownLayer layer1 = i > 0 ? bottomLayers[i - 1] : centerLayer;
            CrownLayer layer2 = bottomLayers[i];

            CreateTriangles(layer1, layer2);
        }

        // foreach (CrownLayer layer in topLayers)
        // {
        //     for (int i = 0; i < layer.vertices.Count; i++)
        //     {
        //         Util.CreatePoint(layer.vertices[i].pos, string.Format("{0}_{1}", layer.index, i));
        //     }
        // }

        UpdateMesh();

        gameObject.name = "crown";
        transform.localPosition = lPos;
    }

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
    private void CreateTriangles(CrownLayer layer1, CrownLayer layer2)
    {
        int i = 0;  // current index of lowerLayer.vertices
        int j = 0;  // current index of upperLayer.vertices

        if (layer1.vertices.Count > 0)
            vertexList.Add(layer1.vertices[i].pos);
        else
            Debug.LogError("0");

        if (layer2.vertices.Count > 0)
            vertexList.Add(layer2.vertices[j].pos);
        else
            Debug.LogError("0");

        int index1 = vertexList.Count - 2; // layer1里的最后一个位置在 vertexList 里的位置
        int index2 = vertexList.Count - 1; // layer2里的最后一个位置在 vertexList 里的位置

        int first = index1; // 头尾相连
        int second = index2;

        while (i < layer1.vertices.Count - 1 || j < layer2.vertices.Count - 1)
        {
            int layerNo = 0;

            if (i < layer1.vertices.Count - 1 && j < layer2.vertices.Count - 1)
            {
                if (layer1.vertices[i + 1].deg <= layer2.vertices[j + 1].deg)
                {
                    layerNo = 1;
                }
                else
                {
                    layerNo = 2;
                }
            }
            else if (j == layer2.vertices.Count - 1)
            {
                layerNo = 1;
            }
            else // i == lowerLayer.vertices.Count - 1
            {
                layerNo = 2;
            }

            // 新增的位置(vertexList里的)一定是和上下两层最后一个位置(vertexList里的)组成三角形，否则就会出现交叉和破面
            indexList.Add(index1);
            indexList.Add(index2);
            indexList.Add(vertexList.Count); // 提前算上新增的一个位置

            if (layerNo == 1)
            {
                vertexList.Add(layer1.vertices[++i].pos);
                index1 = vertexList.Count - 1;
            }
            else if (layerNo == 2)
            {
                vertexList.Add(layer2.vertices[++j].pos);
                index2 = vertexList.Count - 1;
            }
        }

        if (index1 == first)
        {
            indexList.Add(index1);
            indexList.Add(index2);
            indexList.Add(second);
        }
        else if (index2 == second)
        {
            indexList.Add(index1);
            indexList.Add(index2);
            indexList.Add(first);
        }
        else
        {
            indexList.Add(index1);
            indexList.Add(index2);
            indexList.Add(first);


            indexList.Add(index2);
            indexList.Add(first);
            indexList.Add(second);
        }
    }
}













