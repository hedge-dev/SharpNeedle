﻿namespace SharpNeedle.Studio;
using System.Windows.Input;

public class MenuBuilder
{
    private readonly MenuItem mRoot = new();

    public MenuBuilder WithMenu(string path, ICommand command)
    {
        mRoot.WithChild(path, command);
        return this;
    }

    public MenuItem Build()
    {
        return mRoot;
    }

    public static implicit operator MenuItem(MenuBuilder builder) => builder.Build();
}