using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class RestorativeLight : RefractorCardModel<RestorativeLight.CardTop, RestorativeLight.CardBottom>
{
	public override string Name => "Overgrowth";
	public override int Level => 1;
	public override int Initiative => 85;
	protected override int AtlasIndex => 8;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithOnAbilityStarted(DoHealRefraction())
				.WithTarget(Target.Allies)
				.WithRange(2)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.HealAfterTargetConfirmed.Subscription.New(
						parameters => DidRefract(parameters.AbilityState),
						async parameters => await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer)))
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
				.WithCustomGetTargets(CustomRefractionHealTargets())
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
		public override bool Persistent => true;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
					})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}