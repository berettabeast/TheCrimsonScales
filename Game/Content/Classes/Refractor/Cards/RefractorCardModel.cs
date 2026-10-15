using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

public abstract class RefractorLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : RefractorCardSide
	where TBottom : RefractorCardSide
{
	protected override string TexturePath => "res://Content/Classes/Refractor/Cards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 3;
}

public abstract class RefractorCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : RefractorCardSide
	where TBottom : RefractorCardSide
{
	protected override string TexturePath => "res://Content/Classes/Refractor/Cards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 3;
}

public abstract class RefractorCardSide : AbilityCardSideModel<Refractor>
{
	public async GDTask CreateCrystalPrismObstacle(Hex hex, Figure creator)
	{
		String scene = "res://Content/OverlayTiles/Obstacles/Crystal1H.tscn";
		hex.Creator = creator;
		await AbilityCmd.TryCreateObstacle(hex, scene);
	}

	// public static AbilityCardAbility RefractingAttack(int damage, int range = 2, Action<AttackAbility.AttackBuilder> configure = null)
	// {
	// 	return Refractor.RefractingAttack(damage, range, configure);
	// }

	protected AttackAbility.AttackBuilder RefractingAttack(int damage, int range = 2)
	{
		var attack = AttackAbility.Builder()
			.WithDamage(damage)
			.WithRange(range);

			attack.WithOnAbilityStarted(async state =>
			{
				var usedObstacles = new HashSet<Obstacle>();
				Hex currentOrigin = state.Performer.Hex;
				int currentRange = range;

				while (true)
				{
					Hex selected = await AbilityCmd.SelectHex(state, list =>
					{
						foreach (Figure figure in RangeHelper.GetFiguresInRange(currentOrigin, currentRange, false, true)
							.Where(f => f.EnemiesWith(state.Performer)))
							list.Add(figure.Hex);

						foreach (Obstacle obstacle in RangeHelper.GetOverlayTilesInRange<Obstacle>(currentOrigin, currentRange, false, true))
						{
							if (obstacle.Hex.Creator == state.Performer && !usedObstacles.Contains(obstacle))
							{
								list.Add(obstacle.Hex);
							}
						}
					}, true, usedObstacles.Count == 0
						? "Select target or obstacle"
						: "Select target or another obstacle");

					if (selected == null)
					{
						return;
					}

					if (selected.HasHexObjectOfType<Obstacle>())
					{
						Obstacle obstacle = selected.GetHexObjectOfType<Obstacle>();
						usedObstacles.Add(obstacle);

						currentOrigin = selected;

						var bounceAttack = AttackAbility.Builder()
							.WithDamage(damage)
							.WithRange(1)
							.WithCustomGetPerformHex(_ => selected)
							.WithAOEPattern(new AOEPattern([
								new AOEHex(Vector2I.Zero, AOEHexType.Gray),
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
							]))
							.Build();

						await bounceAttack.Perform(state.ActionState);

						// return RefractingAttack(damage, range);
					}

					Figure target = selected.GetFigures() 
						.FirstOrDefault(f => f.EnemiesWith(state.Performer));
					if (target != null)
					{
						state.SetCustomValue(this, "RefractCount", usedObstacles.Count);
						attack.WithCustomGetTargets((state, list) => list.Add(target));
					}
					break;
				}
			});
		return attack;
	}

	private async GDTask PerformRefract(ScenarioEvents.AttackAfterTargetConfirmed.Parameters parameters)
{
    Hex obstacleHex = parameters.AbilityState.Target.Hex;

    // Prompt for a secondary target beyond the obstacle

    

    // Redirect the attack (or apply effects) to the bounce target
    // or apply bonus effects, extra damage, conditions, etc.
}
}