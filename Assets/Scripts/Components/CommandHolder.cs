using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public enum StrategyType
    {
        OrbitalPrecisionStrike,
        OrbitalAirburstStrike,
        Orbital380mmHEStrike
    }

    public class CommandHolder : MonoBehaviour
    {
        /// <summary>
        /// 战略类型与对应输入指令的映射表
        /// </summary>
        public static readonly Dictionary<StrategyType, string> Commands = new()
        {
            { StrategyType.OrbitalPrecisionStrike, "DDW" },
            { StrategyType.OrbitalAirburstStrike, "DDD" },
            { StrategyType.Orbital380mmHEStrike, "DSWWASS" }
        };

        /// <summary>
        /// 指令最长长度限制，超出后移除最早输入
        /// </summary>
        private const int MaxCommandLength = 10;

        /// <summary>
        /// 当前输入的指令序列
        /// </summary>
        private readonly StringBuilder cmd = new();

        /// <summary>
        /// 当前仍然可能被匹配到的策略列表（部分匹配）
        /// </summary>
        private List<StrategyType> currentMatched = new();

        /// <summary>
        /// 当前已推进到的最小匹配长度
        /// </summary>
        private int minMatchLength = 0;

        /// <summary>
        /// 是否已经完整匹配了一条指令，此时不再接受新输入，直到松开控制键
        /// </summary>
        private bool isFullMatchedStrategy = false;

        // 事件
        public event Action OnCmdCleared;
        public event Action<char, List<StrategyType>> OnCmdCharInput;
        public event Action<StrategyType> OnCmdMatched;

        // 预缓存键值对，避免每帧分配
        private static readonly StrategyType[] keys = Commands.Keys.ToArray();
        private static readonly string[] values = Commands.Values.ToArray();

        private void Update()
        {
            // 松开左控制键时重置所有状态
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                cmd.Clear();
                isFullMatchedStrategy = false;
                currentMatched.Clear();
                minMatchLength = 0;
                OnCmdCleared?.Invoke();
                return;
            }

            // 已完整匹配时不再接受新输入
            if (isFullMatchedStrategy)
                return;

            // 未按住控制键时禁止输入，并清空已有序列
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (cmd.Length > 0)
                {
                    cmd.Clear();
                    OnCmdCleared?.Invoke();
                }
                return;
            }

            // 获取本次输入的移动键字符
            if (!TryGetInputChar(out char inputChar))
                return;

            // 追加字符并限制长度
            cmd.Append(inputChar);
            if (cmd.Length > MaxCommandLength)
                cmd.Remove(0, cmd.Length - MaxCommandLength);

            CmdMatch(inputChar);
        }

        /// <summary>
        /// 检测本帧按下的 WASD 键，返回对应字符
        /// </summary>
        private static bool TryGetInputChar(out char inputChar)
        {
            if (Input.GetKeyDown(KeyCode.W)) { inputChar = 'W'; return true; }
            if (Input.GetKeyDown(KeyCode.A)) { inputChar = 'A'; return true; }
            if (Input.GetKeyDown(KeyCode.S)) { inputChar = 'S'; return true; }
            if (Input.GetKeyDown(KeyCode.D)) { inputChar = 'D'; return true; }

            inputChar = default;
            return false;
        }

        /// <summary>
        /// 对当前输入序列进行匹配，更新状态并触发相应事件
        /// </summary>
        private void CmdMatch(char inputChar)
        {
            Debug.Log($"Current cmd: {cmd}, Current Matched Count: {currentMatched.Count},\n Min Match Length: {minMatchLength}");
            string current = cmd.ToString();
            int requiredMinLength = currentMatched.Count == 0 ? 1 : minMatchLength + 1;

            List<StrategyType> newMatches = new List<StrategyType>();
            bool fullMatched = false;
            StrategyType fullStrategy = default;

            if (currentMatched.Count == 0)
            {
                // 首次匹配：从所有策略中筛选出满足前缀对齐的
                for (int i = 0; i < keys.Length; i++)
                {
                    if (EndsWithCommandPrefix(current, values[i], requiredMinLength))
                        newMatches.Add(keys[i]);
                }
            }
            else
            {
                // 已有部分匹配时，继续筛选
                for (int i = 0; i < currentMatched.Count; i++)
                {
                    StrategyType strategy = currentMatched[i];
                    string command = Commands[strategy];

                    // 优先检测完整匹配
                    if (current.EndsWith(command, StringComparison.Ordinal))
                    {
                        cmd.Clear(); // 完整匹配后立即清空输入缓冲区
                        fullMatched = true;
                        fullStrategy = strategy;
                        break;
                    }

                    // 保留仍满足最小推进长度的部分匹配
                    if (EndsWithCommandPrefix(current, command, requiredMinLength))
                        newMatches.Add(strategy);
                }
            }

            // 若完整匹配，则只保留该策略
            if (fullMatched)
            {
                newMatches.Clear();
                newMatches.Add(fullStrategy);
            }

            // 应用新匹配列表
            currentMatched = newMatches;

            if (currentMatched.Count > 0)
            {
                minMatchLength = requiredMinLength;
            }
            else
            {
                minMatchLength = 0;
                OnCmdCleared?.Invoke();
                return;
            }

            OnCmdCharInput?.Invoke(inputChar, currentMatched);

            if (fullMatched)
            {
                isFullMatchedStrategy = true;
                OnCmdMatched?.Invoke(fullStrategy);
                currentMatched.Clear();
                minMatchLength = 0;
            }
        }

        /// <summary>
        /// 检查 current 的后缀是否能与 command 的某个前缀对齐，且对齐长度至少为 minLength
        /// </summary>
        private static bool EndsWithCommandPrefix(string current, string command, int minLength)
        {
            if (minLength <= 0) minLength = 1;

            int maxLength = Math.Min(current.Length, command.Length);
            if (maxLength < minLength) return false;

            // 从最长可能前缀逐步缩短比较，但不少于 minLength
            for (int i = maxLength; i >= minLength; i--)
            {
                int startIndex = current.Length - i;
                bool matched = true;

                for (int j = 0; j < i; j++)
                {
                    if (current[startIndex + j] != command[j])
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched) return true;
            }

            return false;
        }
    }
}