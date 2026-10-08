using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ExtendedBranch : ThornreaperCardModel<ExtendedBranch.CardTop, ExtendedBranch.CardBottom>
{
	public override string Name => "Extended Branch";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 29 - 3;

	public class CardTop : ThornreaperCardSide
	{
		string _hintText = $"{Icons.HintText(Icons.Attack)}2";

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.5f, 0.6f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero,AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), this, new Vector2(0.5f, 0.4f)),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), this, new Vector2(0.5f, 0.4f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => GameController.Instance.ElementManager.GetState(Element.Light) is ElementState.Strong or ElementState.Waning,
						async parameters =>
						{
							await AbilityCmd.GainXP(parameters.Performer, 1);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
							parameters.AbilityState.SingleTargetAttackValue += 1;
						}
					)
				).WithGetTargetingHintText(state => 
					{			
						if (GameController.Instance.ElementManager.GetState(Element.Light) is ElementState.Strong or ElementState.Waning)
						{
							_hintText = $"{Icons.HintText(Icons.Attack)}3";
						}			
						return _hintText;
					})
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async abilityState =>
					{
						ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
							canApplyParameters => abilityState.Performer.AlliedWith(abilityState.Authority),
							applyParameters =>
							{
								applyParameters.SetAffectedByHazardousTerrain(false);
								return GDTask.CompletedTask;
							});
							await GDTask.CompletedTask;
					})
				.WithOnDeactivate(async abilityState =>
					{
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);
					})
				.Build())
		];
		public override bool Loss => true;
		public override int XP => 1;
		public override bool Persistent => true;
	}
}