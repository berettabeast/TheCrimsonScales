using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class OutwardSpurs : ThornreaperLevelUpCardModel<OutwardSpurs.CardTop, OutwardSpurs.CardBottom>
{
	public override string Name => "Outward Spurs";
	public override int Level => 2;
	public override int Initiative => 43;
	protected override int AtlasIndex => 29 - 14;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = state.Performer.Hex;
					if (hex.IsFeatureless())
					{
						await CreateHazardousTerrain(hex, state.Performer);
						state.SetPerformed();
					}
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPierce(2)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),

				]), new AOEHexMark(Vector2I.Zero.Add(Direction.West).Add(Direction.NorthWest), this, new Vector2(0.2f, 0.3f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthWest), this, new Vector2(0.2f, 0.3f)))
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int enemiesDamaged = 0;
					foreach (Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false)
						.Where(f => f.EnemiesWith(state.Performer)))
					{
						await AbilityCmd.SufferDamage(state, figure, 1, state.Performer);
						enemiesDamaged++;
					}
					state.SetCustomValue(this, "EnemiesDamaged", enemiesDamaged);
					state.SetPerformed();
				})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithOnAbilityStarted(async abilityState =>
				{
					int extraMove = abilityState.GetCustomValue<int>(this, "EnemiesDamaged");
					abilityState.AdjustMoveValue(extraMove);
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}
