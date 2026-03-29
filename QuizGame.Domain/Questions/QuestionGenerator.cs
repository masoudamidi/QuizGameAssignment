using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.Questions;

public class QuestionGenerator
{
    private readonly Random _rng = new();
 
    public IReadOnlyList<Question> Generate(IReadOnlyList<StationSnapshot> stations, int count = 20)
    {
        if (stations.Count < 4)
            throw new InvalidOperationException("Need at least 4 stations to generate questions.");
 
        var generators = new List<Func<Question?>>
        {
            () => MostBikesQuestion(stations),
            () => FewestBikesQuestion(stations),
            () => BikeCountQuestion(stations),
            () => EmptyStationsQuestion(stations),
            () => LargestProviderQuestion(stations),
        };
 
        var questions = new List<Question>();
        int attempts = 0;
        while (questions.Count < count && attempts < count * 3)
        {
            attempts++;
            var gen = generators[_rng.Next(generators.Count)];
            var q = gen();
            if (q != null) questions.Add(q);
        }
 
        return questions;
    }
 
    private Question? MostBikesQuestion(IReadOnlyList<StationSnapshot> all)
    {
        var group = PickProviderGroup(all, min: 4);
        if (group is null) return null;
 
        var ordered  = group.OrderByDescending(s => s.BikesAvailable).ToList();
        var correct  = ordered[0];
        var choices  = Shuffle(ordered.Skip(1).Take(3).Select(s => s.Name).Append(correct.Name).ToList());
 
        return Make(
            $"Which station in {correct.City} has the most bikes available right now?",
            choices, correct.Name, correct.ProviderId);
    }
 
    private Question? FewestBikesQuestion(IReadOnlyList<StationSnapshot> all)
    {
        var byProvider = all.GroupBy(s => s.ProviderId).ToList();
        var group = byProvider[_rng.Next(byProvider.Count)];
        var stations = group.OrderBy(s => s.BikesAvailable).ToList();
        if (stations.Count < 4) return null;
 
        var correct = stations[0];
        var choices = Shuffle(stations.Skip(1).Take(3).Select(d => d.Name).Append(correct.Name).ToList());
 
        return Make(
            $"Which station in {correct.City} currently has the fewest bikes?",
            choices, correct.Name, correct.ProviderId);
    }
 
    private Question? BikeCountQuestion(IReadOnlyList<StationSnapshot> all)
    {
        var station = all[_rng.Next(all.Count)];
        var correctCount = station.BikesAvailable;
 
        var wrongs = new List<int>();
        while (wrongs.Count < 3)
        {
            var offset = _rng.Next(-10, 11);
            var wrong = Math.Max(0, correctCount + offset);
            if (wrong != correctCount) wrongs.Add(wrong);
        }
 
        var choices = Shuffle(wrongs.Select(w => w.ToString()).Append(correctCount.ToString()).ToList());
 
        return Make(
            $"How many bikes are currently available at \"{station.Name}\" in {station.City}?",
            choices, correctCount.ToString(), station.ProviderId);
    }
 
    private Question? EmptyStationsQuestion(IReadOnlyList<StationSnapshot> all)
    {
        var group = PickProviderGroup(all, min: 4);
        if (group is null) return null;
 
        var emptyCount = group.Count(s => s.BikesAvailable == 0);
        var wrongs = new List<int>();
        while (wrongs.Count < 3)
        {
            var w = Math.Max(0, emptyCount + _rng.Next(-5, 6));
            if (w != emptyCount) wrongs.Add(w);
        }
 
        var city = group.First().City;
        var choices = Shuffle(wrongs.Select(w => w.ToString()).Append(emptyCount.ToString()).ToList());
 
        return Make(
            $"How many stations in {city} are currently empty (zero bikes)?",
            choices, emptyCount.ToString(), group.First().ProviderId);
    }
 
    private Question? LargestProviderQuestion(IReadOnlyList<StationSnapshot> all)
    {
        var providers = all.GroupBy(s => s.ProviderId)
            .Select(g => (Provider: g.Key, City: g.First().City, Total: g.Sum(s => s.Capacity)))
            .OrderByDescending(p => p.Total)
            .ToList();
 
        if (providers.Count < 2) return null;
 
        var correct = providers[0];
        var distractors = providers.Skip(1).Take(3).Select(p => p.City).ToList();
        var choices = Shuffle(distractors.Append(correct.City).ToList());
 
        return Make(
            "Which city has the largest total bike-sharing docking capacity?",
            choices, correct.City, correct.Provider);
    }
 
    private List<StationSnapshot>? PickProviderGroup(IReadOnlyList<StationSnapshot> all, int min)
    {
        var groups = all
            .GroupBy(s => s.ProviderId)
            .Where(g => g.Count() >= min)
            .Select(g => g.ToList())
            .ToList();
 
        return groups.Count == 0 ? null : groups[_rng.Next(groups.Count)];
    }
 
    private static Question Make(string text, List<string> choices, string correct, string provider) =>
        new(Guid.NewGuid(), text, choices.ToArray(), correct, provider);
 
    private List<string> Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            var j = _rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}