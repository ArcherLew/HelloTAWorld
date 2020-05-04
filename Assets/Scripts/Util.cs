using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    /// <param name="detFactor">若是封闭图形，则默认为1；否则则取决于视角方向，-1为取反</param>
    public static void FixFaceToClockWise(Vector3[] vertices, int[] triangles, int detFactor = 1)
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

            Debug.Log(detFactor);
            if (Determinant(p0, p1, p2) <0)//* detFactor < 0)
            {
                int temp = triangles[i + 1];
                triangles[i + 1] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
            else
            {
                if ((v01.z + v12.z == 0 && v01.z * v12.z == 0) || (v01.z * v12.z != 0 && v01.x / v01.z == v12.x / v12.z))
                {
                    Debug.LogError(v01.ToString());
                    Debug.LogError(v12.ToString());
                    Debug.LogError(string.Format("{0},{1},{2}, 三点共线", p0, p1, p2));
                    CreatePoint(p0, "dot");
                    CreatePoint(p1, "dot");
                    CreatePoint(p2, "dot");
                }
            }
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
}