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
    
    protected void UpdateMesh(string shaderName = "Diffuse")
    {
        Vector3[] vertices = vertexList.ToArray();
        int[] indices = indexList.ToArray();

        Util.FixFaceToClockWise(vertices, indices);

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find(shaderName));
    }


}
