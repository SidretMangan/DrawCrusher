using ProceduralToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

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
            public Material blockMaterial;
            public Gradient gradient;
        }

        private const float blockColorMinValue = 0.6f;
        private const float blockColorMaxValue = 0.8f;

        private const float blockHeight = 0.25f;

        private Config config;

        private Transform blocksContainer;
        private PhysicsMaterial2D nonBouncyMaterial;
        private GameObject borders;
        private List<Block> activeBlocks = new List<Block>();
        private Queue<Block> pool = new Queue<Block>();

        private Dictionary<BlockSize, float> sizeValues = new Dictionary<BlockSize, float>
        {
            {BlockSize.Narrow, 0.125f},
            {BlockSize.Wide, 0.25f},
        };

        public Blocker()
        {
            blocksContainer = new GameObject("Blocks").transform;
            nonBouncyMaterial = new PhysicsMaterial2D { name = "NonBouncy", bounciness = 0, friction = 1 };
        }

        public void Generate(Config config)
        {
            Assert.IsTrue(config.wallWidth > 0);
            Assert.IsTrue(config.wallHeight > 0);

            this.config = config;
            ResetLevel();
        }
        private void ResetLevel()
        {
            GenerateLevel();
        }
        private void CreateBoxCollider2D(Vector2 offset, Vector2 size)
        {
            var collider = borders.AddComponent<BoxCollider2D>();
            collider.sharedMaterial = nonBouncyMaterial;
            collider.offset = offset;
            collider.size = size;
        }

        private void GenerateLevel()
        {
            // Return all active blocks to the pool
            foreach (var block in activeBlocks)
            {
                ReturnBlockToPool(block);
            }
            activeBlocks.Clear();

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
                block.onHit += () =>
                {
                    //TODO create new move not to remove before fall on ground and move to collect machine hub and change it to score money text
                    activeBlocks.Remove(block);
                    ReturnBlockToPool(block);
                    //TODO move this to check after clean all in machine cash out then start. You need to stop ball spawning as well
                    if (activeBlocks.Count == 0)
                    {
                        ResetLevel();
                    }
                };
            }
            return block;
        }

        private void ReturnBlockToPool(Block block)
        {
            block.gameObject.SetActive(false);
            pool.Enqueue(block);
        }

        private Block GenerateBlock()
        {
            var go = new GameObject("Block");
            go.transform.parent = blocksContainer;

            var block = go.AddComponent<Block>();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            block.meshRenderer = go.AddComponent<MeshRenderer>();
            block.meshRenderer.material = config.blockMaterial;

            var blockCollider = go.AddComponent<BoxCollider2D>();
            blockCollider.sharedMaterial = nonBouncyMaterial;
            return block;
        }
    }
}
