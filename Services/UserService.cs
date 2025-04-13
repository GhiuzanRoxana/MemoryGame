using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public class UserService
    {
        private readonly string _usersFilePath = "users.json";

        public List<User> LoadUsers()
        {
            if (!File.Exists(_usersFilePath))
                return new List<User>();

            try
            {
                string json = File.ReadAllText(_usersFilePath);
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public void SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users);
            File.WriteAllText(_usersFilePath, json);
        }

        public void DeleteUser(string username)
        {
            var users = LoadUsers();
            users.RemoveAll(u => u.Username == username);
            SaveUsers(users);

            DeleteUserGames(username);
        }

        private void DeleteUserGames(string username)
        {
            string userGamePath = $"{username}_game.json";
            if (File.Exists(userGamePath))
                File.Delete(userGamePath);
        }
    }
}