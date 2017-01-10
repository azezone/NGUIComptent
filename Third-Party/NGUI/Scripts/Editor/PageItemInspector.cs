//----------------------------------------------
//       PicoEditor
// Copyright © 2016 Xavier 
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit PageItemBase.
/// </summary>

[CustomEditor(typeof(PageItemBase), true)]
public class PageItemInspector : Editor
{
    PageItemBase mTex;

    //protected override void OnEnable()
    //{
    //    base.OnEnable();
    //    mTex = target as PageItemBase;
    //}

    //protected override bool ShouldDrawProperties()
    //{
    //    if (target == null) return false;
        
    //    NGUIEditorTools.DrawRectProperty("UV Rect", serializedObject, "mRect");

    //    EditorGUI.EndDisabledGroup();
    //    return true;
    //}

    /// <summary>
    /// Allow the texture to be previewed.
    /// </summary>

}
