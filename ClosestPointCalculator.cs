using UnityEngine;

public class ClosestPointCalculator : MonoBehaviour
{
    public Vector3 GetClosestPoint(Vector3 point, MeshCollider meshCollider)
    {
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Transform transform = meshCollider.transform;

        Vector3 closestPoint = Vector3.zero;
        float closestDistanceSqr = Mathf.Infinity;

        point = transform.InverseTransformPoint(point);

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Vector3 closest = ClosestPointOnTriangleToPoint(point, v0, v1, v2);
            float distanceSqr = (point - closest).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestPoint = closest;
            }
        }

        return transform.TransformPoint(closestPoint);
    }

    private Vector3 ClosestPointOnTriangleToPoint(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ap = point - a;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);

        if (d1 <= 0f && d2 <= 0f)
            return a;

        Vector3 bp = point - b;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);

        if (d3 >= 0f && d4 <= d3)
            return b;

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0f && d1 >= 0f && d3 <= 0f)
        {
            float v = d1 / (d1 - d3);
            return a + v * ab;
        }

        Vector3 cp = point - c;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);

        if (d6 >= 0f && d5 <= d6)
            return c;

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0f && d2 >= 0f && d6 <= 0f)
        {
            float w = d2 / (d2 - d6);
            return a + w * ac;
        }

        float va = d3 * d6 - d5 * d4;
        if (va <= 0f && (d4 - d3) >= 0f && (d5 - d6) >= 0f)
        {
            float u = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return b + u * (c - b);
        }

        float denom = 1f / (va + vb + vc);
        float v1 = vb * denom;
        float v2 = vc * denom;

        return a + ab * v1 + ac * v2;
    }
}
