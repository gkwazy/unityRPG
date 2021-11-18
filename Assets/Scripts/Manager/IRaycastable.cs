namespace RPG.Manager
{
    public interface IRaycastable
    {
        CursorShape GetShapeOfCursor();
        bool HandleSpherecast(HeroManager callingController);
    }
}

