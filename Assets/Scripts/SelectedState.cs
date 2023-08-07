/* ds18635 2101128
 * ======================
 * This script is one of the States in the state machine & is forcefully switched to when a colonist is left-mouse
 * clicked. This state indicates that the player is directly controlling the movement of this colonist, interrupting
 * any task the colonist was doing. The colonist returns to IdleState once the player right-mouse clicks.
 * ======================
 */
using UnityEngine;

[RequireComponent(typeof(BusyState))]
public class SelectedState : State {
    public bool MoveComplete;
    public IdleState idleState;

    public override State RunCurrentState() {
        if (!MoveComplete)
            return this;
        return idleState;
    }
} 