/* ds18635 2101128
 * ======================
 * This script is an abstract class for the colonist state machine
 * ======================
 */
using UnityEngine;

public abstract class State : MonoBehaviour {
    public abstract State RunCurrentState();
} 