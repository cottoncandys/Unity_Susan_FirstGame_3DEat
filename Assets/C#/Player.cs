using UnityEngine;


//類別 玩家player
public class Player : MonoBehaviour
{
    #region 欄位與屬性
    //移動速度float
    [Header("移動速度"), Range(1, 1000)]
    public float speed = 10;
    //跳躍高度float
    [Header("跳躍高度"), Range(1, 5000)]
    public float height;

   ///<summary>
   ///是否在地板上
   ///</summary>
   //是否在地板上bool
    public bool isGround {
        get
        {
            if (transform.position.y < 0.077f) return true;//落地的y值
            else return false;
        }
        
    }


    /// <summary>
    /// 旋轉角度
    /// </summary>
    private Vector3 angle;
 
  
    private Animator ani;  //動畫控制器animator
    private Rigidbody rig; //剛體rigidbody
    private AudioSource aud; //喇叭
    private GameManager gm; //遊戲管理器 吃東西

    /// <summary>
    /// 跳躍力道:從0慢慢增加
    /// </summary>
    private float jump;

    //碰撞音效
    [Header("蛋糕音效")]
    public AudioClip soundCake;
    [Header("高梁音效")]
    public AudioClip soundBeer;

    #endregion


    #region 方法:移動 跳 碰地
    ///<summary>
    ///移動:透過鍵盤
    ///</summary>
    private void Move()
    {
        #region 移動
        //浮點數 前後值 = 輸入類別.取得軸向值("垂直")，垂直 vs 上下
        float v = Input.GetAxisRaw("Vertical");
        //水平 AD 左右
        float h = Input.GetAxisRaw("Horizontal");


        //剛體.添加推力(x,y,z)-世界座標
        // 官網api查詢Rigidbody.AddForce  void (Vector3 force)
        // rig.AddForce(0, 0, speed * v);
        //剛體.添加推力(三維向量)
        //前方 transform.forward - Z
        //右方 transform.right - X
        //上方 transform.up - Y
        rig.AddForce(transform.forward * speed * Mathf.Abs(v));
        rig.AddForce(transform.forward * speed * Mathf.Abs(h));



        //動畫.設定布林("跑步參數", 布林值) - 當 前後取絕對值 大於0時勾選 就是1 abs還是1 ，-1 bas是1。
        ani.SetBool("跑步開關", Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0) ;
        // ani.SetBool("跑步開關", v== 1 || v >=-1 );使用邏輯運算子

        #endregion

        #region 轉向
        

        if (v == 1) angle = new Vector3(0, 0, 0); //前 Y 0
        else if (v == -1) angle = new Vector3(0, 180, 0);//後 Y 180
        else if (h == 1) angle = new Vector3(0, 90, 0);//左 Y 90
        else if (h == -1) angle = new Vector3(0,270, 0);//右 Y 270
        //只要類別後面有:MonoBehaviour 就可以直接使用關鍵字transform 取得此物件的Transform元件
        //eulerAngles 歐拉角度 0 - 360
        transform.eulerAngles = angle;

        //print("角度:" + angle);
        #endregion
    }


    ///<summary>
    ///跳躍:判斷在地板上並按下空白鍵時跳躍
    ///</summary>
    private void Jump()
    {
        //如果 在地板上 為 勾選 並且 按下空白鍵
        if(isGround && Input.GetButtonDown("Jump"))
        {
            //每次跳躍 值都從0 開始
            jump = 0;
            //剛體.推力(0,跳躍高度,0)
            rig.AddForce(0, height, 0);
        }
        //如果 不在地板上(在空中)
        if(!isGround)
        {
            //跳躍 遞增 時間， 一禎時間
            jump += Time.deltaTime;
        }
        //動畫.設定浮點數("跳躍參數"，跳躍時間)
        ani.SetFloat("跳躍力道", jump);
    }


    ///<summary>
    ///碰到道具:踫到帶有標籤【蛋糕】的物件
    ///</summary>
    private void HitProp(GameObject prop)
    {
        if (prop.tag == "蛋糕")
        {
            aud.PlayOneShot(soundCake, 1.2f);//喇叭.播放一次音效(音效片段，音量)//1代表原來音量 2是原來的音量*2倍
            Destroy(prop);                //刪除(物件)
        }
        else if (prop.tag == "高梁")
        {
            aud.PlayOneShot(soundBeer, 1.2f);
            Destroy(prop);
        }

        gm.GetProp(prop.tag); //告知GM取得道具(將道具標籤傳過去)
    }


    #endregion


    #region 事件
    private void Start()
    {
        //GetComponent<泛型>() 泛型方法- 泛型 所有類型 Rigidbody, Transform, Collider..
        //剛體 =  取得元件<剛體>();
        rig = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        //吃東西
        //foot 只限於場景上只有一個類別存在時使用
        //例如:場景上只有一個GameManager類別時可以使用他來取得
        gm = FindObjectOfType<GameManager>();
    }
 

    //固定更新頻率事件:1秒 50禎，使用物理必須在此事件內
    private void FixedUpdate()
    {
        Move();
    }

        
    //更新事件1秒 約60禎
    private void Update()
    {
        Jump();
    }


    //碰撞事件: 當物件碰撞《開始》時執行《一次》(沒有勾選 Is Trigger)
    //collision 碰到物件的碰撞資訊
    private void OnCollisionEnter(Collision collision)
    {
        
    }

    //碰撞事件: 當物件碰撞《離開》時執行《一次》(沒有勾選 Is Trigger)
    private void OnCollisionExit(Collision collision)
    {

    }
    //碰撞事件: 當物件碰撞《開始》時《持續》執行(沒有勾選 Is Trigger) 60fps//時間差
    private void OnCollisionStay(Collision collision)
    {

    }

    /***********************/
    //觸發事件: 當物件碰撞《開始》時執行《一次》(有勾選 Is Trigger)
    private void OnTriggerEnter(Collider other)
    {
        //碰到道具(碰撞資訊.遊戲物件)
        HitProp(other.gameObject);
    }
    //觸發事件: 當物件碰撞《離開》時執行《一次》(有勾選 Is Trigger)
    private void OnTriggerExit(Collider other)
    {

    }
    //觸發事件: 當物件碰撞《開始》時《持續》執行(有勾選 Is Trigger) 60fps
    private void OnTriggerStay(Collider other)
    {

    }




    #endregion




}
/*
  :int 0 1
:float 0~1~-1
//靜態static，只要是靜態的就直接輸入類別名稱。動態，要有欄位。
//vertical:前後。Hertical:左右。fire: mouse left、ctrl 。jump:space。
 */
