using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformToolCanvasController : MonoBehaviour
{
    public RectTransform target;

    public GameObject posObj;
    public GameObject rotObj;
    public GameObject sclObj;

    public float ScaleSpeed = 0.01f;
    public Image[] AlphaImages;

    public bool drawGiz = false;
    public bool localTransform = false;

    private Vector2 BeginDragPoint;
    private Vector2 DraggingPoint;
    private Vector3 BeginDragRot;
    private int CurrentType = 1;
    
    void Start()
    {
        Array.ForEach(AlphaImages, x => x.alphaHitTestMinimumThreshold = 1.0f);
    }

    void Update()
    {
        if (target == null) return;

        this.GetComponent<RectTransform>().position = target.position;

        if ((localTransform && CurrentType==1) || CurrentType == 3)
            this.transform.rotation = target.rotation;
        else
            this.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    
    void OnDrawGizmos()
    {
        if (!drawGiz) return;
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(BeginDragPoint, target.position);
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(DraggingPoint, target.position);

        Gizmos.color = Color.red;
        var baseAxis = Quaternion.AngleAxis(target.localRotation.eulerAngles.z, Vector3.forward) * new Vector2(1, 0);
        Gizmos.DrawLine(target.position + new Vector3(baseAxis.x, baseAxis.y, 0)*200, target.position);
    }

    public void SwitchTransform(int type)
    {
        CurrentType = type;

        if (target == null)
        {
            posObj.SetActive(false);
            rotObj.SetActive(false);
            sclObj.SetActive(false);
            return;
        }

        switch (type)
        {
            case 1:
                posObj.SetActive(true);
                rotObj.SetActive(false);
                sclObj.SetActive(false);
                break;
            case 2:
                posObj.SetActive(false);
                rotObj.SetActive(true);
                sclObj.SetActive(false);
                break;
            case 3:
                posObj.SetActive(false);
                rotObj.SetActive(false);
                sclObj.SetActive(true);
                break;
            default:
                posObj.SetActive(false);
                rotObj.SetActive(false);
                sclObj.SetActive(false);
                break;
        }
    }

    public void SetTarget(RectTransform _target)
    {   
        target = _target;

        if (target == null)
        {
            posObj.SetActive(false);
            rotObj.SetActive(false);
            sclObj.SetActive(false);
            return;
        }

        SwitchTransform(CurrentType);
    }

    public void SetLocalTransform(bool b)
    {
        localTransform = b;
    }

    public void OnPosDragX(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        var pos = target.anchoredPosition;

        if (localTransform)
        {
            var baseAxis = Quaternion.AngleAxis(target.localRotation.eulerAngles.z, Vector3.forward) * new Vector2(1, 0);
            var baseAxis2 = new Vector2(baseAxis.x, baseAxis.y);
            //var dragDir = new Vector3(PED.position.x, PED.position.y, 0) - target.position;
            var dragDir = PED.delta;
            dragDir = dragDir.normalized;
            
            var deg = Vector2.Angle(baseAxis2, dragDir);
            
            if (deg < 90f)
            {
                pos.x += baseAxis.normalized.x * PED.delta.magnitude;
                pos.y += baseAxis.normalized.y * PED.delta.magnitude;
            }
            else if(deg> 90f)
            {
                pos.x -= baseAxis.normalized.x * PED.delta.magnitude;
                pos.y -= baseAxis.normalized.y * PED.delta.magnitude;
            }
        }
        else
            pos.x += PED.delta.x;

        target.anchoredPosition = pos;
    }

    public void OnPosDragY(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        var pos = target.anchoredPosition;

        if (localTransform)
        {
            var baseAxis = Quaternion.AngleAxis(target.localRotation.eulerAngles.z, Vector3.forward) * new Vector2(0, 1);
            var baseAxis2 = new Vector2(baseAxis.x, baseAxis.y);
            //var dragDir = new Vector3(PED.position.x, PED.position.y, 0) - target.position;
            var dragDir = PED.delta;
            dragDir = dragDir.normalized;

            var deg = Vector2.Angle(baseAxis2, dragDir);

            if (deg < 90f)
            {
                pos.x += baseAxis.normalized.x * PED.delta.magnitude;
                pos.y += baseAxis.normalized.y * PED.delta.magnitude;
            }
            else if (deg > 90f)
            {
                pos.x -= baseAxis.normalized.x * PED.delta.magnitude;
                pos.y -= baseAxis.normalized.y * PED.delta.magnitude;
            }
        }
        else
            pos.y += PED.delta.y;

        target.anchoredPosition = pos;
    }

    public void OnPosDragBoth(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        var pos = target.anchoredPosition;
        pos += PED.delta;
        target.anchoredPosition = pos;
    }

    public void OnRingDragBegin(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        
        BeginDragPoint = PED.position;
        BeginDragRot = target.localRotation.eulerAngles;
    }

    public void OnRingDrag(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        DraggingPoint = PED.position;

        var origin = new Vector2(target.position.x, target.position.y);
        var vec1 = BeginDragPoint - origin;
        var vec2 = DraggingPoint - origin;

        Vector3 newEulers = BeginDragRot;
        newEulers.z += Vector2.SignedAngle(vec1, vec2);

        target.eulerAngles = newEulers;
    }

    public void OnScaleDragX(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        var scl = target.localScale;
        scl.x += PED.delta.x * ScaleSpeed;
        target.localScale = scl;
    }

    public void OnScaleDragY(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        var scl = target.localScale;
        scl.y += PED.delta.y * ScaleSpeed;
        target.localScale = scl;
    }

    float dragDistToTarget;
    public void OnScaleDragBothBegin(BaseEventData data)
    {
        if (target == null) return;
        PointerEventData PED = data as PointerEventData;
        Vector2 tar2d = new Vector2(target.position.x, target.position.y);
        dragDistToTarget = (PED.position - tar2d).magnitude;
    }

    public void OnScaleDragBoth(BaseEventData data)
    {
        if (target == null) return;

        PointerEventData PED = data as PointerEventData;
        var scl = target.localScale;
        /*
        Vector2 tar2d = new Vector2(target.position.x, target.position.y);
        float dragDistToTarget2 = (PED.position - tar2d).magnitude;

        if(dragDistToTarget2 - dragDistToTarget > 0)
        {
            scl.x += PED.delta.magnitude * ScaleSpeed;
            scl.y += PED.delta.magnitude * ScaleSpeed;
        }
        else
        {
            scl.x -= PED.delta.magnitude * ScaleSpeed;
            scl.y -= PED.delta.magnitude * ScaleSpeed;
        }
        dragDistToTarget = dragDistToTarget2;
        */
        var deg = Vector2.Angle(PED.delta.normalized, new Vector2(1, 1));
        if (deg < 90)
        {
            scl.x += PED.delta.magnitude * ScaleSpeed;
            scl.y += PED.delta.magnitude * ScaleSpeed;
        }
        else
        {
            scl.x -= PED.delta.magnitude * ScaleSpeed;
            scl.y -= PED.delta.magnitude * ScaleSpeed;
        }

        target.localScale = scl;
    }
}
