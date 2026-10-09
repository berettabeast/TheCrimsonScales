using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class JaggedClutch : ThornreaperCardModel<JaggedClutch.CardTop, JaggedClutch.CardBottom>
{
	public override string Name => "Jagged Clutch";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 29 - 4;

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
				.WithDamage(4, new AttackSquare(this, new Vector2(0.32887793f, 0.2931021f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.55187505f, 0.2931021f)))
				.WithPull(1)
				.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
					pullParameters => true,
					async pullParameters =>
					{
						ScenarioEvents.FigureEnteredHexEvent.Subscribe(pullParameters.AbilityState, this, 
							enteredHexParameters => enteredHexParameters.Figure == pullParameters.AbilityState.Target &&
													enteredHexParameters.Hex.HasHexObjectOfType<HazardousTerrain>(),
						async enteredHexParameters =>
						{
							await AbilityCmd.AddCondition(pullParameters.AbilityState, enteredHexParameters.Figure, Conditions.Immobilize);
						});
						await GDTask.CompletedTask;
					}))
				.WithOnAbilityEndedPerformed(async state =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
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
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.62026906f, 0.62831855f)))
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithRange(1)
				.WithTarget(Target.Enemies)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(state => AbilityCmd.InfuseElement(state, Element.Earth))
			.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
			.Build())
		];
	}
}