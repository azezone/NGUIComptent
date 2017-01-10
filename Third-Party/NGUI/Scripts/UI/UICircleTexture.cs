using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI CircleTexture")]
public class UICircleTexture : UIWidget
{
    [HideInInspector]
    [SerializeField]
    Rect mRect = new Rect(0f, 0f, 1f, 1f);
    [HideInInspector]
    [SerializeField]
    Texture mTexture;
    [HideInInspector]
    [SerializeField]
    Material mMat;
    [HideInInspector]
    [SerializeField]
    Shader mShader;
    [HideInInspector]
    [SerializeField]
    Vector4 mBorder = Vector4.zero;

    /// <summary>
    /// Texture used by the UITexture. You can set it directly, without the need to specify a material.
    /// </summary>

    public override Texture mainTexture
    {
        get
        {
            if (mTexture != null) return mTexture;
            if (mMat != null) return mMat.mainTexture;
            return null;
        }
        set
        {
            if (mTexture != value)
            {
                RemoveFromPanel();
                mTexture = value;
                //mPMA = -1;
                MarkAsChanged();
            }
        }
    }

    /// <summary>
    /// Material used by the widget.
    /// </summary>

    public override Material material
    {
        get
        {
            return mMat;
        }
        set
        {
            if (mMat != value)
            {
                RemoveFromPanel();
                mShader = null;
                mMat = value;
                //mPMA = -1;
                MarkAsChanged();
            }
        }
    }

    /// <summary>
    /// Shader used by the texture when creating a dynamic material (when the texture was specified, but the material was not).
    /// </summary>

    public override Shader shader
    {
        get
        {
            if (mMat != null) return mMat.shader;
            if (mShader == null) mShader = Shader.Find("Unlit/Transparent Colored");
            return mShader;
        }
        set
        {
            if (mShader != value)
            {
                RemoveFromPanel();
                mShader = value;
                //mPMA = -1;
                mMat = null;
                MarkAsChanged();
            }
        }
    }

    /// <summary>
    /// UV rectangle used by the texture.
    /// </summary>

    public Rect uvRect
    {
        get
        {
            return mRect;
        }
        set
        {
            if (mRect != value)
            {
                mRect = value;
                MarkAsChanged();
            }
        }
    }

    /// <summary>
    /// Widget's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
    /// This function automatically adds 1 pixel on the edge if the texture's dimensions are not even.
    /// It's used to achieve pixel-perfect sprites even when an odd dimension widget happens to be centered.
    /// </summary>

    public override Vector4 drawingDimensions
    {
        get
        {
            Vector2 offset = pivotOffset;

            float x0 = -offset.x * mWidth;
            float y0 = -offset.y * mHeight;
            float x1 = x0 + mWidth;
            float y1 = y0 + mHeight;

            if (mTexture != null)
            {
                int w = mTexture.width;
                int h = mTexture.height;
                int padRight = 0;
                int padTop = 0;

                float px = 1f;
                float py = 1f;

                if (w > 0 && h > 0)
                {
                    if ((w & 1) != 0) ++padRight;
                    if ((h & 1) != 0) ++padTop;

                    px = (1f / w) * mWidth;
                    py = (1f / h) * mHeight;
                }

            }

            Vector4 br = border;

            float fw = br.x + br.z;
            float fh = br.y + br.w;
            float vx = Mathf.Lerp(x0, x1 - fw, mDrawRegion.x);
            float vy = Mathf.Lerp(y0, y1 - fh, mDrawRegion.y);
            float vz = Mathf.Lerp(x0 + fw, x1, mDrawRegion.z);
            float vw = Mathf.Lerp(y0 + fh, y1, mDrawRegion.w);

            return new Vector4(vx, vy, vz, vw);
        }
    }


    Color32 drawingColor
    {
        get
        {
            Color colF = color;
            colF.a = finalAlpha;
            return colF;
        }
    }

    private float mpercent;
    public float Percent
    {
        get
        {
            return mpercent;
        }
        set
        {
            mpercent = value;
            mChanged = true;
            MarkAsChanged();
        }
    }

    private float mheight = 0.14f;
    public float Height
    {
        get
        {
            return mheight;
        }
        set
        {
            mheight = value;
            mChanged = true;
            MarkAsChanged();
        }
    }

    const int n = 5;

    //float SinSita = Mathf.Sin(n * Mathf.PI / 180);
    //float CosSita = Mathf.Cos(n * Mathf.PI / 180);
    private const float SinSita = 0.08715574f;
    private const float CosSita = 0.9961947f;
    /// <summary>
    /// Adjust the scale of the widget to make it pixel-perfect.
    /// </summary>

    public override void MakePixelPerfect()
    {
        base.MakePixelPerfect();

        Texture tex = mainTexture;
        if (tex == null) return;

        if (tex != null)
        {
            int w = tex.width;
            int h = tex.height;

            if ((w & 1) == 1) ++w;
            if ((h & 1) == 1) ++h;

            width = w;
            height = h;
        }
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Texture tex = mainTexture;
        if (tex == null) return;

        Vector4 v = drawingDimensions;
        Vector4 u = new Vector4(0f, 0f, tex.width, tex.height);
        Color32 c = drawingColor;

        float r0 = (v.w - v.y) * 0.5f;
        float r1 = r0 * (1 - Height);
        float x0 = 0;
        float y0 = r0;

        float x1 = 0;
        float y1 = r1;

        float cx = (v.x + v.z) * 0.5f;
        float cy = (v.y + v.w) * 0.5f;

        float x0_t, y0_t, x1_t, y1_t;

        int per = Mathf.CeilToInt(Percent * 360 / n);


        for (int i = 0; i < per; i++)
        {
            verts.Add(new Vector3(x1 + cx, y1 + cy));
            verts.Add(new Vector3(x0 + cx, y0 + cy));

            uvs.Add(new Vector2(u.x, u.y));
            uvs.Add(new Vector2(u.x, u.w));

            cols.Add(c);
            cols.Add(c);

            x0_t = x0 * CosSita + y0 * SinSita;
            y0_t = y0 * CosSita - x0 * SinSita;
            x0 = x0_t;
            y0 = y0_t;

            x1_t = x1 * CosSita + y1 * SinSita;
            y1_t = y1 * CosSita - x1 * SinSita;
            x1 = x1_t;
            y1 = y1_t;


            verts.Add(new Vector3(x0_t + cx, y0_t + cy));
            verts.Add(new Vector3(x1_t + cx, y1_t + cy));

            uvs.Add(new Vector2(u.x, u.y));
            uvs.Add(new Vector2(u.x, u.w));

            cols.Add(c);
            cols.Add(c);
        }



        int offset = verts.size;
        //Debug.LogError("verts size:" + offset + " uvs size:" + uvs.size + " cols size:" + cols.size);


        if (onPostFill != null)
            onPostFill(this, offset, verts, uvs, cols);
    }
}
