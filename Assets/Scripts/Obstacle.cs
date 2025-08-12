using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Size Properties")]
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 2.0f;

    [Header("Movement Speed Properties")]
    [SerializeField] private float minSpeed = 50f;
    [SerializeField] private float maxSpeed = 150f;

    [Header("Spin Speed Properties")]
    [SerializeField] float maxSpinSpeed = 10f;

    [Header("Prefab References")]
    [SerializeField] private GameObject impactEffectPrefab;

    private Rigidbody2D rb;

    private void Start()
    {
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        rb = GetComponent<Rigidbody2D>();

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;

        rb.AddForce(randomDirection * randomSpeed);

        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(impactEffectPrefab, contactPoint, Quaternion.identity);

        Destroy(bounceEffect, 1f);
    }
}
