/* ds18635 2101128
 * ======================
 * This class is utilised by the BusyState for setting tasks. As tasks take time, this class would create a Timer game
 * object that would do an action at the end of the timer (such as reward the player resources) and then destroy itself.
 * Further the timer would be able to check if it was interrupted allowing for cancelation of tasks.
 * ======================
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class TaskTimer {
    private static List<TaskTimer> activeTimers;
    private static GameObject initObject;

    private readonly Action action;
    private readonly GameObject gameObject;
    private bool isDestroyed;
    private readonly string name;
    private float timer;

    private TaskTimer(Action action, float timer, GameObject gameObject, string name) {
        this.action = action;
        this.timer = timer;
        this.name = name;
        this.gameObject = gameObject;
        isDestroyed = false;
    }

    private static void Init() {
        if (initObject == null) {
            initObject = new GameObject("TimerObject");
            activeTimers = new List<TaskTimer>();
        }
    }

    public static TaskTimer Create(Action action, float timer, string name = null) {
        Init();
        var gameObject = new GameObject("TaskTimer", typeof(TaskTimerBehaviour));
        var taskTimer = new TaskTimer(action, timer, gameObject, name);
        gameObject.GetComponent<TaskTimerBehaviour>().onUpdate = taskTimer.Update;
        activeTimers.Add(taskTimer);
        return taskTimer;
    }

    private static void RemoveTimer(TaskTimer taskTimer) {
        Init();
        activeTimers.Remove(taskTimer);
    } 

    public static void StopTimer(string name) {
        for (var i = 0; i < activeTimers.Count; i++)
            if (activeTimers[i].name == name) {
                activeTimers[i].DestroySelf();
                i--;
            }
    }

    private void Update() {
        if (!isDestroyed) {
            timer -= Time.deltaTime;
            if (timer < 0) {
                action();
                DestroySelf();
            }
        }
    }

    private void DestroySelf() {
        isDestroyed = true;
        Object.Destroy(gameObject);
        RemoveTimer(this);
    }

    private class TaskTimerBehaviour : MonoBehaviour {
        public Action onUpdate;

        private void Update() {
            if (onUpdate != null) onUpdate();
        }
    }
}