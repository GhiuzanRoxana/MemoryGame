using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace MemoryGame.Models
{
    public class Card : INotifyPropertyChanged
    {
        private string _imagePath;
        private bool _isFlipped;
        private bool _isMatched;
        private int _position;
        [JsonIgnore]
        private BitmapImage _image;

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    LoadImage();
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            private set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        public bool IsFlipped
        {
            get { return _isFlipped; }
            set
            {
                if (_isFlipped != value)
                {
                    _isFlipped = value;
                    OnPropertyChanged(nameof(IsFlipped));
                }
            }
        }

        public bool IsMatched
        {
            get { return _isMatched; }
            set
            {
                if (_isMatched != value)
                {
                    _isMatched = value;
                    OnPropertyChanged(nameof(IsMatched));
                }
            }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public Card(string imagePath, int position)
        {
            _imagePath = imagePath;
            Position = position;
            IsFlipped = false;
            IsMatched = false;
            LoadImage();
        }

        public Card() { }

        private void LoadImage()
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                if (ImagePath.StartsWith("pack://"))
                {
                    bitmap.UriSource = new Uri(ImagePath);
                }
                else
                {
                    bitmap.UriSource = new Uri(ImagePath, UriKind.RelativeOrAbsolute);
                }

                bitmap.EndInit();
                bitmap.Freeze();
                Image = bitmap;
            }
            catch
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri("pack://application:,,,/Resources/default.jpg", UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    Image = bitmap;
                }
                catch
                {
                    Image = null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}