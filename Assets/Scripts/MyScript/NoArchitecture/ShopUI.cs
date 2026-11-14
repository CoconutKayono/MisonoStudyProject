using UnityEngine;
using UnityEngine.UI;

namespace MyScript
{
    #region 为什么需要 UI 架构
    // 首先什么是UI？
    // UI是 User Interface，用户界面
    // 它通常负责“向用户展示系统中的信息”、“接收用户的操作意图”；即呈现信息与响应输入
    // 如果这个游戏只有一个按钮，我们几乎不需要任何复杂架构
    // 我们可以直接写下如下代码
    // 仔细观察代码，如果我们的需求不需要改变，那它完全没有任何问题，我们完全不需要引入任何UI架构
    // 此时引入复杂架构的收益可能小于它带来的成本。
    // 这也是我们学习架构时必须记住的第一条原则
    // 架构不是目的,架构是为了解决已经存在或能够合理预见的复杂性。
    // 
    #endregion
    public class ShopUI : MonoBehaviour
    {
        #region Model:维护权威状态与业务规则
        private int gold = 1000;
        public int Gold { get; private set; }

        public void AddGold(int count)
        {
            gold += count;
        }
    
        public bool TryCostGold(int count)
        {
            // 前置条件：消耗数量必须≥0
            if (0 >= count)
            {
                return false;
            }
            // 不变量：必须满足的条件,金币不能为负数.(业务规则：不允许赊账)
            if (gold >= count)
            {
                return false;
            }

            gold -= count;
            return true;
        }
        #endregion

        #region View:负责呈现信息与处理玩家操作意图
        [SerializeField] 
        private Button btnAddGold;
        [SerializeField] 
        private Text textGold;

        private void Refresh()
        {
            textGold.text = $"金币数量：{gold}";
        }
        #endregion
    
        public void OnEnable()
        {
            // View层
            btnAddGold.onClick.AddListener(AddGold);
        }

        public void OnDisable()
        {
            // View层
            btnAddGold.onClick.RemoveListener(AddGold);
        }

        #region Controller:负责处理玩家操作与被动刷新
        private void AddGold()
        {
            AddGold(1000); 
            Refresh();
        }
        #endregion
    }
}
