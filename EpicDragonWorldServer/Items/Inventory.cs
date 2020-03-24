﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: May 5th 2019
 */
public class Inventory
{
    private static readonly string RESTORE_INVENTORY = "SELECT * FROM character_items WHERE owner=@owner";
    private static readonly string DELETE_INVENTORY = "DELETE FROM character_items WHERE owner=@owner";
    private static readonly string STORE_ITEM_START = "INSERT INTO character_items VALUES ";
    private readonly Dictionary<int, int> items = new Dictionary<int, int>();

    public Inventory(string ownerName)
    {
        lock (items)
        {
            // Restore information from database.
            try
            {
                MySqlConnection con = DatabaseManager.GetConnection();
                MySqlCommand cmd = new MySqlCommand(RESTORE_INVENTORY, con);
                cmd.Parameters.AddWithValue("owner", ownerName);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(reader.GetInt32("slot_id"), reader.GetInt32("item_id"));
                }
                con.Close();
            }
            catch (Exception e)
            {
                LogManager.Log(e.ToString());
            }
        }
    }

    // Only used when player exits the game.
    public void Store(string ownerName)
    {
        // Delete old records.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(DELETE_INVENTORY, con);
            cmd.Parameters.AddWithValue("owner", ownerName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // No need to store if item list is empty.
        int itemCount = items.Count;
        if (itemCount == 0)
        {
            return;
        }

        // Prepare query.
        StringBuilder query = new StringBuilder(STORE_ITEM_START);
        foreach (KeyValuePair<int, int> item in items)
        {
            query.Append("('");
            query.Append(ownerName);
            query.Append("',");
            query.Append(item.Key);
            query.Append(",");
            query.Append(item.Value);
            query.Append(")");
            query.Append(itemCount-- == 1 ? ";" : ",");
        }
        // Store new records.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(query.ToString(), con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Clear item list?
        // items.Clear();
    }

    public int GetSlot(int slotId)
    {
        if (!items.ContainsKey(slotId))
        {
            return 0;
        }
        return items[slotId];
    }

    public void SetSlot(int slotId, int itemId)
    {
        lock (items)
        {
            items.Add(slotId, itemId);
        }
    }

    public void RemoveSlot(int slotId)
    {
        lock (items)
        {
            items.Remove(slotId);
        }
    }

    public Dictionary<int, int> GetItems()
    {
        return items;
    }
}
