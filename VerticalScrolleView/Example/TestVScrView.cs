using System.Collections.Generic;
using UnityEngine;

public class TestVScrView : VerticalScrolleView
{
    private List<TestPhotoModel> mDataList = null;

    /// <summary>
    /// 此处传入自己需要的数据
    /// TestPhotoModel 需要继承PhotoModelBase
    /// </summary>
    /// <param name="list"></param>
    public void InitData(List<TestPhotoModel> list)
    {
        if (list == null)
        {
            Debug.LogError("the list is null!");
            SetActiveCtrBar(false);
            return;
        }
        mDataList = list;
        base.Init(list.Count);
    }

    public override void FillData(int begin, int end)
    {
        List<TestPhotoModel> list = GetPhotoData(NumPerPage, begin, end);
        for (int i = 0; i < ImgItemList.Count; i++)
        {
            if (mDataList != null && i < list.Count)
            {
                ImageItemBase item = ImgItemList[i].GetComponent<ImageItemBase>();
                item.gameObject.SetActive(true);
                item.Init(list[i]);
            }
            else
            {
                ImgItemList[i].gameObject.SetActive(false);
            }
        }
        if (mDataList == null || mDataList.Count == 0)
        {
            SetActiveCtrBar(false);
        }
        else
        {
            SetActiveCtrBar(true);
        }
    }

    private List<TestPhotoModel> GetPhotoData(int perpage, int head, int end)
    {
        List<TestPhotoModel> list = new List<TestPhotoModel>();
        if (perpage > 0 && head > 0 && end > 0 && end >= head && mDataList.Count > (perpage * (head - 1)))
        {
            int i = perpage * (head - 1);
            while (i < (perpage * end) && i < mDataList.Count)
            {
                list.Add(mDataList[i++]);
            }
        }
        else
        {
            Debug.Log("the perpage or page num is not legal!");
        }

        return list;
    }
}

public class TestPhotoModel : PhotoModelBase
{
    public string Name;
    public string ID;
}