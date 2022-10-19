using System;
using UniRx;
using UnityEngine;
using DrawCrusher.Database;
using Cysharp.Threading.Tasks;

namespace DrawCrusher.BlockExchangerMachine
{
    /// <summary>
    /// Every Money Variable Changes spawn money for 3 seconds
    /// </summary>
    public class MoneySpawner : MonoBehaviour, IPrefabProvidable<Money>
    {
        [SerializeField] private IntVariable moneyVariable;
        [SerializeField] private Transform moneySpawnPoint;
        [SerializeField] private Transform moneyPrefabContainer;
        private MoneyAsyncObjectPool _pool;
        [SerializeField] private Money moneyPrefab;
        private void Start()
        {
            _pool = new MoneyAsyncObjectPool(this);
            
            moneyVariable.ObserveEveryValueChanged(x => x.Value).Subscribe(x=>SpawnMoney());
        }
        private void SpawnMoney()
        {
            _pool.RentAsync().Subscribe(x =>
            {
                x.transform.position = moneySpawnPoint.transform.position;
                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => _pool.Return(x));
            });
        }
        public async UniTask<Money> LoadPrefabAsync()
        {
            await UniTask.Delay(100);
            return Instantiate(moneyPrefab,moneyPrefabContainer);
        }
    }
}
