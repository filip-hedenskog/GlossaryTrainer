namespace GlossaryTrainer.Models;

public class SelectableGlossary : BindableBase
{
    public Glossary Glossary { get; }

    public SelectableGlossary(Glossary glossary)
    {
        Glossary = glossary;
    }

    public bool IsSelected { get => field; set => SetProperty(ref field, value); }
    public string Name => Glossary.Name;
}