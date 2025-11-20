using kakia_talesweaver_packets.Models;
using kakia_talesweaver_packets.Packets;

namespace kakia_talesweaver_emulator.DB;

public class JsonDB
{
	public static List<Character> GetCharacterList(string accountId)
	{
		string filePath = Path.Combine("DB", "Accounts", accountId);
		Directory.CreateDirectory(filePath);

		string[] characters = Directory.GetFiles(filePath, "*.json");
		List<Character> characterList = new();
		foreach (string character in characters)
		{
			string json = File.ReadAllText(character);
			Character? charData = System.Text.Json.JsonSerializer.Deserialize<Character>(json);
			if (charData != null)
			{
				characterList.Add(charData);
			}
		}
		return characterList;
	}

	public static void SaveCharacter(string accountId, Character character)
	{
		string filePath = Path.Combine("DB", "Accounts", accountId);
		Directory.CreateDirectory(filePath);
		string characterFile = Path.Combine(filePath, $"{character.Name}.json");
		string json = System.Text.Json.JsonSerializer.Serialize(character, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(characterFile, json);
	}
}
