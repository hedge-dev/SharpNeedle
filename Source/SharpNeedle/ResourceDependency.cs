﻿namespace SharpNeedle;

public record ResourceDependency
{
    public string Name { get; set; }
    public string Id { get; set; }

    public override string ToString() => $"{Name}:{Id}";
}