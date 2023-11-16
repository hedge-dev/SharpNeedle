namespace SharpNeedle.Ninja.Csd.Extensions;

public static class FontExtensions
{
    public static SurfRide.Draw.Font ToSrdFont(this Font font, string fontName, Scene baseScene, int texListIndex)
    {
        var srdFont = new SurfRide.Draw.Font()
        {
            Name = fontName,
            ID = ++CsdProjectExtensions.GlobalID
        };

        for (int i = 0; i < font.Count; i++)
        {
            srdFont.Add(new()
            {
                Code = (ushort)font[i].SourceIndex,
                TextureListIndex = (ushort)texListIndex,
                TextureIndex = (ushort)baseScene.Sprites[font[i].DestinationIndex].TextureIndex
            });

            int cropIndex = 0;
            while (baseScene.Sprites[cropIndex].TextureIndex != srdFont[i].TextureIndex)
                cropIndex++;

            srdFont[i].CropIndex = (ushort)(font[i].DestinationIndex - cropIndex);
        }

        return srdFont;
    }
}