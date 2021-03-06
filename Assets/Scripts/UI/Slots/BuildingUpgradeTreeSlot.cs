﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingUpgradeTreeSlot : MonoBehaviour
{
    const int connectorWidthPerSlot = 75;
    public string treeId;

    public Text treeLabel;
    public RectTransform connector;
    public RectTransform treeIconRect;

    public List<BuildingUpgradeSlot> upgrades;

    public void UpdateConnector(int lastPurchasedIndex)
    {
        if (lastPurchasedIndex == -1)
            connector.sizeDelta = new Vector2(0, connector.sizeDelta.y);
        else
        {
            if (lastPurchasedIndex >= upgrades.Count)
                lastPurchasedIndex = upgrades.Count - 1;

            connector.sizeDelta = new Vector2(connectorWidthPerSlot*(lastPurchasedIndex + 1), connector.sizeDelta.y);
        }

    }
}
