using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class LoginViewModels : BaseViewModel
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private ObservableCollection<string> _availableImages;
        private int _currentImageIndex = 0;

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);
                OnPropertyChanged(nameof(CanPlay));
                OnPropertyChanged(nameof(CanDeleteUser));
            }
        }

        public ObservableCollection<string> AvailableImages
        {
            get { return _availableImages; }
            set { SetProperty(ref _availableImages, value); }
        }

        public string CurrentImage => AvailableImages?.Count > 0 ? AvailableImages[_currentImageIndex] : null;
        public bool CanPlay => SelectedUser != null;
        public bool CanDeleteUser => SelectedUser != null;
        public bool CanNavigateImages => AvailableImages != null && AvailableImages.Count > 1;

        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand NextImageCommand { get; }

        public LoginViewModels()
        {
            _userService = new UserService();

            NewUserCommand = new RelayCommand(_ => CreateNewUser());
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => CanDeleteUser);
            PlayCommand = new RelayCommand(_ => StartGame(), _ => CanPlay);
            CancelCommand = new RelayCommand(_ => Application.Current.Shutdown());
            PreviousImageCommand = new RelayCommand(_ => NavigateToPreviousImage(), _ => CanNavigateImages);
            NextImageCommand = new RelayCommand(_ => NavigateToNextImage(), _ => CanNavigateImages);

            LoadUsers();
            LoadAvailableImages();
        }

        private void LoadUsers()
        {
            var userList = _userService.LoadUsers();
            Users = new ObservableCollection<User>(userList);
        }

        private void CreateNewUser()
        {
            string username = Microsoft.VisualBasic.Interaction.InputBox("Introduceți numele utilizatorului:", "Utilizator nou", "");

            if (string.IsNullOrWhiteSpace(username))
                return;

            if (AvailableImages != null && AvailableImages.Count > 0)
            {
                string imagePath = AvailableImages[_currentImageIndex];

                var newUser = new User(username, imagePath);
                Users.Add(newUser);

                var userList = new List<User>(Users);
                _userService.SaveUsers(userList);

                SelectedUser = newUser;
            }
            else
            {
                MessageBox.Show("Nu există imagini disponibile în folderul Resources.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser == null)
                return;

            MessageBoxResult result = MessageBox.Show(
                $"Sigur doriți să ștergeți utilizatorul {SelectedUser.Username}?",
                "Confirmare ștergere",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _userService.DeleteUser(SelectedUser.Username);
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
        }

        private void StartGame()
        {
            if (SelectedUser == null)
                return;

           GameWindow gameWindow = new GameWindow(SelectedUser);
           gameWindow.Show();
        }

        private void LoadAvailableImages()
        {
            AvailableImages = new ObservableCollection<string>();

            string resourcesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

            if (!Directory.Exists(resourcesFolder))
            {
                Directory.CreateDirectory(resourcesFolder);
            }

            string[] imageFiles = Directory.GetFiles(resourcesFolder, "*.jpg");

            foreach (string imagePath in imageFiles)
            {
                AvailableImages.Add(imagePath);
            }

            if (AvailableImages.Count == 0)
            {
                MessageBox.Show("Nu s-au găsit imagini în folderul Resources. Adaugă imagini JPG în folderul Resources și asigură-te că sunt setate ca 'Content' cu 'Copy to Output Directory'.", "Avertisment");
            }

            OnPropertyChanged(nameof(CanNavigateImages));
            OnPropertyChanged(nameof(CurrentImage));
        }

        private void NavigateToPreviousImage()
        {
            if (AvailableImages == null || AvailableImages.Count <= 1)
                return;

            _currentImageIndex--;
            if (_currentImageIndex < 0)
                _currentImageIndex = AvailableImages.Count - 1;

            OnPropertyChanged(nameof(CurrentImage));

            if (SelectedUser != null)
            {
                UpdateSelectedUserImage();
            }
        }

        private void NavigateToNextImage()
        {
            if (AvailableImages == null || AvailableImages.Count <= 1)
                return;

            _currentImageIndex++;
            if (_currentImageIndex >= AvailableImages.Count)
                _currentImageIndex = 0;

            OnPropertyChanged(nameof(CurrentImage));

            if (SelectedUser != null)
            {
                UpdateSelectedUserImage();
            }
        }

        private void UpdateSelectedUserImage()
        {
            if (SelectedUser != null && AvailableImages != null && AvailableImages.Count > 0)
            {
                SelectedUser.ImagePath = AvailableImages[_currentImageIndex];
                OnPropertyChanged(nameof(SelectedUser));

                var userList = new List<User>(Users);
                _userService.SaveUsers(userList);
            }
        }
    }
}