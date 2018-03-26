﻿using UnityEngine;
using UnityEditor;
using Assets.Code.References;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(SceneVariable))]
    public class SceneVariableEditor : Editor
    {

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var scene = target as SceneVariable;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.Value) == null)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/CustomAssetError Icon.png");
            }
            if (scene.LoadingZone == null)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/CustomAssetWarning Icon.png");
            }
            return null;
        }

        public override void OnInspectorGUI()
        {
            var sceneVariable = target as SceneVariable;
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneVariable.Value);
            var oldZone = sceneVariable.LoadingZone;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                var scenePathProperty = serializedObject.FindProperty("Value");
                scenePathProperty.stringValue = newPath;

            }
            else
            {
                if (oldScene == null)
                {
                    EditorGUILayout.HelpBox("This scene variable is invalid.", MessageType.Error);
                }
            }

            EditorGUI.BeginChangeCheck();

            var newZone = EditorGUILayout.ObjectField("Loading Zone", oldZone, typeof(SceneLoadingZone), false) as SceneLoadingZone;
            if (EditorGUI.EndChangeCheck())
            {
                var zoneProperty = serializedObject.FindProperty("LoadingZone");
                zoneProperty.objectReferenceValue = newZone;
                oldZone?.RemoveScene(sceneVariable);
                newZone?.AddScene(sceneVariable);
            }


            serializedObject.ApplyModifiedProperties();
        }

    }

}