using System;

public class StoryNode
{
    public string History;
    public string[] Options;
    public bool IsFinal;
    public StoryNode[] NextNode;
    public Action OnNodeVisited;
}
