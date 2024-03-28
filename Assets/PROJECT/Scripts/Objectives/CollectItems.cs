using UnityEngine;

public class CollectItems : Objective
{
    Item[] items;

    private void Start()
    {
        items = FindObjectsOfType<Item>();
    }

    public override bool CheckCompletion()
    {
        bool allCollected = true;
        foreach (var item in items)
        {
            if (!item.Collected)
            {
                allCollected = false;
                break;
            }
        }
        return allCollected;
    }
}

public class Item : MonoBehaviour // placeholder class
{
    Movement player;
    public bool Collected { get; private set; } = false;

    private void Start()
    {
        player = FindObjectOfType<Movement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (player.gameObject == collision.gameObject)
        {
            Collected = true;
        }
    }
}