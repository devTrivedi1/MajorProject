using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Color = UnityEngine.Color;


public static class FlatColorMaterialGenerator
{
    public static void GenerateMaterial(string hexInput,string materialName)
    {
        Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        ColorUtility.TryParseHtmlString(hexInput, out Color generatedHexColor);

        newMaterial.color = generatedHexColor;

        string materialPath = "Assets/PROJECT/ArtAssets/Materials/FlatColorMaterials/"+ materialName+".mat";
        AssetDatabase.CreateAsset(newMaterial, materialPath);
        AssetDatabase.SaveAssets();
        Undo.RegisterCreatedObjectUndo(newMaterial, "Created new material");
    }
}

public class MaterialGeneratorWindow : EditorWindow
{
    private string hexInput = "#FFFFFF"; // Default value
    private string materialName = "NewMaterial";
    private Vector2 windowSize = new Vector2(320,200);

    [MenuItem("Tools/Material Generator")]
    [MenuItem("Assets/Material Generator")]
    static void OpenMaterialGeneratorWindow()
    {
        MaterialGeneratorWindow window = GetWindow<MaterialGeneratorWindow>("Material Generator");
        window.minSize = window.windowSize;
        window.maxSize = window.windowSize;
        window.Show();
    }

    void OnGUI()
    {
        hexInput = EditorGUILayout.TextField("Hex Color:", hexInput);
        materialName = EditorGUILayout.TextField("Material Name:", materialName);


        if (GUILayout.Button("Make Material"))
        {
            FlatColorMaterialGenerator.GenerateMaterial(hexInput,materialName);
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }
}

