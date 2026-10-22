using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BlindingSpeed : RefractorCardModel<BlindingSpeed.CardTop, BlindingSpeed.CardBottom>
{
	public override string Name => "Blinding Speed";
	public override int Level => 1;
	public override int Initiative => 37;
	protected override int AtlasIndex => 3;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(4)
				.WithRange(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						_ => true,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Blind);
						})
				)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => DidRefract(parameters.AbilityState),
						async parameters =>
						{
							SetLastUsedPrism(parameters.AbilityState);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);
						}
					)
				)
				.Build()),

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
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
				.WithOnAbilityStarted(async state =>
				{
					if (await AbilityCmd.AskConsumeElement(state.Authority, Element.Light, false))
					{
						await AbilityCmd.GainXP(state.Performer, 1);

						ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this, parameters => parameters.PotentialAbilityState == state &&
						parameters.Hex.HasHexObjectOfType<Prism>(),
						async parameters =>
						{
							state.AdjustMoveValue(1);
						});
					}
				})
				.Build())
		];
	}
}