using UnityEngine;

[System.Serializable]
public class SaveData
{
    public CameraData camera;
    public MaskDataJSON[] masks;
}

[System.Serializable]
public class CameraData
{
    public SerializedVector3 pos;
    public SerializedVector3 eulerAngles;
}

[System.Serializable]
public class MaskDataJSON
{
    public MaskDataJSON(MaskData m)
    {
        name = m.name;
        type = m.type;
        pos = new SerializedVector3(m.MaskRect.position);
        eulerAngles = new SerializedVector3(m.MaskRect.eulerAngles);
        scale = new SerializedVector3(m.MaskRect.localScale);
    }

    public string name;
    public int type;
    public SerializedVector3 pos;
    public SerializedVector3 eulerAngles;
    public SerializedVector3 scale;
}

[System.Serializable]
public class SerializedVector3
{
    public float x;
    public float y;
    public float z;

    public SerializedVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVec3()
    {
        return new Vector3(x, y, z);
    }
}

public class MaskData
{
    public string name;
    public int type;
    public Mask scriptObj;

    public LayerRowPrefab rowScriptObj;

    public RectTransform MaskRect {
        get
        {
            var r = scriptObj.img.GetComponent<RectTransform>();
            return (r != null) ? r : null;
        }
    }

    public void ApplyTransform(MaskDataJSON data)
    {
        MaskRect.position = data.pos.ToVec3();
        MaskRect.eulerAngles = data.eulerAngles.ToVec3();
        MaskRect.localScale = data.scale.ToVec3();
    }
}
