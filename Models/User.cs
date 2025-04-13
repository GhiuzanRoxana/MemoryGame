using System.ComponentModel;

namespace MemoryGame.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _username;
        private string _imagePath;
        private int _gamesPlayed;
        private int _gamesWon;

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public int GamesPlayed
        {
            get { return _gamesPlayed; }
            set
            {
                if (_gamesPlayed != value)
                {
                    _gamesPlayed = value;
                    OnPropertyChanged(nameof(GamesPlayed));
                }
            }
        }

        public int GamesWon
        {
            get { return _gamesWon; }
            set
            {
                if (_gamesWon != value)
                {
                    _gamesWon = value;
                    OnPropertyChanged(nameof(GamesWon));
                }
            }
        }

        public User(string username, string imagePath)
        {
            Username = username;
            ImagePath = imagePath;
            GamesPlayed = 0;
            GamesWon = 0;
        }

        public User() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}