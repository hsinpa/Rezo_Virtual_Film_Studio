using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MaskControl : MonoBehaviour
{
    [Header("Mask Prefab")]
    public GameObject maskPrefab;
    [Header("Sprites")]
    public Sprite defaultPreview;
    [Header("Mask Sprites")]
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    [Header("Refs")]
    public Camera mainCamera;
    public Canvas mainCanvas;
    public TransformToolCanvasController toolsController;
    public SimpleCamControl camControl;
    public LayerPanel layerPanel;
    public GameObject maskContainer;
    [Header("Object Panel")]
    public TransformBoxPresenter objTransBox;

    public Toggle posTgl;
    public Toggle rotTgl;
    public Toggle sclTgl;

    public Toggle locTgl;

    public Button panelSizeBtn;
    public GameObject objPanelBottomObj;

    public Toggle objTabTgl;
    public Toggle camTabTgl;

    public GameObject objTabRootObj;
    public GameObject camTabRootObj;

    public TransformBoxPresenter camTransBox;

    public Image previewImage;
    [Header("Para")]
    public int MaskLimit = 50;
    //internal data
    //public static MaskControl Inst;

    [Header("Render Object Texture")]
    public RawImage objectImage;
    public Camera objectCamera;
    private RenderTexture objectTexture;
    [Header("Options")]
    public bool bDontSaveCameraPos;

    [HideInInspector]
    public List<MaskData> maskData = new List<MaskData>();

    private MaskData _CurrentTarget;
    private bool _isCameraSetFlag = false;

    private Vector3 loadedCamPos;
    private Vector3 loadedCamRot;

    private string SaveFilePath {
        get
        {
            return $"maskData_{this.gameObject.name}.dat";
        }
    }

    private void OnEnable()
    {
        if (objectCamera == null)
            return;

        objectTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        objectCamera.targetTexture = objectTexture;
        objectCamera.enabled = true;
        objectImage.texture = objectTexture;
        objectImage.color = Color.white;
    }

    private void OnDisable()
    {
        if (objectCamera != null)
        {
            objectCamera.targetTexture = null;
            objectCamera.enabled = false;
        }
        objectImage.color = Color.clear;
        objectImage.texture = null;

        RenderTexture.ReleaseTemporary(objectTexture);
    }

    void Start()
    {
        SetMaskControl(mainCamera);
    }

    private void Update()
    {
        if (mainCamera == null) return;

        if (Input.GetKeyDown("n"))
        {
            SaveObjectTransforms();
        }

        //update obj camera
        if (objectCamera)
        {
            objectCamera.transform.position = mainCamera.transform.position;
            objectCamera.transform.rotation = mainCamera.transform.rotation;
            objectCamera.fieldOfView = mainCamera.fieldOfView;
        }
    }

    private void SetMaskControl(Camera p_camera) {
        if (p_camera == null || _isCameraSetFlag) return;

        mainCamera = p_camera;

        _isCameraSetFlag = true;

        //TOP
        panelSizeBtn.onClick.AddListener(() => objPanelBottomObj.SetActive(!objPanelBottomObj.activeSelf));
        //OBJ PANEL
        posTgl.onValueChanged.AddListener(b => SetTransformType(b, 1));
        rotTgl.onValueChanged.AddListener(b => SetTransformType(b, 2));
        sclTgl.onValueChanged.AddListener(b => SetTransformType(b, 3));

        locTgl.onValueChanged.AddListener(toolsController.SetLocalTransform);

        objTabTgl.onValueChanged.AddListener(b =>
        {
            objTabRootObj.SetActive(b);
            camTabRootObj.SetActive(!b);
        });

        camTabTgl.onValueChanged.AddListener(b =>
        {
            objTabRootObj.SetActive(!b);
            camTabRootObj.SetActive(b);
            camControl.isOn = b;
            if (b)
                camTransBox.SetCurrent(mainCamera.gameObject);
            else
                camTransBox.SetCurrent(null);
        });

        //load save file
        if (!File.Exists(SaveFilePath))
            SaveObjectTransforms();
        else
            LoadObjectTransforms();
    }

    private void OnApplicationQuit()
    {
        SaveObjectTransforms();
    }

    void SetTransformType(bool b, int t)
    {
        if (b)
            toolsController.SwitchTransform(t);
        else
            CheckAllTgls();

        locTgl.interactable = t == 1;
    }

    void CheckAllTgls()
    {
        if (!posTgl.isOn && !rotTgl.isOn && !sclTgl.isOn)
            toolsController.SwitchTransform(0);
    }

    public void TryRemoveTarget(MaskData _target)
    {
        if (_target.scriptObj != _CurrentTarget.scriptObj) return;

        _CurrentTarget = null;
        SetTarget(null);
    }

    public void LookUpTarget(Mask _target)
    {
        foreach (var d in maskData)
        {
            if (_target == d.scriptObj)
            {
                SetTarget(d);
                return;
            }
        }
    }

    public void SetTarget(MaskData _target)
    {
        
        if (_target != null)
        {
            _CurrentTarget = _target;
            toolsController.SetTarget(_target.MaskRect);
            objTransBox.SetCurrent(_target.scriptObj.img.gameObject);
            AssignSprite(previewImage, _CurrentTarget.type);
            previewImage.color = Color.black;
        }
        else
        {
            toolsController.SetTarget(null);
            objTransBox.SetCurrent(null);
            previewImage.sprite = defaultPreview;
            previewImage.color = Color.white;
        }
        
        foreach(var m in maskData)
        {
            if (_target != null && m.scriptObj == _target.scriptObj)
            {
                m.rowScriptObj.SetWithoutTrigger(true);
                m.scriptObj.SetWithoutTrigger(true);
                m.scriptObj.transform.SetAsLastSibling();
                continue;
            }
            m.rowScriptObj.SetWithoutTrigger(false);
            m.scriptObj.SetWithoutTrigger(false);
        }
    }

    public void AddMask(int t)
    {
        if (maskData.Count >= MaskLimit)
        {
            Debug.Log($"Number of mask layers > limited number({MaskLimit})");
            return;
        }
        var data = new MaskData();
        data.name = $"Mask{maskData.Count}";
        data.type = t;

        data.scriptObj = Instantiate(maskPrefab, maskContainer.transform, false).GetComponent<Mask>();
        data.scriptObj.SetController(this);

        AssignSprite(data.scriptObj.img, t);
        AssignSprite(data.scriptObj.cover, t);
        
        maskData.Add(data);
        layerPanel.UpdateData();
    }

    public void RemoveMask(MaskData d)
    {
        SetTarget(null);
        DestroyImmediate(d.scriptObj.gameObject);
        maskData.Remove(d);
        layerPanel.UpdateData();
    }

    public void RemoveCurrentMask()
    {
        if (_CurrentTarget == null) return;
        RemoveMask(_CurrentTarget);
        _CurrentTarget = null;
    }

    public void AssignSprite(Image image, int t)
    {
        switch (t)
        {
            case 1:
                image.sprite = sprite1;
                break;
            case 2:
                image.sprite = sprite2;
                break;
            case 3:
                image.sprite = sprite3;
                break;
            case 4:
                image.sprite = sprite4;
                break;
        }
    }

    public void SaveObjectTransforms()
    {
        if (mainCamera == null) return;

        using (StreamWriter writer = new StreamWriter(SaveFilePath))
        {
            //get camera transform
            CameraData cam = new CameraData();

            cam.pos = new SerializedVector3(mainCamera.transform.position);
            cam.eulerAngles = new SerializedVector3(mainCamera.transform.rotation.eulerAngles);
            if (bDontSaveCameraPos)
            {
                cam.pos = new SerializedVector3(loadedCamPos);
                cam.eulerAngles = new SerializedVector3(loadedCamRot);
            }

            var maskDataJSON = new MaskDataJSON[maskData.Count];

            for (int i = 0; i < maskData.Count; i++)
                maskDataJSON[i] = new MaskDataJSON(maskData[i]);
            
            SaveData saveData = new SaveData();
            saveData.camera = cam;
            saveData.masks = maskDataJSON;

            writer.Write(JsonUtility.ToJson(saveData));

        }

        Debug.Log("saved to " + SaveFilePath);
    }

    public void LoadObjectTransforms()
    {
        if (!File.Exists(SaveFilePath)) return;
        var stream = File.OpenRead(SaveFilePath);

        StreamReader reader = new StreamReader(stream);

        var saveData = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());

        loadedCamPos = saveData.camera.pos.ToVec3();
        loadedCamRot = saveData.camera.eulerAngles.ToVec3();

        this.mainCamera.transform.position = loadedCamPos;
        this.mainCamera.transform.eulerAngles = loadedCamRot;

        LoadMask(saveData.masks);

        reader.Close();
    }

    public void LoadMask(MaskDataJSON[] dataArray)
    {
        for(int i = 0; i < dataArray.Length; i++)
        {
            var data = dataArray[i];

            var md = new MaskData();
            md.name = $"Mask{i}";
            md.type = data.type;

            md.scriptObj = Instantiate(maskPrefab, maskContainer.transform, false).GetComponent<Mask>();
            md.scriptObj.SetController(this);
            md.ApplyTransform(data);

            AssignSprite(md.scriptObj.img, md.type);
            AssignSprite(md.scriptObj.cover, md.type);

            maskData.Add(md);
        }
        layerPanel.UpdateData();
    }
}
