using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MyMesh
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

    protected Mesh mesh;
    protected List<Vector3> vertexList;
    protected List<int> indexList;

    protected MeshRenderer meshRenderer;

    protected int detFactor = 1;

    protected void UpdateMesh(string shaderName = "Diffuse")
    {
        Vector3[] vertices = vertexList.ToArray();
        int[] indices = indexList.ToArray();

        // Util.FixFaceToClockWise(vertices, indices, detFactor);

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find(shaderName));
    }

    // 缓存已经存储到 vertexList 里的 vertex 在 vertexList 里的 index，避免重复保存顶点数据
    private Dictionary<Vector3, int> vertex2Index = new Dictionary<Vector3, int>();

    /// <summary>
    /// 将一个未保存过的顶点存入到 vertexList，并建立索引到 vertex2Index
    /// </summary>
    /// <param name="v">顶点</param>
    /// <returns>顶点在vertexList 里的 index</returns>
    protected int TryAddVertex(Vector3 v)
    {
        int result;
        if (!vertex2Index.TryGetValue(v, out result))
        {
            vertexList.Add(v);
            result = vertexList.Count - 1;
            vertex2Index.Add(v, result);
        }
        return result;
    }

    /// <summary>
    /// 创建两条顶点围成的三角形
    /// </summary>
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    protected void CreateStripsTriangles(List<Vector3> line1, List<Vector3> line2, int detFactor, char comparator = 'x')
    {
        int i = 0;  // current index of lowerLayer.vertices
        int j = 0;  // current index of upperLayer.vertices

        // if (line1.Count > 0)
        //     vertexList.Add(line1[i]);
        // else
        //     Debug.LogError("0");

        // if (line2.Count > 0)
        //     vertexList.Add(line2[j]);
        // else
        //     Debug.LogError("0");

        // int index1 = vertexList.Count - 2; // layer1里的最后一个位置在 vertexList 里的位置
        // int index2 = vertexList.Count - 1; // layer2里的最后一个位置在 vertexList 里的位置

        int index1 = TryAddVertex(line1[i]);
        int index2 = TryAddVertex(line2[j]);

        while (i < line1.Count - 1 || j < line2.Count - 1)
        {
            int lineNo = 0;

            if (i < line1.Count - 1 && j < line2.Count - 1)
            {
                if (comparator == 'x' && line1[i + 1].x <= line2[j + 1].x)
                {
                    lineNo = 1;
                }
                else if (comparator == 'z' && line1[i + 1].z >= line2[j + 1].z)
                {
                    lineNo = 1;
                }
                else
                {
                    lineNo = 2;
                }
            }
            else if (j == line2.Count - 1)
            {
                lineNo = 1;
            }
            else // i == lowerLayer.Count - 1
            {
                lineNo = 2;
            }

            // 新增的位置(vertexList里的)一定是和上下两层最后一个位置(vertexList里的)组成三角形，否则就会出现交叉和破面
            indexList.Add(index1);
            indexList.Add(index2);
            // indexList.Add(vertexList.Count); // 提前算上新增的一个位置

            if (lineNo == 1)
            {
                // vertexList.Add(line1[++i]);
                // index1 = vertexList.Count - 1;
                index1 = TryAddVertex(line1[++i]);
                indexList.Add(index1);
            }
            else if (lineNo == 2)
            {
                // vertexList.Add(line2[++j]);
                // index2 = vertexList.Count - 1;
                index2 = TryAddVertex(line2[++j]);
                indexList.Add(index2);
            }

            Util.FixLastTriangleFace(vertexList, indexList, detFactor);
        }
    }

    /// <summary>
    /// 创建一条顶点外加2个顶点围城的三角形
    /// </summary>
    /// <param name="strip"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    protected void CreateStripCornerTriangles(List<Vector3> strip, Vector3 v1, Vector3 v2, int detFactor)
    {
        int index1, index2, index3;
        for (int i = 0; i < strip.Count - 1; i++)
        {
            index1 = TryAddVertex(strip[i]);
            index2 = TryAddVertex(strip[i + 1]);
            index3 = TryAddVertex(v1);

            indexList.Add(index1);
            indexList.Add(index2);
            indexList.Add(index3);

            Util.FixLastTriangleFace(vertexList, indexList, detFactor);
        }

        index1 = TryAddVertex(strip[strip.Count - 1]);
        index2 = TryAddVertex(v1);
        index3 = TryAddVertex(v2);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
        Util.FixLastTriangleFace(vertexList, indexList, -detFactor);
    }

    /// <summary>
    /// 创建四边形的三角形
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <param name="v4"></param>
    protected void CreateQuadTriangles(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int detFactor)
    {
        int index1, index2, index3;
        index1 = TryAddVertex(v1);
        index2 = TryAddVertex(v2);
        index3 = TryAddVertex(v3);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
        Util.FixLastTriangleFace(vertexList, indexList, detFactor);

        index1 = TryAddVertex(v2);
        index2 = TryAddVertex(v3);
        index3 = TryAddVertex(v4);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
        Util.FixLastTriangleFace(vertexList, indexList, -detFactor);
    }

}
