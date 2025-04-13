using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class GameViewModels : BaseViewModel
    {
        private readonly User _currentUser;
        private readonly UserService _userService;
        private readonly GameService _gameService;
        private readonly DispatcherTimer _gameTimer;
        private Window _window;

        private ObservableCollection<Card> _cards;
        private bool _isGameSetup = true;
        private bool _isGameInProgress;
        private bool _isStandardSize = true;
        private bool _isCustomSize;
        private int _rows = 4;
        private int _columns = 4;
        private int _gameTime = 60;
        private int _timeRemaining;
        private string _selectedCategory;
        private Card _firstSelectedCard;
        private Card _secondSelectedCard;
        private string _statusText = "Click on two tiles to find the two matching images.";

        public ObservableCollection<Card> Cards
        {
            get { return _cards; }
            set { SetProperty(ref _cards, value); }
        }

        public bool IsGameSetup
        {
            get { return _isGameSetup; }
            set { SetProperty(ref _isGameSetup, value); }
        }

        public bool IsGameInProgress
        {
            get { return _isGameInProgress; }
            set { SetProperty(ref _isGameInProgress, value); }
        }

        public bool IsStandardSize
        {
            get { return _isStandardSize; }
            set
            {
                if (SetProperty(ref _isStandardSize, value) && value)
                {
                    IsCustomSize = false;
                    Rows = 4;
                    Columns = 4;
                }
            }
        }

        public bool IsCustomSize
        {
            get { return _isCustomSize; }
            set
            {
                if (SetProperty(ref _isCustomSize, value) && value)
                {
                    IsStandardSize = false;
                }
            }
        }

        public int Rows
        {
            get { return _rows; }
            set { SetProperty(ref _rows, value); }
        }

        public int Columns
        {
            get { return _columns; }
            set { SetProperty(ref _columns, value); }
        }

        public int GameTime
        {
            get { return _gameTime; }
            set { SetProperty(ref _gameTime, value); }
        }

        public int TimeRemaining
        {
            get { return _timeRemaining; }
            set
            {
                SetProperty(ref _timeRemaining, value);
                OnPropertyChanged(nameof(TimeRemainingText));
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public string TimeRemainingText => string.Format("{0:00}:{1:00}", TimeRemaining / 60, TimeRemaining % 60);

        public List<string> Categories { get; } = new List<string> { "Category1", "Category2", "Category3", "Category4" };

        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                SetProperty(ref _selectedCategory, value);
                OnPropertyChanged(nameof(IsCategory1));
                OnPropertyChanged(nameof(IsCategory2));
                OnPropertyChanged(nameof(IsCategory3));
                OnPropertyChanged(nameof(IsCategory4));
            }
        }

        public bool IsCategory1
        {
            get { return SelectedCategory == "Category1"; }
            set { if (value) SelectedCategory = "Category1"; }
        }

        public bool IsCategory2
        {
            get { return SelectedCategory == "Category2"; }
            set { if (value) SelectedCategory = "Category2"; }
        }

        public bool IsCategory3
        {
            get { return SelectedCategory == "Category3"; }
            set { if (value) SelectedCategory = "Category3"; }
        }

        public bool IsCategory4
        {
            get { return SelectedCategory == "Category4"; }
            set { if (value) SelectedCategory = "Category4"; }
        }

        public List<int> RowOptions { get; } = new List<int> { 2, 3, 4, 5, 6 };
        public List<int> ColumnOptions { get; } = new List<int> { 2, 3, 4, 5, 6 };

        public int SelectedRows
        {
            get { return Rows; }
            set { Rows = value; }
        }

        public int SelectedColumns
        {
            get { return Columns; }
            set { Columns = value; }
        }

        public ICommand SelectCategoryCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetBoardSizeCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand FlipCardCommand { get; }

        public GameViewModels(User user, Window window)
        {
            _currentUser = user;
            _userService = new UserService();
            _gameService = new GameService();
            _window = window;

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;

            SelectCategoryCommand = new RelayCommand(param => SelectedCategory = param.ToString());
            NewGameCommand = new RelayCommand(_ => ResetGame());
            OpenGameCommand = new RelayCommand(_ => OpenGame());
            SaveGameCommand = new RelayCommand(_ => SaveGame(), _ => IsGameInProgress);
            StatisticsCommand = new RelayCommand(_ => ShowStatistics());
            ExitCommand = new RelayCommand(_ => Exit());
            SetBoardSizeCommand = new RelayCommand(param => SetBoardSize(param.ToString()));
            AboutCommand = new RelayCommand(_ => ShowAbout());
            StartGameCommand = new RelayCommand(_ => StartGame(), _ => CanStartGame());
            FlipCardCommand = new RelayCommand(param => FlipCard(param as Card), param => CanFlipCard(param as Card));

            SelectedCategory = Categories.First();
            Cards = new ObservableCollection<Card>();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            TimeRemaining--;

            if (TimeRemaining <= 0)
            {
                EndGame(false);
            }
        }

        private void ResetGame()
        {
            _gameTimer.Stop();
            IsGameSetup = true;
            IsGameInProgress = false;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            StatusText = "Click on two tiles to find the two matching images.";
        }

        private void SaveGame()
        {
            if (!IsGameInProgress)
            {
                MessageBox.Show("Nu există un joc în desfășurare pentru a fi salvat.",
                    "Salvare joc", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Game gameToSave = new Game(_currentUser.Username, SelectedCategory, Rows, Columns, GameTime)
            {
                TimeElapsed = GameTime - TimeRemaining,
                IsInProgress = true,
                Cards = new ObservableCollection<Card>(Cards)
            };

            bool saveSuccess = _gameService.SaveGame(gameToSave);

            if (saveSuccess)
            {
                MessageBox.Show("Jocul a fost salvat cu succes!",
                    "Salvare joc", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void OpenGame()
        {
            if (IsGameInProgress)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Există un joc în desfășurare. Doriți să îl salvați înainte de a deschide un alt joc?",
                    "Joc în desfășurare",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveGame();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            Game savedGame = _gameService.LoadGame(_currentUser.Username);
            if (savedGame == null)
            {
                MessageBox.Show("Nu s-a găsit niciun joc salvat pentru acest utilizator.",
                    "Încărcare joc", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (savedGame.TimeRemaining <= 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Timpul pentru jocul salvat a expirat. Doriți să continuați cu un timp nou?",
                    "Timpul a expirat",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    savedGame.TimeLimit = GameTime;
                    savedGame.TimeElapsed = 0;
                }
                else
                {
                    MessageBox.Show("Jocul salvat a expirat și nu poate fi continuat.",
                        "Joc terminat", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            _gameTimer.Stop();

            SelectedCategory = savedGame.Category;
            Rows = savedGame.Rows;
            Columns = savedGame.Columns;
            TimeRemaining = savedGame.TimeRemaining;

            Cards.Clear();
            foreach (var card in savedGame.Cards)
            {
                Cards.Add(card);
            }

            IsGameSetup = false;
            IsGameInProgress = true;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            StatusText = "Joc încărcat. Continuați să găsiți perechi!";

            _gameTimer.Start();

            MessageBox.Show("Jocul a fost încărcat cu succes!",
                "Încărcare joc", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowStatistics()
        {
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.ShowDialog();
        }

        private void Exit()
        {
            MessageBoxResult result = MessageBox.Show("Sigur doriți să ieșiți din joc?", "Confirmare",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _gameTimer.Stop();
                _window.Close();
            }
        }

        private void SetBoardSize(string size)
        {
            if (size == "Standard")
            {
                IsStandardSize = true;
            }
            else if (size == "Custom")
            {
                IsCustomSize = true;
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show("Memory Game\nCreat Ghiuzan Roxana-Ana-Maria\nEmail: ana.ghiuzan@student.unitbv.ro\nGrupa: 10LF232\nSpecializare:Informatica",
                "Despre", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanStartGame()
        {
            if (IsCustomSize)
            {
                return (Rows * Columns) % 2 == 0;
            }

            return true;
        }

        private void StartGame()
        {
            IsGameSetup = false;
            IsGameInProgress = true;
            TimeRemaining = GameTime;
            StatusText = "Click on a tile to reveal an image.";

            GenerateCards();

            _gameTimer.Start();
        }

        private void GenerateCards()
        {
            Cards.Clear();
            _firstSelectedCard = null;
            _secondSelectedCard = null;

            int totalCards = Rows * Columns;

            if (totalCards % 2 != 0)
            {
                MessageBox.Show("Numărul total de cărți trebuie să fie par. Vă rugăm modificați dimensiunile tablei.",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int pairsCount = totalCards / 2;

            List<string> allImages = GetImagesForCategory(SelectedCategory, pairsCount);

            List<string> cardImages = new List<string>();
            for (int i = 0; i < pairsCount; i++)
            {
                cardImages.Add(allImages[i % allImages.Count]);
                cardImages.Add(allImages[i % allImages.Count]);
            }

            Random random = new Random();
            List<string> shuffledImages = cardImages.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < totalCards; i++)
            {
                Cards.Add(new Card(shuffledImages[i], i));
            }
        }

        private List<string> GetImagesForCategory(string category, int count)
        {
            List<string> images = new List<string>();
            string categoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", category);

            if (Directory.Exists(categoryPath))
            {
                string[] imageFiles = Directory.GetFiles(categoryPath, "*.jpg");

                if (imageFiles.Length < count)
                {
                    string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                    if (Directory.Exists(resourcesPath))
                    {
                        imageFiles = imageFiles.Concat(Directory.GetFiles(resourcesPath, "*.jpg")).ToArray();
                    }
                }

                Array.Sort(imageFiles);
                int availableImages = Math.Min(count, imageFiles.Length);

                for (int i = 0; i < availableImages; i++)
                {
                    images.Add(imageFiles[i]);
                }

                if (imageFiles.Length < count)
                {
                    for (int i = imageFiles.Length; i < count; i++)
                    {
                        images.Add(imageFiles[i % imageFiles.Length]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    images.Add($"pack://application:,,,/Resources/default.jpg");
                }
            }

            return images;
        }


        private bool CanFlipCard(Card card)
        {
            if (card == null || !IsGameInProgress || card.IsMatched)
                return false;

            if (card.IsFlipped)
                return false;

            if (_firstSelectedCard != null && _secondSelectedCard != null)
                return false;

            return true;
        }

        private void FlipCard(Card card)
        {
            if (!CanFlipCard(card))
                return;

            card.IsFlipped = true;

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
                StatusText = "Select another tile to find the matching image.";
                return;
            }

            _secondSelectedCard = card;
            StatusText = "Checking for a match...";

            CheckForMatch();
        }

        private async void CheckForMatch()
        {
            if (_firstSelectedCard.ImagePath == _secondSelectedCard.ImagePath)
            {
                _firstSelectedCard.IsMatched = true;
                _secondSelectedCard.IsMatched = true;

                StatusText = "It's a match! Find another pair.";

                _firstSelectedCard = null;
                _secondSelectedCard = null;

                if (Cards.All(c => c.IsMatched))
                {
                    EndGame(true);
                }
            }
            else
            {
                StatusText = "Not a match! Try again.";

                await System.Threading.Tasks.Task.Delay(1000);

                _firstSelectedCard.IsFlipped = false;
                _secondSelectedCard.IsFlipped = false;

                StatusText = "Select a tile to reveal an image.";

                _firstSelectedCard = null;
                _secondSelectedCard = null;
            }
        }

        private void EndGame(bool isWin)
        {
            _gameTimer.Stop();
            IsGameInProgress = false;

            if (isWin)
            {
                MessageBox.Show("Felicitări! Ai câștigat jocul!", "Joc terminat", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentUser.GamesPlayed++;
                _currentUser.GamesWon++;
                StatusText = "Congratulations! You won the game!";
            }
            else
            {
                MessageBox.Show("Timpul a expirat! Ai pierdut jocul.", "Joc terminat", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentUser.GamesPlayed++;
                StatusText = "Time's up! You lost the game.";
            }

            var users = _userService.LoadUsers();
            var userToUpdate = users.FirstOrDefault(u => u.Username == _currentUser.Username);

            if (userToUpdate != null)
            {
                userToUpdate.GamesPlayed = _currentUser.GamesPlayed;
                userToUpdate.GamesWon = _currentUser.GamesWon;
                _userService.SaveUsers(users);
            }

            IsGameSetup = true;
        }
    }
}