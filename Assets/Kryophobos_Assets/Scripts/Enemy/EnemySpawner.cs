using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _delayBeforeEnablingSpawner;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnTimer;
    [SerializeField] private bool _enemyAlwaysChasing;

    [SerializeField] private GameObject[] _enemySpawns;

    private List<GameObject> _enemyInstances = new List<GameObject>();
    [SerializeField] private int _enemyMaxCap;

    public bool IsSpawnerEnabled; //TESTING

    [SerializeField] private float _force;

    [SerializeField] private GameObject[] _enemies;

    [SerializeField] private bool _enableNormalSpawn;

    private void Start()
    {
        //TESTING BOOL
        if (IsSpawnerEnabled && !_enableNormalSpawn) StartCoroutine(EnableSpawner());
    }

    public IEnumerator EnableSpawner()
    {
        yield return new WaitForSeconds(_delayBeforeEnablingSpawner);

        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds (_spawnTimer);
        }
    }

    //Spawn enemigo mientras que no pase del limite de enemigos (_gameManager.EnemyMaxCap)
    private void SpawnEnemy()
    {        
        if (_enemyInstances.Count < _enemyMaxCap)
        {
            if (_enemyPrefab != null)
            {
                int rng = Random.Range(0,_enemySpawns.Length);
                GameObject newEnemyInstance = Instantiate(_enemyPrefab, _enemySpawns[rng].transform.position, _enemySpawns[rng].transform.rotation);
                _enemyInstances.Add(newEnemyInstance);

                if (_enemyAlwaysChasing)
                {
                    Enemy enemy = newEnemyInstance.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        enemy.AlwaysChase = true;
                        enemy.SpawnJump = true;
                        enemy.MovementStateMachine.Initialize(enemy.SpawnState);
                        enemy.GPS.enabled = false;
                        enemy.RB.useGravity = true;
                        enemy.RB.isKinematic = false;
                        Vector3 direction = _enemySpawns[rng].transform.forward;
                        direction.z = direction.z * _force;
                        enemy.RB.AddForce(direction, ForceMode.Force);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!_enableNormalSpawn) StartCoroutine(EnableSpawner());
            else if (_enableNormalSpawn)
            {
                foreach (var enemy in _enemies)
                {
                    if (enemy != null) enemy.SetActive(true);
                }
            }
        }
    }
}
