namespace AbcSynergy.Synergy
{
    internal sealed class HeroesCombinator
    {
        private readonly int[] _combinationIndexes;
        private readonly int[] _maxCombinationIndexes;
        private readonly List<HeroSet> _heroSets;
        private readonly List<HeroData> _selectedHeroes;
        private readonly IndexCombinator _indexCombinator = new();
        private int _heroSetCount = 0;
        private bool _hasNextCombination;

        public bool HasNextCombination => _hasNextCombination;

        public HeroesCombinator(int maxSetsCount, int maxHeroesCount)
        {
            _selectedHeroes = new(maxHeroesCount);
            _heroSets = new(maxSetsCount);
            _combinationIndexes = new int[maxSetsCount];
            _maxCombinationIndexes = new int[maxSetsCount];

            for (int i = 0; i < maxSetsCount; i++)
            {
                _heroSets.Add(new HeroSet());
            }
        }

        public void SetupRules(RulesSet rulesSet)
        {
            UpdateHeroSets(rulesSet);
            for (int i = 0; i < _heroSetCount; i++)
            {
                _combinationIndexes[i] = 0;
                
                HeroSet heroSet = _heroSets[i];
                int heroesCount = heroSet.Heroes.Count;
                int selectCount = heroSet.SelectCount;
                List<IReadOnlyList<int>> indexCombinations = _indexCombinator.GetIndexCombinations(selectCount, heroesCount);

                _maxCombinationIndexes[i] = indexCombinations.Count;
            }

            _hasNextCombination = _heroSetCount != 0;
        }

        public IReadOnlyList<HeroData> GetNextCombination()
        {
            int heroInSetPosition = 0;

            for (var heroSetIndex = 0; heroSetIndex < _heroSetCount; heroSetIndex++)
            {
                HeroSet heroSet = _heroSets[heroSetIndex];
                int heroesCount = heroSet.Heroes.Count;
                int selectCount = heroSet.SelectCount;

                List<IReadOnlyList<int>> indexCombinations = _indexCombinator.GetIndexCombinations(selectCount, heroesCount);
                int combinationPosition = _combinationIndexes[heroSetIndex];
                IReadOnlyList<int> indexCombination = indexCombinations[combinationPosition];
                for (var index = 0; index < indexCombination.Count; index++)
                {
                    int heroIndex = indexCombination[index];
                    _selectedHeroes[heroInSetPosition] = heroSet.Heroes[heroIndex];
                    heroInSetPosition++;
                }
            }

            IncrementCombinationIndexes();

            return _selectedHeroes;
        }

        private void IncrementCombinationIndexes()
        {
            _combinationIndexes[_heroSetCount - 1]++;
                
            for (int i = _heroSetCount - 1; i >= 0; i--)
            {
                int index = _combinationIndexes[i];
                int maxIndex = _maxCombinationIndexes[i];
                if (index < maxIndex)
                    continue;
                
                if (i == 0)
                {
                    _hasNextCombination = false;
                }
                else
                {
                    _combinationIndexes[i] = 0;
                    _combinationIndexes[i - 1]++;
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
                if (rule is ClassRule classRule && !classRule.IsRuleForAnyHeroes)
                {
                    _heroSets[index].Set(StaticData.MightyHeroesByClass[classRule.Class], classRule.Count);
                    _heroSetCount++;
                    heroCount += classRule.Count;
                }
                else if (rule is RaceRule raceRule && !raceRule.IsRuleForAnyHeroes)
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