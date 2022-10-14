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

        private BallObjectPool _objectPool;

        private void Start()
        {
            // Get ObjectPool
            _objectPool = _ballObjectPoolProvider.Get();

            // shoot balls regularly
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => ShootBalls()).AddTo(this);
        }

        private void ShootBalls()
        {
            // 3way
            for (var i = -1; i < 2; i++)
            {
                var b = _objectPool.Rent(); // Get a Ball instance

                // direction of ball
                var dir = Quaternion.AngleAxis(i * 30, transform.up) * transform.forward;

                // ball placement
                var initPos = transform.position + dir * 1.0f;

                // Set the initial position and velocity of the ball
                b.Initialize(initPos, dir * 3.0f);

                // Fires a ball and returns it to the ObjectPool when finished
                b.OnFinishedAsync
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        _objectPool.Return(b);
                    });
            }
        }
    }
}
