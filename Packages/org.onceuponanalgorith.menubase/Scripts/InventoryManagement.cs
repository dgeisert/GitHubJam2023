using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string name;
    public int count = 0;
    public int income = 0;
    //not not, but icon would be great

    public InventoryItem(string setName)
    {
        name = setName;
    }
}

public class InventoryCounts
{
    public string names;
    public string counts;
    public string incomes;
    public InventoryItem power;
    public InventoryItem exp;

    public InventoryCounts(string setNames, string setCounts, string setIncomes, InventoryItem setPower, InventoryItem setExp)
    {
        names = setNames;
        counts = setCounts;
        incomes = setIncomes;
        power = setPower;
        exp = setExp;
    }
}

public class InventoryManagement : MonoBehaviour
{

    private Dictionary<string, InventoryItem> items;

    private void Awake()
    {
        items = new Dictionary<string, InventoryItem>();
        AddIncome("Power", 5);
        AddIncome("Experience", 100);
    }

    public void AddInventory(string item, int toAdd)
    {
        if (!items.ContainsKey(item))
        {
            items.Add(item, new InventoryItem(item));
        }
        items[item].count += toAdd;
    }

    public void AddIncome(string item, int toAdd)
    {
        if (!items.ContainsKey(item))
        {
            items.Add(item, new InventoryItem(item));
        }
        items[item].income += toAdd;
    }

    public void Cost(InventoryItem item)
    {
        AddInventory(item.name, -item.count);
        AddIncome(item.name, -item.income);
    }
    public void Reward(InventoryItem item)
    {
        AddInventory(item.name, item.count);
        AddIncome(item.name, item.income);
    }

    public int GetAmount(string item)
    {
        if (!items.ContainsKey(item))
        {
            return 0;
        }
        return items[item].count;
    }

    public InventoryItem GetItem(string item)
    {
        if (!items.ContainsKey(item))
        {
            return null;
        }
        return items[item];
    }

    public InventoryCounts EndTurn()
    {
        items["Power"].count = 0;
        foreach (KeyValuePair<string, InventoryItem> kvp in items)
        {
            if (kvp.Value.name == "Experience")
            {

            }
            else
            {
                kvp.Value.count += kvp.Value.income;
            }
        }
        return InventoryState();
    }

    public InventoryCounts InventoryState()
    {
        StringBuilder names = new StringBuilder();
        StringBuilder counts = new StringBuilder();
        StringBuilder incomes = new StringBuilder();
        foreach (KeyValuePair<string, InventoryItem> kvp in items)
        {
            if (kvp.Value.name == "Power")
            {

            }
            else if (kvp.Value.name == "Experience")
            {

            }
            else
            {
                names.Append(kvp.Value.name);
                names.Append("\n");

                counts.Append(kvp.Value.count);
                counts.Append("\n");

                if (kvp.Value.income != 0)
                {
                    if (kvp.Value.income > 0)
                    {
                        incomes.Append("+");
                    }
                    incomes.Append(kvp.Value.income);
                }
                incomes.Append("\n");
            }
        }
        return new InventoryCounts(names.ToString(), counts.ToString(), incomes.ToString(), items["Power"], items["Experience"]);
    }
}