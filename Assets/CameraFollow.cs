using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;
    public Vector3 offset = new Vector3(0, 1, -10);
    float shake;

    public void Shake(float amount) { shake = amount; }

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);

        if (shake > 0)
        {
            transform.position += (Vector3)Random.insideUnitCircle * shake;
            shake = Mathf.MoveTowards(shake, 0, Time.deltaTime * 1.5f);
        }
    }
}