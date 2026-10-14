public abstract class RefractorAMDCardModel : AMDCardModel
{
	protected override string GetTexturePath(AMDCardOwner owner) => "res://Content/Classes/Refractor/AMDCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 6;
}