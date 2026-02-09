using GlossaryTrainer.Models;
using System.Windows.Media;

namespace GlossaryTrainer.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly List<GlossaryItem> _items;
    private int _currentIndex;
    private int _correctAnswers;

    public MainWindowViewModel()
    {
        _items = LoadGlossary()
            .OrderBy(_ => Guid.NewGuid()) // random order
            .ToList();

        SubmitCommand = new DelegateCommand(Submit, CanSubmit)
            .ObservesProperty(() => UserInput);

        RestartCommand = new DelegateCommand(Restart);

        LoadCurrent();
    }

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
