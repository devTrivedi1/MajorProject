using System.Collections;
using UnityEngine;

public class CoroutineWorkHorse : MonoBehaviour
{
    public static CoroutineWorkHorse Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartWork(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}