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
						parameters => DidRefract(parameters.AbilityState),
						async parameters =>
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);

							if (RefractCount(parameters.AbilityState) > 1)
							{
								parameters.AbilityState.SingleTargetAddCondition(Conditions.Stun);
								await AbilityCmd.GainXP(parameters.Performer, 1);
							}
						}
					)
				).Build())
		];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithRange(2)
			.Build()),

			new AbilityCardAbility(RefractingAttackAbility.Builder()
			.WithDamage(1)
			.WithRange(2)
			.WithDuringAttackSubscription(
				ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
					parameters => DidRefract(parameters.AbilityState),
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(2);
					}
				)
			)
			.Build())
		];
	}
}