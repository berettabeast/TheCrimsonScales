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
	public Prism LastUsedPrism = null;

	public static CreateObstacleAbility.CreateObstacleBuilder CreatePrismObstacleBuilder()
	{
		return CreateObstacleAbility.Builder()
			.WithCustomAsset("res://Content/Classes/Refractor/Prism.tscn")
			.WithCustomName("Prism");
	}

	public bool DidRefract(AbilityState abilityState, int index = 0)
	{
		return RefractingAttackAbility.RefractionState.DidRefract(abilityState, index);
	}

	public int RefractCount(AbilityState abilityState, int index = 0)
	{
		return RefractingAttackAbility.RefractionState.RefractCount(abilityState, index);
	}

	public List<Prism> GetUsedPrisms(AbilityState abilityState, int index = 0)
	{
		return RefractingAttackAbility.RefractionState.GetUsedPrisms(abilityState, index);
	}

	public bool DidHealRefract(AbilityState abilityState, int index = 0)
	{
		return RefractingHealAbility.RefractionState.DidRefract(abilityState, index);
	}

	public int HealRefractCount(AbilityState abilityState, int index = 0)
	{
		return RefractingHealAbility.RefractionState.RefractCount(abilityState, index);
	}

	public List<Prism> GetUsedPrismsForHeal(AbilityState abilityState, int index = 0)
	{
		return RefractingHealAbility.RefractionState.GetUsedPrisms(abilityState, index);
	}

	public void SetLastUsedPrism(AbilityState abilityState, int index = 0)
	{
		LastUsedPrism = GetUsedPrisms(abilityState, index).Last();
	}

	public Prism GetLastUsedPrism()
	{
		return LastUsedPrism;
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
}