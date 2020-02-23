using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chapter07
{
    private static Vector3 topColor = Vector3.one;
    private static Vector3 bottomColor = new Vector3(0.5f, 0.7f, 1.0f);

    public struct HitRecord
    {
        public float t;
        public Vector3 hitPoint;
        public Vector3 normal;
    }

    public interface Hitable
    {
        bool Hit(Ray r, ref float tMin, ref float tMax, out HitRecord record);
    }

    public class Sphere : Hitable
    {
        public Vector3 center;
        public float radius;

        public Sphere()
        {
            center = Vector3.zero;
            radius = 1;
        }

        public Sphere(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }


        public bool Hit(Ray ray, ref float tMin, ref float tMax, out HitRecord record)
        {
            record = new HitRecord();

            // 射线向量：p(t)= A + t * B
            // a = dot(B, B)
            // b = 2 * dot(B, A - C)
            // c = 2 * dot(A - C, A - C) - R * R
            // oc = A - C
            Vector3 oc = ray.origin - center;
            float a = Vector3.Dot(ray.direction, ray.direction);
            float b = Vector3.Dot(ray.direction, oc);
            float c = Vector3.Dot(oc, oc) - radius * radius;
            // b 计算的时候移除了前面的乘以2，这里 4ac 前面的 4 也就不需要了，节省计算量
            float discriminant = b * b - a * c;
            if (discriminant > 0)
            {
                // 如果有两个根 分别判断他们在不在 tmin~tmax之内
                // 第一个根
                float temp = (-b - Mathf.Sqrt(discriminant)) / a;
                if (temp < tMax && temp > tMin)
                {
                    record.t = temp;
                    record.hitPoint = ray.GetPoint(temp);
                    record.normal = (record.hitPoint - center) / radius; // normalize
                    return true;
                }

                // 第二个根
                temp = (-b + Mathf.Sqrt(discriminant)) / a;
                if (temp < tMax && temp > tMin)
                {
                    record.t = temp;
                    record.hitPoint = ray.GetPoint(temp);
                    record.normal = (record.hitPoint - center) / radius; // normalize
                    return true;
                }
            }

            return false;
        }
    }

    public class HitList : Hitable
    {
        private List<Hitable> list = new List<Hitable>();

        public HitList()
        {
        }

        public int GetCount()
        {
            return list.Count;
        }

        public void Add(Hitable item)
        {
            list.Add(item);
        }

        public bool Hit(Ray ray, ref float tMin, ref float tMax, out HitRecord record)
        {
            HitRecord tempRecord = new HitRecord();
            record = tempRecord;
            bool hitAnything = false;
            float closestSoFar = tMax;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].Hit(ray, ref tMin, ref closestSoFar, out tempRecord))
                {
                    hitAnything = true;
                    closestSoFar = tempRecord.t;
                    record = tempRecord;
                }
            }

            return hitAnything;
        }
    }

    public static Vector3 GetColor(Ray ray, Hitable world)
    {
        HitRecord rec;
        float min = 0;
        float max = float.MaxValue;

        if (world.Hit(ray, ref min, ref max, out rec))
        {
            return 0.5f * (rec.normal.normalized + Vector3.one);
        }

        Vector3 unitDirection = ray.direction.normalized;
        float t = 0.5f * (unitDirection.y + 1); // -1~1 -> 0~1
        return Vector3.Lerp(topColor, bottomColor, t);
    }

    [MenuItem("Raytracing/Chapter07")]
    public static void Main()
    {
        int nx = 1280;
        int ny = 640;
        int ns = 64; // 一个像素需要采样64次

        RayCamera camera = new RayCamera();

        HitList list = new HitList();
        list.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));
        list.Add(new Sphere(new Vector3(0, -100.5f, -1), 100));

        Texture2D tex = ImageUtils.CreateImg(nx, ny);
        // pixels are written from left to right, from top to bottom
        for (int j = ny - 1; j >= 0; j--)
        {
            for (int i = 0; i < nx; i++)
            {
                Vector3 color = Vector3.zero;
                // 对于每个射线，随机采样周围的8格，用平均颜色来作为该射线对应像素的颜色
                // 一次采样需要解2个一元二次方程组。也就是说，一共需要解1280X640X64X2=104857600个一元二次方程组。
                for (int k = 0; k < ns; k++)
                {
                    float u = (i + Random.Range(-1f, 1f)) / nx;
                    float v = (j + Random.Range(-1f, 1f)) / ny;
                    Ray ray = camera.GetRay(u, v);
                    color += GetColor(ray, list);
                }

                color = color / ns;
                ImageUtils.SetPixel(tex, i, j, color);
            }
        }

        ImageUtils.SaveImg(tex, "OutputImg/chapter07.png");
    }
}