using UnityEngine;
using System.Collections.Generic;

public class DrawTool
{
    private static Material _commonMaterial = null;

    private static Material CommonMaterial
    {
        get
        {
            if (_commonMaterial == null)
            {
                _commonMaterial = Resources.Load("TestAttackRange") as Material;
            }
            return _commonMaterial;
        }
    }

    private static LineRenderer GetLineRenderer(Transform t)
    {
        LineRenderer lr = t.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = t.gameObject.AddComponent<LineRenderer>();
            lr.material = CommonMaterial;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.widthMultiplier = 0.1f;
        }

        return lr;
    }


    public static void DrawPoint(Vector3 pos, string name = "point", float size = 0.5f)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.GetComponent<Renderer>().material.color = Color.red;//颜色
        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * size;
    }

    public static void DrawLine(Transform t, Vector3 start, Vector3 end)
    {
        LineRenderer lr = GetLineRenderer(t);
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    private static void DrawLine(Transform t, Vector3 start, Vector3 end, Material mat)
    {
        LineRenderer lr = GetLineRenderer(t);
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = mat;
    }

    //绘制空心扇形  
    public static void DrawSector(Transform t, Vector3 dir, Vector3 center, float angle, float radius)
    {
        LineRenderer lr = GetLineRenderer(t);
        int pointAmount = 100;//点的数目，值越大曲线越平滑  
        float eachAngle = angle / pointAmount;

        lr.positionCount = pointAmount;
        lr.SetPosition(0, center);
        lr.SetPosition(pointAmount - 1, center);

        for (int i = 1; i < pointAmount - 1; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * dir * radius + center;
            lr.SetPosition(i, pos);
        }
    }

    //绘制空心圆  
    public static void DrawCircle(Transform t, Vector3 center, float radius)
    {
        LineRenderer lr = GetLineRenderer(t);
        int pointAmount = 100;//点的数目，值越大曲线越平滑  
        float eachAngle = 360f / pointAmount;
        Vector3 forward = t.forward;

        lr.positionCount = pointAmount + 1;

        for (int i = 0; i <= pointAmount; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, eachAngle * i, 0f) * forward * radius + center;
            lr.SetPosition(i, pos);
        }
    }

    //绘制空心长方形
    //以长方形的中点为攻击方位置(从俯视角度来看)
    public static void DrawRectangle(Transform t, Vector3 centerPos, float length, float width)
    {
        LineRenderer lr = GetLineRenderer(t);
        lr.positionCount = 4;

        lr.SetPosition(0, centerPos - Vector3.forward * (length / 2) - Vector3.right * (width / 2));
        lr.SetPosition(1, centerPos - Vector3.forward * (length / 2) + Vector3.right * (width / 2));
        lr.SetPosition(2, centerPos + Vector3.forward * (length / 2) + Vector3.right * (width / 2));
        lr.SetPosition(3, centerPos + Vector3.forward * (length / 2) - Vector3.right * (width / 2));
        //lr.SetPosition(4, centerPos - t.right * (width / 2));
    }

    //绘制空心长方形2D
    //distance指的是这个长方形与Transform t的中心点的距离
    public static void DrawRectangle2D(Transform t, float distance, float length, float width)
    {
        LineRenderer lr = GetLineRenderer(t);
        lr.positionCount = 5;

        if (MathTool.IsFacingRight(t))
        {
            Vector2 forwardMiddle = new Vector2(t.position.x + distance, t.position.y);
            lr.SetPosition(0, forwardMiddle + new Vector2(0, width / 2));
            lr.SetPosition(1, forwardMiddle + new Vector2(length, width / 2));
            lr.SetPosition(2, forwardMiddle + new Vector2(length, -width / 2));
            lr.SetPosition(3, forwardMiddle + new Vector2(0, -width / 2));
            lr.SetPosition(4, forwardMiddle + new Vector2(0, width / 2));
        }
        else
        {
            Vector2 forwardMiddle = new Vector2(t.position.x - distance, t.position.y);
            lr.SetPosition(0, forwardMiddle + new Vector2(0, width / 2));
            lr.SetPosition(1, forwardMiddle + new Vector2(-length, width / 2));
            lr.SetPosition(2, forwardMiddle + new Vector2(-length, -width / 2));
            lr.SetPosition(3, forwardMiddle + new Vector2(0, -width / 2));
            lr.SetPosition(4, forwardMiddle + new Vector2(0, width / 2));
        }
    }


    public static GameObject go;
    public static MeshFilter mf;
    public static MeshRenderer mr;
    public static Shader shader;
    private static GameObject CreateMesh(List<Vector3> vertices)
    {
        int[] triangles;
        Mesh mesh = new Mesh();

        int triangleAmount = vertices.Count - 2;
        triangles = new int[3 * triangleAmount];

        //根据三角形的个数，来计算绘制三角形的顶点顺序（索引）    
        //顺序必须为顺时针或者逆时针    
        for (int i = 0; i < triangleAmount; i++)
        {
            triangles[3 * i] = 0;//固定第一个点    
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        if (go == null)
        {
            go = new GameObject("mesh");
            go.transform.position = new Vector3(0, 0.1f, 0);//让绘制的图形上升一点，防止被地面遮挡
            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();
            shader = Shader.Find("Unlit/AttackRange");
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;

        mf.mesh = mesh;
        mr.material.shader = shader;
        mr.material.color = Color.red;

        return go;
    }

    //绘制实心扇形  
    public static void DrawSectorSolid(Transform t, Vector3 center, float angle, float radius)
    {
        int pointAmount = 100;//点的数目，值越大曲线越平滑  
        float eachAngle = angle / pointAmount;
        Vector3 forward = t.forward;

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(center);

        for (int i = 1; i < pointAmount - 1; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * radius + center;
            vertices.Add(pos);
        }

        CreateMesh(vertices);
    }

    //绘制实心圆  
    public static void DrawCircleSolid(Transform t, Vector3 center, float radius)
    {
        int pointAmount = 100;//点的数目，值越大曲线越平滑  
        float eachAngle = 360f / pointAmount;
        Vector3 forward = t.forward;

        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i <= pointAmount; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, eachAngle * i, 0f) * forward * radius + center;
            vertices.Add(pos);
        }

        CreateMesh(vertices);
    }

    //绘制实心长方形
    //以长方形的底边中点为攻击方位置(从俯视角度来看)
    public static void DrawRectangleSolid(Transform t, Vector3 bottomMiddle, float length, float width)
    {
        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(bottomMiddle - t.right * (width / 2));
        vertices.Add(bottomMiddle - t.right * (width / 2) + t.forward * length);
        vertices.Add(bottomMiddle + t.right * (width / 2) + t.forward * length);
        vertices.Add(bottomMiddle + t.right * (width / 2));

        CreateMesh(vertices);
    }

    //绘制实心长方形2D
    //distance指的是这个长方形与Transform t的中心点的距离
    public static void DrawRectangleSolid2D(Transform t, float distance, float length, float width)
    {
        List<Vector3> vertices = new List<Vector3>();

        if (MathTool.IsFacingRight(t))
        {
            Vector3 forwardMiddle = new Vector3(t.position.x + distance, t.position.y);
            vertices.Add(forwardMiddle + new Vector3(0, width / 2));
            vertices.Add(forwardMiddle + new Vector3(length, width / 2));
            vertices.Add(forwardMiddle + new Vector3(length, -width / 2));
            vertices.Add(forwardMiddle + new Vector3(0, -width / 2));
        }
        else
        {
            //看不到颜色但点击mesh可以看到形状
            Vector3 forwardMiddle = new Vector3(t.position.x - distance, t.position.y);
            vertices.Add(forwardMiddle + new Vector3(0, width / 2));
            vertices.Add(forwardMiddle + new Vector3(-length, width / 2));
            vertices.Add(forwardMiddle + new Vector3(-length, -width / 2));
            vertices.Add(forwardMiddle + new Vector3(0, -width / 2));
        }

        CreateMesh(vertices);
    }

    public static GameObject DrawArrow(Vector3 startPos, Vector3 endPos, Color color, float arrowSize, string name)
    {
        color = color == null ? Color.red : color;
        
        GameObject go = new GameObject(name);
        MeshFilter filter = go.AddComponent<MeshFilter>();
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        filter.mesh = mesh;
        int n = 12;
        Vector3[] vertices = new Vector3[n + 2];
        Vector3 v = new Vector3(arrowSize, arrowSize / 3, 0);
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, startPos - endPos);

        vertices[0] = endPos;
        vertices[n + 1] = (startPos - endPos).normalized * arrowSize + endPos;
        for (int i = 1; i <= n; i++)
        {
            // vertices[i] = vertices[0]+ (startPos - endPos).normalized*d/3
            Vector3 vi = Quaternion.AngleAxis(360 * i / n, Vector3.right) * v;
            vertices[i] = rot * vi + endPos;
        }
        mesh.vertices = vertices;
        int[] triangles = new int[2 * 3 * n];
        int count = 0;
        for (int i = 1; i <= n; i++)
        {
            int j = i == n ? 1 : i + 1;
            triangles[count] = 0;
            triangles[count + 1] = j;
            triangles[count + 2] = i;
            triangles[count + 3] = n + 1;
            triangles[count + 4] = i;
            triangles[count + 5] = j;
            count = count + 6;
        }
        mesh.triangles = triangles;
        renderer.material = new Material(Shader.Find("Unlit/Color"));
        renderer.material.SetColor("_Color", color);
        DrawLine(go.transform, startPos, endPos, renderer.material);

        return go;
    }

}