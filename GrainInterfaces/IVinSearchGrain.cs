using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
	public interface IVinSearchGrain : IGrainWithGuidKey
	{
		//Task SetNumber(int number);
		//Task<int> GetNumber();

		Task AddVin(string vin);
		Task AddMakeId(int makeId);
	}
}
