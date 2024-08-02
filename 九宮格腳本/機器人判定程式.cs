using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class 機器人判定程式 : MonoBehaviour, Item
{
    [SerializeField] Lockpattern 九宮格的程式;
    [SerializeField] List<int> 九宮格的答案 = new List<int>();
    [SerializeField] UnityEvent 觸發事件;
    [SerializeField] Sprite 答案圖 = null;
    [SerializeField] Sprite 答案圖起點 = null;

    [SerializeField] 九宮格題目[] 題目 = new 九宮格題目[0];

    public bool 可被駭入 = false;
    public bool 第一門檻;
    public bool 卡住;

    public void 互動()
    {
        if (可被駭入 == false)
        {
            Debug.Log("駭入失敗!!!");
            return;
        }

        Debug.Log("機器人可以互動");
        觸發事件.Invoke();
        if(第一門檻 == true)
        {
            for (int i = 0; i < 題目.Length; i++)
            {
                九宮格的程式.添加題目(題目[i]);
            }
            卡住 = false;
        }

        if(卡住 == false)
        {
            第一門檻 = false;
        }

    }

}
[System.Serializable]
public struct 九宮格題目
{
    public List<int> 九宮格答案;
    public Sprite 答案圖;
    public Sprite 答案圖起點;
    public 九宮格題目 (List<int> 九宮格答案, Sprite 答案圖, Sprite 答案圖起點)
    {
        this.九宮格答案 = 九宮格答案;
        this.答案圖 = 答案圖;
        this.答案圖起點 = 答案圖起點;
    }
}