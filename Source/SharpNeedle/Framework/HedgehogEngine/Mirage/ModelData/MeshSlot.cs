namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using System.Text.Json.Serialization;

public struct MeshSlot
{
    [JsonInclude]
    public MeshType Type;

    [JsonInclude]
    public readonly string? Name;

    [JsonConstructor]
    internal MeshSlot(MeshType type, string name)
    {
        Type = type;

        if (type == MeshType.Special)
        {
            Name = name;
        }
    }

    public MeshSlot(string name)
    {
        Name = default;
        switch (name.ToLowerInvariant())
        {
            case "opaq":
            case "opaque":
                Type = MeshType.Opaque;
                break;

            case "trans":
            case "transparent":
                Type = MeshType.Transparent;
                break;

            case "punch":
            case "punchthrough":
                Type = MeshType.PunchThrough;
                break;

            default:
                Type = MeshType.Special;
                Name = name;
                break;
        }
    }

    public MeshSlot(MeshType type)
    {
        Name = default;
        Type = type;
    }

    public readonly override string ToString()
    {
        return Name ?? Type.ToString();
    }

    public readonly override bool Equals(object? obj)
    {
        if (obj is MeshType type)
        {
            return this == type;
        }
        else if (obj is MeshSlot slot)
        {
            if (slot.Type == MeshType.Special && Type == MeshType.Special)
            {
                return slot.Name == Name;
            }

            return slot.Type == Type;
        }

        return false;
    }


    public readonly override int GetHashCode()
    {
        return Type == MeshType.Special ? Name?.GetHashCode() ?? 0 : Type.GetHashCode();
    }

    public static bool operator ==(MeshSlot lhs, MeshType rhs)
    {
        return lhs.Type == rhs;
    }

    public static bool operator !=(MeshSlot lhs, MeshType rhs)
    {
        return lhs.Type != rhs;
    }

    public static implicit operator MeshSlot(MeshType type)
    {
        return new(type);
    }

    public static implicit operator MeshSlot(string name)
    {
        return new(name);
    }
}