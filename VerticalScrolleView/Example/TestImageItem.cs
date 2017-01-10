using UnityEngine;

public class TestImageItem : ImageItemBase
{
    public UILabel mTitle;

    public override void Init(PhotoModelBase data)
    {
        TestPhotoModel model = data as TestPhotoModel;
        mTitle.text = model.Name;
    }
}