using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class 隔空顯示器 : MonoBehaviour
{
    [HideInInspector] [SerializeField] Camera cam = null;
    private void Awake()
    {
        cam = Camera.main;
    }
    // 拉UI物件
    [SerializeField] RectTransform ui;

    [SerializeField] float 寬度 = 500f;
    [SerializeField] float 高度 = 500f;
    [SerializeField] Animator 透視鏡動畫;
    [SerializeField] Animator 按鈕特效;

    public float 最大距離 = 20f;


    private void LateUpdate()
    {
        float distance = Vector3.Distance(cam.transform.position,this.transform.position);

        Vector3 UI位置 = cam.WorldToScreenPoint(transform.position);

        float 中心點X = Screen.width / 2f;
        float 中心點Y = Screen.height / 2f;

        // 範圍內而且玩家有開啟透視鏡就顯示
        if (UI位置.x < 中心點X + (寬度 / 2f) &&
            UI位置.x > 中心點X + ((寬度 / 2f) * -1f) &&
            UI位置.y < 中心點Y + (高度 / 2f) &&
            UI位置.y > 中心點Y + ((高度 / 2f) * -1f) &&
            Player.instance.是否開啟透視鏡 == true &&
            distance < 最大距離)
        {
            ui.anchoredPosition = UI位置;
            ui.localScale = Vector3.one;
            透視鏡動畫.SetBool("Anim", true);
            按鈕特效.SetBool("light 0", true);
        }
        else
        {
            ui.localScale = Vector3.zero;
            透視鏡動畫.SetBool("Anim", false);
            按鈕特效.SetBool("light 0", false);
        }
    }
}
