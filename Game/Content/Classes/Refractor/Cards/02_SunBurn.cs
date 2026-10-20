using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Fractural.Tasks;
using Godot;

public class SunBurn : RefractorCardModel<SunBurn.CardTop, SunBurn.CardBottom>
{
	public override string Name => "Sun Burn";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 2;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(2)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						_ => true,
						async parameters =>
						{
							if (GetRefractedPrisms(parameters.AbilityState) != null)
							{
								int timesRefracted = GetRefractedPrisms(parameters.AbilityState).Count();

								parameters.AbilityState.SingleTargetAdjustAttackValue(2 * timesRefracted);
								await AbilityCmd.GainXP(parameters.Performer, timesRefracted);
							}
						}
					)
				)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						_ => true,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);
						})
				)
				.Build()),
		];
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(2)
				.WithTargets(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => GetRefractedPrisms(parameters.AbilityState).Any(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);
						}
					)
				)
				.Build())
		];
	}
}