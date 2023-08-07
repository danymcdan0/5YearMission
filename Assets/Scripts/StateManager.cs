/* Danyal Saleh ds18635 2101128
 * ======================
 * This class runs the state machine for each individual colonist and is able to track the colonist state as it
 * transitions. It holds the colonist information (name & color) and is able to check if the colonist has been clicked
 * for the SelectedState to be transitioned to.
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StateManager : MonoBehaviour {
    public State currentState;
    public IdleState idleState;
    public WanderState wanderState;
    public MoveState moveState;
    public SelectedState selectedState;
    public BusyState busyState;
    public State previousState;
    public ColonistGridMovement colonistGridMovement;
    public GameObject editHandler;
    public GameObject playerTaskHandler;
    public string colName;
    public Color colColor;
    public bool Selected;
    public bool busyInterrupt;
    //5 10 15 20 25 30 35 40 45 --> 5 15 30 50 80 115 155 200

    private readonly List<Color> ascii_color = new List<Color> {
        Color.red,
        Color.green,
        Color.yellow,
        Color.magenta
    };

    private readonly float offset = 1;
    private SpriteRenderer sprite;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        Selected = false;
        sprite.color = colColor;
        editHandler = GameObject.FindWithTag("EditHandler");
        playerTaskHandler = GameObject.FindWithTag("MainCamera");
    }

    private void Update() {
        RunStateMachine();
        if (currentState == wanderState && wanderState.MoveComplete == false) {
            var p = colonistGridMovement.GetPosition();
            var destination = new Vector3(p.x + offset, p.y + offset, 0);
            colonistGridMovement.SetRandomPosition(destination);
        }
        SelectColor();
        busyState.SetName(colName);
        Interrupted();
    }

    private void Interrupted() {
        if (Selected && busyInterrupt) {
            Debug.LogWarning("Stopping task for: " + colName);
            TaskTimer.StopTimer(colName + " Task");
            busyState.taskSet = false;
            busyInterrupt = false;
        }
        if (!Selected && currentState == busyState) {
            busyInterrupt = true;
        }
    }

    private void OnMouseDown() {
        if (!editHandler.GetComponent<EditHandler>().Mineactive &&
            !editHandler.GetComponent<EditHandler>().Editactive &&
            !playerTaskHandler.GetComponent<PlayerTaskHandler>().selectMode) Selected = true;
    }


    private void RunStateMachine() {
        var nextState = currentState?.RunCurrentState();
        if (nextState != null) SwitchToNextState(nextState);
        if (previousState != currentState) Debug.Log(nextState + colName);
    }

    private void SwitchToNextState(State nextState) {
        previousState = currentState;
        currentState = nextState;
        if (previousState == busyState && currentState != busyState) {
            if (!busyState.taskComplete && busyState.taskSet) {
                TaskTimer.StopTimer(colName + " Task");
                Debug.LogWarning("Stopping task for: "+colName);
            }
        }
    }

    private void SelectColor() {
        if (Selected)
            sprite.color = Color.white;
        else
            sprite.color = colColor;
        if (currentState == busyState && !Selected) {
            sprite.color = Color.gray;
        }
    } 


    public void SetColor(string color) {
        if (color == "Yellow")
            colColor = Color.yellow;
        else if (color == "Magenta")
            colColor = Color.magenta;
        else if (color == "Red")
            colColor = Color.red;
        else if (color == "Green")
            colColor = Color.green;
        else
            colColor = Color.white;
    }

    public void SetRandomColor() {
        int col = Random.Range(1, 5);
            switch (col) {
                case 1:
                    colColor = Color.yellow;
                    break;
                case 2:
                    colColor = Color.green;
                    break;
                case 3:
                    colColor = Color.red;
                    break;
                case 4:
                    colColor = Color.magenta;
                    break;
        }
            if (sprite !=null) {
                sprite.color = colColor;
            }
    }

    public void StateChange() {
        idleState.task = true;
    }

    public void SetName(string s) {
        colName = s;
    }
}