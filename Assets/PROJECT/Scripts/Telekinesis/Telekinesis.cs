using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float telekinesisRange = 10f;
    [SerializeField] private float objectSpeed = 5f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private Transform objectHoverPosition;
    [SerializeField] private float orbitDistance = 1f;
    [SerializeField] private float orbitSpeed = 50f;
    private TelekineticObject[] telekineticObjects;
    private TelekineticObject currentObject;
    private bool isOrbiting = false;

    private void Start()
    {
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
                ThrowCurrentObject();
            }
        }

        if (currentObject != null)
        {
            if (!isOrbiting)
            {
                MoveObjectToHoverPosition();
            }
            else
            {
                OrbitHoverPosition();
            }
        }
    }

    private TelekineticObject GetTelekineticObject()
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

    private void MoveObjectToHoverPosition()
    {
        if (currentObject != null)
        {
            float distance = Vector3.Distance(currentObject.transform.position, objectHoverPosition.position);
            if (distance <= orbitDistance)
            {
                isOrbiting = true;
            }
            else
            {
                currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, objectHoverPosition.position, objectSpeed * Time.deltaTime);
            }
        }
    }

    private void OrbitHoverPosition()
    {
        if (currentObject != null)
        {
            currentObject.transform.RotateAround(objectHoverPosition.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
    }

    private void ThrowCurrentObject()
    {
        if (currentObject != null)
        {
            currentObject.rb.useGravity = true;
            Vector3 throwDirection = (objectHoverPosition.position - transform.position).normalized;
            currentObject.rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            isOrbiting = false;
            currentObject = null;
        }
    }
}