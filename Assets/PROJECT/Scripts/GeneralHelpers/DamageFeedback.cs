using CustomInspector;
using System.Collections;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    [Header("Color Animation Settings")]
    [SerializeField] float duration;
    [SerializeField] Color takeDamageColor;
    [SerializeField] float targetIntensity;

    [Header("scale Animation  Settings")]
    [SerializeField] float takeDamageScale;
    [SerializeField] AnimationCurve damageCurve;
    [SerializeField] bool animateScale;

    [SerializeField] bool getMultipleRenderers = false;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Renderer[] renderers;

    float startZScale;
    float startXScale;

    float[] startZScales;
    float[] startXScales;

    float currentIntensity;

    bool isTakingDamage;
    float elapsedTime;
    Color startColor;
    Color[] startColors;


    private void OnEnable()
    {
        IDamageable.OnDamageTaken += EnableTakeDamageEffects;
    }

    private void OnDisable()
    {
        IDamageable.OnDamageTaken -= EnableTakeDamageEffects;
    }
    private void Start()
    {
        if (meshRenderer == null && !getMultipleRenderers)
        {
            TryGetComponent(out meshRenderer);
            if (meshRenderer != null) return;

            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent(out meshRenderer)) continue;

                startZScale = transform.localScale.z;
                startXScale = transform.localScale.x;
                startColor = meshRenderer.material.color;
                return;
            }
        }

        if (!getMultipleRenderers) return;

        renderers = GetComponentsInChildren<Renderer>();
        startZScales = new float[renderers.Length];
        startXScales = new float[renderers.Length];
        startColors = new Color[renderers.Length];

        if (renderers.Length == 0) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            startColors[i] = renderers[i].material.color;

            if (renderers[i] is SkinnedMeshRenderer)
            {
                startZScale = transform.localScale.z;
                startXScale = transform.localScale.x;
            }
            else
            {
                startZScales[i] = renderers[i].transform.localScale.z;
                startXScales[i] = renderers[i].transform.localScale.x;
            }
        }
    }

    void EnableTakeDamageEffects(int damage, GameObject _damageable)
    {
        if (_damageable == gameObject)
        {
            elapsedTime = 0;
            isTakingDamage = true;
        }
    }

    private void Update()
    {
        ShowTakeDamageEffects();
    }

    void ShowTakeDamageEffects()
    {
        if (!isTakingDamage) return;
        if (meshRenderer == null && renderers.Length == 0) return;
        if (duration >= elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            float _lerpValue = damageCurve.Evaluate(elapsedTime / duration);
            if (getMultipleRenderers && renderers[0] is not SkinnedMeshRenderer)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SetScaleAndColorOfRenderers(renderers[i], renderers[i].transform, startColors[i],
                    _lerpValue, startZScales[i], startZScales[i]);
                }
            }
            else
            {
                SetScaleAndColorOfRenderers(meshRenderer, meshRenderer.transform, startColor,
                _lerpValue, startZScale, startXScale);
            }
        }
        else { isTakingDamage = false; }
    }

    void SetScaleAndColorOfRenderers(Renderer renderer, Transform meshScale, Color targetStartColor,
        float _lerpValue, float startZScale, float startXScale)
    {
        renderer.material.color = Color.Lerp(takeDamageColor, targetStartColor, _lerpValue);
        currentIntensity = Mathf.Lerp(targetIntensity, 0, _lerpValue);
        renderer.material.SetColor("_EmissionColor", Color.Lerp(takeDamageColor / 3f, Color.black, _lerpValue));

        if (!animateScale) return;
        float _localScaleZ = Mathf.Lerp(takeDamageScale, startZScale, _lerpValue);
        float _localScaleX = Mathf.Lerp(takeDamageScale, startXScale, _lerpValue);
        meshScale.localScale = new Vector3(_localScaleX, meshScale.localScale.y, _localScaleZ);
    }
}