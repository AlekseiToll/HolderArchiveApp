using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using DataAccessLevel.Abstract;

namespace DataAccessLevel.Concrete
{
	public class GenericRepository : IGenericRepository
	{
		protected virtual DbContext _context { get; set; }

		public GenericRepository() { }

		public GenericRepository(DbContext cont) { _context = cont; }

		public virtual void Dispose()
		{
			_context?.Dispose();
		}

		public IQueryable<T> GetAsQueryable<T>() where T : class
		{
			return _context.Set<T>().AsQueryable();
		}

		public T GetById<T>(params object[] keyValues) where T : class
		{
			return _context.Set<T>().Find(keyValues);
		}

		public T Add<T>(T entity) where T : class
		{
			T res = _context.Set<T>().Add(entity);
			_context.SaveChanges();

			return res;
		}

		public virtual T Update<T>(T entity, T attached = null) where T : class
		{
			if (attached != null && _context.Set<T>().Local.Any(e => e == attached))
			{
				var attachedEntry = _context.Entry<T>(attached);
				attachedEntry.CurrentValues.SetValues(entity);
				_context.SaveChanges();
				return attached;
			}
			else
			{
				_context.Entry<T>(entity).State = EntityState.Modified;
				_context.SaveChanges();

				return entity;
			}
		}

		public T UpdateFields<T>(T entity, params string[] fieldNames) where T : class
		{
			try
			{
				_context.Configuration.ValidateOnSaveEnabled = false;

				//context.Entry<T>(entity).State = EntityState.Unchanged;

				fieldNames.ToList().ForEach(x => { _context.Entry<T>(entity).Property(x).IsModified = true; });

				_context.SaveChanges();

				_context.Configuration.ValidateOnSaveEnabled = true;
			}
			catch (Exception)
			{
				_context.Configuration.ValidateOnSaveEnabled = true;
				throw;
			}

			return entity;
		}

		public T Attach<T>(T entity) where T : class
		{
			T res = _context.Set<T>().Attach(entity);
			return res;
		}

		public void Detach<T>(T entity) where T : class
		{
			_context.Entry<T>(entity).State = EntityState.Detached;
		}

		public int Remove<T>(T entity) where T : class
		{
			_context.Set<T>().Remove(entity);

			return _context.SaveChanges();
		}
		public int SaveChanges()
		{
			return _context.SaveChanges();
		}

		public DbContextTransaction BeginTransaction(IsolationLevel level)
		{
			if (_context.Database.Connection.State != ConnectionState.Open)
			{
				_context.Database.Connection.Open();
			}

			return _context.Database.BeginTransaction(level);
		}
	}
}
