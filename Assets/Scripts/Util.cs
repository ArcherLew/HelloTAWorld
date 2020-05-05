using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Util
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    /// <param name="revertNormal">若是封闭图形，则默认为1；否则则取决于视角方向，-1为取反</param>
    public static void FixFaceToClockWise(Vector3[] vertices, int[] triangles, int revertNormal = 1)
    {
        Vector3 p0, p1, p2, v01, v12;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            p0 = vertices[triangles[i]];
            p1 = vertices[triangles[i + 1]];
            if (triangles[i + 2] >= vertices.Length)
                Debug.LogError(i);

            p2 = vertices[triangles[i + 2]];
            v01 = p1 - p0;
            v12 = p2 - p1;
            Vector3 vx = Vector3.Cross(v01, v12);

            // Debug.Log(revertNormal);
            // float det = Determinant(p0, p1, p2);
            float det = Determinant(v01, v12, vx);
            if (det * revertNormal < 0)
            {
                int temp = triangles[i + 1];
                triangles[i + 1] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
            else if (det == 0)
            {
                // if ((v01.z + v12.z == 0 && v01.z * v12.z == 0) || (v01.z * v12.z != 0 && v01.x / v01.z == v12.x / v12.z))
                // {
                Debug.LogError(v01.ToString());
                Debug.LogError(v12.ToString());
                Debug.LogError(string.Format("{0},{1},{2}, 三点共线", p0, p1, p2));
                CreatePoint(p0, "dot");
                CreatePoint(p1, "dot");
                CreatePoint(p2, "dot");
                // }
            }
        }
    }

    public static void FixLastTriangleFace(List<Vector3> vertexList, List<int> indexList, int revertNormal = 1)
    {
        int count = indexList.Count;
        int i1 = indexList[count - 3];
        int i2 = indexList[count - 2];
        int i3 = indexList[count - 1];

        Vector3 p0, p1, p2, v01, v12, vx;

        p0 = vertexList[i1];
        p1 = vertexList[i2];
        p2 = vertexList[i3];

        v01 = p1 - p0;
        v12 = p2 - p1;
        vx = Vector3.Cross(v01, v12);

        float det = Determinant(v01, v12, vx);
        if (det * revertNormal < 0)
        {
            indexList[count - 2] = i3;
            indexList[count - 1] = i2;
        }
        else if (det == 0)
        {
            // if ((v01.z + v12.z == 0 && v01.z * v12.z == 0) || (v01.z * v12.z != 0 && v01.x / v01.z == v12.x / v12.z))
            // {
            Debug.LogError(v01.ToString());
            Debug.LogError(v12.ToString());
            Debug.LogError(string.Format("{0},{1},{2}, 三点共线", p0, p1, p2));
            CreatePoint(p0, "dot");
            CreatePoint(p1, "dot");
            CreatePoint(p2, "dot");
            // }
        }
    }

    public static float Determinant(Vector3 a, Vector3 b, Vector3 c)
    {
        float det = a.x * (b.y * c.z - b.z * c.y) + a.y * (b.z * c.x - b.x * c.z) + a.z * (b.x * c.y - b.y * c.x);
        return det;
    }

    public static void CreatePoint(Vector3 pos, string name = "point", float size = 0.5f)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.GetComponent<Renderer>().material.color = Color.red;//颜色
        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * size;
    }

    public static void Log(params object[] args)
    {
        string str = GetLogString(args);
        Debug.Log(str.ToString());
    }

    public static void LogR(params object[] args)
    {
        string str = GetLogString(args);
        Debug.LogError(str.ToString());
    }

    public static string GetLogString(params object[] args)
    {
        StringBuilder sb = new StringBuilder();
        foreach (object o in args)
            sb.Append(o.ToString()).Append("  ");
        return sb.ToString();
    }
}