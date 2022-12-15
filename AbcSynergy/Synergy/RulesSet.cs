namespace AbcSynergy.Synergy
{
    internal sealed class RulesSet
    {
        private readonly int _limit;
        public List<IRule> Rules { get; } = new(4);
        private int _placeForHeroesCount;
        private int _heroesCount;

        public int RemainingCount => _limit - _placeForHeroesCount;
        public int LastIndex { get; private set; } = 0;
        public bool IsFullHeroes => _heroesCount == _limit;
        public float CommonMightMultiplier { get; private set; }
        public bool HasMostDangerRule { get; private set; }
        public float MultiplierForMostDangerRule { get; private set; }
        public bool HasCanHaveManaRule { get; private set; }
        public float MultiplierForCanHaveManaRule { get; private set; }

        public RulesSet(int limit)
        {
            _limit = limit;
        }

        public bool CanAdd(IRule newRule)
        {
            if (_placeForHeroesCount + newRule.Count > _limit)
                return false;

            for (var index = 0; index < Rules.Count; index++)
            {
                if (Rules[index].Index == newRule.Index)
                    return false;
            }

            return true;
        }

        public void Add(IRule newRule)
        {
            Rules.Add(newRule);
            _placeForHeroesCount += newRule.Count;
            LastIndex = newRule.Index;

            RecalcModifiers();
        }

        public RulesSet Clone()
        {
            var rulesSet = new RulesSet(_limit);
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                rulesSet.Add(rule);
            }

            return rulesSet;
        }

        public bool CanAddHero(HeroData heroData)
        {
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                if (rule.CanAddHero(heroData))
                    return true;
            }

            return false;
        }

        public bool TryAddHero(HeroData heroData)
        {
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                if (rule.TryAddHero(heroData))
                {
                    _heroesCount++;
                    return true;
                }
            }

            return false;
        }

        public void RemoveAllHeroes()
        {
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                rule.RemoveAllHeroes();
            }

            _heroesCount = 0;
        }

        public void ResetHeroMight()
        {
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                for (var heroIndex = 0; heroIndex < rule.Heroes.Count; heroIndex++)
                {
                    HeroData ruleHero = rule.Heroes[heroIndex];
                    ruleHero.ModifiedMight = ruleHero.Might;
                }
            }
        }

        public void ModifyMightForPrivateRules()
        {
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                if (rule.BuffType == BuffType.OnlyMyType)
                    for (var heroIndex = 0; heroIndex < rule.Heroes.Count; heroIndex++)
                    {
                        HeroData ruleHero = rule.Heroes[heroIndex];
                        ruleHero.ModifiedMight *= rule.MightMultiplier;
                    }
            }
        }

        private void RecalcModifiers()
        {
            UpdateCommonMightMultiplier();
            UpdateMultiplierForMostDangerRule();
            UpdateMultiplierForCanHaveManaRule();
        }

        private void UpdateCommonMightMultiplier()
        {
            var multiplier = 1f;
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                if (rule.BuffType == BuffType.All)
                    multiplier *= rule.MightMultiplier;
            }

            CommonMightMultiplier = multiplier;
        }

        private void UpdateMultiplierForMostDangerRule()
        {
            HasMostDangerRule = false;
            var multiplier = 1f;
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                if (rule.BuffType == BuffType.MostDanger)
                {
                    multiplier *= rule.MightMultiplier;
                    HasMostDangerRule = true;
                }
            }

            MultiplierForMostDangerRule = multiplier;
        }

        private void UpdateMultiplierForCanHaveManaRule()
        {
            HasCanHaveManaRule = false;
            var multiplier = 1f;
            for (var index = 0; index < Rules.Count; index++)
            {
                IRule rule = Rules[index];
                if (rule.BuffType == BuffType.CanHaveMana)
                {
                    multiplier *= rule.MightMultiplier;
                    HasCanHaveManaRule = true;
                }
            }

            MultiplierForCanHaveManaRule = multiplier;
        }
    }
}