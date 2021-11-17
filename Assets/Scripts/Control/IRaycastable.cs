namespace RPG.Control
{
    public interface IRaycastable
    {
        CursorType GetShapeOfCursor();
        bool HandleSpherecast(HeroController callingController);
    }
}

