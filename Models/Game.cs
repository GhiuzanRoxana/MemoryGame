using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MemoryGame.Models
{
    public class Game : INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<Card> _cards;
        private string _category;
        private int _rows;
        private int _columns;
        private int _timeLimit;
        private int _timeElapsed;
        private bool _isInProgress;

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

        public ObservableCollection<Card> Cards
        {
            get { return _cards; }
            set
            {
                if (_cards != value)
                {
                    _cards = value;
                    OnPropertyChanged(nameof(Cards));
                }
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        public int Rows
        {
            get { return _rows; }
            set
            {
                if (_rows != value)
                {
                    _rows = value;
                    OnPropertyChanged(nameof(Rows));
                }
            }
        }

        public int Columns
        {
            get { return _columns; }
            set
            {
                if (_columns != value)
                {
                    _columns = value;
                    OnPropertyChanged(nameof(Columns));
                }
            }
        }

        public int TimeLimit
        {
            get { return _timeLimit; }
            set
            {
                if (_timeLimit != value)
                {
                    _timeLimit = value;
                    OnPropertyChanged(nameof(TimeLimit));
                }
            }
        }

        public int TimeElapsed
        {
            get { return _timeElapsed; }
            set
            {
                if (_timeElapsed != value)
                {
                    _timeElapsed = value;
                    OnPropertyChanged(nameof(TimeElapsed));
                    OnPropertyChanged(nameof(TimeRemaining));
                }
            }
        }

        public int TimeRemaining
        {
            get { return TimeLimit - TimeElapsed; }
        }

        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                if (_isInProgress != value)
                {
                    _isInProgress = value;
                    OnPropertyChanged(nameof(IsInProgress));
                }
            }
        }

        public Game(string username, string category, int rows, int columns, int timeLimit)
        {
            Username = username;
            Category = category;
            Rows = rows;
            Columns = columns;
            TimeLimit = timeLimit;
            TimeElapsed = 0;
            IsInProgress = true;
            Cards = new ObservableCollection<Card>();
        }

        public Game()
        {
            Cards = new ObservableCollection<Card>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}