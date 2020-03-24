using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

public class CharacterInventoryResult : SendablePacket
{
    private static readonly string CHARACTER_INVENTORY_ITEMS = "SELECT * FROM character_inventory WHERE owner=@owner ORDER BY item_id ASC";
    
    public CharacterInventoryResult(Player player)
    {
        List<InventoryHolder> itemList = new List<InventoryHolder>();
            try
            {
                MySqlConnection con = DatabaseManager.GetConnection();
                MySqlCommand cmd = new MySqlCommand(CHARACTER_INVENTORY_ITEMS, con);
                cmd.Parameters.AddWithValue("owner", player.GetName());
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int itemId = reader.GetInt32("item_id");
                    int equiped = reader.GetInt32("equiped");
                    int amount = reader.GetInt32("amount");
                    int enchant = reader.GetInt32("enchant");

                    InventoryHolder items = new InventoryHolder(itemId, equiped, amount, enchant);
                    itemList.Add(items);
                }
                con.Close();
            }
            catch (Exception e)
            {
                LogManager.Log(e.ToString());
            }

        WriteShort(13); // Packet id.
        WriteInt(itemList.Count);
        foreach (InventoryHolder items in itemList)
        {
            WriteInt(items.GetItemId());
            WriteInt(items.GetEquiped());
            WriteInt(items.GetAmount());
            WriteInt(items.GetEnchant());
        }
    }
}
