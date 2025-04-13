using System.Windows;
using MemoryGame.Models;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class GameWindow : Window
    {
        public GameWindow(User user)
        {
            InitializeComponent();
            DataContext = new GameViewModels(user, this);
        }
    }
}