using UnityEditor;
using UnityEngine;

public class Chapter06_1
{
    private static Vector3 topColor = Vector3.one;
    private static Vector3 bottomColor = new Vector3(0.5f, 0.7f, 1.0f);

    private static Vector3 center = new Vector3(0, 0, -1);
    private static float radius = 0.5f;

    // 一元二次方程求解，根据方程有无根来判断射线是否跟圆相交
    public static float HitSphere(Vector3 center, float radius, Ray ray)
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
        if (d < 0)
        {
            return -1;
        }
        else
        {
            // 返回距离最近的根
            return (-b - Mathf.Sqrt(d)) / (2 * a);
        }
    }

    public static Vector3 GetColor(Ray ray)
    {
        var t = HitSphere(center, radius, ray);
        if (t > 0)
        {
            // 圆心到交点的向量就是法线方向
            // [−1, 1] 映射到 [0, 1]
            Vector3 normal = (ray.GetPoint(t) - center).normalized;
            return 0.5f * (normal + Vector3.one);
        }
           
        Vector3 unitDirection = ray.direction.normalized;
         t = 0.5f * (unitDirection.y + 1); // -1~1 -> 0~1
        return Vector3.Lerp(topColor, bottomColor, t);
    }

    [MenuItem("Raytracing/Chapter06_1")]
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

        ImageUtils.SaveImg(tex, "OutputImg/chapter06_1.png");
    }
}