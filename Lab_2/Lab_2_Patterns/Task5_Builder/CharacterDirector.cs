namespace Lab2_Patterns.Task5_Builder
{
    public class CharacterDirector
    {
        private ICharacterBuilder _builder;
        
        public CharacterDirector(ICharacterBuilder builder)
        {
            _builder = builder;
        }
        
        public Character CreateDefaultHero()
        {
            return _builder
                .SetName("Артеміс Світлоносний")
                .SetHeight("185 см")
                .SetBuild("Атлетична")
                .SetHairColor("Золотистий")
                .SetEyeColor("Блакитний")
                .SetClothing("Блискучі обладунки з драконячої луски")
                .AddInventoryItem("Меч Світла")
                .AddInventoryItem("Щит Непорушності")
                .AddInventoryItem("Зілля зцілення")
                .AddGoodDeed("Врятував королівство від дракона")
                .AddGoodDeed("Зцілив хворих дітей")
                .AddGoodDeed("Побудував притулок для сиріт")
                .Build();
        }
        
        public Character CreateDefaultEnemy()
        {
            return _builder
                .SetName("Мордор Зловісний")
                .SetHeight("210 см")
                .SetBuild("Могутня")
                .SetHairColor("Чорний")
                .SetEyeColor("Червоний")
                .SetClothing("Чорна мантія з черепами")
                .AddInventoryItem("Посох Темряви")
                .AddInventoryItem("Книга проклять")
                .AddInventoryItem("Отруйний кинджал")
                .AddEvilDeed("Захопив три королівства")
                .AddEvilDeed("Перетворив людей на камінь")
                .AddEvilDeed("Викрав принцесу")
                .Build();
        }
        
        public void SetBuilder(ICharacterBuilder builder)
        {
            _builder = builder;
        }
    }
}