using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Lockpattern : MonoBehaviour
{
    public static Lockpattern Instance;
    //抓取線條物件
    public GameObject linePrefab;

    //抓取畫布
    public Canvas canvas;

    private Dictionary<int, Circleldentifier> circles;

    private List<Circleldentifier> lines;

    private GameObject lineOnEdit;
    private RectTransform lineOnEditRcTs;
    private Circleldentifier circleOnEdit;

    //設立是否進行解鎖動作
    public bool unlocking;

    new bool enabled = true;

    public bool 倒數時間 = false;

    List<九宮格題目> 未完成的題目 = new List<九宮格題目>();

    public void 添加題目 (九宮格題目 題目)
    {
        未完成的題目.Add(題目);
        //目標陣列 = 九宮格的答案;
        //指定參考圖(答案圖);
        //指定參考圖起點(答案圖起點);
        設定一道題目();
    }

    void 設定一道題目()
    {
        if (未完成的題目 != null && 未完成的題目.Count > 0)
        {
            目標陣列 = 未完成的題目[0].九宮格答案;
            指定參考圖(未完成的題目[0].答案圖);
            指定參考圖起點(未完成的題目[0].答案圖起點);
        }
    }

    public void 清空題目()
    {
        未完成的題目.Clear();
    }

    void Start()
    {
        circles = new Dictionary<int, Circleldentifier>();
        lines = new List<Circleldentifier>();


        for (int i = 0; i < transform.childCount; i++)
        {
            var circle = transform.GetChild(i);

            var identifier = circle.GetComponent<Circleldentifier>();

            identifier.id = i;

            circles.Add(i, identifier);
        }

        倒數時間 = false;

    }


    // Update is called once per frame
    void Update()
    {
        if (enabled == false)
        {
            return;
        }

        if (unlocking)
        {
            Vector3 mousePos = canvas.transform.InverseTransformPoint(Input.mousePosition);

            //讓線可以跟著滑鼠位置往前
            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(mousePos, circleOnEdit.transform.localPosition));

            //讓線可以轉彎
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (mousePos - circleOnEdit.transform.localPosition).normalized);
        }

    }

    IEnumerator Release()
    {
        enabled = false;

        yield return new WaitForSeconds(3);

        foreach (var circle in circles)
        {
            circle.Value.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            circle.Value.GetComponent<Animator>().enabled = false;
        }

        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();

        lineOnEdit = null;
        lineOnEditRcTs = null;
        circleOnEdit = null;

        enabled = true;
    }

    GameObject CreateLine(Vector3 pos, int id)
    {
        var line = GameObject.Instantiate(linePrefab, canvas.transform);

        line.transform.localPosition = pos;

        var lineIdf = line.AddComponent<Circleldentifier>();

        lineIdf.id = id;

        lines.Add(lineIdf);

        return line;
    }


    void TrySetLineEdit(Circleldentifier circle)
    {
        foreach (var line in lines)
        {
            if (line.id == circle.id)
            {
                return;
            }
        }

        lineOnEdit = CreateLine(circle.transform.localPosition, circle.id);
        lineOnEditRcTs = lineOnEdit.GetComponent<RectTransform>();
        circleOnEdit = circle;
    }

    //處理顏色變化動畫
    void EnableColorFade(Animator anim)
    {
        anim.enabled = true;
        anim.Rebind();
    }

    //建立滑鼠進入圓圈的事件觸發
    public void OnMouseEnterCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        //Debug.Log(idf.id);
        if (unlocking)
        {
            //當線接觸到其他的圓圈的時候，則圓圈生產的中心點將轉移到新的圓圈身上
            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(circleOnEdit.transform.localPosition, idf.transform.localPosition));
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (idf.transform.localPosition - circleOnEdit.transform.localPosition).normalized);

            TrySetLineEdit(idf);
        }
    }

    //建立滑鼠離開圓圈的事件觸發
    public void OnMouseExitCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        //Debug.Log(idf.id);
    }

    //建立滑鼠點下圓圈的事件觸發
    public void OnMouseDownCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        //Debug.Log(idf.id);

        unlocking = true;
        參考圖.gameObject.SetActive(false);

        TrySetLineEdit(idf);

    }

    //建立滑鼠放開圓圈的事件觸發
    public void OnMouseUpCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        結果陣列.Clear(); //清除資料

        //Debug.Log(idf.id);

        if (unlocking)
        {
            foreach (var line in lines)
            {
                EnableColorFade(circles[line.id].gameObject.GetComponent<Animator>());
                //Debug.Log(line.id);
                結果陣列.Add(line.id); //添加資料
            }

            Destroy(lines[lines.Count - 1].gameObject);
            lines.RemoveAt(lines.Count - 1);

            foreach (var line in lines)
            {
                EnableColorFade(line.GetComponent<Animator>());

            }

            StartCoroutine(Release());
        }

        unlocking = false;
        參考圖.gameObject.SetActive(true);

        //音效放這裡
        檢查答案();
    }
    List<int> 結果陣列 = new List<int>();

    public List<int> 目標陣列 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

    public bool 答案正確 = false;
    [SerializeField] GameObject 九宮格;
    [SerializeField] GameObject 操作面板;
    public void 檢查答案()
    {
        //檢查數量是否相等
        if (結果陣列.Count != 目標陣列.Count)
        {
            Debug.Log("數量不對");



            return;
        }

        //用迴圈檢查每個東西是否相等
        for (int i = 0; i < 結果陣列.Count; i++)
        {
            if (結果陣列[i] != 目標陣列[i])
            {
                Debug.Log("第" + i + "位錯了");

                return;
            }
        }

        if (未完成的題目.Count > 1)
        {
            Debug.Log("完成了一題還剩下" + (未完成的題目.Count - 1) + "題");
            完成了一題事件.Invoke();

            // 清除最舊的題目
            未完成的題目.RemoveAt(0);
            // 重新刷題
            設定一道題目();
            關閉();
            多圖形判斷用();
        }
        else
        {
            Debug.Log("九宮格正確");
            狀態改變();
            關閉();
            清空題目();
            全部完成事件.Invoke();
        }
    }
    public UnityEvent 完成了一題事件 = null;
    public UnityEvent 全部完成事件 = null;

    [SerializeField] Image 參考圖;
    [SerializeField] Image 參考圖起點;

    public void 指定參考圖(Sprite 輸入參考圖)
    {
        參考圖.sprite = 輸入參考圖;
    }
    public void 指定參考圖起點(Sprite 輸入參考圖起點)
    {
        參考圖起點.sprite = 輸入參考圖起點;
    }

    [SerializeField] CanvasGroup 九宮格UI;
    [SerializeField] SwitchCamera 切換視角;
    [SerializeField] GameObject 右搖桿;
    [SerializeField] 九宮格彈出 九宮格彈出的程式;
    public bool 多圖形繪畫成功 = false;
    public void 關閉()
    {
        九宮格UI.alpha = 0;
        九宮格UI.blocksRaycasts = false;
        操作面板.SetActive(true);
        切換視角.指定切成第三人稱();
        右搖桿.SetActive(true );
        九宮格彈出的程式.是否正在執行九宮格 = false;
    }

    public void 狀態改變()
    {
        if (答案正確 == false)
        {
            答案正確 = true;
            Debug.Log("布林值已經改變");
        }

        if(倒數時間 == false)
        {
            倒數時間 = !倒數時間;
        }
    }

    public void 多圖形判斷用()
    {
        if (多圖形繪畫成功 == false)
        {
            Debug.Log("多圖形判斷通過");
            多圖形繪畫成功 = true;
        }
        else
        {
            Debug.Log("多圖形變回去了");
            多圖形繪畫成功 = false;
        }
    }

}

