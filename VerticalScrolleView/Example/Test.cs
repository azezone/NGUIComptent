using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    [SerializeField]
    TestVScrView mScolleView;

    void Start()
    {
        PicoInputManager.Init();
        List<TestPhotoModel> list = new List<TestPhotoModel>();
        for (int i = 0; i < 100; i++)
        {
            TestPhotoModel data = new TestPhotoModel();
            data.Name = "I am:" + i.ToString();
            list.Add(data);
        }

        mScolleView.InitData(list);
        mScolleView.SetController(true);
    }
}
