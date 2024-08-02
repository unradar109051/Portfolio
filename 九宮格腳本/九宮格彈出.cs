using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 九宮格彈出 : MonoBehaviour
{
    [SerializeField] GameObject 操作面板;
    [SerializeField] GameObject 右搖桿;
    [SerializeField] CanvasGroup 九宮格UI;
    [SerializeField] Lockpattern 九宮格程式;

    public bool 是否正在執行九宮格 = false;
    public void 互動()
    {
        //Debug.Log("九宮格可以彈出");
        操作面板.SetActive(false);
        右搖桿.SetActive(false );
        九宮格UI.alpha = 1;
        九宮格UI.blocksRaycasts = true;
        九宮格程式.答案正確 = false;
        是否正在執行九宮格 = true;

        ResetCountdown();
        //倒數計時的協程開始
        StartCoroutine(Countdown());
    }

    //倒數計時功能的協程
    public int 初始倒數時間 = 5;
    public int 倒數時間;
    public Text 倒數文字;
    IEnumerator Countdown()
    {
        倒數時間 = 初始倒數時間;
        倒數文字.text = string.Format(倒數時間.ToString("00"));

        while (倒數時間 > 0)
        {
            yield return new WaitForSeconds(1);

            倒數時間--;

            倒數文字.text = string.Format(倒數時間.ToString("00"));

            if (九宮格程式.答案正確 == true || 是否取消入侵 == true)
            {
                yield break;                
            }

            else if (倒數時間 <= 0 && 九宮格程式.答案正確 == false)
            {
                九宮格程式.關閉();
                操作面板.SetActive(true);
            }
        }

        yield return new WaitForSeconds(1);

    }

    public void ResetCountdown()
    {
        倒數時間 = 初始倒數時間;
        倒數文字.text = string.Format(倒數時間.ToString("00"));
    }

    bool 是否取消入侵 = false;

    public void 取消入侵()
    {
        是否取消入侵 = true;
    }
}
