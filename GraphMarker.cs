using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class GraphMarker : MonoBehaviour
{
    LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = true;
    }
    public void DrawLine(Transform tr) => DrawLine(new Pose(tr.position, tr.rotation));
    public void DrawLine(Pose pose)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, pose.position);
    }
}
