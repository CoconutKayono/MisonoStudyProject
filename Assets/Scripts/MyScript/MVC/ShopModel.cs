using System;
using UnityEngine;

#region 第一个现实条件：状态可以由多个系统改变（间接或直接）
// 现在增加一个需求：
// 玩家金币可以通过多个系统改变。

//例如：
//- 商店购买：金币减少。
//- 任务奖励：金币增加。
//- 邮件奖励：金币增加。
//- 后台同步：服务器返回新的金币数量。

//  问题出现了。
//  原来的 gold 存在于 ShopUI 中。
//  但是任务系统想增加金币，应该怎么办？
//  有人可能会写shopUI.AddGold(1000);
//  这样写当然可以运行
//  可是仔细想想，为什么任务系统需要知道商店界面的存在？
//  当我们商店界面关闭时，我们的金币还存在吗？它当然存在。那么金币应该由谁负责呢？
//  答案渐渐浮现：我们的ShopUI承担了太多不必要的职责，我们需要将真实的游戏状态从负责显示它的界面上分离出去。

// 现在ShopModel负责玩家金币和规则；shopView负责把金币显示在屏幕上。
// 我们得到一个重要的规则：金币的生命周期不再由某个 UI 界面决定
#endregion

#region 第二个现实条件：多个界面可以展示同一个状态
// 假设金币会在三个位置显示
// HUD金币、商店界面金币、背包界面金币

// 假设玩家当前有1000金币，当玩家消费100金币，他们会自动刷新吗？答案是并不会。
// 因为普通 C# 变量的修改，不会自动让其他对象执行显示更新
// 至此我们发现
// 把数据与 UI 分离，虽然解决了数据归属问题，却引入了数据同步问题。(其实从ShopUI中看，我们的数据和UI还是分离的，比如textGold并不会因为Gold改变自动刷新)
// 解耦往往会带来其他问题，重要的是trade-off，不能一味追求解耦（解耦其实是减轻耦合，而不是消灭耦合）。

// 数据同步问题：谁来负责同步？
// 主动or被动，主动即Model改变，Model自己去通知。（被动同步可以采用轮询，但请思考轮询在这里适合吗？）
// 我们这里采用主动同步
// Model应该如何通知View的改变呢？Model需要知道自己处理有哪些View吗？
// 想象一下，假如我们直接去修改View中的内容，我们就被迫知道了View的细节，我们当然不希望知道View的细节。
// 我们只希望发布一个事实，那就是金币已经改变了。其他系统自己来处理自己的逻辑，那就正好引入了观察者模式。

// 思考：事实应该发布当前数量吗？还是告诉他们让他们自己来查询呢？
#endregion
public class ShopModel
{
    private int gold = 1000;
    public int Gold { get; private set; }
    
    public Action GoldChanged;
    
    public void AddGold(int count)
    {
        gold += count;
        GoldChanged?.Invoke();
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
        GoldChanged?.Invoke();
        return true;
    }
}
