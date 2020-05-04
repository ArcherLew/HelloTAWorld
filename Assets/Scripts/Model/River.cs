using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MyMesh
{
    int xStep = 5;
    int zStep = 0;
    int zTop = 0;

    List<List<Vector3>> strips;
    List<Vector3> upperTopStrip, upperBottomStrip, upperLeftStrip, upperRightStrip;
    List<Vector3> lowerTopStrip, lowerBottomStrip, lowerLeftStrip, lowerRightStrip;
    Vector3 lowerTL, lowerTR, lowerBL, lowerBR;

    public River(List<int> el, List<int> er, int zs, int zt)
    {
        zStep = zs;
        zTop = zt;

        strips = new List<List<Vector3>>();

        upperLeftStrip = new List<Vector3>();
        upperRightStrip = new List<Vector3>();

        lowerTopStrip = new List<Vector3>();
        lowerBottomStrip = new List<Vector3>();
        lowerLeftStrip = new List<Vector3>();
        lowerRightStrip = new List<Vector3>();

        vertexList = new List<Vector3>();
        indexList = new List<int>();

        CreateWaterVertices(el, er);
        CreateWaterTriangles();

        // detFactor = -1;
        UpdateMesh("MyWorld/River");

        gameObject.name = "river";
        transform.position = new Vector3(0, 2f, 0);
        // transform.localEulerAngles = new Vector3(0, 0, 180);
    }

    private void CreateWaterVertices(List<int> el, List<int> er)
    {
        int z = zTop;
        for (int i = 0; i < el.Count; i++, z -= zStep)
        {
            List<Vector3> strip = new List<Vector3>();
            strips.Add(strip);


            for (int x = el[i]; x <= er[i]; x += xStep)
            {
                strip.Add(new Vector3(x, 0, z));

                if (x == el[i])
                    upperLeftStrip.Add(strip[strip.Count - 1]);
            }
            if (strip[strip.Count - 1].x != er[i])
                strip.Add(new Vector3(er[i], 0, z));

            upperRightStrip.Add(strip[strip.Count - 1]);
        }

        upperTopStrip = strips[0];
        upperBottomStrip = strips[strips.Count - 1];

        foreach (Vector3 v in upperTopStrip)
            lowerTopStrip.Add(new Vector3(v.x, -10, v.z));
        foreach (Vector3 v in upperBottomStrip)
            lowerBottomStrip.Add(new Vector3(v.x, -10, v.z));
        foreach (Vector3 v in upperLeftStrip)
            lowerLeftStrip.Add(new Vector3(v.x, -10, v.z));
        foreach (Vector3 v in upperRightStrip)
            lowerRightStrip.Add(new Vector3(v.x, -10, v.z));

        // lowerTL = lowerTopStrip[0];
        // lowerTR = lowerTopStrip[lowerTopStrip.Count - 1];
        // lowerBL = lowerBottomStrip[0];
        // lowerBR = lowerBottomStrip[lowerTopStrip.Count - 1];
    }

    private void CreateWaterTriangles()
    {
        for (int i = 0; i < strips.Count - 1; i++)
        {
            CreateStripsTriangles(strips[i], strips[i + 1]);
        }

        CreateStripsTriangles(upperTopStrip, lowerTopStrip, 'x');
        CreateStripsTriangles(upperBottomStrip, lowerBottomStrip, 'x');
        CreateStripsTriangles(upperLeftStrip, lowerLeftStrip, 'z');
        CreateStripsTriangles(upperRightStrip, lowerRightStrip, 'z');

        CreateStripCornerTriangles(upperTopStrip, lowerLeftStrip[1], lowerRightStrip[1]);
        CreateStripCornerTriangles(upperBottomStrip, lowerLeftStrip[lowerLeftStrip.Count - 2], lowerRightStrip[lowerRightStrip.Count - 2]);

        for (int i = 1; i < lowerLeftStrip.Count - 2; i++)
            CreateQuadTriangles(lowerLeftStrip[i], lowerRightStrip[i], lowerLeftStrip[i + 1], lowerRightStrip[i + 1]);
    }

    /// <summary>
    /// 创建水面三角形
    /// </summary>
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    private void CreateStripsTriangles(List<Vector3> line1, List<Vector3> line2, char comparator = 'x')
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

        }
    }

    /// <summary>
    /// 创建水体四周三角形
    /// </summary>
    /// <param name="strip"></param>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    private void CreateStripCornerTriangles(List<Vector3> strip, Vector3 corner1, Vector3 corner2)
    {
        int index1, index2, index3;
        for (int i = 0; i < strip.Count - 1; i++)
        {
            index1 = TryAddVertex(strip[i]);
            index2 = TryAddVertex(strip[i + 1]);
            index3 = TryAddVertex(corner1);

            indexList.Add(index1);
            indexList.Add(index2);
            indexList.Add(index3);
        }

        index1 = TryAddVertex(strip[strip.Count - 1]);
        index2 = TryAddVertex(corner1);
        index3 = TryAddVertex(corner2);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
    }

    /// <summary>
    /// 创建水体底面四边形
    /// </summary>
    /// <param name="lowerTL"></param>
    /// <param name="lowerTR"></param>
    /// <param name="lowerBL"></param>
    /// <param name="lowerBR"></param>
    private void CreateQuadTriangles(Vector3 lowerTL, Vector3 lowerTR, Vector3 lowerBL, Vector3 lowerBR)
    {
        int index1, index2, index3;
        index1 = TryAddVertex(lowerTL);
        index2 = TryAddVertex(lowerTR);
        index3 = TryAddVertex(lowerBL);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);

        index1 = TryAddVertex(lowerTR);
        index2 = TryAddVertex(lowerBL);
        index3 = TryAddVertex(lowerBR);

        indexList.Add(index1);
        indexList.Add(index2);
        indexList.Add(index3);
    }

}
