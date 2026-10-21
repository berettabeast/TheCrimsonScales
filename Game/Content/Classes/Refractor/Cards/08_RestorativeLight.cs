using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class RestorativeLight : RefractorCardModel<RestorativeLight.CardTop, RestorativeLight.CardBottom>
{
	public override string Name => "Restorative Light";
	public override int Level => 1;
	public override int Initiative => 85;
	protected override int AtlasIndex => 8;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingHealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Allies)
				.WithRange(2)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Bless);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Add {Icons.Inline(Icons.GetCondition(Conditions.Bless))}"
						)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.HealAfterTargetConfirmed.Subscription.New(
						parameters => DidHealRefract(parameters.AbilityState),
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
			new AbilityCardAbility(RefractingHealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Allies)
				.WithRange(2)
				.Build())
		];
		public override int XP => 1;
	}
}