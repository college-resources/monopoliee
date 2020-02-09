public class StationCard
{
    public readonly string name;
    public readonly int location;
    public readonly int[] rents;
    public readonly int mortgage;

    public StationCard(string name, int location, int[] rents, int mortgage)
    {
        this.name = name;
        this.location = location;
        this.rents = rents;
        this.mortgage = mortgage;
    }
}
