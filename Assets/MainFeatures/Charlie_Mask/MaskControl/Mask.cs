using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mask : MonoBehaviour
{
    public Image img, cover;
    
    public Toggle tgl;

    private MaskControl control;

    void Start()
    {
        tgl.onValueChanged.AddListener(OnToggle);
    }

    public void Unselect()
    {
        if (control.toolsController.target == img.rectTransform) control.SetTarget(null);
    }

    public void OnToggle(bool b)
    {
        if (b)
            control.LookUpTarget(this);
        else
            Unselect();
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
