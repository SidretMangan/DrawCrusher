using UniRx.Toolkit;
using UnityEngine;

namespace DrawCrusher.BallSpawner
{
    /// <summary>
    /// ObjectPool to manage balls
    /// </summary>
    public class BallObjectPool : ObjectPool<Ball>
    {
        /// <summary>
        /// Ball Prefabs
        /// </summary>
        private readonly Ball _prefab;

        /// <summary>
        /// Parent Transform in the hierarchy
        /// </summary>
        private readonly Transform _root;

        public BallObjectPool(Ball prefab)
        {
            _prefab = prefab;

            // Object to be parent
            _root = new GameObject().transform;
            _root.name = "Balls";
            _root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        protected override Ball CreateInstance()
        {
            // Instantiate when a new instance is needed
            var newBall = GameObject.Instantiate(_prefab);

            // Change the parent Transform
            newBall.transform.SetParent(_root);

            return newBall;
        }
    }
}
