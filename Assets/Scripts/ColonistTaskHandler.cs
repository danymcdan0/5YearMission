/* ds18635 2101128
 * ======================
 * This class is instantiated by the PlayerTaskHandler to check if there is a path available for the colonist to
 * approach the location of the task, choosing the shortest path. The class handles both standard tasks which do not
 * need to have a specific colonist and Mission tasks which are only set when the chosen colonist is not in their
 * BusyState.
 * ======================
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonistTaskHandler {
    private BusyState busyState;
    private Queue<GameObject> chosenCol;
    private State colonistState;
    private GameObject currentCol;

    private readonly List<int> illegalLocations = new List<int>
        {11, 12, 13, 14, 15, 16, 17, 18, 21, 22, 23, 31, 32, 33, 41, 42, 43};

    private Queue<Tuple<int, Vector3>> missionTask;
    private MoveState moveState;
    private GameObject playerTaskHandler;
    private State previousState;
    private SelectedState selectedState;
    private Vector3 taskLocation;
    private Queue<Tuple<int, Vector3>> taskPos;
    private List<Vector3> availablePos;

    public void Start() {
        taskPos = new Queue<Tuple<int, Vector3>>();
        missionTask = new Queue<Tuple<int, Vector3>>();
        chosenCol = new Queue<GameObject>();
        availablePos = new List<Vector3>();
        playerTaskHandler = GameObject.FindWithTag("MainCamera");
    }

    public void Update() {
        if (taskPos.Count > 0)
            //Check for if there are colonists available for tasks
            playerTaskHandler.GetComponent<PlayerTaskHandler>().TaskAvailable(taskPos.Dequeue());
        if (missionTask.Count > 0)
            //Check for if there are colonists available for missions
            playerTaskHandler.GetComponent<PlayerTaskHandler>()
                .MissionAvailable(missionTask.Dequeue(), chosenCol.Dequeue());
    }

    public void SetColonistTask(int[,] topography, List<GameObject> colonists, Tuple<int, Vector3> task) {
        taskLocation = task.Item2;
        var left = new Vector3(task.Item2.x - 1, task.Item2.y, task.Item2.z);
        var right = new Vector3(task.Item2.x + 1, task.Item2.y, task.Item2.z);
        var down = new Vector3(task.Item2.x - 1, task.Item2.y - 1, task.Item2.z);
        var up = new Vector3(task.Item2.x, task.Item2.y + 1, task.Item2.z);
        var taskAssigned = false;
        if (illegalLocations.Contains(topography[(int) taskLocation.x, (int) taskLocation.y])) return;
        for (var i = 0; i < colonists.Count; i++) {
            moveState = colonists[i].GetComponent<StateManager>().moveState;
            selectedState = colonists[i].GetComponent<StateManager>().selectedState;
            busyState = colonists[i].GetComponent<StateManager>().busyState;
            colonistState = colonists[i].GetComponent<StateManager>().currentState;
            //First check which positions are open
            //Next send the open locations to ColonistGridMovement
            //The colonist grid movement will then check the vector path list of each
            //& set the one that is the shortest to reach.
            if (colonistState != moveState && colonistState != selectedState && colonistState != busyState) { 
                availablePos.Clear();
                if (topography[(int) left.x, (int) left.y] == 0 || topography[(int) left.x, (int) left.y] == 3) {
                    var valid = colonists[i].GetComponent<StateManager>().colonistGridMovement.CheckPath(left);
                    if (valid) {
                        currentCol = colonists[i];
                        availablePos.Add(left);
                    }
                }

                if (topography[(int) right.x, (int) right.y] == 0 || topography[(int) left.x, (int) left.y] == 3) {
                    var valid = colonists[i].GetComponent<StateManager>().colonistGridMovement.CheckPath(right);
                    if (valid) {
                        currentCol = colonists[i];
                        availablePos.Add(right);
                    }
                }

                if (topography[(int) up.x, (int) up.y] == 0|| topography[(int) left.x, (int) left.y] == 3) {
                    var valid = colonists[i].GetComponent<StateManager>().colonistGridMovement.CheckPath(up);
                    if (valid) {
                        currentCol = colonists[i];
                        availablePos.Add(up);
                    }
                }

                if (topography[(int) down.x, (int) down.y] == 0 || topography[(int) left.x, (int) left.y] == 3) {
                    var valid = colonists[i].GetComponent<StateManager>().colonistGridMovement.CheckPath(down);
                    if (valid) {
                        currentCol = colonists[i];
                        availablePos.Add(down);
                    }
                }
                if (availablePos.Count > 0) {
                    SetToTask(availablePos, task.Item1);
                    taskAssigned = true;
                    break;
                }
            }
        }
        if (!taskAssigned) taskPos.Enqueue(task);
    }


    private void SetToTask(List<Vector3> pos, int taskType) {
        currentCol.GetComponent<StateManager>().StateChange();
        currentCol.GetComponent<StateManager>().busyState.index = taskType;
        currentCol.GetComponent<StateManager>().colonistGridMovement.SetTaskPosition(pos);
        currentCol.GetComponent<StateManager>().busyState.taskTarget = taskLocation;
    }
    
    private void SetToMission(Vector3 pos, int taskType) {
        currentCol.GetComponent<StateManager>().StateChange();
        currentCol.GetComponent<StateManager>().busyState.index = taskType;
        currentCol.GetComponent<StateManager>().colonistGridMovement.SetTargetPosition(pos);
        currentCol.GetComponent<StateManager>().busyState.taskTarget = taskLocation;
        Debug.LogWarning("SetToMission");
    }

    public void setColonistMission(int[,] topography, GameObject colonist, Tuple<int, Vector3> task) {
        taskLocation = task.Item2;
        var left = new Vector3(task.Item2.x - 1, task.Item2.y, task.Item2.z);
        var right = new Vector3(task.Item2.x + 1, task.Item2.y, task.Item2.z);
        var down = new Vector3(task.Item2.x - 1, task.Item2.y - 1, task.Item2.z);
        var up = new Vector3(task.Item2.x, task.Item2.y + 1, task.Item2.z);
        var taskAssigned = false;
        moveState = colonist.GetComponent<StateManager>().moveState;
        selectedState = colonist.GetComponent<StateManager>().selectedState;
        busyState = colonist.GetComponent<StateManager>().busyState;
        colonistState = colonist.GetComponent<StateManager>().currentState;
        if (colonistState != moveState && colonistState != selectedState && colonistState != busyState) {
            if (topography[(int) left.x, (int) left.y] == 0) {
                var valid = colonist.GetComponent<StateManager>().colonistGridMovement.CheckPath(left);
                if (valid) {
                    currentCol = colonist;
                    SetToMission(left, task.Item1);
                    taskAssigned = true;
                }
            }

            else if (topography[(int) right.x, (int) right.y] == 0) {
                var valid = colonist.GetComponent<StateManager>().colonistGridMovement.CheckPath(right);
                if (valid) {
                    currentCol = colonist;
                    SetToMission(right, task.Item1);
                    taskAssigned = true;
                }
            }

            else if (topography[(int) up.x, (int) up.y] == 0) {
                var valid = colonist.GetComponent<StateManager>().colonistGridMovement.CheckPath(up);
                if (valid) {
                    currentCol = colonist;
                    SetToMission(up, task.Item1);
                    taskAssigned = true;
                }
            }

            else if (topography[(int) down.x, (int) down.y] == 0) {
                var valid = colonist.GetComponent<StateManager>().colonistGridMovement.CheckPath(down);
                if (valid) {
                    currentCol = colonist;
                    SetToMission(down, task.Item1);
                    taskAssigned = true;
                }
            }
        }

        if (!taskAssigned) {
            missionTask.Enqueue(task);
            chosenCol.Enqueue(colonist);
        }
    }
} 