using UniRx;
using UnityEngine;

namespace DrawCrusher.BallSpawner
{
    /// <summary>
    /// Provide a BallObjectPool
    /// </summary>
    public class BallObjectPoolProvider : MonoBehaviour
    {
        [SerializeField] private Ball _prefab;

        private BallObjectPool _objectPool;

        public BallObjectPool Get()
        {
            // If it's already prepared, return it
            if (_objectPool != null) return _objectPool;

            // Create ObjectPool
            _objectPool = new BallObjectPool(_prefab);

            // Expand the pool size to 20 in advance
            _objectPool.PreloadAsync(preloadCount: 20, threshold: 20).Subscribe();

            return _objectPool;
        }

        private void OnDestroy()
        {
            // Destroy everything including Ball in the pool
            _objectPool.Dispose();
        }
    }
}
