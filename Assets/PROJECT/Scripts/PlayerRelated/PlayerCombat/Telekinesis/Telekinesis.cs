using CustomInspector;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Telekinesis : MonoBehaviour, IResettable
{
    [Resettable] TelekinesisState state = TelekinesisState.Idle;
    Coroutine objectManipulation;

    [VInspector.Foldout("Debug")]
    [SerializeField] bool showTelekinesisRange = false;
    [SerializeField] bool showOrbitDistance = false;
    [SerializeField] bool showLockOnRange = false;
    [EndFoldout]

    [HorizontalLine("Grabbing", 1, FixedColor.Gray)]
    [SerializeField] float telekinesisRange = 10f;
    [SerializeField, Range(0.1f, 100f)] float slowdownPercentage = 50f;
    [SerializeField] float timeToReachPlayer = 0.5f;
    [SerializeField] AnimationCurve grabAnimationCurve;
    [SerializeField] float curveModifier = 4.5f;

    [HorizontalLine("Holding", 1, FixedColor.Gray)]
    [SerializeField] Transform orbitPosition;
    [SerializeField] float orbitDistance = 1f;
    [SerializeField] float orbitSpeed = 50f;
    [SerializeField] float axisChangeSpeed = 0.5f;
    [SerializeField] float axisChangeInterval = 5f;
    [SerializeField] Image targetUI;
    Vector3 nextTargetDirection;
    float axisChangeTimer = 0f;
    Vector3 orbitAxis = Vector3.up;
    Vector3 currentSteeringDirection;

    [HorizontalLine("Throwing", 1, FixedColor.Gray)]
    [SerializeField] float throwForce = 10f;
    [SerializeField] float lockOnRange = 30f;
    [SerializeField] float screenCenterThreshold = 0.4f;
    [SerializeField] float timeToReachTarget = 0.5f;
    [SerializeField] AnimationCurve throwAnimationCurve;

    TelekineticObject[] telekineticObjects;
    [Resettable] TelekineticObject currentObject;
    [Resettable] (Targetable target, Vector3 viewportPoint) currentTarget;

    private void Start()
    {
        currentSteeringDirection = Random.onUnitSphere * orbitDistance;
        nextTargetDirection = Random.onUnitSphere * orbitDistance;
        telekineticObjects = FindObjectsOfType<TelekineticObject>();
        if (Targeting.Instance == null) { new GameObject("Targeting System").AddComponent<Targeting>(); }
    }

    void Update()
    {
        HandleInput();
        if (currentObject)
        {
            currentTarget = Targeting.Instance.GetClosestTargetOnScreen(Camera.main, transform.position, lockOnRange, screenCenterThreshold);
            if (state == TelekinesisState.Holding) OrbitHoverPosition();
        }
        else
        {
            state = TelekinesisState.Idle;
        }
        DisplayTargetUI();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject == null)
            {
                Grab();
            }
            else
            {
                Throw();
            }
        }
    }

    void Grab()
    {
        TelekineticObject nearestObject = GetTelekineticObject();
        if (nearestObject != null)
        {
            currentObject = nearestObject;
            currentObject.rb.useGravity = false;
            state = TelekinesisState.Grabbing;
            objectManipulation = StartCoroutine(ManipulateObject(orbitPosition, grabAnimationCurve, timeToReachPlayer, TelekinesisState.Holding));
        }
    }

    void Throw()
    {
        Targetable targetable = currentTarget.target;
        currentObject.transform.parent = null;
        if (targetable == null)
        {
            currentObject.rb.useGravity = true;
            currentObject.EnableEffect();
            Vector3 throwDirection = Camera.main.transform.forward;
            currentObject.rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
            state = TelekinesisState.Idle;
            StopCoroutine(objectManipulation);
            currentObject = null;
        }
        else
        {
            StopCoroutine(objectManipulation);
            StartCoroutine(ManipulateObject(targetable.transform, throwAnimationCurve, timeToReachTarget));
            currentObject.EnableEffect(targetable.transform);
            currentObject = null;
            state = TelekinesisState.Idle;
        }
    }

    void DisplayTargetUI()
    {
        bool shouldDisplay = (state == TelekinesisState.Holding || state == TelekinesisState.Grabbing) && currentTarget.target != null;
        targetUI.gameObject.SetActive(shouldDisplay);
        if (shouldDisplay)
        {
            Vector3 worldPos = currentTarget.target.transform.position;
            targetUI.rectTransform.position = Camera.main.ViewportToScreenPoint(currentTarget.viewportPoint);
            targetUI.rectTransform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one * 0.25f, Vector3.Distance(worldPos, transform.position) / lockOnRange);
        }
    }

    TelekineticObject GetTelekineticObject()
    {
        TelekineticObject nearestObject = null;
        float closestToScreenCenter = float.MaxValue;
        foreach (var obj in telekineticObjects)
        {
            if (!obj.gameObject.activeSelf || Vector3.Distance(obj.transform.position, transform.position) > telekinesisRange || obj.Thrown) { continue; }
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
            if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
            {
                float distanceToScreenCenter = Vector2.Distance(new(0.5f, 0.5f), viewportPoint);
                if (closestToScreenCenter > distanceToScreenCenter)
                {
                    closestToScreenCenter = distanceToScreenCenter;
                    nearestObject = obj;
                }
            }
        }
        return nearestObject;
    }

    private IEnumerator ManipulateObject(Transform endPosition, AnimationCurve animationCurve, float timeToReach, TelekinesisState state = TelekinesisState.Idle)
    {
        TelekineticObject obj = currentObject;
        if (obj != null)
        {
            Vector3 startPosition = obj.transform.position;
            Vector3 controlPoint1 = startPosition + Random.onUnitSphere * curveModifier;
            Vector3 controlPoint2 = endPosition.position - Random.onUnitSphere * curveModifier;
            float time = 0;
            while (time < timeToReach)
            {
                float adjustedTimeScale = Time.timeScale < 1 ? Time.timeScale * (1 / (slowdownPercentage / 100)) : Time.timeScale;
                time += Time.deltaTime / (adjustedTimeScale);
                float t = animationCurve.Evaluate(time / timeToReach);
                Vector3 targetPosition = CalculateBezierPoint(t, startPosition, controlPoint1, controlPoint2, endPosition.position);
                obj.transform.position = targetPosition;
                yield return null;
            }
            obj.rb.useGravity = true;
            if (state != TelekinesisState.Idle)
            {
                this.state = state;
                obj.transform.parent = orbitPosition;
            }
            else
            {
                obj.rb.AddForce((endPosition.position - obj.transform.position).normalized * throwForce, ForceMode.VelocityChange);
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    void OrbitHoverPosition()
    {
        if (currentObject != null)
        {
            Vector3 toHoverPosition = orbitPosition.position - currentObject.transform.position;
            Vector3 desiredPosition = orbitPosition.position - toHoverPosition.normalized * orbitDistance;
            Vector3 orbitDirection = Vector3.Cross(toHoverPosition, orbitAxis).normalized;
            float circumference = 2 * Mathf.PI * orbitDistance;
            float orbitTime = circumference / orbitSpeed;
            Vector3 tangentialVelocity = orbitDirection * (circumference / orbitTime);
            float distanceFromDesired = (desiredPosition - currentObject.transform.position).magnitude;
            Vector3 centeringForceDirection = (desiredPosition - currentObject.transform.position).normalized;
            Vector3 centeringVelocity = distanceFromDesired * orbitSpeed * centeringForceDirection;
            currentObject.rb.velocity = centeringVelocity + tangentialVelocity;
            orbitAxis = Vector3.Lerp(orbitAxis, currentSteeringDirection, axisChangeSpeed * Time.fixedDeltaTime);

            if (axisChangeTimer >= axisChangeInterval)
            {
                nextTargetDirection = Random.onUnitSphere * orbitDistance;
                axisChangeTimer = 0;
            }
            currentSteeringDirection = Vector3.Lerp(currentSteeringDirection, nextTargetDirection.normalized, axisChangeSpeed * Time.fixedDeltaTime);
            axisChangeTimer += Time.fixedDeltaTime;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showTelekinesisRange)
        {
            Handles.color = new(0, 0, 1, 0.2f);
            Handles.DrawSolidDisc(transform.position, transform.up, telekinesisRange);
        }

        if (showOrbitDistance)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(orbitPosition.position, orbitDistance);
        }

        if (showLockOnRange)
        {
            Handles.color = new(0, 1, 0, 0.2f);
            Handles.DrawSolidDisc(transform.position, transform.up, lockOnRange);
        }
    }
#endif
}

public enum TelekinesisState
{
    Idle,
    Grabbing,
    Holding,
}