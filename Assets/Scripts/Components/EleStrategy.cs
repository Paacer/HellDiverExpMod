using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Components
{
    public class EleStrategy : MonoBehaviour
    {
        public StrategyType type;
        [Space(20)]
        public Image icon;
        public TextMeshProUGUI s_name;
        public TextMeshProUGUI desc;

        public float maxCoolDownTime;

        private string cmdChar;
        private int currentMatchIndex = -1;
        private float currentCoolDownTime = 0f;
        private StringBuilder descSB = new();

        private const string Activating = "Activating";
        private const string InBoundFormat = "InBound T-";
        private const string Impact = "Impact";
        private const string CoolDownFormat = "CoolDown T-";

        private const string AFormat = "<sprite index=0> ";
        private const string WFormat = "<sprite index=1> ";
        private const string DFormat = "<sprite index=2> ";
        private const string SFormat = "<sprite index=3> ";
        private const string PressedAFormat = "<sprite index=4> ";
        private const string PressedWFormat = "<sprite index=5> ";
        private const string PressedDFormat = "<sprite index=6> ";
        private const string PressedSFormat = "<sprite index=7> ";

        public bool IsCoolingDown => currentCoolDownTime > 1e-6;

        private void Awake()
        {
            cmdChar = CommandHolder.Commands[type];
        }

        /// <summary>
        /// 将输入的字符设置为已匹配状态（即显示为按下状态）
        /// </summary>
        public void SetMatchedChar(char c)
        {
            Normalize();
            currentMatchIndex++;
            descSB.Clear();
            for (int i = 0; i < cmdChar.Length; i++)
            {
                if ( i <= currentMatchIndex)
                {
                    string addon = cmdChar[i] switch
                    {
                        'W' => PressedWFormat,
                        'A' => PressedAFormat,
                        'S' => PressedSFormat,
                        'D' => PressedDFormat,
                        _ => string.Empty
                    };
                    descSB.Append(addon);
                }
                else
                {
                    string addon = cmdChar[i] switch
                    {
                        'W' => WFormat,
                        'A' => AFormat,
                        'S' => SFormat,
                        'D' => DFormat,
                        _ => string.Empty
                    };
                    descSB.Append(addon);
                }
            }
            desc.text = descSB.ToString();
        }

        /// <summary>
        /// 将当前指令设置为完全匹配状态（准备就绪）
        /// </summary>
        public void SetFullMatched()
        {
            desc.text = Activating;
        }

        /// <summary>
        /// 边缘化（未能匹配的状态，重置匹配状态并整体降低透明度）
        /// </summary>
        public void Marginalize()
        {
            icon.material.SetFloat("_alpha", 0.2f);
            s_name.color = new Color(s_name.color.r, s_name.color.g, s_name.color.b, 0.2f);
            desc.color = new Color(desc.color.r, desc.color.g, desc.color.b, 0.2f);
        }

        /// <summary>
        /// 恢复正常显示（重置匹配状态并恢复正常透明度）
        /// </summary>
        public void Normalize()
        {
            icon.material.SetFloat("_alpha", 1f);
            s_name.color = new Color(s_name.color.r, s_name.color.g, s_name.color.b, 1f);
            desc.color = new Color(desc.color.r, desc.color.g, desc.color.b, 1f);
        }

        /// <summary>
        /// 重置匹配状态（即全部显示为未按下状态）
        /// </summary>
        public void ResetMatch()
        {
            currentMatchIndex = -1;
            descSB.Clear();
            for (int i = 0; i < cmdChar.Length; i++)
            {
                string addon = cmdChar[i] switch
                {
                    'W' => WFormat,
                    'A' => AFormat,
                    'S' => SFormat,
                    'D' => DFormat,
                    _ => string.Empty
                };
                descSB.Append(addon);
            }
            desc.text = descSB.ToString();
        }

        
    }
}
