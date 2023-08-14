using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public float timeBetweenEnemies = 1f;
        public EnemySpawnData[] enemySpawnData;
    }

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;
        public int spawnCount = 1;
    }

    public Wave[] waves;
    private int currentWaveIndex = 0;

    private void Start() {
        StartNextWave();
    }

    private void StartNextWave() {
        if (currentWaveIndex < waves.Length) {
            Wave wave = waves[currentWaveIndex];
            StartCoroutine(SpawnWave(wave));
        }
    }

    private IEnumerator SpawnWave(Wave wave) {
        foreach (EnemySpawnData spawnData in wave.enemySpawnData) {
            for (int i = 0; i < spawnData.spawnCount; i++) {
                SpawnEnemy(spawnData.enemyPrefab, spawnData.spawnPoint.position);
                yield return new WaitForSeconds(wave.timeBetweenEnemies);
            }
        }

        currentWaveIndex++;
        StartNextWave();
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition) {
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
