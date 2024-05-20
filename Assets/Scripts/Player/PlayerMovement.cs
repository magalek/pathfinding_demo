using System;
using System.Collections;
using System.Threading.Tasks;
using Game;
using Input;
using Map;
using Pathfinding;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float FALL_TIME = 1;
        
        private static readonly int SpeedAnimatorProperty = Animator.StringToHash("Speed");
        private static readonly int FreeFallProperty = Animator.StringToHash("FreeFall");
        private static readonly int GroundedProperty = Animator.StringToHash("Grounded");
        
        private Animator animator;
        
        private PathResult currentPath;
        private Tile nextTile;
        private Tile currentTile;
        private bool isMoving;
        private bool moveCancelRequested;
        private bool canMove;

        private Coroutine movementCoroutine;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            StartCoroutine(PlayStartAnimationsCoroutine());
        }

        private IEnumerator PlayStartAnimationsCoroutine()
        {
            var endPosition = new Vector3(0, 0.5f, 0);
            var startPosition = endPosition + (Vector3.up * 10);
            float t = 0;
            animator.SetBool(FreeFallProperty, true);
            while (t < FALL_TIME)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, t / FALL_TIME);
                yield return null;
            }
            animator.SetBool(FreeFallProperty, false);
            animator.SetBool(GroundedProperty, true);
            canMove = true;
        }

        private void Start()
        {
            currentTile = MapManager.Current.GetTile(Vector2Int.zero);
            InputManager.Current.SelectedTile += OnTileSelected;
        }
        
        public async void StartMovement(Tile tile)
        {
            if (!canMove) return;
            currentPath = await RecalculatePath(tile);
            if (!currentPath.Successful) return;
            currentPath.Invalidated += OnPathInvalidated;
            currentPath.Broken += OnPathBroken;
            await WaitForMovementEnd();
            if (movementCoroutine != null) StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(Move(currentPath));
        }

        private async Task WaitForMovementEnd()
        {
            while (isMoving)
            {
                await Task.Yield();
            }
        }

        private IEnumerator Move(PathResult result)
        {
            animator.SetFloat(SpeedAnimatorProperty, 0);
            while (result.TilesQueue.Count > 0)
            {
                isMoving = true;
                nextTile = result.TilesQueue.Dequeue();
                float t = 0;
                animator.SetFloat(SpeedAnimatorProperty, 2);
                while (t < 1)
                {
                    transform.position = Vector3.Lerp(currentTile.WalkablePosition, nextTile.WalkablePosition, t);
                    if (GameManager.Current.Options.PathDrawingOption.Value) nextTile.SetAsVisited();
                    t += Time.deltaTime * 5;
                    var direction = (nextTile.WalkablePosition - currentTile.WalkablePosition);
                    direction.y = 0;
                    direction.Normalize();
                    transform.rotation = Quaternion.LookRotation(direction);
                    yield return null;
                }
                currentTile = nextTile;
                if (moveCancelRequested)
                {
                    moveCancelRequested = false;
                    break;
                }
            }
            animator.SetFloat(SpeedAnimatorProperty, 0);
            isMoving = false;
        }
        
        private void OnTileSelected(Tile tile)
        {
            if (tile == null) return;
            if (isMoving && tile == currentPath.Destination) return;
            StartMovement(tile);
        }

        private async Task<PathResult> RecalculatePath(Tile tile)
        {
            if (isMoving && nextTile == tile)
            {
                Debug.Log("No recalculation needed");
                return currentPath;
            }
            RequestMovementCancel();
            PathResult result;
            if (isMoving)
            {
                result = await new Path(nextTile ?? currentTile, tile).Calculate();
            }
            else
            {
                result = await new Path(currentTile, tile).Calculate();
            }

            return result;
        }

        private void RequestMovementCancel()
        {
            if (isMoving) moveCancelRequested = true;
        }

        private void OnPathInvalidated()
        {
            currentPath.Invalidated -= OnPathInvalidated;
            StartMovement(currentPath.Destination);
        }
        
        private void OnPathBroken()
        {
            currentPath.Broken -= OnPathBroken;
            RequestMovementCancel();
        }
    }
}