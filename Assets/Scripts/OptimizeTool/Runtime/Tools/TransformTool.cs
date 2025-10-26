using UnityEngine;

public static class TransformTool
{
    public static void SetTransform(this Transform t, Vector3 position, Quaternion rotation, LocalOrWorld localOrWorld)
    {
        if (localOrWorld == LocalOrWorld.Local)
        {
            t.SetLocalPositionAndRotation(position, rotation);
        }
        else
        {
            t.SetPositionAndRotation(position, rotation);
        }
    }

    public static void SetTransform(this Transform t, Vector3 position, Quaternion rotation, Vector3 scale, LocalOrWorld localOrWorld)
    {
        P.BeginSample("Transform.SetTransform2");
        t.SetTransform(position, rotation, localOrWorld);
        t.SetScale(scale);
        P.EndSample();
    }

    public static void SetScale(this Transform t, Vector3 scale)
    {
        if (t.localScale != scale)
        {
            t.localScale = scale;
        }
    }

    public static void SetParentAndTransform(this Transform t, Transform parent, Vector3 scale)
    {
        t.SetParent(parent, false);
        t.SetScale(scale);
    }

    public static void SetParentAndTransform(this Transform t, Transform parent, Vector3 position, Quaternion rotation, LocalOrWorld localOrWorld)
    {
        t.SetParent(parent, false);
        t.SetTransform(position, rotation, localOrWorld);
    }

    public static void SetParentAndTransform(this Transform t, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, LocalOrWorld localOrWorld)
    {
        P.BeginSample("Transform.SetParentAndTransform");
        t.SetParent(parent, false);
        t.SetTransform(position, rotation, scale, localOrWorld);
        P.EndSample();
    }

    public static void SetParentAndReset(this Transform t, Transform parent)
    {
        P.BeginSample("Transform.SetParentAndReset");
        t.SetParent(parent, false);
        t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (t.localScale != Vector3.one)
        {
            t.localScale = Vector3.one;
        }
        P.EndSample();
    }

    public static void SetReset(this Transform t)
    {
        t.SetTransform(Vector3.zero, Quaternion.identity, Vector3.one, LocalOrWorld.Local);
    }
}