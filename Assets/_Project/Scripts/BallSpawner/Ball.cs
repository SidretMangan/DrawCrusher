using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using DrawCrusher.BlockManagement;
using Cysharp.Threading.Tasks;

namespace DrawCrusher.BallSpawner
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private LayerMask _blockLayerMask;
        [SerializeField] private float _explosionRadius;
        private readonly Subject<Unit> _finishedSubject = new Subject<Unit>();

        /// <summary>
        /// Notify when you're done with an object
        /// </summary>
        public IObservable<Unit> OnFinishedAsync => _finishedSubject.Take(1);

        private void Start()
        {
            // What to do when something hits you
            this.OnCollisionEnter2DAsObservable().Subscribe(_ => BallOnHit());
        }
        /// <summary>
        /// Initialize Ball
        /// </summary>
        public void Initialize(Vector3 initPosition)
        {
            transform.position = initPosition;
            // end after 3.5f seconds
            Observable.Timer(TimeSpan.FromSeconds(3.5))
                .TakeUntilDisable(this)
                .TakeUntilDestroy(this)
                .Subscribe(_ => Finish());
        }
        /// <summary>
        /// Initialize Ball
        /// </summary>
        public void InitializeWithVelocity(Vector3 initPosition, Vector3 velocity)
        {
            transform.position = initPosition;

            Observable.NextFrame(FrameCountType.FixedUpdate)
                .TakeUntilDisable(this)
                .Subscribe(_ => _rigidbody.AddForce(velocity, ForceMode2D.Force))
                .AddTo(this);

            // end after 3.5 seconds
            Observable.Timer(TimeSpan.FromSeconds(3.5))
                .TakeUntilDisable(this)
                .TakeUntilDestroy(this)
                .Subscribe(_ => Finish());
        }
        private void BallOnHit()
        {
            GetExplosionBlocks().Forget();
        }
        private async UniTaskVoid GetExplosionBlocks()
        {
            Collider2D[] collidedBlocks = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _blockLayerMask);
            if (collidedBlocks.Length == 0) return;
            foreach (var block in collidedBlocks)
            {
                block.gameObject.GetComponent<Block>()?.InvokeHit();
                await UniTask.NextFrame();
            }
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
