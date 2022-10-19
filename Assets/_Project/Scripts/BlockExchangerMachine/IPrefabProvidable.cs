using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DrawCrusher.BlockExchangerMachine
{
    /// <summary>
    /// If there is a Provider that asynchronously loads and returns a Prefab
    /// </summary>
    public interface IPrefabProvidable<T> where T : Component
    {
        UniTask<T> LoadPrefabAsync();
    }
}
