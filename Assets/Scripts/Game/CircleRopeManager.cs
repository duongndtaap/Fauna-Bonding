using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRopeManager : MonoBehaviour
{
    [SerializeField] List<Transform> targetCircleTrans;
    [SerializeField] GameObject ropePrefab;
    [SerializeField] float circleRadius;
    [SerializeField] float rotationOffset;
    [SerializeField] float rotationZOffset;
    [SerializeField] float moveTime;
    [SerializeField] float rotateSpeed;

    private List<GameObject> objects = new List<GameObject>();
    private GameObject holder;
    private int circleRopeCount = 0;

    public void AddObjectToCircle(GameObject obj, bool isBegin, bool isFullCircle) {
        if (holder == null) {
            holder = new GameObject("Circle Rope");
            holder.transform.parent = transform;
            holder.transform.localPosition = Vector3.zero;

            circleRopeCount++;
        }

        obj.transform.parent = holder.transform;

        if (objects.Count <= 2) {
            isFullCircle = false;
        }

        if (isBegin) {
            objects.Insert(0, obj);
            if (objects.Count >= 2) {
                CreateRope(0, 1);
                if (isFullCircle) {
                    CreateRope(objects.Count - 1, 0);
                }
            }
        }
        else {
            objects.Add(obj);
            if (objects.Count >= 2) {
                CreateRope(objects.Count - 2, objects.Count - 1);
                if (isFullCircle) {
                    CreateRope(objects.Count - 1, 0);
                }
            }
        }

        ArrangeObjectsInCircle(holder.transform, objects, circleRadius, isFullCircle);

        if (isFullCircle) {
            StartCoroutine(CompleteCircle());
        }
    }

    private void CreateRope(int leftObjIndex, int rightObjIndex) {
        Connect connect1 = objects[leftObjIndex].GetComponent<Connect>();
        Connect connect2 = objects[rightObjIndex].GetComponent<Connect>();
        GameObject ropeObj = Instantiate(ropePrefab, holder.transform);
        Rope rope = ropeObj.GetComponent<Rope>();
        rope.SetConnect(connect1.RightConnect, connect2.LeftConnect);
    }

    private void ArrangeObjectsInCircle(Transform center, List<GameObject> objects, float radius, bool isFullCircle) {
        int count = objects.Count;
        if (count == 0) return;

        int divisor = isFullCircle ? count : count + 1;
        float angleStep = 360f / divisor;

        float startAngle = isFullCircle ? 0f : -angleStep * (count - 1) / 2f + rotationOffset;

        for (int i = 0; i < count; i++) {
            float angle = startAngle + i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            Vector3 worldPos = center.position + offset;
            Vector2 endPos = worldPos;

            Vector3 dir = (worldPos - center.position).normalized;
            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationZOffset;

            Quaternion endRotate = Quaternion.Euler(0, 0, rotZ);

            StartCoroutine(MoveObj(objects[i], objects[i].transform.position, endPos, objects[i].transform.rotation, endRotate, moveTime));
        }
    }

    private IEnumerator MoveObj(GameObject obj, Vector2 startPos, Vector2 endPos, Quaternion startRotate, Quaternion endRotate, float moveTime) {
        GameManager.Instance.SetGameState(GameState.Pause);
        float timer = 0;

        while (timer < moveTime) {
            timer += Time.deltaTime;
            obj.transform.position = Vector2.Lerp(startPos, endPos, timer / moveTime);
            obj.transform.rotation = Quaternion.Lerp(startRotate, endRotate, timer / moveTime);
            yield return null;
        }

        obj.transform.position = endPos;
        obj.transform.rotation = endRotate;
        GameManager.Instance.SetGameState(GameState.Play);
    }

    private IEnumerator CompleteCircle() {
        GameManager.Instance.SetGameState(GameState.Pause);

        yield return StartCoroutine(MoveObj(holder, holder.transform.position, targetCircleTrans[circleRopeCount - 1].position, holder.transform.rotation, holder.transform.rotation, 2 * moveTime));
        ArrangeObjectsInCircle(holder.transform, objects, circleRadius * 3 / 4, true);
        holder.transform.parent = targetCircleTrans[circleRopeCount - 1];
        CircleRope circleRope = targetCircleTrans[circleRopeCount - 1].GetComponent<CircleRope>();
        circleRope.Init(rotateSpeed);

        holder = null;
        objects.Clear();
        GameManager.Instance.SetGameState(GameState.Play);
    }
}
