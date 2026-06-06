using Assets.Scripts.Components;
using System.Collections.Generic;
using UnityEngine;
using UVSF.ComponentsCollection.Animations;

namespace Assets.Scripts
{
    public class GUICaller : MonoBehaviour
    {
        public CommandHolder cmdHolder;
        public LeftControlHidenWindow hiddenWindow;
        public RectTransform strategyItemContent;

        private Dictionary<StrategyType, EleStrategy> strategyItemDict;

        private void Start()
        {
            strategyItemDict = new Dictionary<StrategyType, EleStrategy>(strategyItemContent.childCount);
            for (int i = 0; i < strategyItemContent.childCount; i++)
            {
                var strategy = strategyItemContent.GetChild(i).GetComponent<EleStrategy>();
                if (!strategyItemDict.TryAdd(strategy.type, strategy))
                {
                    Debug.Log("Duplicate strategy type: " + strategy.type);
                }
            }

            cmdHolder.OnCmdCharInput += CmdHolder_OnCmdCharInput;
            cmdHolder.OnCmdMatched += CmdHolder_OnCmdMatched;
            cmdHolder.OnCmdCleared += CmdHolder_OnCmdCleared;
        }

        private void OnDestroy()
        {
            cmdHolder.OnCmdCharInput -= CmdHolder_OnCmdCharInput;
            cmdHolder.OnCmdMatched -= CmdHolder_OnCmdMatched;
            cmdHolder.OnCmdCleared -= CmdHolder_OnCmdCleared;
        }

        private void Update()
        {
            // 更新UI显示状态
        }

        private void CmdHolder_OnCmdCharInput(char inputChar, List<StrategyType> matchedStrategyTypes)
        {
            foreach (var strategyKV in strategyItemDict)
            {
                var type = strategyKV.Key;

                if (strategyItemDict.TryGetValue(type, out var strategy))
                {
                    if (strategy.IsCoolingDown)
                    {
                        continue;
                    }

                    if (matchedStrategyTypes.Contains(type))
                    {
                        strategy.SetMatchedChar(inputChar);
                    }
                    else
                    {
                        strategy.Marginalize();
                        strategy.ResetMatch();
                    }

                }
            }

        }

        private void CmdHolder_OnCmdMatched(StrategyType type)
        {
            if (strategyItemDict.TryGetValue(type, out var strategy))
            {
                if (strategy.IsCoolingDown)
                {
                    return;
                }
                strategy.SetFullMatched();
            }
        }

        private void CmdHolder_OnCmdCleared()
        {
            foreach (var item in strategyItemDict.Values)
            {
                if (item.IsCoolingDown)
                {
                    continue;
                }
                item.Normalize();
                item.ResetMatch();
            }
        }
    }
}
