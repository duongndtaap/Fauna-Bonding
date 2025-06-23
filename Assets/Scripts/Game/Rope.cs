using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] float rotationZRopeOffset;
    private SpriteRenderer spriteRenderer;

    private Transform leftConnect;
    private Transform rightConnect;
    private bool isConnect = false;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetConnect(Transform leftConnect, Transform rightConnect) {
        this.leftConnect = leftConnect;
        this.rightConnect = rightConnect;
        isConnect = true;
    }

    private void Update() {
        if (isConnect) {
            float ropeLength = Vector2.Distance(leftConnect.position, rightConnect.position);
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, ropeLength);

            Vector2 midPoint = (leftConnect.position + rightConnect.position) / 2;
            transform.position = midPoint;

            Vector2 direction = rightConnect.position - leftConnect.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationZRopeOffset;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
