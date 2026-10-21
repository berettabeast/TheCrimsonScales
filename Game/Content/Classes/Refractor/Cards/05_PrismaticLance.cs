using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class PrismaticLance : RefractorCardModel<PrismaticLance.CardTop, PrismaticLance.CardBottom>
{
	public override string Name => "Prismatic Lance";
	public override int Level => 1;
	public override int Initiative => 47;
	protected override int AtlasIndex => 5;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithOnAbilityStarted(async state =>
				{
					if (await AbilityCmd.AskConsumeElement(state.Authority, Element.Light, false))
					{
						state.AbilitySetAOEPattern(new AOEPattern([
								new AOEHex(Vector2I.Zero, AOEHexType.Red), 
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red)
								]));

							await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
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
				.WithDistance(4)
				.WithOnAbilityStarted(async state =>
				{
					state.SetCustomValue(this, "StartingHex", state.Performer.Hex);
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					if (IsStraightLine(state.GetCustomValue<Hex>(this, "StartingHex"), state.Performer.Hex))
					{
						await AbilityCmd.InfuseElement(state, Element.Light, state.Performer);
					}
				})
				.Build())
		];
	}
}