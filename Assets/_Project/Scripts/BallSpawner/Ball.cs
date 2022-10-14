using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DrawCrusher.BallSpawner
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;

        private readonly Subject<Unit> _finishedSubject = new Subject<Unit>();

        /// <summary>
        /// Notify when you're done with an object
        /// </summary>
        public IObservable<Unit> OnFinishedAsync => _finishedSubject.Take(1);

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            // What to do when something hits you
            this.OnTriggerEnterAsObservable()
                .Subscribe(_ => OnHit());
        }

        /// <summary>
        /// Initialize Bullet
        /// </summary>
        public void Initialize(Vector3 initPosition, Vector3 velocity)
        {
            transform.position = initPosition;

            Observable.NextFrame(FrameCountType.FixedUpdate)
                .TakeUntilDisable(this)
                .Subscribe(_ => _rigidbody.AddForce(velocity, ForceMode2D.Force))
                .AddTo(this);

            // end after 3 seconds
            Observable.Timer(TimeSpan.FromSeconds(3))
                .TakeUntilDisable(this)
                .TakeUntilDestroy(this)
                .Subscribe(_ => Finish());
        }
        private void OnHit()
        {
            Debug.Log("bumped into?");

            Finish();
        }

        /// <summary>
        /// Run when you're done using the instance
        /// </summary>
        private void Finish()
        {
            // set speed to zero
            _rigidbody.velocity = Vector3.zero;

            // Discontinued Event Publishing
            _finishedSubject.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _finishedSubject.Dispose();
        }
    }
}
