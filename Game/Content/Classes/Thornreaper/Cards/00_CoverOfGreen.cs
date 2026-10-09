using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class CoverOfGreen : ThornreaperCardModel<CoverOfGreen.CardTop, CoverOfGreen.CardBottom>
{
	public override string Name => "Cover of Green";
	public override int Level => 1;
	public override int Initiative => 34;
	protected override int AtlasIndex => 29 - 0;

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
				.WithDamage(3, new AttackSquare(this, new Vector2(0.234234f, 0.5345345f)))
				.WithConditions(Conditions.Muddle)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), this, new Vector2(0.2f, 0.3f)),
				new AOEHexMark(Vector2I.Zero.Add(Direction.SouthWest), this, new Vector2(0.5f, 0.4f))
				)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()
				)
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
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