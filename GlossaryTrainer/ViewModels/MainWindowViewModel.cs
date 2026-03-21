using GlossaryTrainer.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Numerics;
using System.Text.Json;
using System.Windows.Media;

namespace GlossaryTrainer.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private List<GlossaryItem> _items;
    public List<GlossaryItem> FailedItems { get; } = [];
    private int _currentIndex;
    private int _correctAnswers;

    public MainWindowViewModel()
    {
        var glossaries = GlossaryRepo.Load();
        AvailableGlossaries = new ObservableCollection<Glossary>(glossaries);

        StartCommand = new DelegateCommand(StartQuiz, CanStartQuiz)
            .ObservesProperty(() => SelectedGlossary);
        SubmitCommand = new DelegateCommand(Submit, CanSubmit)
            .ObservesProperty(() => UserInput);
        RestartCommand = new DelegateCommand(Restart);
        RunFailedCommand = new DelegateCommand(RunFailed, CanRunFailed)
            .ObservesProperty(() => FailedItems);
        LoadCommand = new DelegateCommand(OnLoad);
        SaveCommand = new DelegateCommand(OnSave, CanRunFailed)
            .ObservesProperty(() => FailedItems);
        SelectGlossaryCommand = new DelegateCommand(SelectGlossary);

        PassShortcutCommand = new DelegateCommand(OnPassShortcut);
        FailShortcutCommand = new DelegateCommand(OnFailShortcut);
        RevealShortcutCommand = new DelegateCommand(OnRevealShortcut);
    }

    public bool CanRunPassOrFailedCommand { get; set; }
    private void OnPassShortcut()
    {
        if (!CanRunPassOrFailedCommand)
            return;

        UserInput = _items[_currentIndex].ValidTranslations.First();
        Submit();
    }

    private void OnFailShortcut()
    {
        if (!CanRunPassOrFailedCommand)
            return;

        UserInput = Guid.NewGuid().ToString();
        Submit();
    }

    private void OnRevealShortcut()
    {
        if (!CanRunPassOrFailedCommand)
            return;

        var current = _items[_currentIndex];
        FeedbackText = $"All answers: {Environment.NewLine}{string.Join(Environment.NewLine, current.ValidTranslations)}";
        FeedbackColor = Brushes.DodgerBlue;
        PlayRevealSound();
    }

    public ObservableCollection<Glossary> AvailableGlossaries { get; }

    private Glossary? _selectedGlossary;
    public Glossary? SelectedGlossary
    {
        get => _selectedGlossary;
        set => SetProperty(ref _selectedGlossary, value);
    }

    private void OnGlossarySelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectableGlossary.IsSelected))
            StartCommand.RaiseCanExecuteChanged();
    }


    private bool _isQuizStarted;
    public bool IsQuizStarted
    {
        get => _isQuizStarted;
        set => SetProperty(ref _isQuizStarted, value);
    }

    public DelegateCommand StartCommand { get; }
    public DelegateCommand LoadCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand RunFailedCommand { get; }
    public DelegateCommand SelectGlossaryCommand { get; }
    public DelegateCommand PassShortcutCommand { get; }
    public DelegateCommand FailShortcutCommand { get; }
    public DelegateCommand RevealShortcutCommand { get; }

    private void StartQuiz()
    {
        if (SelectedGlossary == null)
            return;

        _items = SelectedGlossary.Items
            .OrderBy(_ => Guid.NewGuid())
            .ToList();

        _currentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        IsQuizStarted = true;

        LoadCurrent();
    }

    private bool CanStartQuiz()
        => SelectedGlossary != null;


    public event Action? NewWordLoaded;

    // --------------------
    // Bindable properties
    // --------------------
    private string _feedbackText;
    public string FeedbackText
    {
        get => _feedbackText;
        set => SetProperty(ref _feedbackText, value);
    }

    private string _currentTooltip;
    public string CurrentTooltip
    {
        get => _currentTooltip;
        set => SetProperty(ref _currentTooltip, value);
    }

    private Brush _feedbackColor;
    public Brush FeedbackColor
    {
        get => _feedbackColor;
        set => SetProperty(ref _feedbackColor, value);
    }

    private string _currentWord;
    public string CurrentWord
    {
        get => _currentWord;
        set => SetProperty(ref _currentWord, value);
    }

    private string _userInput;
    public string UserInput
    {
        get => _userInput;
        set => SetProperty(ref _userInput, value);
    }

    private bool _isFinished;
    public bool IsFinished
    {
        get => _isFinished;
        set => SetProperty(ref _isFinished, value);
    }

    private string _scoreText;
    public string ScoreText
    {
        get => _scoreText;
        set => SetProperty(ref _scoreText, value);
    }

    // --------------------
    // Commands
    // --------------------

    public DelegateCommand SubmitCommand { get; }
    public DelegateCommand RestartCommand { get; }

    // --------------------
    // Logic
    // --------------------

    private void Submit()
    {
        var current = _items[_currentIndex];

        bool isCorrect = current.ValidTranslations
                           .Any(v => string.Equals(UserInput?.Trim(), v, StringComparison.OrdinalIgnoreCase));

        if (isCorrect)
        {
            _correctAnswers++;
            FeedbackText = $"Correct! All answers: {Environment.NewLine}{string.Join(Environment.NewLine, current.ValidTranslations)}";
            FeedbackColor = Brushes.SeaGreen;
            CurrentTooltip = current.Tooltip;
            PlayCorrectSound();
        }
        else
        {
            FeedbackText = $"Wrong! Correct answer:{Environment.NewLine}{string.Join(Environment.NewLine, current.ValidTranslations)}";
            FeedbackColor = Brushes.Red;
            FailedItems.Add(current);
            CurrentTooltip = current.Tooltip;
            PlayFailedSound();
        }

        _currentIndex++;
        LoadCurrent();
    }

    private bool CanSubmit()
        => !string.IsNullOrWhiteSpace(UserInput);

    private void LoadCurrent()
    {
        CanRunPassOrFailedCommand = true;
        UserInput = string.Empty;

        if (_currentIndex >= _items.Count)
        {
            Finish();
            return;
        }

        CurrentWord = _items[_currentIndex].Word;

        NewWordLoaded?.Invoke();
    }

    private void Finish()
    {
        CanRunPassOrFailedCommand = false;
        PlayDoneSound();
        IsFinished = true;
        ScoreText = $"Score: {_correctAnswers} / {_items.Count}";
        RaisePropertyChanged(nameof(FailedItems));
    }

    private void Restart()
    {
        FeedbackText = "";
        FeedbackColor = Brushes.Black;
        _currentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        FailedItems.Clear();

        _items.Sort((_, _) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
        LoadCurrent();
    }

    private bool CanRunFailed() => FailedItems.Count != 0;
    private void RunFailed()
    {
        FeedbackText = "";
        FeedbackColor = Brushes.Black;
        _currentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        _items = [.. FailedItems];
        FailedItems.Clear();

        _items.Sort((_, _) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
        LoadCurrent();
    }

    public void OnLoad()
    {
        string defaultDirectory = @"C:\ProgramData\Glossaries";
        Directory.CreateDirectory(defaultDirectory);

        var dialog = new OpenFileDialog
        {
            Title = "Open File",
            Filter = "JSON files (*.json)|*.json",
            Multiselect = false,
            InitialDirectory = defaultDirectory
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string filePath = dialog.FileName;

            string content = File.ReadAllText(filePath);
            var glossaryItems = JsonSerializer.Deserialize<List<GlossaryItem>>(content);
            var glossary = new Glossary(Path.GetFileNameWithoutExtension(filePath), glossaryItems ?? []);
            AvailableGlossaries.Add(glossary);
            SelectedGlossary = glossary;
        }
    }

    public void OnSave()
    {
        string defaultDirectory = @"C:\ProgramData\Glossaries";
        Directory.CreateDirectory(defaultDirectory);

        var dialog = new SaveFileDialog
        {
            Title = "Save File",
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = ".json",
            AddExtension = true,
            FileName = "errors.json",
            InitialDirectory = defaultDirectory
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string filePath = dialog.FileName;
            var json = JsonSerializer.Serialize(FailedItems, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }

    private void SelectGlossary()
    {
        FeedbackText = "";
        FeedbackColor = Brushes.Black;
        IsFinished = false;
        IsQuizStarted = false;

        SelectedGlossary = null;

        FeedbackText = string.Empty;
        FeedbackColor = Brushes.Black;
        CurrentWord = string.Empty;
        UserInput = string.Empty;
        ScoreText = string.Empty;

        _items = null!;
        _currentIndex = 0;
        _correctAnswers = 0;
        FailedItems.Clear();
    }

    public void PlayRevealSound()
    {
        PlaySound("Reveal.wav");
    }

    public void PlayCorrectSound()
    {
        PlaySound("Correct.wav");
    }

    public void PlayFailedSound()
    {
        PlaySound("Wrong.wav");
    }

    public void PlayDoneSound()
    {
        PlaySound("Done.wav");
    }

    public void PlaySound(string name)
    {
        var player = new MediaPlayer();
        player.Open(new Uri($@"C:\ProgramData\Glossaries\{name}"));
        player.Volume = 0.15;
        player.Play();
    }
}
