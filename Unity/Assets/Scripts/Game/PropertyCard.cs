public class PropertyCard
{
    public readonly string name;
    public readonly string color;
    public readonly int location;
    public readonly int[] rents;
    public readonly int houseCost;
    public readonly int mortgage;

    public PropertyCard(string name, string color, int location, int[] rents, int houseCost, int mortgage)
    {
        this.name = name;
        this.color = color;
        this.location = location;
        this.rents = rents;
        this.houseCost = houseCost;
        this.mortgage = mortgage;
    }
}
