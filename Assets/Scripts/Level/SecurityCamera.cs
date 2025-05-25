using System;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    [SerializeField] private Vector2 rotationRange = new Vector2(-45f, 45f);
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float waitDelay = 2f;

    [SerializeField, Space] private TriggerCheck trigger;

    private float currentY, currentWaitDelay;
    private int rotationDirection = 1;

    private void Awake()
    {
        currentY = camTransform.localEulerAngles.y;
        if (currentY >= 180f) { currentY -= 360f; }
    }

    private void OnEnable()
    {
        trigger.OnTrigger += OnPlayerSpotted;
    }

    private void OnDisable()
    {
        trigger.OnTrigger -= OnPlayerSpotted;
    } 

    private void Update()
    {
        if (LevelManager.Instance.CountdownHasStarted) { return; }

        if (currentWaitDelay > 0)
        {
            currentWaitDelay -= Time.deltaTime;
            return;
        }

        HandleRotation();
    }

    private void OnPlayerSpotted()
    {
        LevelManager.Instance.StartCountdown();
        enabled = false;
    }

    private void HandleRotation()
    {
        currentY += rotationSpeed * rotationDirection * Time.deltaTime;

        if (currentY >= rotationRange.y)
        {
            currentY = rotationRange.y;
            rotationDirection = -1;
            currentWaitDelay = waitDelay;
        }
        else if (currentY <= rotationRange.x)
        {
            currentY = rotationRange.x;
            rotationDirection = 1;
            currentWaitDelay = waitDelay;
        }

        camTransform.localEulerAngles = new Vector3(
            camTransform.localEulerAngles.x,
            currentY,
            camTransform.localEulerAngles.z
        );
    }
}
