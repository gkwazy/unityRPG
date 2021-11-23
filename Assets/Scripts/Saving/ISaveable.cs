namespace RPG.Saving
{
    public interface ISaveable
    {
        object GetState();
        void RestoreState(object state);

    }
}