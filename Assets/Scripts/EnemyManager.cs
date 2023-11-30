using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public List<Enemy> enemyList;
    public bool spawnTogether;
    public float spawnTimer = 1;
}

public class EnemyManager : MonoBehaviour
{
    public List<PlayerCharacter> player;
    public List<Enemy> enemies;
    public List<EnemyWave> enemyWaves;

    private float lastSpawn;

    // Update is called once per frame
    void Update()
    {
        EnemyWave wave = enemyWaves[Mathf.Min(player[0].level, enemyWaves.Count - 1)];
        if (lastSpawn + wave.spawnTimer < Time.time)
        {
            lastSpawn = Time.time;
            SpawnWave(wave);
        }
    }

    public void SpawnWave(EnemyWave wave)
    {
        if (player[0] == null)
        {
            return;
        }
        Vector3 pos = player[0].transform.position + new Vector3((Random.value - 0.5f), 0, (Random.value - 0.5f)).normalized * 10f;
        for (int i = 0; i < wave.enemyList.Count; i++)
        {
            if (!wave.spawnTogether)
            {
                pos = player[0].transform.position + new Vector3((Random.value - 0.5f), 0, (Random.value - 0.5f)).normalized * 10f;
            }
            Enemy ne = Instantiate(wave.enemyList[i], pos + i * new Vector3((Random.value - 0.5f), 0, (Random.value - 0.5f)).normalized * 0.2f, Quaternion.identity, transform);
            ne.em = this;
            ne.moveTarget = player[0].transform;
            enemies.Add(ne);
        }
    }

    public void Die(Enemy e)
    {
        player[0].exp += e.expValue;
        enemies.Remove(e);
    }

}