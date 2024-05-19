using System.Collections;
using System.Threading.Tasks;
using Input;
using Map;
using Pathfinding;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public Tile CurrentTile { get; private set; }

        public bool IsMoving { get; private set; }

        private Animator animator;
        private static readonly int SpeedAnimatorProperty = Animator.StringToHash("Speed");

        private bool moveCancelRequested;

        private Tile nextTile;
        private PathResult currentPath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            CurrentTile = MapManager.Current.GetTile(Vector2Int.zero);
            InputManager.Current.SelectedTile += OnTileSelected;
        }
        
        public async void StartMovement(Tile tile)
        {
            currentPath = await RecalculatePath(tile);
            currentPath.Invalidated += OnPathInvalidated;
            currentPath.Broken += OnPathBroken;
            await WaitForMoveToFinish();
            StartCoroutine(Move(currentPath));
        }

        private IEnumerator Move(PathResult result)
        {
            yield return new WaitWhile(() => IsMoving);
            animator.SetFloat(SpeedAnimatorProperty, 0);
            while (result.TilesQueue.Count > 0)
            {
                IsMoving = true;
                nextTile = result.TilesQueue.Dequeue();
                float t = 0;
                animator.SetFloat(SpeedAnimatorProperty, 2);
                while (t < 1)
                {
                    
                    var lastPosition = transform.position;
                    transform.position = Vector3.Lerp(CurrentTile.WalkablePosition, nextTile.WalkablePosition, t);
                    t += Time.deltaTime * 5;
                    var direction = (transform.position - lastPosition);
                    direction.y = 0;
                    direction.Normalize();
                    transform.rotation = Quaternion.LookRotation(direction);
                    yield return null;
                }
                CurrentTile = nextTile;
                if (moveCancelRequested)
                {
                    moveCancelRequested = false;
                    break;
                }
            }
            animator.SetFloat(SpeedAnimatorProperty, 0);
            IsMoving = false;
        }
        
        private void OnTileSelected(Tile tile)
        {
            StartMovement(tile);
        }

        private async Task<PathResult> RecalculatePath(Tile tile)
        {
            RequestMovementCancel();
            PathResult result;
            if (IsMoving)
            {
                result = await new Path(nextTile ?? CurrentTile, tile).Calculate();
            }
            else
            {
                result = await new Path(CurrentTile, tile).Calculate();
            }

            return result;
        }

        private void RequestMovementCancel()
        {
            if (IsMoving) moveCancelRequested = true;
        }

        private async Task WaitForMoveToFinish()
        {
            while (IsMoving)
            {
                await Task.Yield();
            }
        }

        private void OnPathInvalidated()
        {
            Debug.Log(nameof(OnPathInvalidated));
            currentPath.Invalidated -= OnPathInvalidated;
            StartMovement(currentPath.Destination);
        }
        
        private void OnPathBroken()
        {
            Debug.Log(nameof(OnPathBroken));
            currentPath.Broken -= OnPathBroken;
            RequestMovementCancel();
        }
    }
}