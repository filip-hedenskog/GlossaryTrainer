using GlossaryTrainer.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace GlossaryTrainer.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private List<GlossaryItem> _items;
    public List<GlossaryItem> FailedItems { get; } = [];
    private int _correctAnswers;

    public MainWindowViewModel()
    {
        var glossaries = GlossaryRepo.Load();
        AvailableGlossaries = new ObservableCollection<Glossary>(glossaries);
        SelectedGlossary = AvailableGlossaries.Count != 0 ? AvailableGlossaries.First() : null;

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
        CopyFeedbackCommand = new DelegateCommand(CopyFeedback);
    }

    public DelegateCommand StartCommand { get; }
    public DelegateCommand SubmitCommand { get; }
    public DelegateCommand RestartCommand { get; }
    public DelegateCommand RunFailedCommand { get; }
    public DelegateCommand LoadCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand SelectGlossaryCommand { get; }
    public DelegateCommand PassShortcutCommand { get; }
    public DelegateCommand FailShortcutCommand { get; }
    public DelegateCommand RevealShortcutCommand { get; }
    public DelegateCommand CopyFeedbackCommand { get; }

    public Glossary? SelectedGlossary { get => field; set => SetProperty(ref field, value); }
    public bool IsQuizStarted { get => field; set => SetProperty(ref field, value); }
    public bool UseJapaneseFont { get => field; set => SetProperty(ref field, value); }
    public bool IsFinished { get => field; set => SetProperty(ref field, value); }
    public string FeedbackText { get => field; set => SetProperty(ref field, value); }
    public string CurrentWord { get => field; set => SetProperty(ref field, value); }
    public string UserInput { get => field; set => SetProperty(ref field, value); }
    public string ScoreText { get => field; set => SetProperty(ref field, value); }
    public string ProgressText { get => field; set => SetProperty(ref field, value); }
    public Brush FeedbackColor { get => field; set => SetProperty(ref field, value); }

    public ObservableCollection<Glossary> AvailableGlossaries { get; }
    public bool CanRunPassOrFailedCommand { get; set; }
    public int TotalItems => _items?.Count ?? 0;
    public event Action? NewWordLoaded;

    public int CurrentIndex
    {
        get => field;
        set
        {
            if (SetProperty(ref field, value))
                UpdateProgressText();
        }
    }

    private void UpdateProgressText()
    {
        ProgressText = $"{CurrentIndex}/{TotalItems}";
    }

    private void OnGlossarySelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectableGlossary.IsSelected))
            StartCommand.RaiseCanExecuteChanged();
    }

    private void StartQuiz()
    {
        if (SelectedGlossary == null)
            return;

        _items = SelectedGlossary.Name == "All"
            ? [.. AvailableGlossaries.SelectMany(g => g.Items).OrderBy(_ => Guid.NewGuid())]
            : [.. SelectedGlossary.Items.OrderBy(_ => Guid.NewGuid())];
        RaisePropertyChanged(nameof(TotalItems));
        CurrentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        IsQuizStarted = true;

        LoadCurrent();
    }

    private bool CanStartQuiz() => SelectedGlossary != null;

    private void OnPassShortcut()
    {
        if (!CanRunPassOrFailedCommand)
            return;

        UserInput = _items[CurrentIndex].ValidTranslations.First();
        Submit();
    }

    private void OnFailShortcut()
    {
        if (!CanRunPassOrFailedCommand)
            return;

        UserInput = "<Manually failed>";
        Submit();
    }

    private void OnRevealShortcut()
    {
        if (!CanRunPassOrFailedCommand)
            return;

        var current = _items[CurrentIndex];
        FeedbackText = $"All answers: {Environment.NewLine}{string.Join(Environment.NewLine, current.ValidTranslations.Take(2))}";
        FeedbackColor = Brushes.DodgerBlue;
        PlayRevealSound();
    }

    private void CopyFeedback()
    {
        if (!string.IsNullOrEmpty(_clipboardText))
            Clipboard.SetText(_clipboardText);
    }

    private void Submit()
    {
        var current = _items[CurrentIndex];

        bool isCorrect = current.ValidTranslations
                           .Any(v => string.Equals(UserInput?.Trim().TrimEnd('。'), v.TrimEnd('。'), StringComparison.OrdinalIgnoreCase));

        var allTranslations = string.Join(Environment.NewLine, current.ValidTranslations);
        if (isCorrect)
        {
            _correctAnswers++;
            FeedbackText = $"Correct! All answers: {Environment.NewLine}{allTranslations}";
            FeedbackColor = positiveFeedbackColor;
            PlayCorrectSound();
        }
        else
        {
            FeedbackText = $"Wrong! Correct answer: {Environment.NewLine}{allTranslations}" + Environment.NewLine +
            $"Your answer:" + Environment.NewLine + UserInput;
            FeedbackColor = negativeFeedbackColor;
            FailedItems.Add(current);
            PlayFailedSound();
        }

        _clipboardText = $"""
            {CurrentWord}
            {UserInput}

            All translations:
            {allTranslations}
            """;
        CurrentIndex++;
        LoadCurrent();
    }

    private string _clipboardText = "";
    private static readonly SolidColorBrush positiveFeedbackColor = new((Color)ColorConverter.ConvertFromString("#52ff8b"));
    private static readonly SolidColorBrush negativeFeedbackColor = new((Color)ColorConverter.ConvertFromString("#ff5996"));

    private bool CanSubmit() => !string.IsNullOrWhiteSpace(UserInput);
    private bool CanRunFailed() => FailedItems.Count != 0;

    private void LoadCurrent()
    {
        UpdateProgressText();
        CanRunPassOrFailedCommand = true;
        UserInput = string.Empty;

        if (CurrentIndex >= _items.Count)
        {
            Finish();
            return;
        }

        CurrentWord = _items[CurrentIndex].Word;
        UseJapaneseFont = _items[CurrentIndex].UseJapaneseFont;
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
        CurrentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        FailedItems.Clear();

        _items.Sort((_, _) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
        LoadCurrent();
    }

    private void RunFailed()
    {
        FeedbackText = "";
        FeedbackColor = Brushes.Black;
        CurrentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        _items = [.. FailedItems];
        RaisePropertyChanged(nameof(TotalItems));
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
        CurrentIndex = 0;
        _correctAnswers = 0;
        FailedItems.Clear();
    }

    public void PlayRevealSound() => PlaySound("Reveal.wav");
    public void PlayCorrectSound() => PlaySound("Correct.wav");
    public void PlayFailedSound() => PlaySound("Wrong.wav");
    public void PlayDoneSound() => PlaySound("Done.wav");

    public void PlaySound(string name)
    {
        var player = new MediaPlayer();
        player.Open(new Uri($@"C:\ProgramData\Glossaries\{name}"));
        player.Volume = 0.15;
        player.Play();
    }
}
