using CustomInspector;
using System.Collections;
using UnityEditor;
using UnityEngine;
using VInspector;

public class Telekinesis : MonoBehaviour
{
    TelekinesisState state = TelekinesisState.Idle;
    Coroutine objectManipulation;

    [VInspector.Foldout("Debug")]
    [SerializeField] bool showTelekinesisRange = false;
    [SerializeField] bool showOrbitDistance = false;
    [SerializeField] bool showLockOnRange = false;
    [EndFoldout]

    [HorizontalLine("Grabbing", 1, FixedColor.Gray)]
    [SerializeField] float telekinesisRange = 10f;
    [SerializeField] float timeToReachPlayer = 0.5f;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] float curveModifier = 4.5f;

    [HorizontalLine("Orbitting", 1, FixedColor.Gray)]
    [SerializeField] Transform orbitPosition;
    [SerializeField] float orbitDistance = 1f;
    [SerializeField] float orbitSpeed = 50f;
    [SerializeField] float axisChangeSpeed = 0.5f;
    [SerializeField] float axisChangeInterval = 5f;
    Vector3 nextTargetDirection;
    float axisChangeTimer = 0f;
    Vector3 orbitAxis = Vector3.up;
    Vector3 currentSteeringDirection;

    [HorizontalLine("Throwing", 1, FixedColor.Gray)]
    [SerializeField] float throwForce = 10f;
    [SerializeField] float lockOnRange = 30f;

    TelekineticObject[] telekineticObjects;
    TelekineticObject currentObject;

    private void Start()
    {
        currentSteeringDirection = Random.onUnitSphere * orbitDistance;
        nextTargetDirection = Random.onUnitSphere * orbitDistance;
        telekineticObjects = FindObjectsOfType<TelekineticObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentObject == null)
            {
                TelekineticObject nearestObject = GetTelekineticObject();
                if (nearestObject != null)
                {
                    currentObject = nearestObject;
                    currentObject.Rb.useGravity = false;
                }
            }
            else
            {
                ThrowObject();
            }
        }

        if (currentObject != null)
        {
            switch(state)
            {
                case TelekinesisState.Idle:
                    Vector3 start = currentObject.transform.position;
                    Vector3 end = orbitPosition.position;
                    state = TelekinesisState.Grabbing;
                    objectManipulation = StartCoroutine(ManipulateObject(start, end, TelekinesisState.Holding));
                    break;
                case TelekinesisState.Holding:
                    OrbitHoverPosition();
                    break;
            }
        }
    }

    TelekineticObject GetTelekineticObject()
    {
        TelekineticObject nearestObject = null;
        float closestDistance = telekinesisRange;
        foreach (var obj in telekineticObjects)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                nearestObject = obj;
            }
        }
        return nearestObject;
    }

    private IEnumerator ManipulateObject(Vector3 startPosition, Vector3 endPosition, TelekinesisState state)
    {
        if (currentObject != null)
        {
            Vector3 controlPoint1 = startPosition + Random.onUnitSphere * curveModifier;
            Vector3 controlPoint2 = endPosition - Random.onUnitSphere * curveModifier;
            float time = 0;
            while (time < timeToReachPlayer)
            {
                time += Time.deltaTime;
                float t = animationCurve.Evaluate(time / timeToReachPlayer);
                Vector3 targetPosition = CalculateBezierPoint(t, startPosition, controlPoint1, controlPoint2, endPosition);
                currentObject.transform.position = targetPosition;
                yield return null;
            }
            currentObject.Rb.useGravity = true;
            currentObject.Rb.AddForce(currentObject.transform.position - endPosition);
            this.state = state;
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
            currentObject.Rb.velocity = centeringVelocity + tangentialVelocity;
            orbitAxis = Vector3.Lerp(orbitAxis, currentSteeringDirection, axisChangeSpeed * Time.fixedDeltaTime);

            if (axisChangeTimer >= axisChangeInterval)
            {
                nextTargetDirection = Random.onUnitSphere * orbitDistance;
                axisChangeTimer = 0; // Reset timer
            }
            currentSteeringDirection = Vector3.Lerp(currentSteeringDirection, nextTargetDirection.normalized, axisChangeSpeed * Time.fixedDeltaTime);
            axisChangeTimer += Time.fixedDeltaTime;
        }
    }

    void ThrowObject()
    {
        if (currentObject != null)
        {
            currentObject.Rb.useGravity = true;
            Vector3 throwDirection = Camera.main.transform.forward;
            currentObject.Rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            state = TelekinesisState.Idle;
            StopCoroutine(objectManipulation);
            currentObject = null;
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
    Throwing
}