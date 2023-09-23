using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransformBoxPresenter : MonoBehaviour
{
    public TMP_InputField posX, posY, posZ;
    public TMP_InputField rotateX, rotateY, rotateZ;
    public TMP_InputField scaleX, scaleY, scaleZ;

    private GameObject currentObj;
    private bool keepUpdate = true;
    
    void Start()
    {
        SetListeners();
    }
    
    void Update()
    {
        if (!keepUpdate) return;
        if (!this.gameObject.activeInHierarchy) return;

        SetCurrent(currentObj);
    }

    public void ClearData()
    {
        currentObj = null;
        keepUpdate = true;
        posX.text    = "" + 0;
        posY.text    = "" + 0;
        posZ.text    = "" + 0;
        rotateX.text = "" + 0;
        rotateY.text = "" + 0;
        rotateZ.text = "" + 0;
        scaleX.text  = "" + 0;
        scaleY.text  = "" + 0;
        scaleZ.text  = "" + 0;
    }

    private void OnEditStart(string s)
    {
        keepUpdate = false;
    }

    public void SetCurrent(GameObject o)
    {
        if (o == null) return;

        currentObj = o;

        posX.text = "" + currentObj.transform.localPosition.x;
        posY.text = "" + currentObj.transform.localPosition.y;
        posZ.text = "" + currentObj.transform.localPosition.z;

        rotateX.text = "" + FloatToZero(currentObj.transform.localEulerAngles.x);
        rotateY.text = "" + FloatToZero(currentObj.transform.localEulerAngles.y);
        rotateZ.text = "" + FloatToZero(currentObj.transform.localEulerAngles.z);

        scaleX.text = "" + currentObj.transform.localScale.x;
        scaleY.text = "" + currentObj.transform.localScale.y;
        scaleZ.text = "" + currentObj.transform.localScale.z;
    }

    private float FloatToZero(float f)
    {
        return Mathf.Abs(f) < 0.00001f ? 0f : f;
    }

    private void SetListeners()
    {
        posX.onEndEdit.AddListener(s => TransformObject(0, 0, s));
        posY.onEndEdit.AddListener(s => TransformObject(0, 1, s));
        posZ.onEndEdit.AddListener(s => TransformObject(0, 2, s));

        rotateX.onEndEdit.AddListener(s => TransformObject(1, 0, s));
        rotateY.onEndEdit.AddListener(s => TransformObject(1, 1, s));
        rotateZ.onEndEdit.AddListener(s => TransformObject(1, 2, s));

        scaleX.onEndEdit.AddListener(s => TransformObject(2, 0, s));
        scaleY.onEndEdit.AddListener(s => TransformObject(2, 1, s));
        scaleZ.onEndEdit.AddListener(s => TransformObject(2, 2, s));

        posX.onSelect.AddListener(OnEditStart);
        posY.onSelect.AddListener(OnEditStart);
        posZ.onSelect.AddListener(OnEditStart);

        rotateX.onSelect.AddListener(OnEditStart);
        rotateY.onSelect.AddListener(OnEditStart);
        rotateZ.onSelect.AddListener(OnEditStart);

        scaleX.onSelect.AddListener(OnEditStart);
        scaleY.onSelect.AddListener(OnEditStart);
        scaleZ.onSelect.AddListener(OnEditStart);
    }

    private void TransformObject(int type, int axis, string val)
    {
        float newVal = 0.0f;

        if (!float.TryParse(val, out newVal))
        {
            SetCurrent(currentObj);
            return;
        }

        Vector3 vec = new Vector3(0, 0, 0);

        switch(type)
        {
            case 0:
                vec = currentObj.transform.localPosition;
                break;
            case 1:
                vec = currentObj.transform.localEulerAngles;
                break;
            case 2:
                vec = currentObj.transform.localScale;
                break;
        }

        switch (axis)
        {
            case 0:
                vec.x = newVal;
                break;
            case 1:
                vec.y = newVal;
                break;
            case 2:
                vec.z = newVal;
                break;
        }

        switch (type)
        {
            case 0:
                currentObj.transform.localPosition = vec;
                break;
            case 1:
                currentObj.transform.localEulerAngles = vec;
                break;
            case 2:
                currentObj.transform.localScale = vec;
                break;
        }

        keepUpdate = true;
    }
}
