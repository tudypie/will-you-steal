using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        Vector3 targetPos = player.position + offset;

        transform.position = targetPos;
    }
}
