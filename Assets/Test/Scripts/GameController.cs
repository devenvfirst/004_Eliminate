using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Gemstone gemstone;

    public int rowNum = 7;
    public int columNum = 10;

    public ArrayList gemstoneList;
    private ArrayList matchesGemstone;

    private Gemstone currentGemstone;

	// Use this for initialization
	void Start () {


        InitMap();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 初始化地图
    /// </summary>
    private void InitMap()
    {
        gemstoneList = new ArrayList();
        matchesGemstone = new ArrayList();

        for (int i = 0; i < rowNum; i++)
        {
            ArrayList temp = new ArrayList();
            for (int j = 0; j < columNum; j++)
            {
                Gemstone g = AddGemstone(i, j);
                temp.Add(g);
            }
            gemstoneList.Add(temp);
        }

        //生成地图之后及检测消除一次
        if(CheckHorizontalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
    }

    /// <summary>
    /// 创建一个宝石的父容器
    /// </summary>
    /// <param name="rowIndex">行数</param>
    /// <param name="columIndex">列数</param>
    /// <returns></returns>
    public Gemstone AddGemstone(int rowIndex,int columIndex)
    {
        Gemstone g = Instantiate(gemstone) as Gemstone;
        g.transform.parent = this.transform;
        g.GetComponent<Gemstone>().RandomCreateGemstoneBg();
        g.GetComponent<Gemstone>().UpdatePosition(rowIndex,columIndex);

        return g;
    }


    /// <summary>
    /// 选中宝石
    /// </summary>
    /// <param name="g"></param>
    public void Select(Gemstone g)
    {
        if(currentGemstone == null)
        {
            currentGemstone = g;
            currentGemstone.IsSelected = true;
            return;
        }
        else
        {
            if(Mathf.Abs(currentGemstone.rowIndex - g.rowIndex) 
                + Mathf.Abs(currentGemstone.columIndex - g.columIndex)==1)
            {
                StartCoroutine(ExangeAndMatches(currentGemstone,g));
            }
            currentGemstone.IsSelected = false;
            currentGemstone = null; 
        }
    }

    /// <summary>
    /// 设置宝石到二维数组
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columIndex"></param>
    /// <param name="g"></param>
    public void SetGemStone(int rowIndex,int columIndex,Gemstone g)
    {
        ArrayList temp = gemstoneList[rowIndex] as ArrayList;
        temp[columIndex] = g;
    }


    /// <summary>
    /// 交换位置和数据
    /// </summary>
    /// <param name="g1"></param>
    /// <param name="g2"></param>
    public void ExChange(Gemstone g1,Gemstone g2)
    {
        SetGemStone(g1.rowIndex, g1.columIndex, g2);
        SetGemStone(g2.rowIndex, g2.columIndex, g1);

        //交换行号和列号并开始位置动画
        int tempRowIndex, tempColumeIndex;
        tempRowIndex = g1.rowIndex;
        tempColumeIndex = g1.columIndex;

        g1.rowIndex = g2.rowIndex;
        g1.columIndex = g2.columIndex;

        g2.rowIndex = tempRowIndex;
        g2.columIndex = tempColumeIndex;

        g1.TweenToPosition(g1.rowIndex,g1.columIndex);
        g2.TweenToPosition(g2.rowIndex, g2.columIndex);
    }

    /// <summary>
    /// 得到宝石
    /// </summary>
    /// <param name="_rowIndex"></param>
    /// <param name="_columIndex"></param>
    /// <returns></returns>
    public Gemstone GetGemstone(int _rowIndex,int _columIndex)
    {
        ArrayList temp = gemstoneList[_rowIndex] as ArrayList;
        Gemstone g = temp[_columIndex] as Gemstone;
        return g;
    }


    /// <summary>
    /// 交换宝石
    /// </summary>
    /// <param name="g1"></param>
    /// <param name="g2"></param>
    /// <returns></returns>
    IEnumerator ExangeAndMatches(Gemstone g1,Gemstone g2)
    {
        ExChange(g1,g2);

        //交换后判断是否可以消除
        yield return new WaitForSeconds(0.5f);
        if(CheckHorizontalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
        else
        {
            ExChange(g1,g2);
        }
    }


    /// <summary>
    /// 广度搜索可以消除的宝石
    /// </summary>
    /// <returns></returns>
    public bool CheckHorizontalMatches()
    {
        bool isMatches = false;
        for (int rowIndex = 0; rowIndex < rowNum; rowIndex++)
        {
            for (int columIndex = 0; columIndex < columNum - 2; columIndex++)
            {
                if (GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex, columIndex + 1).gemstoneType && 
                    GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex, columIndex + 2).gemstoneType)
                {
                    AddMatches(GetGemstone(rowIndex, columIndex));
                    AddMatches(GetGemstone(rowIndex, columIndex+1));
                    AddMatches(GetGemstone(rowIndex, columIndex+2));
                    isMatches = true;
                }
            }
        }
        return isMatches;
    }

    /// <summary>
    /// 深度搜索可以消除的宝石
    /// </summary>
    /// <returns></returns>
    public bool CheckVerticalMatches()
    {
        bool isMatches = false;
        for (int columIndex = 0; columIndex < columNum; columIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowNum-2; rowIndex++)
            {
                if (GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex+1, columIndex).gemstoneType &&
                    GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex+2, columIndex).gemstoneType)
                {
                    AddMatches(GetGemstone(rowIndex, columIndex));
                    AddMatches(GetGemstone(rowIndex+1, columIndex));
                    AddMatches(GetGemstone(rowIndex+2, columIndex));
                    isMatches = true;
                }
            }
        }
        return isMatches;
    }

    /// <summary>
    /// 将可以移除的宝石添
    /// </summary>
    /// <param name="g"></param>
    private void AddMatches(Gemstone g)
    {
        if (matchesGemstone == null)
            matchesGemstone = new ArrayList();

        //检测宝石g是否在临时数组中
        int index = matchesGemstone.IndexOf(g);
        if(index == -1)
        {
            matchesGemstone.Add(g);
        }
    }

    /// <summary>
    /// 消除一个宝石
    /// </summary>
    /// <param name="g"></param>
    private void RemoveGemstone(Gemstone g)
    {
        g.Dispose();

        //消除并下降
        for (int i = g.rowIndex+1; i < rowNum; i++)
        {
            Gemstone tempGemstone = GetGemstone(i, g.columIndex);
            tempGemstone.rowIndex--;
            SetGemStone(tempGemstone.rowIndex,tempGemstone.columIndex,tempGemstone);
            tempGemstone.TweenToPosition(tempGemstone.rowIndex,tempGemstone.columIndex);
        }

        //生成新的宝石
        Gemstone newGemstone = AddGemstone(rowNum,g.columIndex);
        newGemstone.rowIndex--;
        SetGemStone(newGemstone.rowIndex,newGemstone.columIndex,newGemstone);
        newGemstone.TweenToPosition(newGemstone.rowIndex, newGemstone.columIndex);
    }

    /// <summary>
    /// 删除匹配的宝石
    /// </summary>
    void RemoveMatches()
    {
        for (int i = 0; i < matchesGemstone.Count; i++)
        {
            Gemstone g = matchesGemstone[i] as Gemstone;
            RemoveGemstone(g);
        }
        matchesGemstone = new ArrayList();
        StartCoroutine(WaitForCheckMatchesAgain());
    }


    /// <summary>
    /// 循环检测并显示连击
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForCheckMatchesAgain()
    {
        yield return new WaitForSeconds(0.5f);
        if(CheckHorizontalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();  

            //以下连击最少消除2次会显示
            GameObject.Find("Text").GetComponent<Text>().text = "连击";
            Debug.Log("lianji");
            yield return new WaitForSeconds(3f);
            GameObject.Find("Text").GetComponent<Text>().text = "";
       
        }
    }
}
