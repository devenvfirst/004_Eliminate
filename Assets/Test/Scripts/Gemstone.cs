using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gemstone : MonoBehaviour {

    //宝石的起始位置
    private float xOffset = -4.5f;
    private float yOffset = -3.0f;

    public int rowIndex = 0;
    public int columIndex = 0;

    //宝石数组
    public GameObject[] gemstoneBgs;

    //宝石类型
    public int gemstoneType;

    private GameObject gemstoneBg;
    private SpriteRenderer spriteRenderer;

    private GameController gameController;


    /// <summary>
    /// 宝石是否选中 value为bool值 gamecontroller控制
    /// </summary>
    public bool IsSelected {
        set
        {
            if(value)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }


	// Use this for initialization
	void Start () {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        spriteRenderer = gemstoneBg.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 创建一个宝石
    /// </summary>
    public void RandomCreateGemstoneBg()
    {
        if (gemstoneBg != null)
            return;

        gemstoneType = Random.Range(0,gemstoneBgs.Length);
        gemstoneBg = Instantiate(gemstoneBgs[gemstoneType]) as GameObject;
        gemstoneBg.transform.parent = this.transform;
    }

    /// <summary>
    /// 更新宝石的位置
    /// </summary>
    /// <param name="_rowIndex"></param>
    /// <param name="_columIndex"></param>
    public void UpdatePosition(int _rowIndex,int _columIndex)
    {
        rowIndex = _rowIndex;
        columIndex = _columIndex;

        //设置宝石的生成位置
        this.transform.position = new Vector3(columIndex+xOffset,rowIndex+yOffset,0);
    }

    /// <summary>
    /// 鼠标点击宝石
    /// </summary>
    private void OnMouseDown()
    {
        gameController.Select(this);
    }

    /// <summary>
    /// 宝石位移功能
    /// </summary>
    /// <param name="_rowIndex"></param>
    /// <param name="_columIndex"></param>
    public void TweenToPosition(int _rowIndex,int _columIndex)
    {
        rowIndex = _rowIndex;
        columIndex = _columIndex;
        iTween.MoveTo(gameObject,
            iTween.Hash("x", columIndex + xOffset,
            "y", rowIndex + yOffset,
            "time", 0.5f));
    }

    /// <summary>
    /// 销毁自身
    /// </summary>
    public void Dispose()
    {
        Destroy(gameObject);
        Destroy(gemstoneBg.gameObject);
        gameController = null;
    }
}
