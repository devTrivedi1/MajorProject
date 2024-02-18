using System.Collections;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] float telekinesisRange = 10f;
    [SerializeField] float timeToReachPlayer = 0.5f;
    [SerializeField] float throwForce = 10f;
    [SerializeField] Transform objectHoverPosition;
    [SerializeField] float orbitDistance = 1f;
    [SerializeField] float orbitSpeed = 50f;
    [SerializeField] AnimationCurve animationCurve;
    TelekinesisState state = TelekinesisState.Idle;
    TelekineticObject[] telekineticObjects;
    TelekineticObject currentObject;
    private Vector3 orbitAxis = Vector3.up;
    [SerializeField] float steeringSpeed = 1f;
    private Vector3 currentSteeringDirection;

    private void Start()
    {
        currentSteeringDirection = Random.onUnitSphere;
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
                    currentObject.rb.useGravity = false;
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
                    StartCoroutine(GrabObject());
                    state = TelekinesisState.Grabbing;
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

    IEnumerator GrabObject()
    {
        if (currentObject != null)
        {
            Vector3 startPosition = currentObject.transform.position;
            Vector3 endPosition = objectHoverPosition.position - (objectHoverPosition.position - startPosition).normalized * orbitDistance;

            // Define control points for the Bezier curve
            Vector3 controlPoint1 = startPosition + Random.onUnitSphere;
            Vector3 controlPoint2 = endPosition - Random.onUnitSphere;

            float time = 0;
            while (time < timeToReachPlayer)
            {
                time += Time.deltaTime;
                float t = animationCurve.Evaluate(time / timeToReachPlayer);
                currentObject.transform.position = CalculateBezierPoint(t, startPosition, controlPoint1, controlPoint2, endPosition);
                yield return null;
            }
            state = TelekinesisState.Holding;
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
            Vector3 toHoverPosition = objectHoverPosition.position - currentObject.transform.position;
            Vector3 desiredPosition = objectHoverPosition.position - toHoverPosition.normalized * orbitDistance;
            Vector3 orbitDirection = Vector3.Cross(toHoverPosition, orbitAxis).normalized;
            float circumference = 2 * Mathf.PI * orbitDistance;
            float orbitTime = circumference / orbitSpeed;
            Vector3 tangentialVelocity = orbitDirection * (circumference / orbitTime);
            float distanceFromDesired = (desiredPosition - currentObject.transform.position).magnitude;
            Vector3 centeringForceDirection = (desiredPosition - currentObject.transform.position).normalized;
            Vector3 centeringVelocity = distanceFromDesired * orbitSpeed * centeringForceDirection;
            currentObject.rb.velocity = centeringVelocity + tangentialVelocity;
            orbitAxis = Vector3.Lerp(orbitAxis, currentSteeringDirection, Time.deltaTime * steeringSpeed).normalized;
            if (orbitAxis == currentSteeringDirection)
            {
                currentSteeringDirection = Random.onUnitSphere;
            }
        }
    }

    void ThrowObject()
    {
        if (currentObject != null)
        {
            currentObject.rb.useGravity = true;
            Vector3 throwDirection = Camera.main.transform.forward;
            currentObject.rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            state = TelekinesisState.Idle;
            currentObject = null;
        }
    }
}

public enum TelekinesisState
{
    Idle,
    Grabbing,
    Holding,
    Throwing
}