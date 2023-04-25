using System;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace DataAccessLevel.Abstract
{
	public interface IGenericRepository : IDisposable
	{
		IQueryable<T> GetAsQueryable<T>() where T : class;

		T GetById<T>(params object[] keyValues) where T : class;

		T Add<T>(T entity) where T : class;

		int Remove<T>(T entity) where T : class;

		T Update<T>(T entity, T attached = null) where T : class;

		T Attach<T>(T entity) where T : class;

		void Detach<T>(T entity) where T : class;

		T UpdateFields<T>(T entity, params string[] fieldNames) where T : class;

		int SaveChanges();

		DbContextTransaction BeginTransaction(IsolationLevel level);
	}
}
