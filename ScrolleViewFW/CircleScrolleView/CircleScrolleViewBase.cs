using System.Collections.Generic;
using UnityEngine;

public class CircleScrolleViewBase : MonoBehaviour
{
    private const int NumPerPage = 5;   //item nums per page
    private int CurPage = 1;            //current page id
    private int TotalPage = 0;          //total page num

    public float Width = 10;            //step length
    public float Radis = 100;           //circle radis

    private float Sita;
    private float Degree;
    private float DegreePerPage;
    private float DegreeClipPerPage;

    private bool isInit = false;
    private void Init()
    {
        if (!isInit)
        {
            Sita = 2 * Mathf.Atan2(0.5f * Width, Radis);
            Degree = 180f * Sita / Mathf.PI;
            DegreePerPage = (NumPerPage - 1) * Degree;
            DegreeClipPerPage = NumPerPage * Degree;
            isInit = true;
        }
    }

    void Start()
    {
        Reposition();
        MoveToPage(1);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            MoveToRight();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            MoveToLeft();
        }
    }

    public void Add(Transform tran)
    {
        tran.parent = transform;
    }

    private void ResetPosition(List<Transform> list)
    {
        int count = list.Count;
        TotalPage = count / NumPerPage;
        List<PositionData> poslist = CalculatePosition(count);

        for (int i = 0; i < count; i++)
        {
            list[i].localPosition = poslist[i].position;
            list[i].localRotation = poslist[i].quaternion;
            if (i < (CurPage - 1) * NumPerPage || i > (CurPage * NumPerPage))
            {
                TweenAlpha ta = TweenAlpha.Begin(list[i].gameObject, 0, 0);
                ta.PlayForward();
            }
        }
    }

    private List<PositionData> CalculatePosition(int num)
    {
        this.Init();
        List<PositionData> data = new List<PositionData>();
        for (int i = 0; i < num; i++)
        {
            float x = Radis * Mathf.Sin(i * Sita);
            float z = Radis * Mathf.Cos(i * Sita);
            Vector3 tem_pos = new Vector3(x, 0, z);
            Quaternion tem_qua = Quaternion.AngleAxis(i * Degree, Vector3.up);
            PositionData tem_data = new PositionData(tem_pos, tem_qua);
            data.Add(tem_data);
        }
        return data;
    }

    private List<Transform> GetChildList()
    {
        Transform myTrans = transform;
        List<Transform> list = new List<Transform>();

        for (int i = 0; i < myTrans.childCount; ++i)
        {
            Transform t = myTrans.GetChild(i);
            if (t && NGUITools.GetActive(t.gameObject))
                list.Add(t);
        }
        return list;
    }

    private bool islock = false;
    public void MoveToPage(int page)
    {
        if (page > TotalPage || page < 1 || islock)
        {
            return;
        }
        islock = true;
        CurPage = page;
        Quaternion tem_qua;
        float sita = 2 * Mathf.Atan2(0.5f * Width, Radis);
        float degree = 180f * sita / Mathf.PI;
        tem_qua = Quaternion.Euler(0, -(DegreePerPage / 2 + (CurPage - 1) * DegreeClipPerPage), 0);

        FadeInPage(CurPage, true);

        TweenRotation tr = TweenRotation.Begin(gameObject, 0.6f, tem_qua);
        tr.AddOnFinished(() =>
        {
            islock = false;
            FadeInPage(CurPage - 1, false);
            FadeInPage(CurPage + 1, false);
        });
        tr.PlayForward();
    }

    private void FadeInPage(int page, bool isactive)
    {
        if (page > TotalPage || page < 1)
        {
            return;
        }
        List<Transform> list = GetChildList();
        for (int i = (page - 1) * NumPerPage; i < list.Count && i < page * NumPerPage; i++)
        {
            TweenAlpha ta = TweenAlpha.Begin(list[i].gameObject, 0.3f, isactive ? 1f : 0);
            ta.PlayForward();
        }
    }

    public void MoveToRight()
    {
        MoveToPage(CurPage + 1);
    }

    public void MoveToLeft()
    {
        MoveToPage(CurPage - 1);
    }

    [ContextMenu("Execute")]
    private void Reposition()
    {
        isInit = false;
        ResetPosition(GetChildList());
    }
}

public class PositionData
{
    public Vector3 position;
    public Quaternion quaternion;

    public PositionData(Vector3 pos, Quaternion qua)
    {
        position = pos;
        quaternion = qua;
    }
}