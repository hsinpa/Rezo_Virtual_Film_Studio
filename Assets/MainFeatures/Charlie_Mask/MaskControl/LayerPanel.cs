using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerPanel : MonoBehaviour
{
    public Button expButton;

    public Button addButton;
    public Button rmvButton;

    public GameObject bottomPanelObj;

    public GameObject spriteMenu;

    public GameObject container;

    public GameObject rowPrefab;

    public MaskControl control;

    void Start()
    {
        //TOP
        expButton.onClick.AddListener(() => bottomPanelObj.SetActive(!bottomPanelObj.activeSelf));
        addButton.onClick.AddListener(() => spriteMenu.SetActive(true));
        rmvButton.onClick.AddListener(() => control.RemoveCurrentMask());
    }

    public void UpdateData()
    {
        //foreach (Transform child in container.transform)
        //    DestroyImmediate(child.gameObject);

        while (container.transform.childCount > 0)
            DestroyImmediate(container.transform.GetChild(0).gameObject);

        foreach (var d in control.maskData)
        {
            var r = Instantiate(rowPrefab, container.transform, false);
            r.GetComponent<LayerRowPrefab>().SetController(control);
            r.GetComponent<LayerRowPrefab>().SetData(d);
        }
    }
}
