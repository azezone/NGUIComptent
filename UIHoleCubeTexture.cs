using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI UIHoleCubeTexture")]
public class UIHoleCubeTexture : UIWidget
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
    [System.NonSerialized]
    Rect mOuterUV = new Rect();


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

    private float mradis = -16f;
    public float Radis
    {
        get
        {
            return mradis;
        }
        set
        {
            mradis = value;
            MarkAsChanged();
        }
    }
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


    private float stepLen = 0.001f;
    private int curStep = 0;
    public int CurStep
    {
        get
        {
            return curStep;
        }
        set
        {
            curStep = value;
            if (curStep > 500)
            {
                curStep = 0;
            }
            MarkAsChanged();
        }
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Texture tex = mainTexture;
        if (tex == null) return;


        Vector4 v = drawingDimensions;
        Vector4 u = new Vector4(0, 0, 1, 1);
        Color32 c = drawingColor;

        float vc_x = (v.x + v.z) * 0.5f;
        float vc_y = (v.y + v.w) * 0.5f;

        float uc_x = 0.5f;
        float uc_y = 0.5f;
        float uRadis = 0f;

        if (Radis > tex.width * 0.5f || Radis > tex.height * 0.5f)
        {
            Radis = tex.width > tex.height ? tex.height * 0.5f : tex.width * 0.5f;
            uRadis = tex.width > tex.height ? Radis / (tex.height * 0.5f) : Radis / (tex.height * 0.5f);
        }

        float step = curStep * stepLen;
        #region top
        verts.Add(new Vector3(v.z - Radis, v.w - Radis));
        verts.Add(new Vector3(v.z, v.w));
        verts.Add(new Vector3(v.x, v.w));
        verts.Add(new Vector3(v.x + Radis, v.w - Radis));

        uvs.Add(new Vector3(u.z, u.w - step));
        uvs.Add(new Vector3(u.x, u.w - step));
        uvs.Add(new Vector3(u.x ,uc_y - step));
        uvs.Add(new Vector3(u.z, uc_y - step));
        #endregion

        #region right
        verts.Add(new Vector3(v.z, v.y));
        verts.Add(new Vector3(v.z - Radis, v.y + Radis));
        verts.Add(new Vector3(v.z - Radis, v.w - Radis));
        verts.Add(new Vector3(v.z, v.w));

        uvs.Add(new Vector3(u.z, u.w - step));
        uvs.Add(new Vector3(u.x, u.w - step));
        uvs.Add(new Vector3(u.x, uc_y - step));
        uvs.Add(new Vector3(u.z, uc_y - step));
        #endregion

        #region bottom
        verts.Add(new Vector3(v.x + Radis, v.y + Radis));
        verts.Add(new Vector3(v.x, v.y));
        verts.Add(new Vector3(v.z, v.y));
        verts.Add(new Vector3(v.z - Radis, v.y + Radis));

        uvs.Add(new Vector3(u.z, u.w - step));
        uvs.Add(new Vector3(u.x, u.w - step));
        uvs.Add(new Vector3(u.x, uc_y - step));
        uvs.Add(new Vector3(u.z, uc_y - step));
        #endregion

        #region left
        verts.Add(new Vector3(v.x + Radis, v.w - Radis));
        verts.Add(new Vector3(v.x, v.w));
        verts.Add(new Vector3(v.x, v.y));
        verts.Add(new Vector3(v.x + Radis, v.y + Radis));

        uvs.Add(new Vector3(u.z, u.w - step));
        uvs.Add(new Vector3(u.x, u.w - step));
        uvs.Add(new Vector3(u.x, uc_y - step));
        uvs.Add(new Vector3(u.z, uc_y - step));
        #endregion

        #region colors
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        cols.Add(c);
        #endregion

        int offset = verts.size;
        if (onPostFill != null)
            onPostFill(this, offset, verts, uvs, cols);
    }
}

