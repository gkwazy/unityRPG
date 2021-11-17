namespace RPG.Saving
{
    public interface ISaveable
    {
        object GetWeaponState();
        void RestoreWeaponState(object state);

    }
}