/* ds18635 2101128
 * ======================
 * This script is one of the States in the state machine & is randomly chosen to switch to by the IdleState.
 * This state would return to IdleState according to a Boolean for checking if the movement is complete.
 * ======================
 */
public class WanderState : State {
    public IdleState idleState;
    public bool MoveComplete;
    private ColonistGridMovement colonistGridMovement;

    public override State RunCurrentState() {
        if (!MoveComplete)
            return this;
        return idleState;
    }
}   