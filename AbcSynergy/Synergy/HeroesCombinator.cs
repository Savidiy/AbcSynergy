namespace AbcSynergy.Synergy
{
    internal sealed class HeroesCombinator
    {
        private readonly List<HeroSet> _heroSets = new(4);
        private readonly List<HeroData> _selectedHeroes = new(8);
        private readonly IndexCombinator _indexCombinator = new();

        public IEnumerable<IReadOnlyList<HeroData>> GetHeroesCombinations(RulesSet rulesSet)
        {
            UpdateHeroSets(rulesSet);

            _selectedHeroes.Clear();
            foreach (HeroSet heroSet in _heroSets)
                for (int i = 0; i < heroSet.SelectCount; i++)
                    _selectedHeroes.Add(null);

            foreach (bool unused in CombineSets(0, 0))
            {
                yield return _selectedHeroes;
            }
        }

        private IEnumerable<bool> CombineSets(int startSet, int startIndex)
        {
            if (startSet >= _heroSets.Count)
            {
                yield return false;
            }
            else
            {
                HeroSet heroSet = _heroSets[startSet];
                int heroesCount = heroSet.Heroes.Count;
                int selectCount = heroSet.SelectCount;

                foreach (IReadOnlyList<int> indexCombination in _indexCombinator.GetIndexCombinations(selectCount, heroesCount))
                {
                    for (var index = 0; index < indexCombination.Count; index++)
                    {
                        int heroIndex = indexCombination[index];
                        _selectedHeroes[startIndex + index] = heroSet.Heroes[heroIndex];
                    }

                    foreach (bool result in CombineSets(startSet + 1, startIndex + selectCount))
                    {
                        yield return true;
                    }
                }
            }
        }

        private void UpdateHeroSets(RulesSet rulesSet)
        {
            _heroSets.Clear();
            foreach (IRule rule in rulesSet.Rules)
            {
                if (rule is ClassRule classRule && classRule.Class != Class.Any)
                {
                    var heroSet = new HeroSet(StaticData.MightyHeroesByClass[classRule.Class], classRule.Count);
                    _heroSets.Add(heroSet);
                }
                else if (rule is RaceRule raceRule && raceRule.Race != Race.Any)
                {
                    var heroSet = new HeroSet(StaticData.MightyHeroesByRace[raceRule.Race], raceRule.Count);
                    _heroSets.Add(heroSet);
                }
            }
        }
    }
}