using UnityEditor;
using UnityEngine;

/// <summary>
/// Ray 类用 Unity 原生带的
/// </summary>
public class Chapter04
{
    private static Vector3 topColor = Vector3.one;
    private static Vector3 bottomColor = new Vector3(0.5f, 0.7f, 1.0f);

    public static Vector3 GetColor(Ray ray)
    {
        Vector3 unitDirection = ray.direction.normalized;
        float t = 0.5f * (unitDirection.y + 1); // -1~1 -> 0~1
        return Vector3.Lerp(topColor, bottomColor, t);
    }

    [MenuItem("Raytracing/Chapter04")]
    public static void Main()
    {
        int nx = 1280;
        int ny = 720;

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

        ImageUtils.SaveImg(tex, "OutputImg/chapter04.png");
    }
}