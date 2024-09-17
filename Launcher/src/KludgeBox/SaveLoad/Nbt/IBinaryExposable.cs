namespace KludgeBox.SaveLoad.Nbt;

public interface IBinaryExposable
{
    TagCompound Expose();

    void RestoreFrom(TagCompound compound);
}