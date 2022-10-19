using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Toolkit;

namespace DrawCrusher.BlockExchangerMachine
{
    public class MoneyAsyncObjectPool : AsyncObjectPool<Money>
    {
        /// <summary>
        /// Serving Money Prefabs Asynchronously
        /// </summary>
        private readonly IPrefabProvidable<Money> _moneyPrefabProvider;

        public MoneyAsyncObjectPool(IPrefabProvidable<Money> prefabProvider)
        {
            _moneyPrefabProvider = prefabProvider;
        }

        protected override IObservable<Money> CreateInstanceAsync()
        {
            // Instantiate after Prefab asynchronous loading is finished
            return _moneyPrefabProvider.LoadPrefabAsync().ToObservable();
        }
    }
}
