using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private void Awake()
    {
        instance = this;
    }
    public bool 是否開啟透視鏡 = false;
    [SerializeField] bool 手機模式 = false;
    [SerializeField] Rigidbody 物理引擎 = null;
    [SerializeField] Transform 垂直旋轉軸 = null;
    [SerializeField] Transform 水平旋轉軸 = null;
    [SerializeField] float 跳躍力 = 5f;
    public Transform 眼睛 = null;
    float mouseY = 0f;
    Quaternion 角色旋轉 = Quaternion.identity;
    [SerializeField] Transform 模型旋轉軸 = null;
    float ad, ws;
    [SerializeField] Animator animator = null;
    [SerializeField] float 走路速度, 蹲下速度, 跑步速度;
    float sp = 0f;
    [SerializeField] VariableJoystick 左搖桿 = null;
    [SerializeField] 九宮格彈出 九宮格彈出相關 = null;
    float 實際速度 = 0f;
    float 維持移動多久 = 0f;
    float 校正時間 = 0f;
    //private CapsuleCollider 玩家高度;
    //private Vector3 原始中心;
    //private float 原始高度;
    public CapsuleCollider 站立高度;
    public CapsuleCollider 蹲下高度;
    private CapsuleCollider 原始高度;





    private void Start()
    {
        //隱藏滑鼠
        //Cursor.lockState = CursorLockMode.Locked;
        // 訂閱觸控事件
        右搖桿觸控板.instance.拖曳事件 += 轉視角;
        //玩家高度 = GetComponent<CapsuleCollider>();
        //原始中心 = 玩家高度.center;
        //原始高度 = 玩家高度.height;
        原始高度 = 站立高度;
        站立高度.enabled = true;


    }
    private void OnDisable()
    {
        // 退訂觸控事件
        右搖桿觸控板.instance.拖曳事件 -= 轉視角;
    }

    void 轉視角(float 水平, float 垂直)
    {
        水平旋轉軸.Rotate(0f, 水平, 0f);
        mouseY += 垂直;
        Debug.Log("MMM");
    }

    bool 透視鏡模式 = false;

    private void Update()
    {
        //移動
        ad = Mathf.Lerp(ad, Input.GetAxisRaw("Horizontal"), Time.deltaTime * 10f);
        ws = Mathf.Lerp(ws, Input.GetAxisRaw("Vertical"), Time.deltaTime * 10f);
        if (手機模式)
        {
            ad = 左搖桿.Horizontal;
            ws = 左搖桿.Vertical;
        }


        if (Mathf.Abs(ws) + Mathf.Abs(ad) > 0.1f)
        {
            維持移動多久 += Time.deltaTime;
        }
        else
        {
            維持移動多久 = 0f;

        }

        if (維持移動多久 > 0.5f)
        {
            sp = Mathf.Lerp(sp, 1f, Time.deltaTime * 10f);

        }
        else if (是否蹲下)
        {
            sp = Mathf.Lerp(sp, -1, Time.deltaTime * 10f);
            //玩家高度.center = 原始中心 / 0.5f;
            //玩家高度.height = 原始高度 ;
            站立高度.enabled = false;
            原始高度 = 蹲下高度;
            原始高度.enabled = true;
            維持移動多久 = 0f;
        }
        else
        {
            sp = Mathf.Lerp(sp, 0f, Time.deltaTime * 10f);
            //玩家高度.center = 原始中心;
            //玩家高度.height = 原始高度;
            蹲下高度.enabled = false;
            原始高度 = 站立高度;
            原始高度.enabled = true;
        }

        if (sp > 0f)
        {
            實際速度 = Mathf.Lerp(走路速度, 跑步速度, sp);
        }
        else
        {
            實際速度 = Mathf.Lerp(走路速度, 蹲下速度, -sp);
        }

        //如果開始對話，中斷移動
        if (SaySystem.instance.isPlay == true || 是否開啟透視鏡 == true || 九宮格彈出相關.是否正在執行九宮格 == true)
        {
            ad = 0;
            ws = 0;
        }


        Vector3 世界移動方向 = 水平旋轉軸.TransformDirection(ad * 實際速度, 0f, ws * 實際速度);

        物理引擎.velocity = new Vector3(世界移動方向.x, 物理引擎.velocity.y, 世界移動方向.z);

        //畫面旋轉
        float mouseX = 0f;
        if (手機模式 == false) 
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY += Input.GetAxis("Mouse Y");
        }
        mouseY = Mathf.Clamp(mouseY, -80f, 80f);

        transform.TransformDirection(0f, 0f, 1f);

        水平旋轉軸.Rotate(0f, mouseX, 0f);
        垂直旋轉軸.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);

        if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical"))) > 0f || (Mathf.Abs(左搖桿.Horizontal) + Mathf.Abs(左搖桿.Vertical) > 0f))
        {
            //LooKRotation 將一個向量轉換成角度
            if (校正時間 <= 0f)
            {
                if (世界移動方向 != Vector3.zero)
                {
                    Quaternion 旋轉 = Quaternion.LookRotation(世界移動方向, new Vector3(0f, 1f, 0f));

                    角色旋轉 = Quaternion.Lerp(角色旋轉, 旋轉, Time.deltaTime * 10f);
                }



            }


        }

        if (校正時間 > 0f || 是否開啟透視鏡)
        {
            角色旋轉 = Quaternion.Lerp(角色旋轉, 水平旋轉軸.rotation, Time.deltaTime * 10f);
            校正時間 -= Time.deltaTime;
        }
        //跳躍
        if (Input.GetKeyDown(KeyCode.Space) && 是否踩到地板)
        {
            物理引擎.velocity = Vector3.up * 跳躍力;
        }

        // 偵測鍵盤E
        if (Input.GetKeyDown(KeyCode.E) && 準心前是否有東西 == true)
        {
            互動按鈕();
        }

        //新增玩家可以針對飛行(監視)機器人的入侵雷射互動
        if(Input.GetKeyDown(KeyCode.E) && 雷射打到東西 == true && 是否開啟透視鏡 == true)
        {
            Debug.Log("雷射打到東西了");
            透視鏡的互動();

        }

        模型旋轉軸.rotation = 角色旋轉;

        //動畫
        //animator.SetFloat("ws", ws);
        //animator.SetFloat("ad", ad);
        animator.SetFloat("sp", sp);
        float 操作量 = Mathf.Abs(ws) + Mathf.Abs(ad);
        操作量 = Mathf.Clamp(操作量, 0f, 1f);
        animator.SetFloat("ws", 操作量);

        if (是否開啟透視鏡 == true)
        {
            animator.SetBool("穿上透視鏡", true);
        }
        else
        {
            animator.SetBool("穿上透視鏡", false);
        }

    }

    bool 是否踩到地板
    {
        get { return _是否踩到地板; }
        set { _是否踩到地板 = value; animator.SetBool("Onfloor", value); }
    }
    bool _是否踩到地板;

    [SerializeField] LayerMask 可以互動的圖層;

    bool 準心前是否有東西
    {
        // 回傳偵測到的碰撞器的數量是否 不等於 0
        get { return 半徑內碰撞器.Count != 0; }
    }

    [SerializeField] float 互動半徑 = 3f;
    List<Collider> 半徑內碰撞器 = new List<Collider>();

    [SerializeField] float 腳底往下可偵測距離 = 0.5f;
    [SerializeField] float 腳底往上可偵測距離 = 3f;
    public bool 雷射打到東西 = false;
    [HideInInspector][SerializeField] Camera cam = null;
    RaycastHit 雷射打到的東西;
    [SerializeField] LayerMask 透視鏡針對的怪物;
    [SerializeField] GameObject 過肩視角;
    public float 雷射打的距離 = 50f;
    public Color 顏色 = Color.red;
    [SerializeField] GameObject 駭入機器人用攝影機位置 = null;
    private void FixedUpdate()
    {
        是否踩到地板 = Physics.Raycast(this.transform.position, Vector3.down, 1.5f);

        //建立針對飛行(監視)機器人的互動雷射

        //cam = Camera.main;
        Vector3 雷射的初始點 = 駭入機器人用攝影機位置.transform.position;
        Vector3 雷射的方向 = 駭入機器人用攝影機位置.transform.forward;
        雷射打到東西 = Physics.Raycast(雷射的初始點, 雷射的方向, out 雷射打到的東西, 50f, 透視鏡針對的怪物);     

        if(雷射打到東西 == true)
        {
            Debug.DrawLine(雷射的初始點, 雷射打到的東西.point,顏色);
        }
        else
        {
            Debug.DrawLine(雷射的初始點, 雷射的方向 * 雷射打的距離, 顏色);
        }

        // 放出一個瞬間的碰撞器 捕捉半徑內的碰撞器
        Collider[] 圓形範圍內 = Physics.OverlapSphere(this.transform.position, 互動半徑, 可以互動的圖層);
        // 一開始清乾淨陣列中的東西
        半徑內碰撞器.Clear();
        // 只允許身高範圍內的東西可互動
        for (int i = 0; i < 圓形範圍內.Length; i++)
        {
            // 如果該物件的大於 腳底位置 - 腳底向下偵測距離
            if (圓形範圍內[i].transform.position.y > (水平旋轉軸.transform.position.y - 腳底往下可偵測距離))
            {
                // 如果該物件小於 腳底位置 + 往上的偵測距離
                if (圓形範圍內[i].transform.position.y < (水平旋轉軸.transform.position.y + 腳底往上可偵測距離))
                {
                    半徑內碰撞器.Add(圓形範圍內[i]);
                }
            }
        }
    }

    [SerializeField] Transform 互動UI = null;
    [SerializeField] Transform 透視鏡互動 = null;

    private void LateUpdate()
    {
        if (準心前是否有東西)
        {
            互動UI.localScale = Vector3.one;
        }
        // 否則就顯示
        else
        {
            互動UI.localScale = Vector3.zero;
        }

        if(雷射打到東西 == true && 是否開啟透視鏡 == true)
        {
            透視鏡互動.localScale = Vector3.one;
        }
        else
        {
            透視鏡互動.localScale = Vector3.zero;
        }
    }

    public   void 透視鏡的互動()
    {
        雷射打到的東西.collider.transform.root.GetComponent<Item>().互動();
    }

    public void 互動按鈕()
    {
        // 1.優先對面向自己的東西互動( O
        // 2.優先對靠近自己的東西互動( X

        float 最近的角度 = 999f;
        int 最近的物品的號碼 = 0;

        for (int i = 0; i < 半徑內碰撞器.Count; i++)
        {
            // 讓雙方的Y值一致避免上下角度問題
            Vector3 c = 半徑內碰撞器[i].transform.position;
            c.y = 水平旋轉軸.position.y;
            Vector3 ab = 水平旋轉軸.forward;
            Vector3 ac = 半徑內碰撞器[i].transform.position - 水平旋轉軸.position;

            float 夾角 = Vector3.Angle(ab, ac);

            Debug.Log(半徑內碰撞器[i].gameObject.name + " 夾角 " + 夾角);

            // 找出最近的對象
            if (夾角 < 最近的角度)
            {
                最近的角度 = 夾角;
                最近的物品的號碼 = i;
            }
        }

        半徑內碰撞器[最近的物品的號碼].transform.GetComponent<Item>().互動();

        校正時間 = 1f;
    }

    public void 跳躍按鈕()
    {
        if (是否踩到地板)
        {
            物理引擎.velocity = Vector3.up * 跳躍力;
        }
    }
    bool 是否蹲下 = false;

    public void 蹲下按鈕()
    {
        是否蹲下 = !是否蹲下;

    }

    //處理切換視角
    public void 透視鏡()
    {
        透視鏡模式 = !透視鏡模式;
        是否開啟透視鏡 = true;
    }

    public void 受傷()
    {
        入侵失敗視窗.ins.Open();
        Boss.instance.status = Boss狀態.待機;
    }
    
}
