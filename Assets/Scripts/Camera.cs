using UnityEngine;

public class RayCamera
{
    private Vector3 origin;
    private Vector3 lowerLeftCorner;
    private Vector3 horizontal;
    private Vector3 vertical;


    public RayCamera()
    {
        origin = Vector3.zero;
        lowerLeftCorner = new Vector3(-2.0f, -1.0f, -1.0f);
        horizontal = new Vector3(4, 0, 0);
        vertical = new Vector3(0, 2, 0);
    }

    public RayCamera(Vector3 ori, Vector3 corner, Vector3 h, Vector3 v)
    {
        origin = ori;
        lowerLeftCorner = corner;
        horizontal = h;
        vertical = v;
    }

    public RayCamera(float fov, float aspect)
    {
        float theta = Mathf.Deg2Rad * fov;
        float halfHeight = Mathf.Tan(theta * 0.5f);
        float halfWidth = aspect * halfHeight;
        lowerLeftCorner = new Vector3(-halfWidth, -halfHeight, -1.0f);
        horizontal = new Vector3(2 * halfWidth, 0, 0);
        vertical = new Vector3(0, 2 * halfHeight, 0);
        origin = Vector3.zero;
    }

    public RayCamera(Vector3 lookFrom, Vector3 lookAt, Vector3 vup, float fov, float aspect)
    {
        Vector3 u, v, w;
        float theta = Mathf.Deg2Rad * fov;
        float halfHeight = Mathf.Tan(theta * 0.5f);
        float halfWidth = aspect * halfHeight;
        origin = lookFrom;
        w = (lookFrom - lookAt).normalized;
        u = Vector3.Cross(vup, w).normalized;
        v = Vector3.Cross(w, u);

        lowerLeftCorner = new Vector3(-halfWidth, -halfHeight, -1.0f);
        lowerLeftCorner = origin - halfWidth * u - halfHeight * v - w;
        horizontal = 2 * halfWidth * u;
        vertical = 2 * halfHeight * v;
    }


    public Ray GetRay(float u, float v)
    {
        return new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
    }
}