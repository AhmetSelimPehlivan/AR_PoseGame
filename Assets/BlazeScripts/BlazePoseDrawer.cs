using System;
using UnityEngine;
using TensorFlowLite;

public class BlazePoseDrawer : IDisposable
{
    private readonly PrimitiveDraw draw;
    private readonly Camera camera;
    private readonly Vector4[] viewportLandmarks;

    public BlazePoseDrawer(Camera camera, int layer)
    {
        draw = new PrimitiveDraw(camera, layer);
        this.camera = camera;
        viewportLandmarks = new Vector4[PoseLandmarkDetect.LandmarkCount];
    }

    public void Dispose()
    {
        draw.Dispose();
    }

    public void DrawLandmarkResult(PoseLandmarkDetect.Result result, float visibilityThreshold, float zOffset)
    {
        if (result == null)
        {
            return;
        }

        draw.color = Color.blue;

        Vector4[] landmarks = result.viewportLandmarks;
        // Update world joints
        for (int i = 0; i < landmarks.Length; i++)
        {
            Vector3 p = camera.ViewportToWorldPoint(landmarks[i]);
            viewportLandmarks[i] = new Vector4(p.x, p.y, p.z + zOffset, landmarks[i].w);
        }

        // Draw
        for (int i = 0; i < viewportLandmarks.Length; i++)
        {
            Vector4 p = viewportLandmarks[i];
            if (p.w > visibilityThreshold)
            {
                draw.Cube(p, 0.2f);
            }
        }
        var connections = PoseLandmarkDetect.Connections;
        for (int i = 0; i < connections.Length; i += 2)
        {
            var a = viewportLandmarks[connections[i]];
            var b = viewportLandmarks[connections[i + 1]];
            if (a.w > visibilityThreshold || b.w > visibilityThreshold)
            {
                draw.Line3D(a, b, 0.05f);
            }
        }
        draw.Apply();
    }
}

