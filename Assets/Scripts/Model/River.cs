using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : TerrianMesh
{
    public River(List<int> el, List<int> er, int zs, int zt)
    {
        Init();

        zStep = zs;
        zTop = zt;

        CreateUpperVertices(el, er);
        CreateUpperTriangles();

        // detFactor = -1;
        UpdateMesh("MyWorld/River");

        gameObject.name = "river";
        transform.position = new Vector3(0, 0, 0);
        // transform.localEulerAngles = new Vector3(0, 0, 180);
    }
}
