using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    private ShopController _shopController;
    private ShopModel _shopModel;
    
    [SerializeField] 
    private Button btnAddGold;
    [SerializeField] 
    private Text textGold;

    public void Awake()
    {
        _shopModel = new ShopModel();
        _shopController = new ShopController(_shopModel);
    }

    public void OnEnable()
    {
        // View层
        btnAddGold.onClick.AddListener(_shopController.AddGold);
        _shopModel.GoldChanged += Refresh;
    }

    public void OnDisable()
    {
        // View层
        btnAddGold.onClick.RemoveListener(_shopController.AddGold);
        _shopModel.GoldChanged -= Refresh;
    }

    private void Refresh()
    {
        textGold.text = $"金币数量：{_shopModel.Gold}";
    }
}
