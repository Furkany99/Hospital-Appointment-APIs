using Microsoft.EntityFrameworkCore;
using Moq;

namespace xUnitTest
{
	public static class MockDatabaseHelper
	{
		public static DbSet<T> CreateMockDbSet<T>(List<T> data) where T : class
		{
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			return mockSet.Object;
		}
	}
}
