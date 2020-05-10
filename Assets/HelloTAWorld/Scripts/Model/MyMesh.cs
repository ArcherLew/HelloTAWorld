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
    protected List<Vector3> normalList;

    protected MeshRenderer meshRenderer;

    protected int revertNormal = 1;

    protected virtual void Init()
    {
        vertexList = new List<Vector3>();
        indexList = new List<int>();
        normalList = new List<Vector3>();
    }

    protected void UpdateMesh(string shaderName = "Diffuse")
    {
        // Util.LogR(vertexList.Count, normalList.Count);
        // return;

        Vector3[] vertices = vertexList.ToArray();
        int[] indices = indexList.ToArray();
        Vector3[] normals = normalList.ToArray();

        // Util.FixFaceToClockWise(vertices, indices, revertNormal);

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        // mesh.normals = normals;

        // mesh.

        // mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find(shaderName));

        gameObject.AddComponent<MeshCollider>().convex = true;
    }

    /// <summary>
    /// 创建两条顶点围成的三角形
    /// </summary>
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    protected void CreateStripsTriangles(List<Vector3> line1, List<Vector3> line2, int revertNormal, bool hardNormal = false, char comparator = 'x')
    {
        int i = 0;  // current index of lowerLayer.vertices
        int j = 0;  // current index of upperLayer.vertices

        int index1 = -1;
        int index2 = -1;

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
            index1 = hardNormal ? AddVertex(line1[i]) : TryAddVertex(line1[i]);
            index2 = hardNormal ? AddVertex(line2[j]) : TryAddVertex(line2[j]);

            indexList.Add(index1);
            indexList.Add(index2);
            // indexList.Add(vertexList.Count); // 提前算上新增的一个位置

            if (lineNo == 1)
            {
                // vertexList.Add(line1[++i]);
                // index1 = vertexList.Count - 1;
                index1 = hardNormal ? AddVertex(line1[++i]) : TryAddVertex(line1[++i]);
                indexList.Add(index1);
            }
            else if (lineNo == 2)
            {
                // vertexList.Add(line2[++j]);
                // index2 = vertexList.Count - 1;
                index2 = hardNormal ? AddVertex(line2[++j]) : TryAddVertex(line2[++j]);
                indexList.Add(index2);
            }

            RecaculateFaceNormal(revertNormal);
        }
    }

    /// <summary>
    /// 创建一条顶点外加2个顶点围城的三角形
    /// </summary>
    /// <param name="strip"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    protected void CreateStripCornerTriangles(List<Vector3> strip, Vector3 v1, Vector3 v2, int revertNormal, bool hardNormal = false)
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

            RecaculateFaceNormal(revertNormal);
        }

        index1 = TryAddVertex(strip[strip.Count - 1]);
        index2 = TryAddVertex(v1);
        index3 = TryAddVertex(v2);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
        RecaculateFaceNormal(-revertNormal);
    }

    /// <summary>
    /// 创建四边形的三角形
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <param name="v4"></param>
    protected void CreateQuadTriangles(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int revertNormal, bool hardNormal = false)
    {
        int index1, index2, index3;
        index1 = TryAddVertex(v1);
        index2 = TryAddVertex(v2);
        index3 = TryAddVertex(v3);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
        RecaculateFaceNormal(revertNormal);

        index1 = TryAddVertex(v2);
        index2 = TryAddVertex(v3);
        index3 = TryAddVertex(v4);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
        RecaculateFaceNormal(-revertNormal);
    }

    public Vector3 RecaculateFaceNormal(int revertNormal)
    {
        int count = indexList.Count;
        int i1 = indexList[count - 3];
        int i2 = indexList[count - 2];
        int i3 = indexList[count - 1];

        Vector3 p0, p1, p2, v01, v12, normal;

        p0 = vertexList[i1];
        p1 = vertexList[i2];
        p2 = vertexList[i3];

        v01 = p1 - p0;
        v12 = p2 - p1;
        normal = Vector3.Cross(v01, v12);

        float det = Util.Determinant(v01, v12, normal);
        if (det == 0)
        {
            // if ((v01.z + v12.z == 0 && v01.z * v12.z == 0) || (v01.z * v12.z != 0 && v01.x / v01.z == v12.x / v12.z))
            // {
            Debug.LogError(v01.ToString());
            Debug.LogError(v12.ToString());
            Debug.LogError(string.Format("{0},{1},{2}, 三点共线", p0, p1, p2));
            DrawTool.DrawPoint(p0, "dot");
            DrawTool.DrawPoint(p1, "dot");
            DrawTool.DrawPoint(p2, "dot");
            // }
            return Vector3.zero;
        }
        if (det * revertNormal < 0)
        {
            indexList[count - 2] = i3;
            indexList[count - 1] = i2;
            return -normal;
        }
        else
        {
            return normal;
        }
    }

    // 缓存已经存储到 vertexList 里的 vertex 在 vertexList 里的 index，避免重复保存顶点数据
    private Dictionary<Vector3, int> vertex2Index = new Dictionary<Vector3, int>();

    /// <summary>
    /// 加入共面顶点
    /// </summary>
    /// <param name="v">顶点</param>
    /// <returns>顶点在vertexList 里的 index</returns>
    protected int TryAddVertex(Vector3 v)
    {
        int result;
        // Util.Log(v.GetHashCode());
        if (!vertex2Index.TryGetValue(v, out result))
        {
            // Util.Log("xxxxxxxxx");
            vertexList.Add(v);
            // Util.Log("<color=lime>", vertexList.Count, "</color>");
            result = vertexList.Count - 1;
            vertex2Index.Add(v, result);
        }
        return result;
    }

    /// <summary>
    /// 加入不共面顶点
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    protected int AddVertex(Vector3 v)
    {
        vertexList.Add(v);
        return vertexList.Count - 1;
    }

    protected void AddNormal(Vector3 normal, int count)
    {
        for (int i = 0; i < count; i++)
        {
            normalList.Add(normal);
            // Util.Log("<color=red>", normalList.Count, "</color>");
        }
    }

}
