using GlossaryTrainer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace GlossaryTrainer.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private List<GlossaryItem> _items;
    private int _currentIndex;
    private int _correctAnswers;

    public MainWindowViewModel()
    {
        var glossaries = LoadGlossaries();
        AvailableGlossaries = new ObservableCollection<SelectableGlossary>(
            glossaries.Select(g => new SelectableGlossary(g)));

        foreach (var glossary in AvailableGlossaries)
            glossary.PropertyChanged += OnGlossarySelectionChanged;

        StartCommand = new DelegateCommand(StartQuiz, CanStartQuiz);
        SubmitCommand = new DelegateCommand(Submit, CanSubmit)
            .ObservesProperty(() => UserInput);
        RestartCommand = new DelegateCommand(Restart);
        SelectGlossaryCommand = new DelegateCommand(SelectGlossary);
    }

    private void OnGlossarySelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectableGlossary.IsSelected))
            StartCommand.RaiseCanExecuteChanged();
    }

    public ObservableCollection<SelectableGlossary> AvailableGlossaries { get; }

    private bool _isQuizStarted;
    public bool IsQuizStarted
    {
        get => _isQuizStarted;
        set => SetProperty(ref _isQuizStarted, value);
    }

    public DelegateCommand StartCommand { get; }
    public DelegateCommand SelectGlossaryCommand { get; }

    private void StartQuiz()
    {
        _items = AvailableGlossaries
            .Where(g => g.IsSelected)
            .SelectMany(g => g.Glossary.Items)
            .OrderBy(_ => Guid.NewGuid())
            .ToList();

        _currentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;
        IsQuizStarted = true;

        LoadCurrent();
    }

    private bool CanStartQuiz() => AvailableGlossaries.Any(g => g.IsSelected);

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
            FeedbackText = "Correct!";
            FeedbackColor = Brushes.Green;
        }
        else
        {
            FeedbackText = $"Wrong! Correct answer:{Environment.NewLine}{string.Join(Environment.NewLine, current.ValidTranslations)}";
            FeedbackColor = Brushes.Red;
        }

        _currentIndex++;
        LoadCurrent();
    }

    private bool CanSubmit()
        => !string.IsNullOrWhiteSpace(UserInput);

    private void LoadCurrent()
    {
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
        IsFinished = true;
        FeedbackText = "";
        FeedbackColor = Brushes.Black;
        ScoreText = $"Score: {_correctAnswers} / {_items.Count}";
    }

    private void Restart()
    {
        _currentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;

        _items.Sort((_, _) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
        LoadCurrent();
    }

    private void SelectGlossary()
    {
        // Reset quiz state
        IsFinished = false;
        IsQuizStarted = false;

        FeedbackText = string.Empty;
        FeedbackColor = Brushes.Black;
        CurrentWord = string.Empty;
        UserInput = string.Empty;
        ScoreText = string.Empty;

        _items = null!;
        _currentIndex = 0;
        _correctAnswers = 0;

        // Optional: unselect all glossaries
        foreach (var glossary in AvailableGlossaries)
            glossary.IsSelected = false;

        StartCommand.RaiseCanExecuteChanged();
    }

    private static List<Glossary> LoadGlossaries()
    => new()
    {
        new Glossary
        {
            Name = "People & Jobs",
            Items =
            {
                new() { Word = "Office worker", ValidTranslations = { "kaishain", "かいしゃいん", "会社員" } },
                new() { Word = "Nurse", ValidTranslations = { "kangoshi", "かんごし", "看護師" } },
            }
        },
        new Glossary
        {
            Name = "Family",
            Items =
            {
                new() { Word = "Husband", ValidTranslations = { "otto", "おっと", "夫" } },
                new() { Word = "Wife", ValidTranslations = { "tsuma", "つま", "妻" } },
            }
        },
        new Glossary
        {
            Name = "Adjectives",
            Items =
            {
                new() { Word = "Smart", ValidTranslations = { "atamagaii", "あたまがいい", "頭がいい" } },
                new() { Word = "Famous", ValidTranslations = { "yuumei", "ゆうめい", "有名" } },
                new() { Word = "Fashionable",  ValidTranslations = new() { "oshare", "おしゃれ" } },
            }
        }
    };

    private static List<GlossaryItem> LoadGlossary()
        => new()
        {
        new() { Word = "Office worker", ValidTranslations = new() { "kaishain", "かいしゃいん", "会社員" } },
        new() { Word = "Husband",      ValidTranslations = new() { "otto", "おっと", "夫" } },
        new() { Word = "Wife",         ValidTranslations = new() { "tsuma", "つま", "妻" } },
        new() { Word = "Smart",        ValidTranslations = new() { "atamagaii", "あたまがいい", "頭がいい" } },
        new() { Word = "Famous",       ValidTranslations = new() { "yuumei", "ゆうめい", "有名" } },
        new() { Word = "Nurse",        ValidTranslations = new() { "kangoshi", "かんごし", "看護師" } },
        new() { Word = "Fashionable",  ValidTranslations = new() { "oshare", "おしゃれ" } },
        };
}
