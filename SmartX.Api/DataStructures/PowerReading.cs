namespace SmartX.Api.DataStructures;

public readonly struct PowerReading
{
    public int Watts { get; }

    public PowerReading(int watts)
    {
        Watts = watts;
    }

    public static PowerReading operator +(PowerReading left, PowerReading right)
    {
        return new PowerReading(left.Watts + right.Watts);
    }

    public static int operator -(PowerReading left, PowerReading right)
    {
        return left.Watts - right.Watts;
    }

    public static bool operator >(PowerReading left, PowerReading right)
    {
        return left.Watts > right.Watts;
    }

    public static bool operator <(PowerReading left, PowerReading right)
    {
        return left.Watts < right.Watts;
    }

    public override string ToString()
    {
        return $"{Watts} W";
    }
}
