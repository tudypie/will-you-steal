using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        Vector3 targetPos = player.position + offset;

        transform.position = targetPos;
    }
}
