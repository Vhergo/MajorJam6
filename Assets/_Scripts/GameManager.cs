using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeScaleValue;

    void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
            }else if (Time.timeScale == 1) {
                Time.timeScale = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            if (Time.timeScale == timeScaleValue) {
                Time.timeScale = 1;
            }else {
                Time.timeScale = timeScaleValue;
            }
        }
    }
}
