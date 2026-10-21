using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StrongFoundation : RefractorCardModel<StrongFoundation.CardTop, StrongFoundation.CardBottom>
{
	public override string Name => "Jagged Clutch";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 4;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
				
			new AbilityCardAbility(CreatePrismObstacleBuilder()
				.WithRange(2)
				.Build()),

			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(2)
				.WithRange(2)
				.WithOnAbilityEndedPerformed(async state => await AbilityCmd.GainXP(state.Performer, 1))
				.WithConditionalAbilityCheck(async state => await AbilityCmd.AskConsumeElement(state.Authority, Element.Light, false))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
	}

	public class CardBottom : RefractorCardSide
	{
		int _range = 6;
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreatePrismObstacleBuilder()
				.WithOnAbilityStarted( async state =>
				{
					if (await AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
					{
						_range = 2;
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.WithRange(_range)
				.WithObstacleCount(3)
				.Build())
		];
	}
}