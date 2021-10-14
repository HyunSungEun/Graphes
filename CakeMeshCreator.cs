using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeMeshCreator : MonoBehaviour
{
    public Transform parent;
    public GameObject testPrefab, testCakePrefab;
    public float smoothFactor;
    public float radius=1f;
    public float height;
    public float centralAngle;

    /// <summary>
    /// xz평면이 바닥
    /// </summary>
    /// <param name="height"></param>
    /// <param name="radius"></param>
    /// <param name="centralAngle"></param>
    /// <param name="smoothFactor"></param>
    public static Mesh GetCake(float height, float radius, float centralAngle,float smoothFactor)
    {
        Mesh mesh = new Mesh();

        Vector3 downCenter = Vector3.zero;
        Vector3 upCenter = Vector3.up * height;
        List<Vector3> upArc = new List<Vector3>();
        List<Vector3> downArc = new List<Vector3>();
        float theta = 0f;
        while (theta < centralAngle)
        {
            float x = Mathf.Cos(theta * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(theta * Mathf.Deg2Rad) * radius;
            downArc.Add(new Vector3(x, 0f, z));
            upArc.Add(new Vector3(x, height, z));
            theta += 1f*1/smoothFactor;
        }
        float cx = Mathf.Cos(centralAngle * Mathf.Deg2Rad) * radius;
        float cz = Mathf.Sin(centralAngle * Mathf.Deg2Rad) * radius;
        downArc.Add(new Vector3(cx, 0f, cz));
        upArc.Add(new Vector3(cx, height, cz));

        int[] triangles = new int[(4 //옆면
            + ((upArc.Count - 1) * 2) //윗면 아랫면
            + ((upArc.Count - 1) * 2))*3]; //Arc부분(사각형이라 *2)
        Vector3[] vertices = new Vector3[2 + (upArc.Count * 2)];
        vertices[0] = downCenter;
        vertices[1] = upCenter;


        Vector3[] normals = new Vector3[vertices.Length];


        int downArcVCursur = 2;
        int upArcVCursur = downArcVCursur;
        foreach (Vector3 v in downArc)
        {
            vertices[upArcVCursur] = v;
            upArcVCursur++;
        }
        int tempCursur = upArcVCursur;
        foreach (Vector3 v in upArc)
        {
            vertices[tempCursur] = v;
            tempCursur++;
        }
        Debug.Log(string.Format("버텍스수_{0},다운점커서_{1},업점커서_{2},다운끝_{3},업끝_{4},다운카운트_{5}", vertices.Length, downArcVCursur, upArcVCursur,
            downArcVCursur + downArc.Count-1, upArcVCursur + upArc.Count-1,downArc.Count
            ));
        
        //옆면
        SetTriangle(triangles, 0, 0, 1, upArcVCursur);
        SetTriangle(triangles, 3, 0,  upArcVCursur,downArcVCursur);
        SetTriangle(triangles, 6, 0, upArcVCursur+upArc.Count-1, 1);
        SetTriangle(triangles, 9, 0, downArcVCursur+downArc.Count-1, upArcVCursur + upArc.Count-1);
       

        Vector3 middleAngleV = new Vector3(Mathf.Cos(centralAngle / 2f * Mathf.Deg2Rad), 0f, Mathf.Sin(centralAngle / 2f * Mathf.Deg2Rad));

        /* 노멀 주변점 계산
        normals[0] = (Vector3.down+ (-1f* middleAngleV)).normalized;
        Debug.Log(normals[0]);
        normals[1] = (Vector3.up + (-1f * middleAngleV)).normalized;
        //  Debug.DrawLine(vertices[0], vertices[0] + normals[0], Color.red,5f);
        // Debug.DrawLine(vertices[1], vertices[1] + normals[1], Color.red, 5f);
      

        for(int i= downArcVCursur+1;i< downArcVCursur + downArc.Count; i++)
        {
            normals[i] =   ((vertices[i]-vertices[0])+Vector3.down).normalized;
        }
        for (int i = upArcVCursur + 1; i < upArcVCursur + upArc.Count; i++)
        {
            normals[i] = ((vertices[i] - vertices[1]) + Vector3.up).normalized;
        } 
        
        normals[downArcVCursur] = (Vector3.right + Vector3.back*(1-(centralAngle/ 360f)) + Vector3.down).normalized;
        normals[upArcVCursur] = (Vector3.right +Vector3.back * (1 - (centralAngle / 360f)) + Vector3.up).normalized;

        normals[downArcVCursur+downArc.Count-1] = (
            Vector3.right 
            +Vector3.back * (1 - (centralAngle / 360f)) + Vector3.down).normalized;
        */



        //윗면
        int faceTriangleCursur=12;
        int upCursur = upArcVCursur;
        int upEndCursur = upArcVCursur + upArc.Count - 1;
        while(upCursur < upEndCursur)
        {
            SetTriangle(triangles, faceTriangleCursur, upCursur, 1, upCursur+1);
            faceTriangleCursur += 3;
            upCursur ++;
        }
        //아랫면
        int downCursur = downArcVCursur;
        int downEndCursur = downArcVCursur + downArc.Count - 1;
        while (downCursur < downEndCursur)
        {
            SetTriangle(triangles, faceTriangleCursur, 0, downCursur, downCursur + 1);
            faceTriangleCursur += 3;
            downCursur++;
        }
        upCursur = upArcVCursur; downCursur = downArcVCursur;
        while (upCursur < upEndCursur)
        {
            SetTriangle(triangles, faceTriangleCursur, upCursur,  upCursur + 1,downCursur);
            faceTriangleCursur += 3;
            upCursur++;
            downCursur++;
        }
        upCursur = upArcVCursur+1; downCursur = downArcVCursur;
        while (upCursur < upEndCursur+1)
        {
            SetTriangle(triangles, faceTriangleCursur, downCursur, upCursur, downCursur+1);
            faceTriangleCursur += 3;
            upCursur++;
            downCursur++;
        }

        for(int i = 0; i < normals.Length; i++)
        {
            Vector3 center = Vector3.up * height / 2f;
            normals[i] = (vertices[i] - center).normalized;
        }
        normals[0] = (Vector3.down + (-1f * middleAngleV)).normalized;
        Debug.Log(normals[0]);
        normals[1] = (Vector3.up + (-1f * middleAngleV)).normalized;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
       // mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
    static void SetTriangle(int[] triangles,int cursur,int a,int b, int c)
    {
        triangles[cursur] = a;
        triangles[cursur+1] = b;
        triangles[cursur+2] = c;
    }


    [ContextMenu("시험")]
    public void Test()
    {
        Mesh mesh = testCakePrefab.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh = GetCake(height, radius, centralAngle, smoothFactor);
        testCakePrefab.GetComponent<MeshFilter>().mesh = mesh;
        Vector3[] vs = mesh.vertices;
        Vector3[] ns = mesh.normals;
        int i = 0;
        foreach(var v in vs)
        {
            Debug.DrawLine(vs[i], vs[i] + ns[i], Color.red, 3f);
            i++;
        }
    }


    [ContextMenu("원 체크해보기")]
    public void qw()
    {
        float theta = 0f;
        while (theta <= 270)
        {
            float x = Mathf.Cos(theta * Mathf.Deg2Rad)* radius;
            float y = Mathf.Sin(theta * Mathf.Deg2Rad)* radius;
            GameObject temp= Instantiate<GameObject>(testPrefab, parent);
            temp.transform.position = new Vector3(x, y, 0f);
            theta += 1f;
        }
    }
}
