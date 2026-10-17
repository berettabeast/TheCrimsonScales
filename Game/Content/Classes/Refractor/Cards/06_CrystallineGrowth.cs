using System.Collections.Generic;
using System.Linq;
using Godot;

public class CrystallineGrowth : RefractorCardModel<CrystallineGrowth.CardTop, CrystallineGrowth.CardBottom>
{
	public override string Name => "Crystalline Growth";
	public override int Level => 1;
	public override int Initiative => 51;
	protected override int AtlasIndex => 6;

	public class CardTop : RefractorCardSide
	{
		int _targets = 1;
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithConditions(Conditions.Muddle)
				.WithOnAbilityStarted(DoAttackRefraction())
				.WithCustomGetTargets(CustomRefractionAttackTargets())
				.WithOnAbilityStarted(async state =>
				{
					if (await AbilityCmd.AskConsumeElement(state.Performer, Element.Light))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
						_targets = 2;
					}
				})
				.WithTargets(_targets)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => GetRefractedPrisms(parameters.AbilityState).Any(),
						async parameters =>
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithCustomSelectHexes((state, list) =>
			{
				foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 2, false, true))
				{
					if (RangeHelper.Distance(state.Performer.Hex, hex) == 2)
					{
						list.Add(hex);
					}
				}
			})
			.Build()),

			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithCustomSelectHexes((state, list) =>
			{
				foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 4, false, true))
				{
					if (RangeHelper.Distance(state.Performer.Hex, hex) == 4)
					{
						list.Add(hex);
					}
				}
			})
			.Build()),

			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithCustomSelectHexes((state, list) =>
			{
				foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 6, false, true))
				{
					if (RangeHelper.Distance(state.Performer.Hex, hex) == 6)
					{
						list.Add(hex);
					}
				}
			})
			.Build()),
		];
		public override int XP => 1;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}
}