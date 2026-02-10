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
        AvailableGlossaries = new ObservableCollection<Glossary>(glossaries);

        StartCommand = new DelegateCommand(StartQuiz, CanStartQuiz)
            .ObservesProperty(() => SelectedGlossary);
        SubmitCommand = new DelegateCommand(Submit, CanSubmit)
            .ObservesProperty(() => UserInput);
        RestartCommand = new DelegateCommand(Restart);
        SelectGlossaryCommand = new DelegateCommand(SelectGlossary);
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
    public DelegateCommand SelectGlossaryCommand { get; }

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
        ScoreText = $"Score: {_correctAnswers} / {_items.Count}";
    }

    private void Restart()
    {
        FeedbackText = "";
        FeedbackColor = Brushes.Black;
        _currentIndex = 0;
        _correctAnswers = 0;
        IsFinished = false;

        _items.Sort((_, _) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
        LoadCurrent();
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
    }

    private static List<Glossary> LoadGlossaries()
    => new()
    {
        new Glossary
        {
            Name = "Section 2, Unit 3 - Describe your Family",
            Items =
            {
                new() { Word = "Office worker", ValidTranslations = { "kaishain", "かいしゃいん", "会社員" } },
                new() { Word = "Nurse", ValidTranslations = { "kangoshi", "かんごし", "看護師" } },
                new() { Word = "Husband", ValidTranslations = { "otto", "おっと", "夫" } },
                new() { Word = "Wife", ValidTranslations = { "tsuma", "つま", "妻" } },
                new() { Word = "Smart", ValidTranslations = { "atamagaii", "あたまがいい", "頭がいい" } },
                new() { Word = "Famous", ValidTranslations = { "yuumei", "ゆうめい", "有名" } },
                new() { Word = "Fashionable",  ValidTranslations = new() { "oshare", "おしゃれ" } },
            }
        }
    };
}
