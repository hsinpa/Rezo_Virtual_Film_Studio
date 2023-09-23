using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayerRowPrefab : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image img;

    public Toggle tgl;

    public MaskData d;
    private MaskControl control;
    void Start()
    {
        tgl.onValueChanged.AddListener(OnToggle);
    }

    public void SetData(MaskData data)
    {
        d = data;
        text.text = d.name;
        d.rowScriptObj = this;
        control.AssignSprite(img, d.type);
    }

    private void OnToggle(bool b)
    {
        if (b)
            control.SetTarget(d);
        else
            control.TryRemoveTarget(d);
    }

    public void SetWithoutTrigger(bool b)
    {
        tgl.onValueChanged.RemoveAllListeners();
        tgl.isOn = b;
        tgl.onValueChanged.AddListener(OnToggle);
    }

    public void SetController(MaskControl c)
    {
        control = c;
    }
}
