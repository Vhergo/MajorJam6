using System.Collections;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    public static CoroutineHandler Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public Coroutine StartManagedCoroutine(IEnumerator routine) {
        return StartCoroutine(routine);
    }

    public void StopManagedCoroutine(Coroutine routine) {
        StopCoroutine(routine);
    }
}
