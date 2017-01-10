using UnityEngine;

public class TestPageItem : PageItemBase
{
    public override void SetStyle()
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            int row, col;
            row = i / 4;
            col = i % 4;
            Transform tran = ItemList[i].transform;
            tran.localPosition = new Vector3(col * 90, -row * 60, 0);
        }
    }
}