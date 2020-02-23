using UnityEditor;
using UnityEngine;

public class Chapter02
{
    [MenuItem("Raytracing/Chapter02")]
    public static void Main()
    {
        int nx = 1280;
        int ny = 720;

        Texture2D tex = ImageUtils.CreateImg(nx, ny);
        // pixels are written from left to right, from top to bottom
        for (int j = ny - 1; j >= 0; j--)
        {
            for (int i = 0; i < nx; i++)
            {
                float r = i / (float) nx;
                float g = j / (float) ny;
                float b = 0.2f;
                ImageUtils.SetPixel(tex, i, j, r, g, b);
            }
        }

        ImageUtils.SaveImg(tex, "OutputImg/chapter02.png");
    }
}