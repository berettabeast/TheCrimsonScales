using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class CalculatedTrajectory : RefractorCardModel<CalculatedTrajectory.CardTop, CalculatedTrajectory.CardBottom>
{
	public override string Name => "CalculatedTrajectory";
	public override int Level => 1;
	public override int Initiative => 24;
	protected override int AtlasIndex => 1;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => RefractCount(parameters.AbilityState) >= 2,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Stun);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				).Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
			.WithDamage(1)
			.WithRange(2)
			.WithAfterTargetConfirmedSubscription(
				ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
					parameters => RefractCount(parameters.AbilityState) >= 2,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(2);
						await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);
					}
				)
			)
			.Build())
		];
	}
}