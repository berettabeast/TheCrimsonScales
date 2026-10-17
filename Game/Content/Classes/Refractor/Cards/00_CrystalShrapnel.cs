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
				.WithDamage(3)
				.WithRange(2)
				.WithOnAbilityStarted(DoAttackRefraction())
				.WithCustomGetTargets(CustomRefractionAttackTargets())
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => DidRefract(parameters.AbilityState),
						async parameters => await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer)))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					if (await AbilityCmd.TryDestroyObstacle(GetRefractedPrisms(attackAbilityState).Last()))
					{
						var targets = RangeHelper.GetFiguresInRange(attackAbilityState.Target.Hex, 1, true, false);
						
						foreach(Figure target in targets)
						{
							await AbilityCmd.AddCondition(attackAbilityState, target, Conditions.Wound1);
						}
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					ConfirmPrompt.Answer confirmAnswer =
						await PromptManager.Prompt(new ConfirmPrompt(null, () => "Destroy prism to wound?"), state.Authority);

					return confirmAnswer.Confirmed;
				})
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