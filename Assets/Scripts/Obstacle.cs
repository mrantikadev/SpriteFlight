using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 2.0f;

    [SerializeField] private float minSpeed = 50f;
    [SerializeField] private float maxSpeed = 150f;

    [SerializeField] float maxSpinSpeed = 10f;

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


    private void Update()
    {

    }
}
