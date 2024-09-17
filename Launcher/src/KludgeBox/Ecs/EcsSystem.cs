﻿
#region

using KludgeBox.Ecs.Systems.Interfaces;

#endregion

namespace KludgeBox.Ecs;

public abstract class EcsSystem : ISystem
{
    public EcsWorld World { get; set; }
    public bool Enabled { get; set; } = true;
}