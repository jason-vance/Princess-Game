using UnityEngine;
using System.Collections.Generic;

public class ListNode<T>
{
    public T Value;
    public ListNode<T> Next;

    public ListNode(T val)
    {
        Value = val;
    }
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float MoveSpeed;

    private ListNode<Vector2> CurrentPath;
    private Vector2? TargetPosition;

    public int[][] GameWorld;

    void Start()
    {
    }

    void Update()
    {
        HandleUserInput();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (TargetPosition != null) {
            Vector2 targetPosition = (Vector2)TargetPosition;
            transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * MoveSpeed);

            var closeEnoughToTarget = Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetPosition) < Vector2.SqrMagnitude(new Vector2(0.02f, 0.02f));
            if (closeEnoughToTarget) {
                transform.position = targetPosition;
                if (CurrentPath != null) {
                    CurrentPath = CurrentPath.Next;
                    TargetPosition = CurrentPath?.Value;
                }
            }
        }
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

        CurrentPath = FindPathTo(worldPoint);
        if (CurrentPath != null) {
            TargetPosition = CurrentPath.Value;
        }
    }

    private ListNode<Vector2> FindPathTo(Vector2 gameWorldLocation)
    {
        // Initialize all the helpers
        var startLocation = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        var destination = new Vector2Int((int)gameWorldLocation.x, (int)gameWorldLocation.y);

        if (startLocation == destination) { return null;  }

        Vector2Int[] directions = {
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1)
        };

        int[][] grid = new int[GameWorld.Length][];
        for (var i = 0; i < grid.Length; i++) {
            grid[i] = new int[GameWorld[i].Length];
            for (var j = 0; j < grid[i].Length; j++) {
                grid[i][j] = -1;
            }
        }

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(startLocation);

        // Compute the length of the shortest path to each cell (Lee algorithm)
        var level = 0;
        while (queue.Count > 0 && grid[destination.y][destination.x] == -1) {
            var n = queue.Count;

            for (var i = 0; i < n; i++) {
                var location = queue.Dequeue();
                grid[location.y][location.x] = level;

                if (location == destination) {
                    break;
                }

                foreach (var direction in directions) {
                    var neighborLocation = location + direction;

                    if (
                        neighborLocation.y < 0 || neighborLocation.y >= grid.Length
                        || neighborLocation.x < 0 || neighborLocation.x >= grid[neighborLocation.y].Length
                        || grid[neighborLocation.y][neighborLocation.x] != -1
                        || GameWorld[neighborLocation.y][neighborLocation.x] != 0
                    ) {
                        continue;
                    }

                    queue.Enqueue(neighborLocation);
                }

            }

            level++;
        }

        var failedToReachDestination = grid[destination.y][destination.x] == -1;
        if (failedToReachDestination) {
            return null;
        }

        // Turn the grid into a path
        var stack = new Stack<Vector2Int>();
        var currentLocation = destination;

        while (currentLocation != startLocation) {
            stack.Push(currentLocation);

            foreach (var direction in directions) {
                var neighborLocation = currentLocation + direction;

                if (
                    neighborLocation.y < 0 || neighborLocation.y >= grid.Length
                    || neighborLocation.x < 0 || neighborLocation.x >= grid[neighborLocation.y].Length
                ) {
                    continue;
                }

                if (
                    grid[neighborLocation.y][neighborLocation.x] >= grid[currentLocation.y][currentLocation.x]
                    || grid[neighborLocation.y][neighborLocation.x] == -1
                ) {
                    continue;
                }

                stack.Push(neighborLocation);
                currentLocation = neighborLocation;
                break;
            }
        }

        // Construct linked list from stack
        var pathHead = new ListNode<Vector2>(stack.Pop());
        var pathTail = pathHead;
        while (stack.Count > 0) {
            pathTail.Next = new ListNode<Vector2>(stack.Pop());
            pathTail = pathTail.Next;
        }
        return pathHead;
    }
}
