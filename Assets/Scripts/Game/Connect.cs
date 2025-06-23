using UnityEngine;

public class Connect : MonoBehaviour
{
    [SerializeField] Transform leftConnect;
    [SerializeField] Transform rightConnect;

    public Transform LeftConnect => leftConnect;
    public Transform RightConnect => rightConnect;
}
