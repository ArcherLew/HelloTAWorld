using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : TerrianMesh
{
    public Ground(List<int> el, List<int> er, int zs, int zt)
    {
        Init();

        zStep = zs;
        zTop = zt;

        CreateUpperVertices(el, er);
        // SetSlope();
        CreateUpperTriangles();

        // revertNormal = -1;
        UpdateMesh("MyWorld/Ground");

        gameObject.name = "ground";
        transform.position = new Vector3(0, 0, 0);
        // transform.localEulerAngles = new Vector3(0, 0, 180);
    }

    private List<float> heightList = new List<float>()
        { 1.0f,  0.985f,  0.94f,  0.866f, 0.766f, 0.642f, 0.5f, 0.342f, 0.173f, 0.087f};

    private float topHeight = 3.0f;

    private void SetSlope()
    {
        int z = Random.Range(0, strips.Count - 1);
        int x = Random.Range(0, strips[z].Count - 1);

        Vector3 v = strips[z][x];
        v.Set(v.x, topHeight, v.z);

        for (int i = 1; i < heightList.Count; i++)
        {
            int jMax = Mathf.FloorToInt(Mathf.Sqrt(heightList.Count ^ 2 - i ^ 2));
            int _z = z + i;
            if (_z < strips.Count - 1)
            {
                for (int j = 1; j < jMax; j++)
                {
                    int _x = x + j;
                    if (_x < strips[_z].Count)
                    {
                        int k = Mathf.FloorToInt(Mathf.Sqrt(i ^ 2 + j ^ 2));
                        float y = heightList[k] * topHeight;
                        v = strips[_z][_x];
                        strips[_z][_x] = new Vector3(v.x, y, v.z);
                    }
                }
            }

            _z = z - i;
            if (_z >= 0)
            {
                for (int j = 1; j < jMax; j++)
                {
                    int _x = x - j;
                    if (_x >= 0)
                    {
                        int k = Mathf.FloorToInt(Mathf.Sqrt(i ^ 2 + j ^ 2));
                        float y = heightList[k] * topHeight;
                        v = strips[_z][_x];
                        strips[_z][_x] = new Vector3(v.x, y, v.z);
                    }
                }
            }
        }
    }
    /*
        int xStep = 5;
        int zStep = 0;
        int zTop = 0;

        List<Vector3> edgeL;
        List<Vector3> edgeR;

        public Ground(List<int> el, List<int> er, int zs, int zt)
        {
            zStep = zs;
            zTop = zt;

            edgeL = new List<Vector3>();
            edgeR = new List<Vector3>();

            vertexList = new List<Vector3>();
            indexList = new List<int>();

            // CreateGroundVertices(el, er);
            // CreateGroundTriangles();
            CreateGroundVertices(el, er);
            return;

            CreateStripTriangles(edgeL, edgeR);

            revertNormal = -1;
            UpdateMesh();

            gameObject.name = "ground";
            transform.position = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 180);
        }

        private void CreateGroundVertices(List<int> el, List<int> er)
        {
            int z = zTop;
            for (int i = 0; i < el.Count; i++, z -= zStep)
            {
                edgeL.Add(new Vector3(el[i], 0, z));
            }

            z = zTop;
            for (int i = 0; i < er.Count; i++, z -= zStep)
            {
                edgeR.Add(new Vector3(er[i], 0, z));
            }

            foreach (Vector3 pos in edgeL)
                Util.CreatePoint(pos);
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
        */
}