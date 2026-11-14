using UnityEngine;

public class ShopController
{
    private ShopModel _shopModel;

    public ShopController(ShopModel shopModel)
    {
        _shopModel = shopModel;
    }

    // 这里的命名并不标准，按理说这里应该取更偏业务的名字，而不是AddGold
    public void AddGold()
    {
        _shopModel.AddGold(1000);
    }

    public bool TryCostGold(int count)
    {
        return _shopModel.TryCostGold(count);
    }
    
    
}
