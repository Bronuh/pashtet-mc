namespace KludgeBox.Ecs.Systems.Interfaces;

public interface IPhysicsProcessSystem : ISystem
{
    void PhysicsProcess(double delta);
}