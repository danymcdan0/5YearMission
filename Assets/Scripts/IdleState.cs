/* ds18635 2101128
 * ======================
 * This script is one of the States in the state machine & is the default state for all colonists.
 * This state would determine which state to change to according to Booleans.
 * ======================
 */
using UnityEngine;

public class IdleState : State {
    public WanderState wanderState;
    public MoveState moveState;
    public bool decision;
    public bool task;

    public override State RunCurrentState() {
        if (task) {
            task = false;
            moveState.MoveComplete = false;
            return moveState;
        }

        chooseRandomState();
        if (!decision) {
            wanderState.MoveComplete = false;
            decision = true;
            return wanderState;
        }

        return this;
    }

    private void chooseRandomState() {
        var choice = Random.Range(0, 50);
        if (choice == 1) //No wander state currently
            decision = true;
        else
            decision = true;
    }
} 