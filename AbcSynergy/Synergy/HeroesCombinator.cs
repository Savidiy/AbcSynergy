namespace AbcSynergy.Synergy
{
    internal sealed class HeroesCombinator
    {
        private readonly List<HeroSet> _heroSets;
        private readonly List<HeroData> _selectedHeroes;
        private readonly IndexCombinator _indexCombinator = new();
        private int _heroSetCount = 0;

        public HeroesCombinator(int maxSetsCount, int maxHeroesCount)
        {
            _selectedHeroes = new(maxHeroesCount);
            _heroSets = new(maxSetsCount);
            for (int i = 0; i < maxSetsCount; i++) 
                _heroSets.Add(new HeroSet());
        }

        public IEnumerable<IReadOnlyList<HeroData>> GetHeroesCombinations(RulesSet rulesSet)
        {
            UpdateHeroSets(rulesSet);

            foreach (bool unused in CombineSets(0, 0))
            {
                yield return _selectedHeroes;
            }
        }

        private IEnumerable<bool> CombineSets(int startSet, int startIndex)
        {
            if (startSet >= _heroSetCount)
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
            var heroCount = 0;
            _heroSetCount = 0;
            for (var index = 0; index < rulesSet.Rules.Count; index++)
            {
                IRule rule = rulesSet.Rules[index];
                if (rule is ClassRule classRule && classRule.Class != Class.Any)
                {
                    _heroSets[index].Set(StaticData.MightyHeroesByClass[classRule.Class], classRule.Count);
                    _heroSetCount++;
                    heroCount += classRule.Count;
                }
                else if (rule is RaceRule raceRule && raceRule.Race != Race.Any)
                {
                    _heroSets[index].Set(StaticData.MightyHeroesByRace[raceRule.Race], raceRule.Count);
                    _heroSetCount++;
                    heroCount += raceRule.Count;
                }
            }
            
            _selectedHeroes.Clear();
                for (int i = 0; i < heroCount; i++)
                    _selectedHeroes.Add(null);
        }
    }
}