using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class FocusedRay : RefractorCardModel<FocusedRay.CardTop, FocusedRay.CardBottom>
{
	public override string Name => "Focused Ray";
	public override int Level => 1;
	public override int Initiative => 70;
	protected override int AtlasIndex => 7;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithPierce(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						_ => true,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustRange(1);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						})
				)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => DidRefract(parameters.AbilityState),
						async parameters =>
						{
							SetLastUsedPrism(parameters.AbilityState);
						}
					)
				).Build()),

				new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					var prism = GetLastUsedPrism();

					if (await AbilityCmd.TryDestroyObstacle(prism))
					{						
						await AbilityCmd.InfuseElement(state, Element.Light, state.Performer);
					}
				})
				.WithConditionalAbilityCheck(async state => GetLastUsedPrism() != null)
				.Build())
		];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state => {
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.AbilityRangeType == RangeType.Range,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustRange(2);
							if (DidRefract(parameters.AbilityState))
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(1);
								await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);
							}
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
				})
				.Build())
		];
		public override bool Round => true;
	}
}