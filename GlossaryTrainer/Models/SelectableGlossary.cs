using GlossaryTrainer.Models;

namespace GlossaryTrainer.Models;

public class SelectableGlossary : BindableBase
{
    public Glossary Glossary { get; }

    public SelectableGlossary(Glossary glossary)
    {
        Glossary = glossary;
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string Name => Glossary.Name;
}