using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MyMesh
{
    int xStep = 5;
    int zStep = 0;
    int zTop = 0;

    List<List<Vector3>> waterVertices;


    public River(List<int> el, List<int> er, int zs, int zt)
    {
        zStep = zs;
        zTop = zt;

        waterVertices = new List<List<Vector3>>();
        vertexList = new List<Vector3>();
        indexList = new List<int>();

        CreateWaterVertices(el, er);
        CreateWaterTriangles();

        UpdateMesh("MyWorld/River");

        gameObject.name = "river";
        transform.position = new Vector3(0, 2f, 0);
        transform.localEulerAngles = new Vector3(0, 0, 180);
    }

    private void CreateWaterVertices(List<int> el, List<int> er)
    {
        int z = zTop;
        for (int i = 0; i < el.Count; i++, z -= zStep)
        {
            List<Vector3> strip = new List<Vector3>();
            waterVertices.Add(strip);

            for (int x = el[i]; x <= er[i]; x += xStep)
            {
                strip.Add(new Vector3(x, 0, z));

                int d = er[i] - x;
                if (d > 0 && d < 5)
                    strip.Add(new Vector3(er[i], 0, z));
            }
        }
    }

    private void CreateWaterTriangles()
    {
        for (int i = 0; i < waterVertices.Count - 1; i++)
        {
            CreateStripTriangles(waterVertices[i], waterVertices[i + 1]);
        }
    }

    // 缓存已经存储到 vertexList 里的 vertex 在 vertexList 里的 index，避免重复保存顶点数据
    private Dictionary<Vector3, int> vertex2Index = new Dictionary<Vector3, int>();

    /// <summary>
    /// 将一个未保存过的顶点存入到 vertexList，并建立索引到 vertex2Index
    /// </summary>
    /// <param name="v">顶点</param>
    /// <returns>顶点在vertexList 里的 index</returns>
    private int TryAddVertex(Vector3 v)
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

    private void CreateStripTriangles(List<Vector3> line1, List<Vector3> line2)
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
                if (line1[i + 1].x <= line2[j + 1].x)
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

        }
    }
}
