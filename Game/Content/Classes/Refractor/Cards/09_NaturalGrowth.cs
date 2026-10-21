using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class NaturalGrowth : RefractorCardModel<NaturalGrowth.CardTop, NaturalGrowth.CardBottom>
{
	public override string Name => "Natural Growth";
	public override int Level => 1;
	public override int Initiative => 87;
	protected override int AtlasIndex => 9;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Regenerate)
				.WithTarget(Target.Self)
				.Build()),

			new AbilityCardAbility(CreatePrismObstacleBuilder().WithRange(2).Build())
		];
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light), CardElementInfusion.Infuse(Element.Earth)];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(CreatePrismObstacleBuilder()
				.WithRange(2)
				.Build())
		];
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
	}
}