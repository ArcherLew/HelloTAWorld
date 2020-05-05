using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrianMesh : MyMesh
{
    protected int xStep = 5;
    protected int zStep = 0;
    protected int zTop = 0;
    protected int yUpper = 0;
    protected List<List<Vector3>> strips;
    protected List<Vector3> upperTopStrip, upperBottomStrip, upperLeftStrip, upperRightStrip;
    protected List<Vector3> lowerTopStrip, lowerBottomStrip, lowerLeftStrip, lowerRightStrip;


    protected void Init()
    {
        strips = new List<List<Vector3>>();

        upperLeftStrip = new List<Vector3>();
        upperRightStrip = new List<Vector3>();

        lowerTopStrip = new List<Vector3>();
        lowerBottomStrip = new List<Vector3>();
        lowerLeftStrip = new List<Vector3>();
        lowerRightStrip = new List<Vector3>();

        vertexList = new List<Vector3>();
        indexList = new List<int>();
    }

    protected void CreateUpperVertices(List<int> el, List<int> er)
    {
        int z = zTop;
        for (int i = 0; i < el.Count; i++, z -= zStep)
        {
            List<Vector3> strip = new List<Vector3>();
            strips.Add(strip);


            for (int x = el[i]; x <= er[i]; x += xStep)
            {
                strip.Add(new Vector3(x, yUpper, z));

                if (x == el[i])
                    upperLeftStrip.Add(strip[strip.Count - 1]);
            }
            if (strip[strip.Count - 1].x != er[i])
                strip.Add(new Vector3(er[i], yUpper, z));

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

    }

    protected void CreateUpperTriangles()
    {
        // 上面
        for (int i = 0; i < strips.Count - 1; i++)
        {
            CreateStripsTriangles(strips[i], strips[i + 1], -1);
        }

        // 侧面上下左右
        CreateStripsTriangles(upperTopStrip, lowerTopStrip, 1, 'x');
        CreateStripsTriangles(upperBottomStrip, lowerBottomStrip, -1, 'x');
        CreateStripsTriangles(upperLeftStrip, lowerLeftStrip, -1, 'z');
        CreateStripsTriangles(upperRightStrip, lowerRightStrip, 1, 'z');

        // 底面
        CreateStripCornerTriangles(lowerTopStrip, lowerLeftStrip[1], lowerRightStrip[1], -1);
        CreateStripCornerTriangles(lowerBottomStrip, lowerLeftStrip[lowerLeftStrip.Count - 2], lowerRightStrip[lowerRightStrip.Count - 2], 1);

        for (int i = 1; i < lowerLeftStrip.Count - 2; i++)
            CreateQuadTriangles(lowerLeftStrip[i], lowerRightStrip[i], lowerLeftStrip[i + 1], lowerRightStrip[i + 1], -1);
    }
}
