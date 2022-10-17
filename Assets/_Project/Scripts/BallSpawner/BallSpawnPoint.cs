using System;
using UniRx;
using UnityEngine;

namespace DrawCrusher.BallSpawner
{
    public class BallSpawnPoint : MonoBehaviour
    {
        /// <summary>
        /// BallObjectPoolProvider set via inspector
        /// </summary>
        [SerializeField]
        private BallObjectPoolProvider _ballObjectPoolProvider;
        [SerializeField] private float _spawnTime=1f;
        [SerializeField] private float _speed=3f;

        private BallObjectPool _objectPool;

        private void Start()
        {
            // Get ObjectPool
            _objectPool = _ballObjectPoolProvider.Get();

            // shoot balls regularly
            Observable.Interval(TimeSpan.FromSeconds(_spawnTime))
                .Subscribe(_ => ShootBalls()).AddTo(this);
        }

        private void ShootBalls()
        {
            var b = _objectPool.Rent(); // Get a Ball instance

            // ball placement
            var initPos = transform.position;
            Vector3 dir = Vector3.up * -_speed;
            // Set the initial position and velocity of the ball
            b.Initialize(initPos, dir);

            // Fires a ball and returns it to the ObjectPoo"l when finished
            b.OnFinishedAsync
                .Take(1)
                .Subscribe(_ =>
                {
                    _objectPool.Return(b);
                });
        }
    }
}
