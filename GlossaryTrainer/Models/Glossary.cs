using System;
using System.Collections.Generic;
using System.Text;

namespace GlossaryTrainer.Models;

public class Glossary
{
    public Glossary(string name, IEnumerable<GlossaryItem> items)
    {
        Name = name;
        Items = [.. items];
    }
    public string Name { get; set; } = string.Empty;
    public List<GlossaryItem> Items { get; set; } = [];
}
