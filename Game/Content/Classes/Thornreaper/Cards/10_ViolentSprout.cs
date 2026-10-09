using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ViolentSprout : ThornreaperCardModel<ViolentSprout.CardTop, ViolentSprout.CardBottom>
{
	public override string Name => "Violent Sprout";
	public override int Level => 1;
	public override int Initiative => 63;
	protected override int AtlasIndex => 29 - 10;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.ConsumeElement([CardElementConsumption.Consume(Element.Earth)],
							applyFunction: async _ =>
							{
								state.SetPerformed();
								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Consume {Icons.Inline(Icons.GetElement(Element.Earth))}"),
							effectType: EffectType.SelectableMandatory),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async _ =>
							{
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters("res://Art/Icons/Elements/EarthEmpty.svg"),
							effectInfoViewParameters: new TextEffectInfoView.Parameters("Skip action"),
							effectType: EffectType.SelectableMandatory
						)
					], false, $"Consume {Icons.Inline(Icons.GetElement(Element.Earth))} to perform this action or skip");
				})
				.Build()),

				new AbilityCardAbility(AttackAbility.Builder()
					.WithDamage(2, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
					.WithTargets(3)
					.WithCustomGetTargets(async (state, list) =>
					{
						Hex hex = await AbilityCmd.SelectHex(state, createList =>
						{
							foreach (Hex possibleHex in RangeHelper.GetHexesInRange(state.Performer.Hex, 3))
							{
								if (possibleHex != null && possibleHex.IsFeatureless())
								{
									createList.Add(possibleHex);
								}
							}
						}, false, $"Create one 1-hex hazardous terrain in one hex within {Icons.Inline(Icons.Range)}3");
						if (hex != null)
						{
							await CreateHazardousTerrain(hex, state.Performer);
							list.AddRange(GameController.Instance.Map.Figures
								.Where(figure => RangeHelper.GetHexesInRange(hex, 1, true, true)
								.Any()));
						}
					})
					.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
					.Build())
		];
		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						if (GameController.Instance.ElementManager.GetState(Element.Light) is ElementState.Waning or ElementState.Strong)
						{
							await AbilityCmd.InfuseElement(state, Element.Earth);
						}
					})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithConditions(Conditions.Immobilize)
				.Build())
		];
	}
}