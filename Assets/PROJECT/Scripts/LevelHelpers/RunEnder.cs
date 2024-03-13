using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunEnder : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Movement player))
        {
            GameManager.Instance.Win();
        }
    }
}
