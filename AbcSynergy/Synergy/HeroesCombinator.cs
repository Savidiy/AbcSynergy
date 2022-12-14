namespace AbcSynergy.Synergy
{
    internal sealed class HeroesCombinator
    {
        private readonly List<HeroData> _selectedHeroes = new(8);
        private readonly IndexCombinator _indexCombinator = new();

        public IEnumerable<IReadOnlyList<HeroData>> GetHeroesCombinations(RulesSet rulesSet)
        {
            List<HeroSet> heroSets = GetHeroSets(rulesSet);

            _selectedHeroes.Clear();
            foreach (HeroSet heroSet in heroSets)
                for (int i = 0; i < heroSet.SelectCount; i++)
                    _selectedHeroes.Add(null);

            foreach (bool unused in CombineSets(heroSets, 0, _selectedHeroes, 0))
            {
                yield return _selectedHeroes;
            }
        }

        private IEnumerable<bool> CombineSets(
            List<HeroSet> heroSets,
            int startSet,
            List<HeroData> selectedHeroes,
            int startIndex)
        {
            if (startSet >= heroSets.Count)
            {
                yield return false;
            }
            else
            {
                HeroSet heroSet = heroSets[startSet];
                int heroesCount = heroSet.Heroes.Count;
                int selectCount = heroSet.SelectCount;

                foreach (IReadOnlyList<int> indexCombination in _indexCombinator.GetIndexCombinations(selectCount, heroesCount))
                {
                    for (var index = 0; index < indexCombination.Count; index++)
                    {
                        int heroIndex = indexCombination[index];
                        selectedHeroes[startIndex + index] = heroSet.Heroes[heroIndex];
                    }

                    foreach (bool result in CombineSets(heroSets, startSet + 1, selectedHeroes, startIndex + selectCount))
                    {
                        yield return true;
                    }
                }
            }
        }

        private static List<HeroSet> GetHeroSets(RulesSet rulesSet)
        {
            List<HeroSet> heroSets = new(rulesSet.Rules.Count);

            foreach (IRule rule in rulesSet.Rules)
            {
                if (rule is ClassRule classRule && classRule.Class != Class.Any)
                {
                    var heroSet = new HeroSet(StaticData.MightyHeroesByClass[classRule.Class], classRule.Count);
                    heroSets.Add(heroSet);
                }
                else if (rule is RaceRule raceRule && raceRule.Race != Race.Any)
                {
                    var heroSet = new HeroSet(StaticData.MightyHeroesByRace[raceRule.Race], raceRule.Count);
                    heroSets.Add(heroSet);
                }
            }

            return heroSets;
        }
    }
}