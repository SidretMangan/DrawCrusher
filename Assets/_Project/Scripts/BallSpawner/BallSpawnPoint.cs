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
        [SerializeField] private float _spawnTime=1.5f;
        [Space]
        [Header("With Velocity")]
        [SerializeField] private bool _withVelocity=false;
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
            if (!_withVelocity)
            {
                // Set the initial position
                b.Initialize(initPos);
            }
            else
            {
                // Set the initial position and velocity of the ball
                Vector3 dir = Vector3.up * -_speed;
                b.InitializeWithVelocity(initPos, dir);
            }

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
