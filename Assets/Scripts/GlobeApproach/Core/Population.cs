using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Population
{
    public List<PopClass> Classes = new();

    public int Total => Classes.Sum(c => c.Count);

    public PopClass GetClass(PopType type) => Classes.FirstOrDefault(c => c.Type == type);
}

public enum PopType
{
    Slaves,
    Peasants,
    Burghers,
    Nobles,
    Clergy
}

[System.Serializable]
public class PopClass
{
    public PopType Type;
    public int Count;

    public float GrowthModifier = 1f; // mno¿nik do wzrostu (np. 0.9f - klêska, 1.1f - bonus)
    public float Happiness = 1f;       // wp³ywa na bunty, produkcjê itd.
    public float Influence = 0f;       // wp³yw polityczny/ideologiczny tej klasy

    public PopClass(PopType type, int count)
    {
        Type = type;
        Count = count;
    }
}