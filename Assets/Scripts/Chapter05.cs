using UnityEditor;
using UnityEngine;

public class Chapter05
{
    private static Vector3 topColor = Vector3.one;
    private static Vector3 bottomColor = new Vector3(0.5f, 0.7f, 1.0f);
    private static Vector3 ballColor = new Vector3(1, 0, 0);

    private static Vector3 center = new Vector3(0, 0, -1);
    private static float radius = 0.5f;

    // 一元二次方程求解，根据方程有无根来判断射线是否跟圆相交
    public static bool HitSphere(Vector3 center, float radius, Ray ray)
    {
        // 射线向量：p(t)= A + t * B
        // a = dot(B, B)
        // b = 2 * dot(B, A - C)
        // c = 2 * dot(A - C, A - C) - R * R
        // oc = A - C
        Vector3 oc = ray.origin - center;
        float a = Vector3.Dot(ray.direction, ray.direction);
        float b = 2.0f * Vector3.Dot(ray.direction, oc);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float d = b * b - 4 * a * c;
        return d > 0;
    }

    public static Vector3 GetColor(Ray ray)
    {
        if (HitSphere(center, radius, ray))
            return ballColor;
        Vector3 unitDirection = ray.direction.normalized;
        float t = 0.5f * (unitDirection.y + 1); // -1~1 -> 0~1
        return Vector3.Lerp(topColor, bottomColor, t);
    }

    [MenuItem("Raytracing/Chapter05")]
    public static void Main()
    {
        int nx = 1280;
        int ny = 640;

        Vector3 lowerLeftCorner = new Vector3(-2.0f, -1.0f, -1.0f);
        Vector3 horizontal = new Vector3(4.0f, 0.0f, 0.0f);
        Vector3 vertical = new Vector3(0.0f, 2.0f, 0.0f);
        Vector3 origin = Vector3.zero;

        Texture2D tex = ImageUtils.CreateImg(nx, ny);
        // pixels are written from left to right, from top to bottom
        for (int j = ny - 1; j >= 0; j--)
        {
            for (int i = 0; i < nx; i++)
            {
                float u = i / (float) nx;
                float v = j / (float) ny;
                Vector3 direction = lowerLeftCorner + u * horizontal + v * vertical;
                Ray ray = new Ray(origin, direction);
                Vector3 color = GetColor(ray);
                ImageUtils.SetPixel(tex, i, j, color);
            }
        }

        ImageUtils.SaveImg(tex, "OutputImg/chapter05.png");
    }
}