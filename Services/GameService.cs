using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MemoryGame.Models;
namespace MemoryGame.Services
{
    public class GameService
    {
        private readonly string _gameFileExtension = "_game.json";
        public bool SaveGame(Game game)
        {
            try
            {
                string filePath = $"{game.Username}{_gameFileExtension}";

                var gameData = new GameSaveData
                {
                    Username = game.Username,
                    Category = game.Category,
                    Rows = game.Rows,
                    Columns = game.Columns,
                    TimeRemaining = game.TimeRemaining,
                    TimeElapsed = game.TimeElapsed,
                    Cards = new List<CardSaveData>()
                };

                foreach (var card in game.Cards)
                {
                    gameData.Cards.Add(new CardSaveData
                    {
                        ImagePath = card.ImagePath,
                        IsFlipped = card.IsFlipped,
                        IsMatched = card.IsMatched,
                        Position = card.Position
                    });
                }

                string json = JsonSerializer.Serialize(gameData);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Eroare la salvarea jocului: {ex.Message}",
                    "Eroare", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        public Game LoadGame(string username)
        {
            string filePath = $"{username}{_gameFileExtension}";
            if (!File.Exists(filePath))
                return null;

            try
            {
                string json = File.ReadAllText(filePath);
                var gameData = JsonSerializer.Deserialize<GameSaveData>(json);

                var game = new Game(gameData.Username, gameData.Category, gameData.Rows, gameData.Columns, gameData.TimeRemaining)
                {
                    TimeElapsed = gameData.TimeElapsed,
                    IsInProgress = true,
                    Cards = new ObservableCollection<Card>()
                };

                foreach (var cardData in gameData.Cards)
                {
                    var card = new Card(cardData.ImagePath, cardData.Position)
                    {
                        IsFlipped = cardData.IsFlipped,
                        IsMatched = cardData.IsMatched
                    };
                    game.Cards.Add(card);
                }

                return game;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteGame(string username)
        {
            string filePath = $"{username}{_gameFileExtension}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        public List<string> GetImagesForCategory(string category, int count)
        {
            List<string> result = new List<string>();
            string categoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", category);
            if (Directory.Exists(categoryPath))
            {
                string[] imageFiles = Directory.GetFiles(categoryPath, "*.jpg");
                Array.Sort(imageFiles);
                int availableImages = Math.Min(count, imageFiles.Length);
                for (int i = 0; i < availableImages; i++)
                {
                    result.Add(imageFiles[i]);
                }
                if (imageFiles.Length < count)
                {
                    for (int i = imageFiles.Length; i < count; i++)
                    {
                        result.Add(imageFiles[i % imageFiles.Length]);
                    }
                }
            }
            return result;
        }
        public void UpdateStatistics(User user, bool isWin)
        {
            user.GamesPlayed++;
            if (isWin)
                user.GamesWon++;
        }
    }
}