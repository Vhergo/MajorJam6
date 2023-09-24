using UnityEngine;

public class BulletMovementTest : MonoBehaviour
{
    public AnimationCurve bulletPath;
    public float speed = 5f;

    private float currentTime = 0f;

    private void Update() {
        currentTime += Time.deltaTime * speed;
        if (currentTime > bulletPath.keys[bulletPath.length - 1].time) {
            currentTime = 0f; // Reset to loop the path
        }

        float yPos = bulletPath.Evaluate(currentTime);
        Vector3 newPosition = new Vector3(transform.position.x, yPos, transform.position.z);
        transform.position = newPosition;
    }
}
