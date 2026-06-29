public class NomadJob
{
    public string Name { get; set; }
    public string Type { get; set; } = "service";
    public List<TaskGroup> Groups { get; set; } = new();
}

public class TaskGroup
{
    public string Name { get; set; }
    public int Count { get; set; } = 1;
    public List<NomadTask> Tasks { get; set; } = new();
}

public class NomadTask
{
    public string Name { get; set; }
    public string Driver { get; set; }
    public Dictionary<string, object> Config { get; set; } = new();
    public Resources Resources { get; set; } = new();
}

public class Resources
{
    public int Cpu { get; set; }
    public int MemoryMb { get; set; }
}

public class JobBuilder
{
    private readonly NomadJob _job = new();

    public JobBuilder Named(string name)
    {
        _job.Name = name;
        return this;
    }

    public JobBuilder Type(string type)
    {
        _job.Type = type;
        return this;
    }

    public JobBuilder Group(string name, Action<TaskGroupBuilder> configure)
    {
        var builder = new TaskGroupBuilder(name);
        configure(builder);
        _job.Groups.Add(builder.Build());
        return this;
    }

    public NomadJob Build() => _job;
}

public class TaskGroupBuilder
{
    private readonly TaskGroup _group;

    public TaskGroupBuilder(string name)
    {
        _group = new TaskGroup { Name = name };
    }

    public TaskGroupBuilder Count(int count)
    {
        _group.Count = count;
        return this;
    }

    public TaskGroupBuilder Task(string name, Action<TaskBuilder> configure)
    {
        var builder = new TaskBuilder(name);
        configure(builder);
        _group.Tasks.Add(builder.Build());
        return this;
    }

    public TaskGroup Build() => _group;
}

public class TaskBuilder
{
    private readonly NomadTask _task;

    public TaskBuilder(string name)
    {
        _task = new NomadTask { Name = name };
    }

    public TaskBuilder Driver(string driver)
    {
        _task.Driver = driver;
        return this;
    }

    public TaskBuilder Config(string key, object value)
    {
        _task.Config[key] = value;
        return this;
    }

    public TaskBuilder Resources(Action<ResourceBuilder> configure)
    {
        var builder = new ResourceBuilder();
        configure(builder);
        _task.Resources = builder.Build();
        return this;
    }

    public NomadTask Build() => _task;
}

public class ResourceBuilder
{
    private readonly Resources _resources = new();

    public ResourceBuilder Cpu(int cpu)
    {
        _resources.Cpu = cpu;
        return this;
    }

    public ResourceBuilder Memory(int mb)
    {
        _resources.MemoryMb = mb;
        return this;
    }

    public Resources Build() => _resources;
}

public NomadJob Build()
{
    if (string.IsNullOrEmpty(_job.Name))
        throw new InvalidOperationException("Job must have a name");

    return _job;
}

