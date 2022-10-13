using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Flow.Launcher.Plugin;
using Flow.Launcher.Infrastructure.Image;
using Flow.Launcher.Core.Plugin;
using System.Windows.Controls;

namespace Flow.Launcher.ViewModel
{
    public class PluginViewModel : BaseModel
    {
        private readonly PluginPair _pluginPair;
        public PluginPair PluginPair
        {
            get => _pluginPair;
            init
            {
                _pluginPair = value;
                value.Metadata.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(PluginPair.Metadata.AvgQueryTime))
                        OnPropertyChanged(nameof(QueryTime));
                };
            }
        }


        private async void LoadIconAsync()
        {
            Image = await ImageLoader.LoadAsync(PluginPair.Metadata.IcoPath);
        }

        public ImageSource Image
        {
            get
            {
                if (_image == ImageLoader.DefaultImage)
                    LoadIconAsync();

                return _image;
            }
            set => _image = value;
        }
        public bool PluginState
        {
            get => !PluginPair.Metadata.Disabled;
            set => PluginPair.Metadata.Disabled = !value;
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SettingControl));
            }
        }

        private Control _settingControl;
        private bool _isExpanded;
        public Control SettingControl 
            => IsExpanded
                ? _settingControl
                    ??= PluginPair.Plugin is not ISettingProvider settingProvider
                        ? new Control() 
                        : settingProvider.CreateSettingPanel() 
                : null;
        private ImageSource _image = ImageLoader.DefaultImage;

        public Visibility ActionKeywordsVisibility => PluginPair.Metadata.ActionKeywords.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
        public string InitilizaTime => PluginPair.Metadata.InitTime + "ms";
        public string QueryTime => PluginPair.Metadata.AvgQueryTime + "ms";
        public string ActionKeywordsText => string.Join(Query.ActionKeywordSeparator, PluginPair.Metadata.ActionKeywords);
        public int Priority => PluginPair.Metadata.Priority;

        public void ChangeActionKeyword(string newActionKeyword, string oldActionKeyword)
        {
            PluginManager.ReplaceActionKeyword(PluginPair.Metadata.ID, oldActionKeyword, newActionKeyword);
            OnPropertyChanged(nameof(ActionKeywordsText));
        }

        public void ChangePriority(int newPriority)
        {
            PluginPair.Metadata.Priority = newPriority;
            OnPropertyChanged(nameof(Priority));
        }

        public static bool IsActionKeywordRegistered(string newActionKeyword) => PluginManager.ActionKeywordRegistered(newActionKeyword);
    }

}
