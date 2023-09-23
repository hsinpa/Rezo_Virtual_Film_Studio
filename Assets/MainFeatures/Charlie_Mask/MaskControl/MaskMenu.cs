using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskMenu : MonoBehaviour
{
    public Button closeBtn;

    public Button spriteButton1;
    public Button spriteButton2;
    public Button spriteButton3;
    public Button spriteButton4;

    public MaskControl control;

    void Start()
    {
        closeBtn.onClick.AddListener(() => this.gameObject.SetActive(false));
        spriteButton1.onClick.AddListener(() => CreateMask(1));
        spriteButton2.onClick.AddListener(() => CreateMask(2));
        spriteButton3.onClick.AddListener(() => CreateMask(3));
        spriteButton4.onClick.AddListener(() => CreateMask(4));
    }

    private void CreateMask(int t)
    {
        control.AddMask(t);
        //this.gameObject.SetActive(false);
    }
}
