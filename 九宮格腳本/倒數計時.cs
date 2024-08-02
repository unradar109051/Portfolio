using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 倒數計時 : MonoBehaviour
{
    public int 倒數時間;
    public Text 倒數文字;
    public Lockpattern 九宮格;
    public bool test;
    public int 初始倒數 = 5;

    private void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        倒數文字.text = string.Format("0", 倒數時間.ToString("00"));
        倒數時間 = 初始倒數;

        while (倒數時間 > 0)
        {           

            yield return new WaitForSeconds(1);

            倒數時間--;

            倒數文字.text = string.Format("0", 倒數時間.ToString("00"));

            if(倒數時間>0 && 九宮格.倒數時間 == true)
            {
                break;
            }
            else if(倒數時間 == 0 && 九宮格.倒數時間 == false)
            {
                九宮格.關閉();
            }

        }

        yield return new WaitForSeconds(1);



    }
}
