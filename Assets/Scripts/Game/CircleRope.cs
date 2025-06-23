using UnityEngine;

public class CircleRope : MonoBehaviour
{
    private float rotateSpeed;

    public void Init(float rotateSpeed) {
        this.rotateSpeed = rotateSpeed;
    }

    private void Update() {
        Quaternion rotate = Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime);
        transform.rotation *= rotate;
    }
}
