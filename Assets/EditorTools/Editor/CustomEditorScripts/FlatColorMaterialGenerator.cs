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
    public static Material GenerateMaterialWithHexCode(string hexInput, string materialName,string filePath)
    {
        Material newMaterial = GenerateURPMaterial(materialName,filePath);
        ColorUtility.TryParseHtmlString(hexInput, out Color generatedHexColor);

        newMaterial.color = generatedHexColor;
        return newMaterial;
    }

    public static Material GenerateMaterialWithColor(Color color, string materialName, string filePath)
    {
        Material newMaterial = GenerateURPMaterial(materialName,filePath);
        newMaterial.color = color;
        return newMaterial;
    }

    static Material GenerateURPMaterial(string materialName, string filePath)
    {
        Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        string materialPath = filePath + materialName + ".mat";
        AssetDatabase.CreateAsset(newMaterial, materialPath);
        AssetDatabase.SaveAssets();
        Undo.RegisterCreatedObjectUndo(newMaterial, "Created new material");
        return newMaterial;
    }
    public static void SetEmissionForMaterial(Material material, Color emissionColor, float emissionIntensity)
    {
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", emissionColor * emissionIntensity);
    }
}

public class MaterialGeneratorWindow : EditorWindow
{

    private Vector2 windowSize = new Vector2(340, 200);

    //Common Material Properties
    private string hexInput = "#FFFFFF"; // Default value
    private string materialName = "NewMaterial";
    private Color albedoColor = Color.white;
    bool useHexInput = true;

    //Material Emission Properties
    private bool enableEmission = false;
    private Color emissionColor = Color.black;
    private float emissionIntensity = 3.0f;

    Dictionary<FilePathType, string> filePathTypes = new Dictionary<FilePathType, string>();
    FilePathType selectedFilePathType = 0;

    string customMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/Custom/";
    string flatColorMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/FlatColor/";
    string flatColorEmissiveMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/FlatColorEmmisive/";
    string particleMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/Particle/";
    string UIMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/UI/";

    enum FilePathType
    {
        CustomMaterials,
        FlatColorEmissiveMaterials,
        FlatColorMaterials,
        ParticleMaterials,
        UIMaterials
    }

    [MenuItem("Tools/Material Generator")]
    [MenuItem("Assets/Material Generator")]
    static void OpenMaterialGeneratorWindow()
    {
        MaterialGeneratorWindow window = GetWindow<MaterialGeneratorWindow>("Material Generator");
        window.minSize = window.windowSize;
        window.maxSize = window.windowSize;

        window.filePathTypes.Add(FilePathType.CustomMaterials, window.customMaterialsPath);
        window.filePathTypes.Add(FilePathType.FlatColorMaterials, window.flatColorMaterialsPath);
        window.filePathTypes.Add(FilePathType.FlatColorEmissiveMaterials, window.flatColorEmissiveMaterialsPath);
        window.filePathTypes.Add(FilePathType.ParticleMaterials, window.particleMaterialsPath);
        window.filePathTypes.Add(FilePathType.UIMaterials, window.UIMaterialsPath);

        window.Show();
    }

    void OnGUI()
    {
        DrawingColorInput();

        materialName = EditorGUILayout.TextField("Material Name:", materialName);
        enableEmission = EditorGUILayout.Toggle("Enable Emission", enableEmission);

        if (enableEmission)
        {
            emissionColor = EditorGUILayout.ColorField("Emission Color", emissionColor);
            emissionIntensity = EditorGUILayout.Slider("Emission Intensity", emissionIntensity, 0.0f, 10f);
        }

        selectedFilePathType = (FilePathType)EditorGUILayout.EnumPopup("Material File Path", selectedFilePathType);

        SetupGenerateMaterialButton();

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }

    private void SetupGenerateMaterialButton()
    {
        if (GUILayout.Button("Make Material"))
        {

            Material generatedMaterial;
            string selectedFilePath = filePathTypes[selectedFilePathType];

            if (useHexInput)
            {
                generatedMaterial = FlatColorMaterialGenerator.GenerateMaterialWithHexCode(hexInput, materialName,selectedFilePath);
            }
            else
            {
                generatedMaterial = FlatColorMaterialGenerator.GenerateMaterialWithColor(albedoColor, materialName,selectedFilePath);
            }
            if (enableEmission)
            {
                FlatColorMaterialGenerator.SetEmissionForMaterial(generatedMaterial, emissionColor, emissionIntensity);
            }
        }
    }

    private void DrawingColorInput()
    {
        GUILayout.Label("Color Input Type", EditorStyles.boldLabel);
        int selection = GUILayout.SelectionGrid(useHexInput ? 0 : 1, new string[] { "HexCode", "ColorField" }, 2);
        useHexInput = selection == 0;

        if (useHexInput)
        {
            hexInput = EditorGUILayout.TextField("Hex Color:", hexInput);
        }
        else
        {
            albedoColor = EditorGUILayout.ColorField("Albedo Color", albedoColor);
        }
    }
}

