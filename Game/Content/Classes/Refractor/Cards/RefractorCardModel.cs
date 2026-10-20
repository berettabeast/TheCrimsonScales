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

	public Func<AttackAbility.State, GDTask> DoAttackRefraction()
	{
		return async state =>
		{
			Hex origin = state.Performer.Hex;
			int range = state.AbilityRange;
			List<Prism> usedPrisms = new List<Prism>();
			List<Figure> targets = new List<Figure>();

			for (int i = 0; i < state.AbilityTargets; i++)
			{
				usedPrisms.Clear();
				origin = state.Performer.Hex;

				while (true)
				{
					var targetableHexes = RangeHelper.GetHexesInRange(origin, range, false, true)
					.Where(h => 
						h.GetHexObjectsOfType<Prism>().Any(p => !usedPrisms.Contains(p)) || 
						h.GetHexObjectsOfType<Figure>().Any(f => f.EnemiesWith(state.Performer) && !targets.Contains(f)) &&
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
						targets.Add(target);
					}
					state.SetCustomValue(this, $"RefractedPrisms_{i}", new List<Prism>(usedPrisms));
					break;
				}
			}
			state.SetCustomValue(this, "CustomAttackTargets", targets);
		};
	}

	public Action<AttackAbility.State, List<Figure>> CustomRefractionAttackTargets()
	{
		return (state, list) =>
		{
			var targets = state.GetCustomValue<List<Figure>>(this, "CustomAttackTargets");
			if (targets != null)
			{
				foreach (Figure target in targets)
				{
					list.Add(target);
				}
			}
		};
	}

	public Func<HealAbility.State, GDTask> DoHealRefraction()
	{
		return async state =>
		{
			Hex origin = state.Performer.Hex;
			int range = state.AbilityRange;
			List<Prism> usedPrisms = new List<Prism>();
			List<Figure> targets = new List<Figure>();

			for (int i = 0; i < state.AbilityTargets; i++)
			{
				usedPrisms.Clear();
				
				while (true)
				{
					var targetableHexes = RangeHelper.GetHexesInRange(origin, range, false, true)
					.Where(h => 
						h.GetHexObjectsOfType<Prism>().Any(p => !usedPrisms.Contains(p)) || 
						h.GetHexObjectsOfType<Figure>().Any(f => f.AlliedWith(state.Performer)) &&
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
						.FirstOrDefault(f => f.AlliedWith(state.Performer));

					if (target != null)
					{
						targets.Add(target);
					}
					state.SetCustomValue(this, "RefractedPrisms", usedPrisms);
					break;
				}
			}
			state.SetCustomValue(this, "CustomHealTargets", targets);
		};
	}

	public Action<HealAbility.State, List<Figure>> CustomRefractionHealTargets()
	{
		return (state, list) =>
		{
			var targets = state.GetCustomValue<List<Figure>>(this, "CustomHealTargets");
			if (targets != null)
			{
				foreach (Figure target in targets)
				{
					list.Add(target);
				}
			}
		};
	}

	// public bool DidRefract(AbilityState state, int abilityIndex = 0)
	// {
	// 	return state.GetCustomValue<List<Prism>>(this, $"RefractedPrisms_{abilityIndex}").Count() > 0;
	// }

	public bool DidAnyAttackRefract(AttackAbility.State state)
	{
		bool anyRefracted = false;

		for (int i = 0; i < state.UniqueTargetedFigures.Count(); i++)
		{
			if (state.GetCustomValue<List<Prism>>(this, $"RefractedPrisms_{i}").Any())
			{
				anyRefracted = true;
			}
		}
		return anyRefracted;
	}

	public List<Prism> GetRefractedPrisms(AbilityState state, int abilityIndex = 0)
	{
		return state.GetCustomValue<List<Prism>>(this, $"RefractedPrisms_{abilityIndex}");
	}
}