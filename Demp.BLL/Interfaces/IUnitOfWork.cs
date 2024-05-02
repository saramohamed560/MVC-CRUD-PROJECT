using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demp.BLL.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        //Signature of properties of type Repositories
        public IDepartmentRepository DepartmentRepository { get; set; }
        public IEmployeeRepository EmployeeRepository { get; set; }

        Task<int> Complete();
    }
}
