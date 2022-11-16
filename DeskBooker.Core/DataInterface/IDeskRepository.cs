using DeskBooker.Core.Domian;

namespace DeskBooker.Core.DataInterface;

public interface IDeskRepository
{
    IEnumerable<Desk> GetAvailableDesks(DateTime date);
}
