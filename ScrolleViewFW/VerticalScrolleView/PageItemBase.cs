using UnityEngine;
using System.Collections.Generic;

public class PageItemBase : MonoBehaviour
{
    [SerializeField]
    ImageItemBase mImageItem;
    [HideInInspector]
    public List<ImageItemBase> ItemList = new List<ImageItemBase>();

    public int ImgCountPerPage = 8;
    private bool isInit = false;

    public void Init()
    {
        if (isInit)
        {
            return;
        }

        //mImageItem.gameObject.SetActive(true);
        for (int i = 0; i < ImgCountPerPage; i++)
        {
            GameObject go = Instantiate(mImageItem.gameObject) as GameObject;
            Transform tran = go.transform;
            tran.parent = transform;
            tran.localPosition = Vector3.zero;
            tran.localScale = Vector3.one;

            ItemList.Add(go.GetComponent<ImageItemBase>());
        }
        mImageItem.gameObject.SetActive(false);

        SetStyle();
        isInit = true;
    }

    public virtual void SetStyle() { }
}