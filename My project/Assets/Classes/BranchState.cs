public class BranchState
{
    public float heading { get; set; }
    public float pitch { get; set; }
    public float roll { get; set; }
    public float radius { get; set; }
    public float step { get; set; }
    public float randomizer { get; set; }

    public BranchState()
    {
        this.heading = 0;
        this.pitch = 0;
        this.roll = 0;
        this.radius = 0;
        this.step = 0;
        this.randomizer = 0;
    }

    public BranchState(float heading, float pitch, float roll, float radius, float step, float randomizer)
    {
        this.heading = heading;
        this.pitch = pitch;
        this.roll = roll;
        this.radius = radius;
        this.step = step;
        this.randomizer = randomizer;
    }

    public BranchState(BranchState state)
    {
        this.heading = state.heading;
        this.pitch = state.pitch;
        this.roll = state.roll;
        this.radius = state.radius;
        this.step = state.step;
        this.randomizer = state.randomizer;
    }

    public void update(BranchState state)
    {
        this.heading = state.heading;
        this.pitch = state.pitch;
        this.roll = state.roll;
        this.radius = state.radius;
        this.step = state.step;
        this.randomizer = state.randomizer;
    }

    //to string method
    public override string ToString()
    {
        return "(h p r) = (" + this.heading +" "+ this.pitch +" "+ this.roll + ") Rad = " + this.radius + " step: " + this.step;
    }
}