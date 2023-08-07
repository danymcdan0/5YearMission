/* ds18635 2101128
 * ======================
 * This class receives the path that the Pathfinding found and sets the colonists Rigidbody to follow the list of
 * vectors. It handles the animation of the colonist and holds its trait values. This class further controls Booleans
 * from the MoveState, WanderState, and SelectedState to indicate that they can transition states once again.
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;

public class ColonistGridMovement : MonoBehaviour {
    private const float Speed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public Vector3 vector;
    public MoveState moveState;
    public SelectedState selectedState;
    public WanderState wanderState;
    public List<int> traits;
    public List<int> progression;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;

    private void Start() {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update() {
        HandleMovement();
        var pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    } 

    private void HandleMovement() { //Movement follows a list of Vector3s
        if (pathVectorList != null) { //Checks if there is a location to move to
            var targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(rb.transform.position, targetPosition) > 1f) { //Move to the next vector3 in the list
                var moveDir = (targetPosition - rb.transform.position).normalized;
                animator.SetFloat("Horizontal", moveDir.x);
                animator.SetFloat("Vertical", moveDir.y);
                animator.SetFloat("Speed", moveDir.sqrMagnitude);
                rb.transform.position += moveDir * Speed * Time.deltaTime;
            }
            else { //If reached, increase the index for the next vector 3 in the list
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) { //Reached final destination
                    StopMoving();
                    vector = Vector3.zero;
                    animator.SetFloat("Horizontal", vector.x);
                    animator.SetFloat("Vertical", vector.y);
                    animator.SetFloat("Speed", vector.sqrMagnitude);
                }
            }
        }
    }

    private void StopMoving() {
        pathVectorList = null;
        moveState.MoveComplete = true;
        wanderState.MoveComplete = true;
        selectedState.MoveComplete = true;
    }

    public Vector3 GetPosition() {
        return rb.transform.position;
    }

    public bool CheckPath(Vector3 targetPosition) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition); //Gets the path for the movement
        if (pathVectorList != null)
            return true;
        return false;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition); //Gets the path for the movement

        if (pathVectorList != null && pathVectorList.Count > 1) pathVectorList.RemoveAt(0);
    }
    
    public void SetTaskPosition(List<Vector3> targetPosition) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition[0]);
        if (targetPosition.Count>1) {
            for (int i = 1; i < targetPosition.Count; i++) {
                List<Vector3>pathVectorList2 = new List<Vector3>(Pathfinding.Instance.FindPath(GetPosition(), targetPosition[i]));Pathfinding.Instance.FindPath(GetPosition(), targetPosition[i]);
                if (pathVectorList2.Count < pathVectorList.Count ) {
                    pathVectorList = pathVectorList2;
                    Debug.LogWarning("Shorter path found");
                }
            }
        }
        if (pathVectorList != null && pathVectorList.Count > 1) pathVectorList.RemoveAt(0);
    }

    public void SetRandomPosition(Vector3 targetPosition) { //Gets the path for wandering 
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1) pathVectorList.RemoveAt(0);
    }

    public void SetTraits(List<int> t) {
        traits = new List<int>();
        traits.AddRange(t);
    }

    public void SetProgression(List<int> p) {
        progression = new List<int>();
        progression.AddRange(p);
    }

    public void NewProgression() {
        progression = new List<int>();
        for (var j = 0; j < 5; j++) progression.Add(1);
    }

    public void GenTraits() {
        for (var i = 0; i < 5; i++) {
            var number = Random.Range(0, 10);
            traits.Add(number);
        }
    }
    public void Dead() {
        Debug.LogWarning("Kill colonist");
        Destroy(gameObject);
    }
}