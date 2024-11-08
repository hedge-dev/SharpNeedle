namespace SharpNeedle.SonicTeam.DiEvent;

public abstract class BaseNodeData : IBinarySerializable<GameType>
{
    public virtual void Read(BinaryObjectReader reader, GameType type) { }

    public virtual void Write(BinaryObjectWriter writer, GameType game) { }
}

public enum NodeType
{
    Root = 0,
    Path = 1,
    PathMotion = 2,
    Camera = 3,
    CameraMotion = 4,
    Character = 5,
    CharacterMotion = 6,
    CharacterBehavior = 7,
    ModelCustom = 8,
    Asset = 9,
    ModelMotion = 10,
    Attachment = 11,
    Parameter = 12,
    Stage = 13,
    StageScenarioFlag = 14,
    InstanceMotion = 15,
    InstanceMotionData = 16,
    FolderCondition = 17,
    CharacterBehaviorSimpleTalk = 18,
}