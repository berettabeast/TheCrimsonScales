using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class CrystallineGrowth : RefractorCardModel<CrystallineGrowth.CardTop, CrystallineGrowth.CardBottom>
{
	public override string Name => "Crystalline Growth";
	public override int Level => 1;
	public override int Initiative => 51;
	protected override int AtlasIndex => 6;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light, null,
						async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}"))
				)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => DidRefract(parameters.AbilityState),
						async parameters =>
						{
							SetLastUsedPrism(parameters.AbilityState);
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
			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithCustomSelectHexes((state, list) =>
			{
				foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 2, false, true))
				{
					if (RangeHelper.Distance(state.Performer.Hex, hex) == 2 && hex.IsEmpty())
					{
						list.Add(hex);
					}
				}
			})
			.Build()),

			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithCustomSelectHexes((state, list) =>
			{
				foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 4, false, true))
				{
					if (RangeHelper.Distance(state.Performer.Hex, hex) == 4 && hex.IsEmpty())
					{
						list.Add(hex);
					}
				}
			})
			.Build()),

			new AbilityCardAbility(CreatePrismObstacleBuilder()
			.WithCustomSelectHexes((state, list) =>
			{
				foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 6, false, true))
				{
					if (RangeHelper.Distance(state.Performer.Hex, hex) == 6 && hex.IsEmpty())
					{
						list.Add(hex);
					}
				}
			})
			.Build()),
		];
		public override int XP => 1;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}
}