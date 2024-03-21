using CustomInspector;
using VInspector;
using UnityEngine.UI;
using UnityEngine;

public class TimeSlowdown : MonoBehaviour
{
    [VInspector.Foldout("Debug")]
    [SerializeField, ReadOnly] float slowTimeRemaining;
    [SerializeField, ReadOnly] bool onCooldown = false;
    [SerializeField, ReadOnly] bool canActivate = false;
    [SerializeField, ReadOnly] bool slowing = false;
    [EndFoldout]

    [HorizontalLine("Settings", 1, FixedColor.Gray)]
    [SerializeField, Range(1, 10)] int segments = 2;
    [SerializeField, Range(0, 5f)] float timePerSegment = 3f;
    [SerializeField, Range(0, 100f)] float slowdownPercentage = 50f;
    [SerializeField, Range(1, 10)] int rechargableSegments = 1;
    [SerializeField, Range(0, 1f)] float activationCooldown = 0.5f;

    [HorizontalLine("UI", 1, FixedColor.Gray)]
    [SerializeField] Transform slowdownUIParent;
    [SerializeField] GameObject segmentPrefab;
    
    Image[] spawnedSegments;
    float fixedDeltaTime;
    float activationTime = 0f;

    private void Start()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
        slowTimeRemaining = segments * timePerSegment;
        rechargableSegments = Mathf.Clamp(rechargableSegments, 0, segments);
        SpawnUI();
    }

    private void SpawnUI()
    {
        spawnedSegments = new Image[segments];
        for (int i = 0; i < segments; i++)
        {
            GameObject newSegment = Instantiate(segmentPrefab, slowdownUIParent);
            spawnedSegments[i] = newSegment.GetComponent<Image>();
            spawnedSegments[i].color = (i < rechargableSegments) ? Color.red : Color.cyan;
        }
    }

    private void Update()
    {
        HandleActivationCooldown();

        if (Input.GetKey(KeyCode.Mouse1) && slowTimeRemaining > 0 && !onCooldown && canActivate)
        {
            HandleSlowdown();
        }
        else if (slowing)
        {
            ResetTimeScale();
        }

        RechargeSlowdown();
        UpdateSlowdownUI();
    }

    private void HandleActivationCooldown()
    {
        if (!canActivate)
        {
            activationTime -= Time.deltaTime;
            canActivate = activationTime <= 0;
        }
    }

    private void HandleSlowdown()
    {
        slowing = true;
        Time.timeScale = slowdownPercentage / 100;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;

        slowTimeRemaining -= Time.unscaledDeltaTime;
        slowTimeRemaining = Mathf.Max(0, slowTimeRemaining);

        onCooldown = slowTimeRemaining == 0;
    }

    private void ResetTimeScale()
    {
        slowing = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime;
        canActivate = false;
        activationTime = activationCooldown;
    }

    private void RechargeSlowdown()
    {
        if (!slowing && slowTimeRemaining < timePerSegment * rechargableSegments)
        {
            slowTimeRemaining += Time.deltaTime;
            slowTimeRemaining = Mathf.Min(slowTimeRemaining, timePerSegment * rechargableSegments);
            onCooldown = slowTimeRemaining < timePerSegment;
        }
    }

    void UpdateSlowdownUI()
    {
        float filledSegmentValue = slowTimeRemaining / timePerSegment;

        for (int i = 0; i < segments; i++)
        {
            if (i < filledSegmentValue)
            {
                spawnedSegments[i].fillAmount = filledSegmentValue - i;
            }
            else
            {
                spawnedSegments[i].fillAmount = 0;
            }
        }
    }
}