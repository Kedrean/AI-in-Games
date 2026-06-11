using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 15f, -10f);

    [Header("Movement")]
    public float followSpeed = 8f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(target.forward, Vector3.up);

        Vector3 rotatedOffset =
            targetRotation * offset;

        Vector3 desiredPosition =
            target.position + rotatedOffset;

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime);

        Quaternion desiredRotation =
            Quaternion.LookRotation(
                target.position - transform.position,
                Vector3.up);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSpeed * Time.deltaTime);
    }
}