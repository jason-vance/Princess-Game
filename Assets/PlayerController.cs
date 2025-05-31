using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float MoveSpeed;

    private Vector2? targetPosition;

    void Start()
    {
    }

    void Update()
    {
        HandleUserInput();
        HandleMovement();
    }

    private void HandleUserInput()
    {
        if (Input.GetMouseButtonUp(0)) {
            OnTap(Input.mousePosition);
        } else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
            OnTap(Input.GetTouch(0).position);
        }
    }

    private void OnTap(Vector2 position)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector2(position.x, position.y));

        // Use the worldPoint to interact with your scene (e.g., raycast, move objects)
        targetPosition = new Vector2((int)worldPoint.x, (int)worldPoint.y);
    }

    private void HandleMovement()
    {
        if (targetPosition != null) {
            Vector2 targetPosition = (Vector2)this.targetPosition;
            transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * MoveSpeed);

            var closeEnoughToTarget = Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetPosition) < Vector2.SqrMagnitude(new Vector2(0.02f, 0.02f));
            if (closeEnoughToTarget) {
                transform.position = targetPosition;
                this.targetPosition = null;
            }
        }
    }
}
