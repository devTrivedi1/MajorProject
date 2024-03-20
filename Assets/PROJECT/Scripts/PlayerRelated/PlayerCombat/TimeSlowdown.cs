using CustomInspector;
using VInspector;
using UnityEngine.UI;
using UnityEngine;

public class TimeSlowdown : MonoBehaviour
{
    [VInspector.Foldout("Debug")]
    [SerializeField, ReadOnly] float slowTimeRemaining;
    [SerializeField, ReadOnly] bool onCooldown = false;
    [EndFoldout]

    [SerializeField, Range(1, 10)] int segments = 2;
    [SerializeField, Range(0, 5f)] float timePerSegment = 3f;
    [SerializeField, Range(0, 100f)] float slowdownPercentage = 50f;
    [SerializeField, Range(1, 10)] int rechargableSegments = 1;
    [SerializeField] Transform slowdownUIParent;
    [SerializeField] GameObject segmentPrefab;
    
    Image[] spawnedSegments;
    float fixedDeltaTime;
    bool slowing = false;
    

    private void Start()
    {  
        fixedDeltaTime = Time.fixedDeltaTime;
        slowTimeRemaining = segments * timePerSegment;
        rechargableSegments = Mathf.Clamp(rechargableSegments, 0, segments);
        spawnedSegments = new Image[segments];
        for (int i = 0; i < segments; i++)
        {
            GameObject newSegment = Instantiate(segmentPrefab, slowdownUIParent);
            spawnedSegments[i] = newSegment.GetComponent<Image>();
            if (i < rechargableSegments) 
            { 
                spawnedSegments[i].color = Color.red; 
            }
            else { spawnedSegments[i].color = Color.cyan; }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1) && slowTimeRemaining > 0 && !onCooldown)
        {
            if (!slowing)
            {
                Time.timeScale = slowdownPercentage / 100;
                Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
                slowing = true;
            }

            slowTimeRemaining -= Time.unscaledDeltaTime;
            slowTimeRemaining = Mathf.Max(0, slowTimeRemaining);

            if (slowTimeRemaining == 0)
            {
                onCooldown = true;
            }
        }
        else
        {
            if (slowing)
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = fixedDeltaTime;
                slowing = false;
            }

            if (slowTimeRemaining < timePerSegment * rechargableSegments)
            {
                slowTimeRemaining += Time.deltaTime;
                slowTimeRemaining = Mathf.Min(slowTimeRemaining, timePerSegment * rechargableSegments);
            }
            else
            {
                onCooldown = false;
            }
        }

        UpdateSlowdownUI();
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