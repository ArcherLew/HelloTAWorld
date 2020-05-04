using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyMesh
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

        Util.FixFaceToClockWise(vertices, indices, detFactor);

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

}
