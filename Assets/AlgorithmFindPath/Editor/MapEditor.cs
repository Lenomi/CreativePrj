using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using UnityEngine.AI;

[CustomEditor(typeof(Map))]
public class MapEditor : UnityEditor.Editor
{
    Map map;

    public void OnEnable()
    {
        map = (Map)target;
    }

    //public override void OnInspectorGUI()
    //{
    //    if (target == null)
    //        return;

    //    if (GUILayout.Button("Reset"))
    //    {
    //        map.Clear();
    //    }
    //}
}