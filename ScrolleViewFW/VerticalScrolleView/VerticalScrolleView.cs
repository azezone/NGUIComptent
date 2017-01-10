using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerticalScrolleView : MonoBehaviour
{
    public PageItemBase mPageOrigin;
    public Transform mRoot;
    public UIScrollBar MainBar;
    public UIScrollBar CorBar;

    public float Width = 200f;
    public float Height = 70f;

    public float MoveSpeed = 0.3f;

    private const int PersistPageNum = 5;
    private int CurrentPage = 1;
    private int mCurDataPage = 1;
    private int TotalNum = 0;
    private int mNumPerPage = 8;
    public int NumPerPage
    {
        get { return mNumPerPage; }
        set { mNumPerPage = value; }
    }

    public int CurDataPage
    {
        get
        {
            return mCurDataPage;
        }
    }

    private List<PageItemBase> mPageItemList = new List<PageItemBase>();
    public List<PageItemBase> PageItemList
    {
        get
        {
            return mPageItemList;
        }
    }
    private List<ImageItemBase> mImgItemList = new List<ImageItemBase>();
    public List<ImageItemBase> ImgItemList
    {
        get
        {
            return mImgItemList;
        }
    }

    protected void Init(int total)
    {
        TotalNum = total;
        NumPerPage = mPageOrigin.ImgCountPerPage;
        ResetData();
        StartCoroutine(CreatePageItems());
    }

    private void ResetData()
    {
        mCurDataPage = 1;
        CurrentPage = 1;
        mRoot.localPosition = Vector3.zero;
        //for (int i = 0; i < mPageItemList.Count; i++)
        //{
        //    mPageItemList[i].transform.GetComponent<UIWidget>().alpha = i == 0 ? 1 : 0;
        //}

        RefreshCtrBar();
    }

    private IEnumerator CreatePageItems()
    {
        if (mImgItemList.Count == 0)
        {
            mPageOrigin.gameObject.SetActive(true);
            for (int i = 0; i < PersistPageNum; i++)
            {
                GameObject go = Instantiate(mPageOrigin.gameObject) as GameObject;
                Transform tran = go.transform;
                tran.parent = mRoot;
                tran.localPosition = new Vector3(0, -i * Height, 0);
                tran.localScale = Vector3.one;
                
                PageItemBase pb = go.GetComponent<PageItemBase>();
                pb.Init();
                mImgItemList.AddRange(pb.ItemList);
                mPageItemList.Add(pb);
                yield return new WaitForEndOfFrame();
            }
            mPageOrigin.gameObject.SetActive(false);
        }

        FillData(1, PersistPageNum);
    }

    public virtual void FillData(int begin, int end) { }

    public bool IsLastPage()
    {
        return (TotalNum <= NumPerPage * CurDataPage) && (TotalNum > NumPerPage * (CurDataPage - 1));
    }

    private bool islock = false;
    private IEnumerator MoveNext(bool isnext)
    {
        islock = true;
        if (CheckCanMove(isnext))
        {
            if (CurrentPage == 1 && !isnext)
            {
                ReSetPosition(false);
                CurrentPage = PersistPageNum;
            }
            else if (CurrentPage == PersistPageNum && isnext)
            {
                ReSetPosition(true);
                CurrentPage = 1;
            }

            yield return new WaitForEndOfFrame();

            if (isnext)
            {
                CurrentPage++;
                mCurDataPage++;
            }
            else
            {
                CurrentPage--;
                mCurDataPage--;
            }
            RefreshCtrBar();

            Vector3 tem = mRoot.localPosition;
            Vector3 pos = new Vector3(tem.x, isnext ? (tem.y + Height) : (tem.y - Height), tem.z);

            TweenPosition tr = TweenPosition.Begin(mRoot.gameObject, MoveSpeed, pos);
            tr.method = UITweener.Method.EaseInOut;
            tr.AddOnFinished(() =>
            {
                islock = false;
                if (mRoot.GetComponent<TweenPosition>() != null)
                {
                    DestroyImmediate(mRoot.GetComponent<TweenPosition>());
                }
            });
            tr.PlayForward();
        }
    }

    private bool CheckCanMove(bool isnext)
    {
        if (mCurDataPage == 1 && !isnext)
        {
            PlayAnimation(true);
            return false;
        }
        if (IsLastPage() && isnext)
        {
            PlayAnimation(false);
            return false;
        }
        return true;
    }

    private void MoveToNext(bool isnext)
    {
        if (islock || animLock)
        {
            return;
        }
        else if (gameObject.activeInHierarchy)
        {
            StartCoroutine(MoveNext(isnext));
        }
    }

    public void MoveNext()
    {
        MoveToNext(true);
    }

    public void MoveLast()
    {
        MoveToNext(false);
    }

    private bool animLock = false;
    private void PlayAnimation(bool istop)
    {
        if (animLock)
        {
            return;
        }
        animLock = true;
        islock = true;
        Vector3 rpos = mRoot.localPosition;
        Vector3 pos = istop ? new Vector3(rpos.x, rpos.y - Height / 2, rpos.z) : new Vector3(rpos.x, rpos.y + Height / 2, rpos.z);
        TweenPosition tp = TweenPosition.Begin(mRoot.gameObject, 0.3f, pos);
        tp.method = UITweener.Method.EaseInOut;
        tp.AddOnFinished(() =>
        {
            DestroyImmediate(tp);
            if (this != null)
            {
                TweenPosition temp = TweenPosition.Begin(mRoot.gameObject, 0.3f, rpos);
                temp.AddOnFinished(() =>
                {
                    animLock = false;
                    islock = false;
                    DestroyImmediate(temp);
                });
                temp.PlayForward();
            }
            else
            {
                animLock = false;
                islock = false;
            }
        });
        tp.PlayForward();

        if (MainBar != null)
        {
            float ori = MainBar.barSize;
            float size = ori / 2;
            TweenScrollBarSize tb = TweenScrollBarSize.Begin(MainBar, 0.6f, 0);
            tb.method = UITweener.Method.EaseInOut;
            tb.AddOnFinished(() =>
            {
                DestroyImmediate(tb);
                TweenScrollBarSize tb2 = TweenScrollBarSize.Begin(MainBar, 0.2f, ori);
                tb2.PlayForward();
                tb2.AddOnFinished(() =>
                {
                    DestroyImmediate(tb2);
                });
            });
            tb.PlayForward();
        }

        if (CorBar != null)
        {
            float ori = CorBar.barSize;
            float size = ori / 2;
            TweenScrollBarSize tb = TweenScrollBarSize.Begin(CorBar, 0.6f, 0);
            tb.method = UITweener.Method.EaseInOut;
            tb.AddOnFinished(() =>
            {
                DestroyImmediate(tb);
                TweenScrollBarSize tb2 = TweenScrollBarSize.Begin(CorBar, 0.2f, ori);
                tb2.PlayForward();
                tb2.AddOnFinished(() =>
                {
                    DestroyImmediate(tb2);
                });
            });
            tb.PlayForward();
        }
    }

    private void ReSetPosition(bool botom2top)
    {
        Vector3 tem = mRoot.localPosition;
        List<PageItemBase> pList = new List<PageItemBase>();
        mImgItemList.Clear();
        int begin = 0;
        int end = 0;
        if (botom2top)
        {
            CurrentPage = 1;
            mRoot.localPosition = new Vector3(tem.x, 0, tem.z);
            pList.Add(mPageItemList[PersistPageNum - 1]);
            for (int i = 0; i < PersistPageNum - 1; i++)
            {
                pList.Add(mPageItemList[i]);
            }
            begin = mCurDataPage;
            end = mCurDataPage + PersistPageNum - 1;
        }
        else
        {
            CurrentPage = PersistPageNum;
            mRoot.localPosition = new Vector3(tem.x, (PersistPageNum - 1) * Height, tem.z);

            for (int i = 1; i < PersistPageNum; i++)
            {
                pList.Add(mPageItemList[i]);
            }
            pList.Add(mPageItemList[0]);
            begin = mCurDataPage - PersistPageNum + 1;
            end = mCurDataPage;
        }
        mPageItemList = pList;
        for (int i = 0; i < mPageItemList.Count; i++)
        {
            mImgItemList.AddRange(mPageItemList[i].ItemList);
        }
        RePosition();

        FillData(begin, end);
    }

    private void RePosition()
    {
        for (int i = 0; i < mPageItemList.Count; i++)
        {
            mPageItemList[i].transform.localPosition = new Vector3(0, -i * Height, 0);
        }
    }

    private void RefreshCtrBar()
    {
        int total = Mathf.CeilToInt((float)TotalNum / NumPerPage);
        float size = 1.0f / total;
        if (mCurDataPage == 1)
        {
            SetScrolleBar(0, size);
        }
        else if (mCurDataPage == total)
        {
            SetScrolleBar(1, size);
        }
        else
        {
            float val = (float)mCurDataPage / (total + 1);
            SetScrolleBar(val, size);
        }
    }

    private void SetScrolleBar(float val, float size)
    {
        if (MainBar != null)
        {
            MainBar.barSize = size;
            TweenScrollBar ts = TweenScrollBar.Begin(MainBar.gameObject, MoveSpeed, val);
            ts.PlayForward();
        }
        if (CorBar != null)
        {
            CorBar.barSize = size;
            TweenScrollBar ts = TweenScrollBar.Begin(CorBar.gameObject, MoveSpeed, val);
            ts.PlayForward();
        }
    }

    public void SetController(bool add)
    {
        if (add)
        {
            PicoInputManager.OnDown += MoveNext;
            PicoInputManager.OnUp += MoveLast;
        }
        else
        {
            PicoInputManager.OnDown -= MoveNext;
            PicoInputManager.OnUp -= MoveLast;
        }
    }

    public void SetActiveCtrBar(bool isactive)
    {
        if (MainBar != null)
        {
            MainBar.gameObject.SetActive(isactive);
        }
        if (CorBar != null)
        {
            CorBar.gameObject.SetActive(isactive);
        }
    }
}