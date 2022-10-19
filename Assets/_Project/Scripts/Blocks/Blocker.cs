using ProceduralToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DrawCrusher.Core;

namespace DrawCrusher.BlockManagement
{
    public enum BlockSize
    {
        Narrow,
        Wide,
    }
    /// <summary>
    /// Blocker clone with procedurally generated levels
    /// </summary>
    public class Blocker
    {
        [Serializable]
        public class Config
        {
            public int wallWidth = 9;
            public int wallHeight = 7;
            public int wallHeightOffset = 0;
            public Transform blocksContainer;
            public Material blockMaterial;
            public Gradient gradient;
            public Mesh blockMesh;
            public Transform blockCollectTransform;
        }

        private const float blockColorMinValue = 0.6f;
        private const float blockColorMaxValue = 0.8f;

        private const float blockHeight = 0.25f;

        private Config config;

        private PhysicsMaterial2D nonBouncyMaterial;
        private List<Block> activeBlocks = new List<Block>();
        private Queue<Block> pool = new Queue<Block>();

        private Dictionary<BlockSize, float> sizeValues = new Dictionary<BlockSize, float>
        {
            {BlockSize.Narrow, 0.125f},
            {BlockSize.Wide, 0.25f},
        };
        public Blocker()
        {
            nonBouncyMaterial = new PhysicsMaterial2D { name = "NonBouncy", bounciness = 0, friction = 1 };
        }
        public void Generate(Config config)
        {
            Assert.IsTrue(config.wallWidth > 0);
            Assert.IsTrue(config.wallHeight > 0);

            this.config = config;
            ResetLevel();
        }
        public void ClearActiveBlocks()
        {
            if (activeBlocks.Count == 0) return;
            // Return all active blocks to the pool
            foreach (var block in activeBlocks)
            {
                ReturnBlockToPool(block);
            }
            activeBlocks.Clear();
        }
        private void ResetLevel()
        {
            GenerateLevel().Forget();
        }
        private async UniTaskVoid GenerateLevel()
        {
            await UniTask.NextFrame();
            ClearActiveBlocks();
            await UniTask.NextFrame();
            for (int y = 0; y < config.wallHeight; y++)
            {
                // Select a color for the current line
                var currentColor = new ColorHSV(config.gradient.Evaluate(y / (config.wallHeight - 1f)));

                // Generate block sizes for the current line
                List<BlockSize> blockSizes = FillWallWithBlocks(config.wallWidth);

                //Vector3 leftEdge = Vector3.left * config.wallWidth / 2 + Vector3.up * (config.wallHeightOffset + y * blockHeight);
                Vector3 leftEdge = Vector3.up * (config.wallHeightOffset + y * blockHeight);
                foreach (var blockSize in blockSizes)
                {
                    var position = leftEdge + Vector3.right * sizeValues[blockSize] / 2;
                    // Randomize the tint of the current block
                    float colorValue = Random.Range(blockColorMinValue, blockColorMaxValue);
                    Color color = currentColor.WithV(colorValue).ToColor();

                    var block = GetBlock();
                    block.transform.position = position;
                    block.transform.localScale = new Vector3(sizeValues[blockSize], blockHeight, 0.125f);
                    block.transform.rotation = Quaternion.Euler(0, 0, 0);
                    block.meshRenderer.material.color = color;

                    activeBlocks.Add(block);

                    leftEdge.x += sizeValues[blockSize];
                }
            }
        }

        private List<BlockSize> FillWallWithBlocks(float width)
        {
            // https://en.wikipedia.org/wiki/Knapsack_problem
            // We are using knapsack problem solver to fill a fixed width with blocks of random width
            Dictionary<BlockSize, int> knapsack;
            float knapsackWidth;
            do
            {
                // Prefill the knapsack to get a nicer distribution of widths
                knapsack = GetRandomKnapsack(width);
                // Calculate a sum of widths in the knapsack
                knapsackWidth = CalculateKnapsackWidth(knapsack);
            } while (knapsackWidth > width);

            width -= knapsackWidth;
            knapsack = PTUtils.Knapsack(sizeValues, width, knapsack);
            var blockSizes = new List<BlockSize>();
            foreach (var pair in knapsack)
            {
                for (var i = 0; i < pair.Value; i++)
                {
                    blockSizes.Add(pair.Key);
                }
            }
            blockSizes.Shuffle();
            return blockSizes;
        }

        private Dictionary<BlockSize, int> GetRandomKnapsack(float width)
        {
            var knapsack = new Dictionary<BlockSize, int>();
            foreach (var key in sizeValues.Keys)
            {
                knapsack[key] = (int)Random.Range(0, width / 3);
            }
            return knapsack;
        }

        private float CalculateKnapsackWidth(Dictionary<BlockSize, int> knapsack)
        {
            float knapsackWidth = 0f;
            foreach (var key in knapsack.Keys)
            {
                knapsackWidth += knapsack[key] * sizeValues[key];
            }
            return knapsackWidth;
        }

        private Block GetBlock()
        {
            Block block;
            if (pool.Count > 0)
            {
                block = pool.Dequeue();
                block.gameObject.SetActive(true);
            }
            else
            {
                block = GenerateBlock();
                block.blockOnHit += () =>
                {
                    CollectThisBlockToMachine(block);
                };
            }
            return block;
        }
        private void CollectThisBlockToMachine(Block block)
        {
            block.transform.DOMoveZ(config.blockCollectTransform.position.z, 0.5f).SetLink(block.gameObject, LinkBehaviour.KillOnDisable).OnComplete(() =>
            {
                block.transform.DOMove(config.blockCollectTransform.position, 0.5f).SetLink(block.gameObject, LinkBehaviour.KillOnDisable);
                block.transform.DORotate(new Vector3(-180f, -180f, 0), 0.5f).SetLink(block.gameObject, LinkBehaviour.KillOnDisable).OnComplete(() =>
                {
                   
                    GameManager.instance.uiManager.moneyVariable.ApplyChange(1);
                    activeBlocks.Remove(block);
                    ReturnBlockToPool(block);
                    if (activeBlocks.Count == 0)
                    {
                        //ResetLevel();
                        GameManager.instance.UpdateGameState(GameManager.GameState.EndGame);
                    }
                });
            });
            
            
        }
        private void ReturnBlockToPool(Block block)
        {
            block.gameObject.SetActive(false);
            pool.Enqueue(block);
        }
        private Block GenerateBlock()
        {
            var go = new GameObject("Block");
            go.transform.parent = config.blocksContainer;
            var block = go.AddComponent<Block>();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = config.blockMesh;
            block.meshRenderer = go.AddComponent<MeshRenderer>();
            block.meshRenderer.material = config.blockMaterial;
            block.gameObject.layer = 8;
            var blockCollider = go.AddComponent<BoxCollider2D>();
            blockCollider.sharedMaterial = nonBouncyMaterial;
            return block;
        }
    }
}
