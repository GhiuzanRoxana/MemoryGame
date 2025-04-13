using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModels : BaseViewModel
    {
        private readonly Window _window;
        private readonly UserService _userService;
        private ObservableCollection<UserStatistics> _users;

        public ObservableCollection<UserStatistics> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }

        public ICommand CloseCommand { get; }

        public StatisticsViewModels(Window window)
        {
            _window = window;
            _userService = new UserService();

            CloseCommand = new RelayCommand(_ => CloseWindow());

            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var userList = _userService.LoadUsers();
            Users = new ObservableCollection<UserStatistics>(
                userList.Select(u => new UserStatistics
                {
                    Username = u.Username,
                    GamesPlayed = u.GamesPlayed,
                    GamesWon = u.GamesWon,
                    WinRate = u.GamesPlayed > 0 ? (double)u.GamesWon / u.GamesPlayed : 0
                })
            );
        }

        private void CloseWindow()
        {
            _window.Close();
        }
    }

    public class UserStatistics
    {
        public string Username { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public double WinRate { get; set; }
    }
}