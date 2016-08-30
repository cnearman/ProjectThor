using UnityEngine;
using System.Collections;

namespace Assets.Scripts {
    public class EnemySpawner : MonoBehaviour {
        public GameObject enemy;
        public float SpawnRate;
        public bool _isReady;
        public GameObject target;
        // Use this for initialization
        void Start() {
            if (enemy.GetComponent<BaseEnemy>() == null)
            {
                Debug.LogError("YOU FUCKED UP BITCH! WHERE'S MY ENEMY GUY!");
            }
            _isReady = true;
        }

        // Update is called once per frame
        void Update() {
            StartCoroutine(Spawn());
        }

        IEnumerator Spawn()
        {
            if (_isReady)
            {
                _isReady = false;
                yield return new WaitForSeconds(SpawnRate);
                var enemy_inst = Instantiate(enemy);
                enemy_inst.GetComponent<Orc>().Target = target;
                _isReady = true;
            }
        }
    }

}