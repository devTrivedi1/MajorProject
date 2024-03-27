using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunEnder : MonoBehaviour
{
    bool open = true;

    private void Start()
    {
        Objective objective = FindObjectOfType<Objective>();
        if (objective != null)
        {
            objective.ObjectiveComplete += () => open = true;
            open = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (open && other.TryGetComponent(out Movement player))
        {
            GameManager.Instance.Win();
        }
    }
}
