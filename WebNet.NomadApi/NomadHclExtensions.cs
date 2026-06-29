namespace WebNet.NomadApi
{
    public static class NomadHclExtensions
    {
        public static string ToHcl(this NomadJob job)
        {
            var hcl = new HclWriter();

            hcl.Block("job", job.Name, () =>
            {
                hcl.Attribute("type", job.Type);

                foreach (var group in job.Groups)
                {
                    WriteGroup(hcl, group);
                }
            });

            return hcl.ToString();
        }

        private static void WriteGroup(HclWriter hcl, TaskGroup group)
        {
            hcl.Block("group", group.Name, () =>
            {
                hcl.Attribute("count", group.Count);

                foreach (var task in group.Tasks)
                {
                    WriteTask(hcl, task);
                }
            });
        }

        private static void WriteTask(HclWriter hcl, NomadTask task)
        {
            hcl.Block("task", task.Name, () =>
            {
                hcl.Attribute("driver", task.Driver);

                // config block
                if (task.Config.Any())
                {
                    hcl.AppendLine("config {");
                    foreach (var kv in task.Config)
                    {
                        var value = kv.Value is string s ? $"\"{s}\"" : kv.Value;
                        hcl.AppendLine($"  {kv.Key} = {value}");
                    }
                    hcl.AppendLine("}");
                }

                // resources block
                hcl.AppendLine("resources {");
                hcl.AppendLine($"  cpu    = {task.Resources.Cpu}");
                hcl.AppendLine($"  memory = {task.Resources.MemoryMb}");
                hcl.AppendLine("}");
            });
        }
    }
}