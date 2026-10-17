using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Fractural.Tasks.Triggers;
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
	public static CreateObstacleAbility.CreateObstacleBuilder CreatePrismObstacleBuilder()
	{
		return CreateObstacleAbility.Builder()
			.WithCustomAsset("res://Content/Classes/Refractor/Prism.tscn")
			.WithCustomName("Prism");
	}

	public bool IsStraightLine(Hex from, Hex to)
	{
		if (from == to)
			return true;

		int dq = to.Coords.X - from.Coords.X;
		int dr = to.Coords.Y - from.Coords.Y;

		int ds = -dq - dr;
		return dq == 0 || dr == 0 || ds == 0;
	}

	public Func<AttackAbility.State, GDTask> DoRefraction()
	{
		return async state =>
		{
			var origin = state.Performer.Hex;
			var range = state.AbilityRange;
			var usedPrisms = new HashSet<Prism>();

			while (true)
			{
				var targetableHexes = RangeHelper.GetHexesInRange(origin, range, false, true)
				.Where(h => 
					h.GetHexObjectsOfType<Prism>().Any(p => !usedPrisms.Contains(p)) || 
					h.GetHexObjectsOfType<Figure>().Any(f => f.EnemiesWith(state.Performer)) &&
					(usedPrisms.Count == 0 || IsStraightLine(origin, h)))
				.ToList();

				if (!targetableHexes.Any())
				{
					break;
				}

				Hex selectedHex = await AbilityCmd.SelectHex(state, list =>
				{
					foreach (Hex hex in targetableHexes)
					{
						list.Add(hex);
					}
				}, false, "Select a prism or target:");

				if (selectedHex == null)
				{
					break;
				}

				if (selectedHex.HasHexObjectOfType<Prism>())
				{
					usedPrisms.Add(selectedHex.GetHexObjectOfType<Prism>());
					origin = selectedHex;
					continue;
				}
				
				Figure target = selectedHex.GetHexObjectsOfType<Figure>()
					.FirstOrDefault(f => f.EnemiesWith(state.Performer));

				if (target != null)
				{
					state.SetCustomValue(this, "ForcedTarget", target);
				}
				state.SetCustomValue(this, "RefractedPrisms", usedPrisms);
				break;
			}
		};
	}

	public Action<AttackAbility.State, List<Figure>> CustomRefractionTargets()
	{
		return (state, list) =>
		{
			if (state.GetCustomValue<Figure>(this, "ForcedTarget") != null)
			{
				list.Add(state.GetCustomValue<Figure>(this, "ForcedTarget"));
			}
		};
	}
}