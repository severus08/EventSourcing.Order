using System.Threading.Tasks;

namespace Order.Infrastructure.Command
{
    public interface ICommandBus
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}