/* ds18635 2101128
 * ======================
 * This script is one of the States in the state machine & is transitioned through the IdleState.cs.
 * This state acts according to a Boolean for checking if the movement is complete and returns BusyState if true.
 * ======================
 */
public class MoveState : State {
    public IdleState idleState;
    public WanderState wanderState;
    public BusyState busyState;
    public bool MoveComplete;

    public override State RunCurrentState() {
        if (!MoveComplete)
            return this;
        return busyState;
    }
} 