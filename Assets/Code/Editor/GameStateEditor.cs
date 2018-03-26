﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Assets.Code.Editors
{
    [CustomEditor(typeof(GameState.GameState))]
    public class GameStateEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var gameState = target as GameState.GameState;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset"))
            {
                gameState.Reinitialize();
            }
            if (GUILayout.Button("Load from file"))
            {
                string filePath = UnityEditor.EditorUtility.OpenFilePanel(
                    "Load save data",
                    Utils.SaveFileDirectory,
                    "json"
                );
                gameState.Load(filePath);
            }
            if (GUILayout.Button("Save to file"))
            {
                gameState.Save();
            }

            EditorGUILayout.EndHorizontal();
        }

    }
}
