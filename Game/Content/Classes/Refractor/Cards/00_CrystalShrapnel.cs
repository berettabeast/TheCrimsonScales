using System.Collections.Generic;
using Godot;
using Fractural.Tasks;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

public class CrystalShrapnel : RefractorCardModel<CrystalShrapnel.CardTop, CrystalShrapnel.CardBottom>
{
	public override string Name => "Crystal Shrapnel";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 0;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(2)
				.WithConditions(Conditions.Wound1)
				.WithOnAbilityStarted(DoRefraction())
				.WithCustomGetTargets(CustomRefractionTargets())
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.GetCustomValue<HashSet<Prism>>(this, "RefractedPrisms").Count > 0,
						async parameters => await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer)))
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
			.WithDistance(2, new MoveSquare(this, new Vector2(0.3f, 0.5f)))
			.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
			.WithShieldValue(1)
			.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(state => AbilityCmd.InfuseElement(state, Element.Earth))
			.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
			.Build()
			)
		];
		public override bool Round => true;
	}
}