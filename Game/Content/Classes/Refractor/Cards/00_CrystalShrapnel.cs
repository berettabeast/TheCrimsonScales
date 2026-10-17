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
	}

	public class CardBottom : RefractorCardSide
	{
		List<Prism> _prismsMovedThrough = new List<Prism>();
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
			.WithDistance(3, new MoveSquare(this, new Vector2(0.3f, 0.5f)))
			.WithMoveType(MoveType.Jump)
			.WithOnAbilityStarted(async state =>
			{
				ScenarioEvents.FigureExitingHexEvent.Subscribe(state.Performer, this,
					parameters => parameters.PotentialAbilityState == state,
					async parameters =>
					{
						if (parameters.Figure.Hex.HasHexObjectOfType<Prism>())
						{
							_prismsMovedThrough.Add(parameters.Figure.Hex.GetHexObjectOfType<Prism>());
						}
					});
			})
			.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				var selected = await AbilityCmd.SelectOverlayTile(state, list =>
				{
					foreach (Prism prism in _prismsMovedThrough)
					{
						list.Add(prism);
					}
				}, false, "Select a prism to destroy:");

				if (selected != null)
				{
					await AbilityCmd.TryDestroyObstacle((Prism)selected);
					await AbilityCmd.InfuseElement(state, Element.Earth);
					var createPrismAbility = new AbilityCardAbility(CreatePrismObstacleBuilder().Build());
					await createPrismAbility.Ability.Perform(state.ActionState);
				}
			})
			.WithConditionalAbilityCheck(async _ => _prismsMovedThrough.Any())
			.Build())
		];
	}
}