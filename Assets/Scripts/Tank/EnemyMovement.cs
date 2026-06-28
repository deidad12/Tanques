using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public float moveDistance = 10f;

    private Vector3 startPos;
    private Vector3 direction = Vector3.right;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            direction *= -1f;
            startPos = transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Rebote al chocar con cualquier cosa sólida
        direction *= -1f;
    }
}