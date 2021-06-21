using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoulSphereController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float startImpulse = 4f;
    [SerializeField]
    float forceValue = 2f;
    [SerializeField]
    float flyingDuration = 2f;
    [SerializeField]
    float speed = 10f;

    Vector3 startPosition;
    bool isVibrate = false;

    private void Start()
    {
        startPosition = transform.position;

        rb = GetComponent<Rigidbody>();

        StartCoroutine(FlyingUpward());
    }

    private void FixedUpdate()
    {
        if (isVibrate)
            rb.AddForce((startPosition - transform.position).normalized * forceValue);
    }

    private IEnumerator FlyingUpward()
    {
        transform.position -= 15f * Vector3.up;
        float length = Mathf.Abs(transform.position.y - startPosition.y);

        while(transform.position.y < startPosition.y)
        {
            rb.velocity = (startImpulse + 5f * (Mathf.Abs(transform.position.y - startPosition.y) / length)) * Vector3.up;

            yield return new WaitForFixedUpdate();
        }

        transform.position = startPosition;

        isVibrate = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * startImpulse, ForceMode.Impulse);
    }
}
